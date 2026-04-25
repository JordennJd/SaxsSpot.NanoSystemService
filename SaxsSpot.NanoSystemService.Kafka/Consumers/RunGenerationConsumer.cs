using System.Text.Json;
using MassTransit;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Contracts.Messages;
using SaxsSpot.Shared.ProgressTrackerClient.Contracts.Services;
using JobModels = SaxsSpot.Shared.ProgressTrackerClient.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Kafka.Consumers;

public class RunGenerationConsumer(
    IRunGenerationInboxStorage inboxStorage,
    IJobServiceClient jobServiceClient,
    ILogger<RunGenerationConsumer> logger,
    JsonSerializerOptions jsonSerializerOptions)
    : IConsumer<RunGenerationRequest>
{
    public async Task Consume(ConsumeContext<RunGenerationRequest> context)
    {
        var request = context.Message;
        
        // Log partition and offset for debugging duplicate processing
        var partition = context.GetHeader<int>("kafka-partition", -1);
        var offset = context.GetHeader<long>("kafka-offset", -1);
        
        // Alternative: try to get from Kafka-specific headers
        if (partition == -1)
        {
            context.Headers.TryGetHeader("kafka_partition", out var partHeader);
            if (partHeader != null && int.TryParse(partHeader.ToString(), out var part))
                partition = part;
        }
        
        if (offset == -1)
        {
            context.Headers.TryGetHeader("kafka_offset", out var offHeader);
            if (offHeader != null && long.TryParse(offHeader.ToString(), out var off))
                offset = off;
        }
        
        logger.LogInformation("Received RunGenerationRequest from Kafka. OperationId={OperationId}, SeriesId={SeriesId}, Partition={Partition}, Offset={Offset}, Parameters: Count={Count}, NC={Nc}, Excess={Excess}, ZoneCount={ZoneCount}, PointCount={PointCount}, NeedAnalysis={NeedAnalysis}",
            request.OperationId, request.SeriesId, partition, offset, request.Parameters.Count, request.Parameters.NumericalConcentration, request.Parameters.Excess,
            request.ZoneCount, request.Parameters.PointCount, request.NeedAnalysis);

        try
        {
            var payload = JsonSerializer.Serialize(request, jsonSerializerOptions);
            var enqueued = await inboxStorage.EnqueueAsync(
                request.OperationId,
                request.SeriesId,
                payload,
                context.CancellationToken);

            if (enqueued)
            {
                await NotifyJobQueuedAsync(request.OperationId);
                logger.LogInformation("RunGenerationRequest persisted to inbox. OperationId={OperationId}, SeriesId={SeriesId}, Partition={Partition}, Offset={Offset}. Offset will be committed now.",
                    request.OperationId, request.SeriesId, partition, offset);
            }
            else
            {
                logger.LogInformation("RunGenerationRequest already exists in inbox (duplicate). OperationId={OperationId}, Partition={Partition}, Offset={Offset}. Offset will be committed now.",
                    request.OperationId, partition, offset);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error saving RunGenerationRequest into inbox. OperationId: {OperationId}", request.OperationId);
            throw;
        }
    }

    private async Task NotifyJobQueuedAsync(Guid operationId)
    {
        logger.LogInformation("Operation {OperationId} is waiting in inbox queue", operationId);

        try
        {
            await jobServiceClient.ChangeJobMessageAsync(new JobModels.ChangeJobMessageQuery(
                operationId.ToString(),
                "Message received and saved to inbox queue. Status: waiting"));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to notify job service about waiting inbox message. OperationId={OperationId}", operationId);
        }
    }
}
