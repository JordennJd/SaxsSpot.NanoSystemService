using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;

namespace SaxsSpot.NanoSystemService.Domain;

[Table("nanosystem_series")]
public class NanosystemSeries
{
    [Key]
    [Column("id")]
    public Guid Id { get; set; }

    [Column("particle_kind")]
    public ParticleKind ParticleKind { get; set; }

    [Column("particle_count_from")]
    public int ParticleCountFrom { get; set; }

    [Column("particle_count_to")]
    public int ParticleCountTo { get; set; }

    [Column("global_size_from")]
    public double GlobalSizeFrom { get; set; }

    [Column("global_size_to")]
    public double GlobalSizeTo { get; set; }

    [Column("numerical_concentration_from")]
    public double NumericalConcentrationFrom { get; set; }

    [Column("numerical_concentration_to")]
    public double NumericalConcentrationTo { get; set; }

    [Column("excess_from")]
    public double? ExcessFrom { get; set; }

    [Column("excess_to")]
    public double? ExcessTo { get; set; }

    [Column("max_particle_size_from")]
    public float MaxParticleSizeFrom { get; set; }

    [Column("max_particle_size_to")]
    public float MaxParticleSizeTo { get; set; }

    [Column("min_particle_size_from")]
    public float MinParticleSizeFrom { get; set; }

    [Column("min_particle_size_to")]
    public float MinParticleSizeTo { get; set; }

    [Column("k_from")]
    public float KFrom { get; set; }

    [Column("k_to")]
    public float KTo { get; set; }

    [Column("theta_from")]
    public float ThetaFrom { get; set; }

    [Column("theta_to")]
    public float ThetaTo { get; set; }

    [Column("series_comment")]
    public string? Comment { get; set; }

    [Column("created_at")]
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// True if generation used SAT-only intersection checks (no spatial tree / cheap filters).
    /// </summary>
    [Column("disable_intersection_optimizations")]
    public bool DisableIntersectionOptimizations { get; set; }
}