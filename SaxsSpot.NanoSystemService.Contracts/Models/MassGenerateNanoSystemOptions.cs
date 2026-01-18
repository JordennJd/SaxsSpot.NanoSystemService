using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;

namespace SaxsSpot.NanoSystemService.Contracts.Models;

public record MassGenerateNanoSystemOptions<TParticle>(
    IList<TParticle> Options,
    ParticleKind NanoSystemsKind) where TParticle : ParticleGenerationParameters;
    
public record MassGenerateNanoSystemOptions(
    IList<CommonParticleGenerationParameters> Options,
    ParticleKind NanoSystemsKind,
    IList<int>? PointCounts = null);