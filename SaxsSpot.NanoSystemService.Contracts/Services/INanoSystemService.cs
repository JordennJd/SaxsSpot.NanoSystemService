using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Contracts.Services;

public interface INanoSystemService
{
    Task RunGeneration(ParticleGenerationParameters options);
}