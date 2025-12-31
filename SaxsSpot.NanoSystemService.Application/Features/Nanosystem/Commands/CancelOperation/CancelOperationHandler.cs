using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.CancelOperation;
using SaxsSpot.NanoSystemService.Application.Services;
using SaxsSpot.Shared.ProgressTrackerClient.Contracts.Services;
using JobModels = SaxsSpot.Shared.ProgressTrackerClient.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.CancelOperation;

public class CancelOperationHandler(
    IJobServiceClient jobService,
    ILogger<CancelOperationHandler> logger,
    IOperationCancellationService cancellationService) 
    : IRequestHandler<CancelOperationCommand, Result<Unit>>
{
    public async Task<Result<Unit>> Handle(CancelOperationCommand request, CancellationToken cancellationToken)
    {
        try
        {
            logger.LogInformation("Attempting to cancel operation {OperationId}", request.OperationId);

            if (!cancellationService.IsOperationRegistered(request.OperationId))
            {
                logger.LogWarning("Operation {OperationId} not found or already completed", request.OperationId);
                return FluentResults.Result.Fail($"Operation {request.OperationId} not found or already completed");
            }

            string? operationType = null;
            if (cancellationService.TryGetOperationType(request.OperationId, out var storedType))
            {
                operationType = storedType;
            }
            else if (!string.IsNullOrEmpty(request.OperationType))
            {
                operationType = request.OperationType;
            }

            if (!string.IsNullOrEmpty(operationType) && !OperationType.IsSupported(operationType))
            {
                logger.LogWarning("Operation type {OperationType} is not supported for cancellation", operationType);
                return FluentResults.Result.Fail($"Operation type '{operationType}' is not supported for cancellation");
            }

            if (!cancellationService.CancelOperation(request.OperationId))
            {
                logger.LogWarning("Failed to cancel operation {OperationId}", request.OperationId);
                return FluentResults.Result.Fail($"Failed to cancel operation {request.OperationId}");
            }

            try
            {
                var result = await jobService.CompleteJobAsync(new JobModels.CompleteJobQuery(
                    request.OperationId.ToString(),
                    "Operation cancelled by user",
                    true));

                if (result.IsSuccessful)
                {
                    logger.LogInformation("Job service notified about cancellation for operation {OperationId}", request.OperationId);
                }
                else
                {
                    logger.LogWarning("Failed to notify job service about cancellation for operation {OperationId}. Error: {ErrorMessage}",
                        request.OperationId, result.ErrorMessage);
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error while notifying job service about cancellation for operation {OperationId}", request.OperationId);
            }

            cancellationService.RemoveOperation(request.OperationId);
            logger.LogInformation("Operation {OperationId} cancelled successfully", request.OperationId);

            return FluentResults.Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error cancelling operation {OperationId}", request.OperationId);
            return FluentResults.Result.Fail($"Failed to cancel operation: {ex.Message}");
        }
    }
}
