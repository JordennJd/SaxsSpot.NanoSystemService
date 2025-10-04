using Gridify;
using Gridify.EntityFramework;
using Microsoft.EntityFrameworkCore;
using SaxsSpot.Core.GenericStorage.Engine;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Domain;
using SaxsSpot.NanoSystemService.Storage.DbContexts;

namespace SaxsSpot.NanoSystemService.Storage;

public class NanoSystemStorage(NanoSystemDbContext dbContext)
    : GenericStorage<Nanosystem>(dbContext), INanoSystemStorage
{
    public Task<Paging<Nanosystem>> Gridify(GridifyQuery query)
    {
        return dbContext.Entities.GridifyAsync(query);
    }
    
    public async Task<IEnumerable<Nanosystem>> WhereByGridifyStringAsync(string filter)
    {
        return await dbContext.Entities
            .ApplyFiltering(filter)
            .ToListAsync();
    }
}