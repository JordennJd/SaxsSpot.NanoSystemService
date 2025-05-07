using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using SaxsSpot.Core.GenericStorage.Engine;
using SaxsSpot.NanoSystemService.Entities;

namespace SaxsSpot.NanoSystemService.Storage.DbContexts;

public class NanoSystemDbContext : GenericDbContext<Nanosystem>
{
    public NanoSystemDbContext(IConfiguration configuration) : base(configuration)
    {
    }
}