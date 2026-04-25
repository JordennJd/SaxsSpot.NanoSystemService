using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Inbox;

namespace SaxsSpot.NanoSystemService.Application.Interfaces;

public interface IRunGenerationInboxStorage
{
    Task<bool> EnqueueAsync(Guid operationId, Guid seriesId, string payload, CancellationToken cancellationToken);

    Task<ClaimedRunGenerationInboxMessage?> ClaimNextPendingAsync(CancellationToken cancellationToken);

    Task MarkProcessedAsync(Guid id, CancellationToken cancellationToken);

    Task MarkFailedAsync(Guid id, string error, CancellationToken cancellationToken);
}
