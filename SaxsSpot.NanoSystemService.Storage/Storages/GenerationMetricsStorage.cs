using Microsoft.EntityFrameworkCore;
using SaxsSpot.Core.GenericStorage.Engine;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Domain;
using SaxsSpot.NanoSystemService.Storage.DbContexts;

namespace SaxsSpot.NanoSystemService.Storage;

public class GenerationMetricsStorage(
    GenerationMetricsDbContext dbContext,
    NanoSystemDbContext nanoSystemDbContext)
    : GenericStorage<GenerationMetrics>(dbContext), IGenerationMetricsStorage
{
    public async Task DeleteRangeAsync(IEnumerable<GenerationMetrics> entities)
    {
        dbContext.Entities.RemoveRange(entities);
        await dbContext.SaveChangesAsync();
    }
}