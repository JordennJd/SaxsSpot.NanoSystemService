using System.ComponentModel.DataAnnotations.Schema;

namespace SaxsSpot.NanoSystemService.Domain;

[Table("radial_analysis_layer")]
public class RadialAnalysisLayer
{
    [Column("id")]
    public Guid Id { get; set; }

    [Column("radial_analysis_id")]
    public Guid RadialAnalysisId { get; set; }

    [Column("nanosystem_id")]
    public Guid NanosystemId { get; set; }

    [Column("layer_index")]
    public int LayerIndex { get; set; }

    [Column("layer_from")]
    public double LayerFrom { get; set; }

    [Column("layer_to")]
    public double LayerTo { get; set; }

    [Column("numerical_concentration")]
    public double NumericalConcentration { get; set; }

    [Column("point_count")]
    public int PointCount { get; set; }
}
