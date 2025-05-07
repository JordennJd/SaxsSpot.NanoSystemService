using SaxsSpot.Core.GenericStorage.Engine;
using SaxsSpot.NanoSystemService.Entities;
using SaxsSpot.NanoSystemService.Storage.Contracts;
using SaxsSpot.NanoSystemService.Storage.DbContexts;

namespace SaxsSpot.NanoSystemService.Storage;

public class NanoSystemStorage : GenericStorage<Nanosystem>, INanoSystemStorage
{
    public NanoSystemStorage(NanoSystemDbContext dbContext) : base(dbContext)
    {
    }
}