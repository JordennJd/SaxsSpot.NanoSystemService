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
    public float GlobalSizeFrom { get; set; }

    [Column("global_size_to")]
    public float GlobalSizeTo { get; set; }

    [Column("numerical_concentration_from")]
    public float NumericalConcentrationFrom { get; set; }

    [Column("numerical_concentration_to")]
    public float NumericalConcentrationTo { get; set; }

    [Column("excess_from")]
    public float? ExcessFrom { get; set; }

    [Column("excess_to")]
    public float? ExcessTo { get; set; }

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
}