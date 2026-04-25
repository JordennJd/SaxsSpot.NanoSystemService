namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Inbox;

public record ClaimedRunGenerationInboxMessage(
    Guid Id,
    Guid OperationId,
    Guid SeriesId,
    string Payload,
    int Attempts);
