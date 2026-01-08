using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Contracts.Services;

public interface INanoSystemService
{
    Task RunGeneration(ParticleGenerationParameters options, EventHandler<float>? progressHandler = null,
        CancellationToken cancellationToken = default, Guid seriesId = default, int analysisZoneCount = 20, int analysisVectorCount = 5_000_000);

    Task RunSeriesGeneration(MassGenerateNanoSystemOptions options, CancellationToken cancellationToken = default,
        EventHandler<int>? progressHandler = null);
}