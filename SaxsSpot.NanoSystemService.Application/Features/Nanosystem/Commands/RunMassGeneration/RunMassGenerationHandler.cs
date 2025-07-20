using System.Text.Json;
using FluentResults;
using Grpc.Core;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemService.Contracts.Services;
using SaxsSpot.Shared.ProgressTrackerClient.Contracts.Services;
using JobModels = SaxsSpot.Shared.ProgressTrackerClient.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunGeneration;

/// <summary>
/// Run single series nanosystem generation
/// </summary>
/// <param name="scopeFactory"></param>
/// <param name="logger"></param>
public class RunMassGenerationHandler(IServiceScopeFactory scopeFactory, ILogger<RunMassGenerationHandler> logger) : IRequestHandler<RunMassGenerationCommand, Result<Guid>>
{
    private readonly string jobType = "ManualRunMassGeneration";
    private readonly string message = "Nanosystem series generation";

    public async Task<Result<Guid>> Handle(RunMassGenerationCommand request, CancellationToken cancellationToken)
    {
        var operationGuid = Guid.NewGuid();
        
        try
        {
            logger.Log(LogLevel.Information, $"Run generation started with operation id: {operationGuid}");
            
            var cancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);
            
            _ = Task.Run(async () =>
            {
                try
                {
                    var scope = scopeFactory.CreateScope();
                    var nanoSystemService = scope.ServiceProvider.GetService<INanoSystemService>();
                    var jobService = scope.ServiceProvider.GetService<IJobServiceClient>();

                    var workingJobs = await jobService.GetWorkingJobsAsync(new JobModels.GetWorkingJobRequest(jobType));

                    var result = await jobService.CreateJobAsync(new JobModels.CreateJobQuery(operationGuid.ToString(),
                        jobType, message,
                        JsonSerializer.Serialize(request)));

                    if (result.IsSuccessful is false)
                    {
                        throw new InvalidOperationException(
                            $"Operation not created with id {operationGuid} with error on remote server {result.ErrorMessage}");
                    }

                    if (workingJobs.Jobs?.Any() is true)
                    {
                        throw new InvalidOperationException(
                            "$\"Operation not started with id {operationGuid} operation with same type already running\"");
                    }

                    result = await jobService.StartJobAsync(new JobModels.StartJobQuery(operationGuid.ToString()));

                    if (result.IsSuccessful is false)
                    {
                        throw new InvalidOperationException(
                            $"Operation not started with id {operationGuid} with error on remote server {result.ErrorMessage}");
                    }

                    await nanoSystemService.RunSeriesGeneration(request.Parameters,
                        cancellationToken: cancellationTokenSource.Token);

                    result = await jobService.CompleteJobAsync(new JobModels.CompleteJobQuery(operationGuid.ToString(),
                        $"Operation with id {operationGuid} completed successfully"));

                    if (result.IsSuccessful is false)
                    {
                        throw new InvalidOperationException(
                            $"Operation completed with id {operationGuid} but job service is not working with error" +
                            $" on remote server {result.ErrorMessage}");
                    }
                }
                catch (RpcException ex)
                {
                    logger.LogCritical("Remote service is not working with error on remote server: " + ex.Message);
                    throw;

                }
                catch (InvalidOperationException ex)
                {
                    var scope = scopeFactory.CreateScope();

                    var jobService = scope.ServiceProvider.GetService<IJobServiceClient>();
                    logger.LogError(
                        ex.Message);
                    _ = await jobService.CompleteJobAsync(new JobModels.CompleteJobQuery(
                        operationGuid.ToString(), ex.Message, true));
                    throw;
                }

                
            }, cancellationTokenSource.Token);
            
            return operationGuid;
        }
        catch (OperationCanceledException e)
        {
            logger.Log(LogLevel.Information, $"Run generation canceled with operationId: {operationGuid}");
            throw;
        }
    }
}