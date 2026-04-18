using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;

namespace SaxsSpot.NanoSystemService.Contracts.Models;

public record CommonParticleGenerationParameters(int Count, double? NumericalConcentration, double? GlobalSize, float MinSize, float MaxSize, float Theta, float K, double Excess, float? Epsilon, bool DisableIntersectionOptimizations = false)
    : ParticleGenerationParameters(Count, NumericalConcentration, GlobalSize, MinSize, MaxSize, Theta, K, Excess)
{

    public override ParticleKind GetParticleKind()
    {
        return Epsilon.HasValue ? ParticleKind.Parallelepiped : ParticleKind.Sphere;
    }
}