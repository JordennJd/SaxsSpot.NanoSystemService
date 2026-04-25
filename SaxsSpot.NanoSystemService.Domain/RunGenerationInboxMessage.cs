using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaxsSpot.NanoSystemService.Domain;

[Table("run_generation_inbox")]
public class RunGenerationInboxMessage
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("operation_id")]
    public Guid OperationId { get; set; }

    [Column("series_id")]
    public Guid SeriesId { get; set; }

    [Column("payload")]
    public string Payload { get; set; } = string.Empty;

    [Column("status")]
    public RunGenerationInboxMessageStatus Status { get; set; }

    [Column("attempts")]
    public int Attempts { get; set; }

    [Column("last_error")]
    public string? LastError { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    [Column("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [Column("processing_started_at")]
    public DateTime? ProcessingStartedAt { get; set; }

    [Column("processed_at")]
    public DateTime? ProcessedAt { get; set; }
}
