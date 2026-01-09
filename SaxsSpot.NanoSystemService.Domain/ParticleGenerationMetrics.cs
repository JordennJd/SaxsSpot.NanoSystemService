using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SaxsSpot.NanoSystemService.Domain;

[Table("particle_generation_metrics")]
public class ParticleGenerationMetrics
{
    [Column("id")] 
    [Key]
    public Guid Id { get; set; }
    
    [Column("nanosystem_id")]
    public Guid NanosystemId { get; set; }
    
    [Column("particle_index")]
    public int ParticleIndex { get; set; }
    
    // Particle-specific metrics
    [Column("total_attempts")]
    public int TotalAttempts { get; set; }
    
    [Column("positive_attempts")]
    public int PositiveAttempts { get; set; }
    
    [Column("total_change_position_attempts")]
    public int TotalChangePositionAttempts { get; set; }
    
    [Column("generation_time_ms")]
    public long GenerationTimeMs { get; set; }
    
    [Column("volume")]
    public double Volume { get; set; }
    
    [Column("diameter")]
    public float Diameter { get; set; }
    
    [Column("particles_checked_for_intersection")]
    public int ParticlesCheckedForIntersection { get; set; }
    
    [Column("out_of_zone_attempts")]
    public int OutOfZoneAttempts { get; set; }
    
    // Intersection detection metrics
    [Column("first_node_intersection_find_times")]
    public int FirstNodeIntersectionFindTimes { get; set; }
    
    [Column("total_neighbors_nodes_checked_count")]
    public int TotalNeighborsNodesCheckedCount { get; set; }
    
    // Distance check metrics
    [Column("is_inter_center_distance_more_then_diagonal_check_times_positive")]
    public int IsInterCenterDistanceMoreThenDiagonalCheckTimesPositive { get; set; }
    
    [Column("is_inter_center_distance_more_then_diagonal_check_times_total")]
    public int IsInterCenterDistanceMoreThenDiagonalCheckTimesTotal { get; set; }
    
    [Column("is_inter_center_distance_less_then_sides_check_times_positive")]
    public int IsInterCenterDistanceLessThenSidesCheckTimesPositive { get; set; }
    
    [Column("is_inter_center_distance_less_then_sides_check_times_total")]
    public int IsInterCenterDistanceLessThenSidesCheckTimesTotal { get; set; }
    
    // Elementary intersection check metrics
    [Column("elementary_intersect_check_only_borders_new_transformation_times_positive")]
    public int ElementaryIntersectCheckOnlyBordersNewTransformationTimesPositive { get; set; }
    
    [Column("elementary_intersect_check_only_borders_new_transformation_times_total")]
    public int ElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal { get; set; }
    
    [Column("elementary_intersect_check_only_borders_old_transformation_times_positive")]
    public int ElementaryIntersectCheckOnlyBordersOldTransformationTimesPositive { get; set; }
    
    [Column("elementary_intersect_check_only_borders_old_transformation_times_total")]
    public int ElementaryIntersectCheckOnlyBordersOldTransformationTimesTotal { get; set; }
    
    [Column("back_rotate_matrix_reused")]
    public int BackRotateMatrixReused { get; set; }
    
    // SAT check metrics
    [Column("sat_check_times_positive")]
    public int SATCheckTimesPositive { get; set; }
    
    [Column("sat_check_times_total")]
    public int SATCheckTimesTotal { get; set; }
    
    [Column("input_date")]
    public DateTime InputDate { get; set; }
}
