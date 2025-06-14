using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemService.Application.Behaviors;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.Get;
using SaxsSpot.NanoSystemService.Contracts.Services;
using SaxsSpot.NanoSystemService.Storage;
using SaxsSpot.NanoSystemService.Storage.Contracts;
using SaxsSpot.NanoSystemService.Storage.DbContexts;

namespace SaxsSpot.NanoSystemService.Application.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var domain = AppDomain.CurrentDomain.GetAssemblies();
        
        return services.AddDbContext<NanoSystemDbContext>()
            .AddDbContext<NanoSystemSeriesDbContext>()
            .AddScoped<INanoSystemService, Services.NanoSystemService>()
            .AddScoped<INanoSystemStorage, NanoSystemStorage>()
            .AddScoped<INanoSystemSeriesStorage, NanoSystemSeriesStorage>()
            .AddScoped<INanoSystemObjectStorage, NanoSystemObjectStorage>()
            .AddLogging(cfg => cfg.AddConsole())
            .AddMediatR(cfg =>
                {
                    cfg.RegisterServicesFromAssemblies(domain);
                    cfg.AddOpenBehavior(typeof(ValidationBehavior<,>));
                })
            .AddAutoMapper(cfg => cfg.AddMaps(domain))
            .AddValidatorsFromAssemblies(domain);
    }
}