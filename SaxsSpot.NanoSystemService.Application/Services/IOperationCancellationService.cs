namespace SaxsSpot.NanoSystemService.Application.Services;

public interface IOperationCancellationService
{
    void RegisterOperation(Guid operationId, CancellationTokenSource cancellationTokenSource, string? operationType = null);
    
    bool TryGetCancellationTokenSource(Guid operationId, out CancellationTokenSource? cancellationTokenSource);
    
    bool TryGetOperationType(Guid operationId, out string? operationType);
    
    bool CancelOperation(Guid operationId);
    
    void RemoveOperation(Guid operationId);
    
    bool IsOperationRegistered(Guid operationId);
}
