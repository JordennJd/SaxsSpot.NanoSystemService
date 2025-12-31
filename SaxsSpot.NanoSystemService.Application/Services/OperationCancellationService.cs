using System.Collections.Concurrent;
using Microsoft.Extensions.Logging;

namespace SaxsSpot.NanoSystemService.Application.Services;

public class OperationCancellationService(ILogger<OperationCancellationService> logger) : IOperationCancellationService
{
    private readonly ConcurrentDictionary<Guid, CancellationTokenSource> _operations = new();
    private readonly ConcurrentDictionary<Guid, string> _operationTypes = new();
    private readonly ILogger<OperationCancellationService> _logger = logger;

    public void RegisterOperation(Guid operationId, CancellationTokenSource cancellationTokenSource, string? operationType = null)
    {
        if (_operations.TryAdd(operationId, cancellationTokenSource))
        {
            if (!string.IsNullOrEmpty(operationType))
            {
                _operationTypes.TryAdd(operationId, operationType);
            }
            _logger.LogDebug("Operation {OperationId} registered for cancellation with type {OperationType}", operationId, operationType ?? "unknown");
        }
        else
        {
            _logger.LogWarning("Operation {OperationId} already registered", operationId);
        }
    }

    public bool TryGetCancellationTokenSource(Guid operationId, out CancellationTokenSource? cancellationTokenSource)
    {
        return _operations.TryGetValue(operationId, out cancellationTokenSource);
    }

    public bool CancelOperation(Guid operationId)
    {
        if (!_operations.TryGetValue(operationId, out var cts))
        {
            _logger.LogWarning("Operation {OperationId} not found for cancellation", operationId);
            return false;
        }

        try
        {
            cts.Cancel();
            _logger.LogInformation("Operation {OperationId} cancelled", operationId);
            return true;
        }
        catch (ObjectDisposedException)
        {
            _logger.LogWarning("CancellationTokenSource for operation {OperationId} already disposed", operationId);
            _operations.TryRemove(operationId, out _);
            return false;
        }
    }

    public bool TryGetOperationType(Guid operationId, out string? operationType)
    {
        return _operationTypes.TryGetValue(operationId, out operationType);
    }

    public void RemoveOperation(Guid operationId)
    {
        if (_operations.TryRemove(operationId, out var cts))
        {
            try
            {
                cts.Dispose();
                _logger.LogDebug("Operation {OperationId} removed and disposed", operationId);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Error disposing CancellationTokenSource for operation {OperationId}", operationId);
            }
        }
        
        _operationTypes.TryRemove(operationId, out _);
    }

    public bool IsOperationRegistered(Guid operationId)
    {
        return _operations.ContainsKey(operationId);
    }
}
