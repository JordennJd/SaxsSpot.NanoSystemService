using SaxsSpot.NanoSystemService.Contracts.Enums;

namespace SaxsSpot.NanoSystemService.Contracts.Models;

public record ScatteringCalculationDto
{
    public Guid Id { get; set; }

    public Guid NanosystemId { get; set; }

    public Guid ObjectId { get; set; }

    public ScatteringCalculationKind CalculationKind { get; set; }

    public double QVectorFrom { get; set; }

    public double QVectorTo { get; set; }

    public int QSpaceMethod { get; set; }

    public int QScaleMethod { get; set; }

    public double QSpaceParameter { get; set; }

    public double? Excess { get; set; }

    public DateTime InputDate { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }
}
