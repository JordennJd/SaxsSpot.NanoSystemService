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
        
        logger.LogInformation("Received RunGenerationRequest from Kafka. OperationId={OperationId}, SeriesId={SeriesId}, Parameters: Count={Count}, NC={Nc}, Excess={Excess}, ZoneCount={ZoneCount}, PointCount={PointCount}, NeedAnalysis={NeedAnalysis}",
            request.OperationId, request.SeriesId, request.Parameters.Count, request.Parameters.NumericalConcentration, request.Parameters.Excess,
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
                request.NeedAnalysis ?? true);
            
            var result = await mediator.Send(command, context.CancellationToken);
            
            if (result.IsSuccess)
            {
                logger.LogInformation("RunGenerationRequest processed successfully. OperationId: {OperationId}, SeriesId: {SeriesId}", 
                    request.OperationId, request.SeriesId);
            }
            else
            {
                logger.LogError("RunGenerationRequest processing failed. Errors: {Errors}", 
                    string.Join(", ", result.Errors.Select(e => e.Message)));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing RunGenerationRequest");
            throw;
        }
    }
}
