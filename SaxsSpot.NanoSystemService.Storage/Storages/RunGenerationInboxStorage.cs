using Microsoft.EntityFrameworkCore;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Inbox;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Domain;
using SaxsSpot.NanoSystemService.Storage.DbContexts;

namespace SaxsSpot.NanoSystemService.Storage.Storages;

public class RunGenerationInboxStorage(RunGenerationInboxDbContext dbContext) : IRunGenerationInboxStorage
{
    public async Task<bool> EnqueueAsync(Guid operationId, Guid seriesId, string payload, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var entity = new RunGenerationInboxMessage
        {
            Id = Guid.NewGuid(),
            OperationId = operationId,
            SeriesId = seriesId,
            Payload = payload,
            Status = RunGenerationInboxMessageStatus.Pending,
            Attempts = 0,
            CreatedAt = now,
            UpdatedAt = now
        };

        dbContext.Entities.Add(entity);
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException)
        {
            // operation_id is unique; duplicate message should be treated as already enqueued.
            return false;
        }
    }

    public async Task<ClaimedRunGenerationInboxMessage?> ClaimNextPendingAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        var claimed = await dbContext.Entities
            .FromSqlInterpolated($@"
                UPDATE run_generation_inbox
                SET status = {(int)RunGenerationInboxMessageStatus.InProgress},
                    attempts = attempts + 1,
                    processing_started_at = {now},
                    updated_at = {now},
                    last_error = NULL
                WHERE id = (
                    SELECT id
                    FROM run_generation_inbox
                    WHERE status = {(int)RunGenerationInboxMessageStatus.Pending}
                    ORDER BY created_at
                    FOR UPDATE SKIP LOCKED
                    LIMIT 1
                )
                RETURNING *")
            .AsNoTracking()
            .FirstOrDefaultAsync(cancellationToken);

        if (claimed is null)
        {
            return null;
        }

        return new ClaimedRunGenerationInboxMessage(
            claimed.Id,
            claimed.OperationId,
            claimed.SeriesId,
            claimed.Payload,
            claimed.Attempts);
    }

    public async Task MarkProcessedAsync(Guid id, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        await dbContext.Entities
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(x => x.Status, RunGenerationInboxMessageStatus.Processed)
                    .SetProperty(x => x.ProcessedAt, now)
                    .SetProperty(x => x.UpdatedAt, now)
                    .SetProperty(x => x.LastError, (string?)null),
                cancellationToken);
    }

    public async Task MarkFailedAsync(Guid id, string error, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        await dbContext.Entities
            .Where(x => x.Id == id)
            .ExecuteUpdateAsync(
                setters => setters
                    .SetProperty(x => x.Status, RunGenerationInboxMessageStatus.Failed)
                    .SetProperty(x => x.UpdatedAt, now)
                    .SetProperty(x => x.LastError, error),
                cancellationToken);
    }
}
