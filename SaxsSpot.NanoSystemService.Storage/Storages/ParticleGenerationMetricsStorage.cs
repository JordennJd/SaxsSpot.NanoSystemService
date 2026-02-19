using Microsoft.EntityFrameworkCore;
using SaxsSpot.Core.Contracts.Services;
using SaxsSpot.Core.GenericStorage.Engine;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Domain;
using SaxsSpot.NanoSystemService.Storage.DbContexts;

namespace SaxsSpot.NanoSystemService.Storage;

public class ParticleGenerationMetricsStorage(ParticleGenerationMetricsDbContext dbContext)
    : GenericStorage<ParticleGenerationMetrics>(dbContext), IParticleGenerationMetricsStorage
{
    public async Task UpdateOrInsertAsync(IEnumerable<ParticleGenerationMetrics> metrics)
    {
        dbContext.AddRange(metrics);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteRangeAsync(IEnumerable<ParticleGenerationMetrics> entities)
    {
        dbContext.Entities.RemoveRange(entities);
        await dbContext.SaveChangesAsync();
    }
}
