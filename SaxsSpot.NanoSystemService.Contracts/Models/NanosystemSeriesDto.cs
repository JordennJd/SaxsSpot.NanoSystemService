using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;

namespace SaxsSpot.NanoSystemService.Contracts.Models;

public record NanosystemSeriesDto
{
    public Guid Id { get; set; }

    public ParticleKind ParticleKind { get; set; }

    public int ParticleCountFrom { get; set; }

    public int ParticleCountTo { get; set; }

    public float GlobalSizeFrom { get; set; }

    public float GlobalSizeTo { get; set; }

    public float NumericalConcentrationFrom { get; set; }

    public float NumericalConcentrationTo { get; set; }

    public float? ExcessFrom { get; set; }

    public float? ExcessTo { get; set; }

    public float MaxParticleSizeFrom { get; set; }

    public float MaxParticleSizeTo { get; set; }

    public float MinParticleSizeFrom { get; set; }

    public float MinParticleSizeTo { get; set; }

    public float KFrom { get; set; }

    public float KTo { get; set; }

    public float ThetaFrom { get; set; }

    public float ThetaTo { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedAt { get; set; }
}