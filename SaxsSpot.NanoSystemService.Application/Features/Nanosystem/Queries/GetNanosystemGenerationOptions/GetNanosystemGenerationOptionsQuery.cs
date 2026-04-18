using FluentResults;
using MediatR;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystemGenerationOptions;

public class GetNanosystemGenerationOptionsQuery : IRequest<Result<MassGenerateNanoSystemOptions>>
{
    public int Count { get; set; }
    
    public ParticleKind ParticleKind { get; set; }

    public float? EpsilonFrom { get; set; }

    public float? EpsilonTo { get; set; }
    
    public int ParticleCountFrom { get; set; }

    public int ParticleCountTo { get; set; }

    public float? GlobalSizeFrom { get; set; }

    public float? GlobalSizeTo { get; set; }

    public float? NumericalConcentrationFrom { get; set; }

    public float? NumericalConcentrationTo { get; set; }

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
    
    public int? PointCountFrom { get; set; }
    
    public int? PointCountTo { get; set; }

    /// <summary>
    /// Parallelepiped only: SAT-only pairwise checks against all placed particles (no cheaper intersection shortcuts, no spatial tree).
    /// </summary>
    public bool DisableIntersectionOptimizations { get; set; }
}