using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunGeneration;
using SaxsSpot.NanoSystemService.Contracts.Messages;

namespace SaxsSpot.NanoSystemService.Kafka.Consumers;

public class RunGenerationConsumer(IMediator mediator, ILogger<RunGenerationConsumer> logger)
    : IConsumer<RunGenerationRequest>
{
    public async Task Consume(ConsumeContext<RunGenerationRequest> context)
    {
        var request = context.Message;
        
        logger.LogInformation("Received RunGenerationRequest from Kafka. OperationId={OperationId}, SeriesId={SeriesId}, Parameters: Count={Count}, NC={Nc}, Excess={Excess}",
            request.OperationId, request.SeriesId, request.Parameters.Count, request.Parameters.NumericalConcentration, request.Parameters.Excess);

        try
        {
            var command = new RunGenerationCommand(
                request.Parameters,
                request.OperationId,
                request.SeriesId);
            
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
