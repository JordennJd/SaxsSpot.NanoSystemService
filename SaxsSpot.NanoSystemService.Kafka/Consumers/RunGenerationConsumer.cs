using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunGeneration;
using SaxsSpot.NanoSystemService.Contracts.Messages;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Kafka.Consumers;

public class RunGenerationConsumer(IMediator mediator, ILogger<RunGenerationConsumer> logger)
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
            // Map DTO to CommonParticleGenerationParameters with default MinSize/MaxSize
            // MinSize and MaxSize are required by base class but not used in generation
            var parameters = new CommonParticleGenerationParameters(
                request.Parameters.Count,
                request.Parameters.NumericalConcentration,
                request.Parameters.GlobalSize,
                MinSize: 1.0f, // Default value, not used
                MaxSize: 3.0f, // Default value, not used
                request.Parameters.Theta,
                request.Parameters.K,
                request.Parameters.Excess,
                request.Parameters.Epsilon);
            
            var command = new RunGenerationCommand(
                parameters,
                request.OperationId,
                request.SeriesId,
                request.ZoneCount ?? 20,
                request.Parameters.PointCount ?? 0, // 0 means no analysis
                request.NeedAnalysis ?? true,
                request.NeedMetrics ?? false);
            
            var result = await mediator.Send(command, context.CancellationToken);
            
            if (result.IsSuccess)
            {
                logger.LogInformation("RunGenerationRequest processed successfully. OperationId: {OperationId}, SeriesId: {SeriesId}, Partition={Partition}, Offset={Offset}. Offset will be committed automatically.", 
                    request.OperationId, request.SeriesId, partition, offset);
                
                // Method completes successfully - MassTransit will automatically commit the offset
                // No exception thrown = offset committed = message marked as processed
                // This prevents other instances from processing the same message
                return;
            }
            else
            {
                var errorMessage = string.Join(", ", result.Errors.Select(e => e.Message));
                logger.LogError("RunGenerationRequest processing failed. OperationId: {OperationId}, Errors: {Errors}", 
                    request.OperationId, errorMessage);
                // Throw exception to prevent offset commit, allowing message to be retried
                throw new InvalidOperationException($"RunGenerationCommand failed: {errorMessage}");
            }
        }
        catch (OperationCanceledException)
        {
            logger.LogWarning("RunGenerationRequest processing was canceled. OperationId: {OperationId}", request.OperationId);
            return;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing RunGenerationRequest. OperationId: {OperationId}", request.OperationId);
            // Re-throw to prevent offset commit, allowing message to be retried
            throw;
        }
    }
}
