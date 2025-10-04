using Microsoft.Extensions.DependencyInjection;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Storage.DbContexts;
using SaxsSpot.NanoSystemService.Storage.Storages;

namespace SaxsSpot.NanoSystemService.Storage.Extensions;

public static class DependencyInjections
{
    public static IServiceCollection AddNanoSystemServiceStorage(this IServiceCollection services)
    {
     
        return services.AddDbContext<NanoSystemDbContext>()
            .AddDbContext<NanoSystemSeriesDbContext>()
            .AddScoped<INanoSystemStorage, NanoSystemStorage>()
            .AddScoped<INanoSystemSeriesStorage, NanoSystemSeriesStorage>()
            .AddScoped<INanoSystemObjectStorage, NanoSystemObjectStorage>();
    }
}