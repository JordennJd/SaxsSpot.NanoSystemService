using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;

namespace SaxsSpot.NanoSystemService.Contracts.Models;

public record CommonParticleGenerationParameters(int Count, float? NumericalConcentration, float? GlobalSize, float MinSize, float MaxSize, float Theta, float K, float Excess, float? Epsilon)
    : ParticleGenerationParameters(Count, NumericalConcentration, GlobalSize, MinSize, MaxSize, Theta, K, Excess)
{

    public override ParticleKind GetParticleKind()
    {
        return Epsilon.HasValue ? ParticleKind.Parallelepiped : ParticleKind.Sphere;
    }
}