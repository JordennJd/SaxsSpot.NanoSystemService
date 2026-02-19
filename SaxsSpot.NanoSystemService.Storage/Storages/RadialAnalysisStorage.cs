using Gridify;
using Gridify.EntityFramework;
using SaxsSpot.Core.GenericStorage.Engine;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Domain;
using SaxsSpot.NanoSystemService.Storage.DbContexts;

namespace SaxsSpot.NanoSystemService.Storage.Storages;

public class RadialAnalysisStorage(RadialAnalysisDbContext dbContext)
    : GenericStorage<RadialAnalysis>(dbContext), IRadialAnalysisStorage
{
    public Task<Paging<RadialAnalysis>> Gridify(GridifyQuery query)
    {
        return dbContext.Entities.GridifyAsync(query);
    }

    public async Task DeleteRangeAsync(IEnumerable<RadialAnalysis> entities)
    {
        dbContext.Entities.RemoveRange(entities);
        await dbContext.SaveChangesAsync();
    }
}