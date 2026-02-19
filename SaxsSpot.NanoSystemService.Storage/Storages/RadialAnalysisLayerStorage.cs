using SaxsSpot.Core.GenericStorage.Engine;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Domain;
using SaxsSpot.NanoSystemService.Storage.DbContexts;

namespace SaxsSpot.NanoSystemService.Storage.Storages;

public class RadialAnalysisLayerStorage(RadialAnalysisLayerDbContext dbContext)
    : GenericStorage<RadialAnalysisLayer>(dbContext), IRadialAnalysisLayerStorage
{
    public async Task AddRangeAsync(IEnumerable<RadialAnalysisLayer> layers, CancellationToken cancellationToken = default)
    {
        await dbContext.AddRangeAsync(layers, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteRangeAsync(IEnumerable<RadialAnalysisLayer> entities)
    {
        dbContext.Entities.RemoveRange(entities);
        await dbContext.SaveChangesAsync();
    }
}
