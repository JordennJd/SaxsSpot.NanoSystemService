using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemService.Application.Behaviors;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Application.Services;
using SaxsSpot.NanoSystemService.Contracts.Services;

namespace SaxsSpot.NanoSystemService.Application.Extensions;

public static class ServiceCollectionExtension
{
    public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
    {
        var domain = AppDomain.CurrentDomain.GetAssemblies();
        
        return services
            .AddSingleton<IOperationCancellationService, OperationCancellationService>()
            .AddScoped<INanoSystemService, Services.NanoSystemService>()
            .AddScoped<IChartService, ChartService>()
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