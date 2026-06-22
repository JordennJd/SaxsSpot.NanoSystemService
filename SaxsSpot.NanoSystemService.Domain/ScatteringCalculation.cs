using System.ComponentModel.DataAnnotations.Schema;

namespace SaxsSpot.NanoSystemService.Domain;

[Table("scattering_calculation")]
public class ScatteringCalculation
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("nanosystem_id")]
    public Guid NanosystemId { get; set; }

    [Column("object_id")]
    public Guid ObjectId { get; set; }

    [Column("calculation_kind")]
    public int CalculationKind { get; set; }

    [Column("q_vector_from")]
    public double QVectorFrom { get; set; }

    [Column("q_vector_to")]
    public double QVectorTo { get; set; }

    [Column("q_space_method")]
    public int QSpaceMethod { get; set; }

    [Column("q_scale_method")]
    public int QScaleMethod { get; set; }

    [Column("q_space_parameter")]
    public double QSpaceParameter { get; set; }

    [Column("excess")]
    public double? Excess { get; set; }

    [Column("input_date")]
    public DateTime InputDate { get; set; }

    [Column("start_date")]
    public DateTime StartDate { get; set; }

    [Column("end_date")]
    public DateTime EndDate { get; set; }
}
