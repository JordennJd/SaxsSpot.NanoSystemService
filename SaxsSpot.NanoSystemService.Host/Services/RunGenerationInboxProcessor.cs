using System.Text.Json;
using MediatR;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunGeneration;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Contracts.Messages;
using SaxsSpot.Shared.ProgressTrackerClient.Contracts.Services;
using JobModels = SaxsSpot.Shared.ProgressTrackerClient.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Host.Services;

public class RunGenerationInboxProcessor(
    IServiceScopeFactory scopeFactory,
    ILogger<RunGenerationInboxProcessor> logger,
    JsonSerializerOptions jsonSerializerOptions) : BackgroundService
{
    private static readonly TimeSpan EmptyQueueDelay = TimeSpan.FromSeconds(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("RunGeneration inbox processor started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = scopeFactory.CreateScope();
                var inboxStorage = scope.ServiceProvider.GetRequiredService<IRunGenerationInboxStorage>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var jobServiceClient = scope.ServiceProvider.GetRequiredService<IJobServiceClient>();

                var claimed = await inboxStorage.ClaimNextPendingAsync(stoppingToken);
                if (claimed is null)
                {
                    await Task.Delay(EmptyQueueDelay, stoppingToken);
                    continue;
                }

                RunGenerationRequest? request;
                try
                {
                    request = JsonSerializer.Deserialize<RunGenerationRequest>(claimed.Payload, jsonSerializerOptions);
                    if (request is null)
                    {
                        throw new InvalidOperationException("Inbox payload deserialized into null request");
                    }
                }
                catch (Exception ex)
                {
                    await inboxStorage.MarkFailedAsync(claimed.Id, $"Invalid payload: {ex.Message}", stoppingToken);
                    await TryNotifyJobFailedAsync(jobServiceClient, claimed.OperationId, $"Inbox payload is invalid: {ex.Message}");
                    logger.LogError(ex, "Invalid inbox payload. InboxId={InboxId}, OperationId={OperationId}", claimed.Id, claimed.OperationId);
                    continue;
                }

                try
                {
                    await TryNotifyJobMessageAsync(jobServiceClient, claimed.OperationId, "Inbox message claimed, generation is starting");
                    var command = RunGenerationCommandFactory.Create(request);
                    var result = await mediator.Send(command, stoppingToken);

                    if (result.IsSuccess)
                    {
                        await inboxStorage.MarkProcessedAsync(claimed.Id, stoppingToken);
                        logger.LogInformation("Inbox message processed. InboxId={InboxId}, OperationId={OperationId}, Attempts={Attempts}",
                            claimed.Id, claimed.OperationId, claimed.Attempts);
                    }
                    else
                    {
                        var error = string.Join(", ", result.Errors.Select(e => e.Message));
                        await inboxStorage.MarkFailedAsync(claimed.Id, error, stoppingToken);
                        await TryNotifyJobFailedAsync(jobServiceClient, claimed.OperationId, $"Generation failed: {error}");
                        logger.LogWarning("Inbox message failed without exception. InboxId={InboxId}, OperationId={OperationId}, Error={Error}",
                            claimed.Id, claimed.OperationId, error);
                    }
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    logger.LogInformation("RunGeneration inbox processor stopped");
                    throw;
                }
                catch (Exception ex)
                {
                    await inboxStorage.MarkFailedAsync(claimed.Id, ex.Message, stoppingToken);
                    await TryNotifyJobFailedAsync(jobServiceClient, claimed.OperationId, $"Generation failed: {ex.Message}");
                    logger.LogError(ex, "Inbox message processing failed. InboxId={InboxId}, OperationId={OperationId}",
                        claimed.Id, claimed.OperationId);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Unhandled error in RunGeneration inbox processor loop");
                await Task.Delay(TimeSpan.FromSeconds(2), stoppingToken);
            }
        }
    }

    private async Task TryNotifyJobMessageAsync(IJobServiceClient jobServiceClient, Guid operationId, string message)
    {
        try
        {
            await jobServiceClient.ChangeJobMessageAsync(new JobModels.ChangeJobMessageQuery(operationId.ToString(), message));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to update job message. OperationId={OperationId}", operationId);
        }
    }

    private async Task TryNotifyJobFailedAsync(IJobServiceClient jobServiceClient, Guid operationId, string errorMessage)
    {
        try
        {
            await jobServiceClient.CompleteJobAsync(new JobModels.CompleteJobQuery(
                operationId.ToString(),
                errorMessage,
                IsFailed: true));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to complete failed job from inbox processor. OperationId={OperationId}", operationId);
        }
    }
}
