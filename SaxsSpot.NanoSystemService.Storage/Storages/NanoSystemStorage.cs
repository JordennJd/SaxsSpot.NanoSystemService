using Gridify;
using Gridify.EntityFramework;
using SaxsSpot.Core.GenericStorage.Engine;
using SaxsSpot.NanoSystemService.Domain;
using SaxsSpot.NanoSystemService.Storage.Contracts;
using SaxsSpot.NanoSystemService.Storage.DbContexts;

namespace SaxsSpot.NanoSystemService.Storage;

public class NanoSystemStorage(NanoSystemDbContext dbContext)
    : GenericStorage<Nanosystem>(dbContext), INanoSystemStorage
{
    public Task<Paging<Nanosystem>> Gridify(GridifyQuery query)
    {
        return dbContext.Entities.GridifyAsync(query);
    }
}