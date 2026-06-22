using Gridify;
using Microsoft.Extensions.DependencyInjection;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Storage.DbContexts;
using SaxsSpot.NanoSystemService.Storage.Storages;

namespace SaxsSpot.NanoSystemService.Storage.Extensions;

public static class DependencyInjections
{
    public static IServiceCollection AddNanoSystemServiceStorage(this IServiceCollection services)
    {
        // Npgsql + timestamptz: filter parameters must be UTC, not Unspecified (Gridify default for parsed dates).
        GridifyGlobalConfiguration.DefaultDateTimeKind = DateTimeKind.Utc;

        return services.AddDbContext<NanoSystemDbContext>()
                .AddDbContext<NanoSystemSeriesDbContext>()
                .AddDbContext<RadialAnalysisDbContext>()
                .AddDbContext<RadialAnalysisLayerDbContext>()
                .AddDbContext<GenerationMetricsDbContext>()
                .AddDbContext<RunGenerationInboxDbContext>()
                .AddDbContext<ParticleGenerationMetricsDbContext>()
                .AddDbContext<ScatteringCalculationDbContext>()
                .AddScoped<INanoSystemStorage, NanoSystemStorage>()
                .AddScoped<INanoSystemSeriesStorage, NanoSystemSeriesStorage>()
                .AddScoped<INanoSystemObjectStorage, NanoSystemObjectStorage>()
                .AddScoped<IRadialAnalysisStorage, RadialAnalysisStorage>()
                .AddScoped<IRadialAnalysisLayerStorage, RadialAnalysisLayerStorage>()
                .AddScoped<IGenerationMetricsStorage, GenerationMetricsStorage>()
                .AddScoped<IRunGenerationInboxStorage, RunGenerationInboxStorage>()
                .AddScoped<IParticleGenerationMetricsStorage, ParticleGenerationMetricsStorage>()
                .AddScoped<IScatteringCalculationStorage, ScatteringCalculationStorage>()
                .AddScoped<IScatteringResultObjectStorage, ScatteringResultObjectStorage>()
            ;
    }
}