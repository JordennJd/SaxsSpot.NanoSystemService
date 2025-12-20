using System.ComponentModel.DataAnnotations.Schema;

namespace SaxsSpot.NanoSystemService.Domain;

[Table("radial_analysis")]
public class RadialAnalysis
{
    [Column("id")]
    public Guid Id { get; set; }
    
    [Column("nanosystem_id")]
    public Guid NanosystemId { get; set; }
    
    [Column("object_id")]
    public Guid ObjectId { get; set; }
    
    [Column("layer_count")]
    public int LayerCount { get; set; }
    
    [Column("point_count")]
    public int PointCount { get; set; }
}