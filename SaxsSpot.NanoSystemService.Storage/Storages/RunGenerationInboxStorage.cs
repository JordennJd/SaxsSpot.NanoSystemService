using Microsoft.EntityFrameworkCore;
using System.Data;
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
        var connection = dbContext.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open)
        {
            await connection.OpenAsync(cancellationToken);
        }

        await using var command = connection.CreateCommand();
        command.CommandText = @"
            UPDATE run_generation_inbox
            SET status = @in_progress_status,
                attempts = attempts + 1,
                processing_started_at = @now,
                updated_at = @now,
                last_error = NULL
            WHERE id = (
                SELECT id
                FROM run_generation_inbox
                WHERE status = @pending_status
                ORDER BY created_at
                FOR UPDATE SKIP LOCKED
                LIMIT 1
            )
            RETURNING id, operation_id, series_id, payload, attempts;";

        var now = DateTime.UtcNow;

        var inProgressParam = command.CreateParameter();
        inProgressParam.ParameterName = "in_progress_status";
        inProgressParam.Value = (int)RunGenerationInboxMessageStatus.InProgress;
        command.Parameters.Add(inProgressParam);

        var pendingParam = command.CreateParameter();
        pendingParam.ParameterName = "pending_status";
        pendingParam.Value = (int)RunGenerationInboxMessageStatus.Pending;
        command.Parameters.Add(pendingParam);

        var nowParam = command.CreateParameter();
        nowParam.ParameterName = "now";
        nowParam.Value = now;
        command.Parameters.Add(nowParam);

        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return null;
        }

        return new ClaimedRunGenerationInboxMessage(
            reader.GetGuid(0),
            reader.GetGuid(1),
            reader.GetGuid(2),
            reader.GetString(3),
            reader.GetInt32(4));
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
