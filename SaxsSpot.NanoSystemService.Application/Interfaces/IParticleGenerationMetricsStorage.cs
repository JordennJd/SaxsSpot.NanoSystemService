using SaxsSpot.Core.Contracts.Services;
using SaxsSpot.NanoSystemService.Domain;

namespace SaxsSpot.NanoSystemService.Application.Interfaces;

public interface IParticleGenerationMetricsStorage : IGenericStorage<ParticleGenerationMetrics>
{
    Task UpdateOrInsertAsync(IEnumerable<ParticleGenerationMetrics> metrics);
}