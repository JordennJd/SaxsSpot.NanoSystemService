using Gridify;
using Gridify.EntityFramework;
using SaxsSpot.Core.GenericStorage.Engine;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Domain;
using SaxsSpot.NanoSystemService.Storage.DbContexts;

namespace SaxsSpot.NanoSystemService.Storage.Storages;

public class ScatteringCalculationStorage(ScatteringCalculationDbContext dbContext)
    : GenericStorage<ScatteringCalculation>(dbContext), IScatteringCalculationStorage
{
    public Task<Paging<ScatteringCalculation>> Gridify(GridifyQuery query)
    {
        return dbContext.Entities.GridifyAsync(query);
    }

    public async Task DeleteRangeAsync(IEnumerable<ScatteringCalculation> entities)
    {
        dbContext.Entities.RemoveRange(entities);
        await dbContext.SaveChangesAsync();
    }
}
