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
        var isOperationRegistered = cancellationService.IsOperationRegistered(request.OperationId);
        var localCancellationSucceeded = false;
        string? errorMessage = null;
        
        if (!string.IsNullOrEmpty(request.OperationType) && !OperationType.IsSupported(request.OperationType))
        {
            errorMessage = $"Operation type '{request.OperationType}' is not supported for cancellation";
            logger.LogWarning("Operation type {OperationType} is not supported for cancellation", request.OperationType);
            return FluentResults.Result.Fail(errorMessage);
        }
        
        try
        {
            logger.LogInformation("Attempting to cancel operation {OperationId}", request.OperationId);

            if (!isOperationRegistered)
            {
                logger.LogWarning("Operation {OperationId} not found in local registry, will still attempt to cancel in job service", request.OperationId);
            }
            else
            {
                localCancellationSucceeded = cancellationService.CancelOperation(request.OperationId);
                if (!localCancellationSucceeded)
                {
                    logger.LogWarning("Failed to cancel operation {OperationId} locally", request.OperationId);
                }
            }

            try
            {
                var jobServiceResult = await jobService.CompleteJobAsync(new JobModels.CompleteJobQuery(
                    request.OperationId.ToString(),
                    "Operation cancelled by user",
                    true));

                if (jobServiceResult.IsSuccessful)
                {
                    logger.LogInformation("Job service notified about cancellation for operation {OperationId}", request.OperationId);
                }
                else
                {
                    var jobServiceError = $"Failed to cancel operation in job service: {jobServiceResult.ErrorMessage}";
                    logger.LogWarning("Failed to notify job service about cancellation for operation {OperationId}. Error: {ErrorMessage}",
                        request.OperationId, jobServiceResult.ErrorMessage);
                    
                    if (string.IsNullOrEmpty(errorMessage))
                    {
                        errorMessage = jobServiceError;
                    }
                    else
                    {
                        errorMessage = $"{errorMessage}. {jobServiceError}";
                    }
                }
            }
            catch (Exception ex)
            {
                var jobServiceError = $"Error while notifying job service: {ex.Message}";
                logger.LogError(ex, "Error while notifying job service about cancellation for operation {OperationId}", request.OperationId);
                
                if (string.IsNullOrEmpty(errorMessage))
                {
                    errorMessage = jobServiceError;
                }
                else
                {
                    errorMessage = $"{errorMessage}. {jobServiceError}";
                }
            }

            if (isOperationRegistered)
            {
                cancellationService.RemoveOperation(request.OperationId);
            }

            if (!string.IsNullOrEmpty(errorMessage))
            {
                logger.LogWarning("Operation {OperationId} cancellation completed with errors: {ErrorMessage}", request.OperationId, errorMessage);
                return FluentResults.Result.Fail(errorMessage);
            }

            if (!isOperationRegistered && !localCancellationSucceeded)
            {
                logger.LogInformation("Operation {OperationId} cancellation attempted (operation was not found locally, but job service was notified)", request.OperationId);
            }
            else
            {
                logger.LogInformation("Operation {OperationId} cancelled successfully", request.OperationId);
            }

            return FluentResults.Result.Ok(Unit.Value);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error cancelling operation {OperationId}", request.OperationId);
            return FluentResults.Result.Fail($"Failed to cancel operation: {ex.Message}");
        }
    }
}
