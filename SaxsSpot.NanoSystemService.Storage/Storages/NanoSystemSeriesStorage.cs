using Gridify;
using Gridify.EntityFramework;
using SaxsSpot.Core.GenericStorage.Engine;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Domain;
using SaxsSpot.NanoSystemService.Storage.DbContexts;

namespace SaxsSpot.NanoSystemService.Storage;

public class NanoSystemSeriesStorage(NanoSystemSeriesDbContext dbContext)
    : GenericStorage<NanosystemSeries>(dbContext), INanoSystemSeriesStorage
{
    private static readonly NanosystemSeriesGridifyMapper GridifyMapper = new();

    public Task<Paging<NanosystemSeries>> Gridify(GridifyQuery query)
    {
        return dbContext.Entities.GridifyAsync(query, GridifyMapper);
    }

    public async Task DeleteAsync(NanosystemSeries entity)
    {
        dbContext.Entities.Remove(entity);
        await dbContext.SaveChangesAsync();
    }
}