using MassTransit;
using MediatR;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunGeneration;
using SaxsSpot.Shared.ProgressTrackerClient.Contracts.Services;

namespace SaxsSpot.NanoSystemService.Kafka.Consumers;

public class RunGenerationConsumer(IMediator mediator, ILogger<RunGenerationConsumer> logger)
    : IConsumer<RunGenerationCommand>
{
    public async Task Consume(ConsumeContext<RunGenerationCommand> context)
    {
        var command = context.Message;
        
        logger.LogInformation("Received RunGenerationCommand from Kafka. Parameters: Count={Count}, NC={Nc}, Excess={Excess}",
            command.Parameters.Count, command.Parameters.NumericalConcentration, command.Parameters.Excess);

        try
        {
            var result = await mediator.Send(command, context.CancellationToken);
            
            if (result.IsSuccess)
            {
                logger.LogInformation("RunGenerationCommand processed successfully. OperationId: {OperationId}", command.OperationId);
            }
            else
            {
                logger.LogError("RunGenerationCommand processing failed. Errors: {Errors}", 
                    string.Join(", ", result.Errors.Select(e => e.Message)));
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error processing RunGenerationCommand");
            throw;
        }
    }
}
