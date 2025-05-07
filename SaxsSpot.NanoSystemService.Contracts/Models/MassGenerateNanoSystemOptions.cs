using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;

namespace SaxsSpot.NanoSystemService.Contracts.Models;

public record MassGenerateNanoSystemOptions(
    IList<ParticleGenerationParameters> Options,
    int NanoSystemsCount,
    ParticleKind NanoSystemsKind);