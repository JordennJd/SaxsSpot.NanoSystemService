using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemService.Contracts.Services;
using SaxsSpot.NanoSystemService.Application.Services;
using SaxsSpot.NanoSystemService.Contracts.Models;
using SaxsSpot.NanoSystemService.Storage;
using SaxsSpot.NanoSystemService.Storage.Contracts;
using SaxsSpot.NanoSystemService.Storage.DbContexts;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json");
        
        IConfiguration configuration = builder.Build();

        var serviceProvider = new ServiceCollection()
            .AddDbContext<NanoSystemDbContext>()
            .AddDbContext<NanoSystemSeriesDbContext>()
            .AddScoped<INanoSystemService, NanoSystemService>()
            .AddScoped<INanoSystemStorage, NanoSystemStorage>()
            .AddScoped<INanoSystemSeriesStorage, NanoSystemSeriesStorage>()
            .AddScoped<INanoSystemObjectStorage, NanoSystemObjectStorage>()
            .AddScoped<IConfiguration>(_ => configuration)
            .BuildServiceProvider();


        using var scope = serviceProvider.CreateScope();
        var service = scope.ServiceProvider.GetService<INanoSystemService>();

        var options = new MassGenerateNanoSystemOptions([
            new ParallelepipedGenerationParameters(
                1, 10000, 0.2f, null, 1f * (1 / (MathF.PI / 6f)),
                3f * (1 / (MathF.PI / 6f)), 1f, 3, 0),
            new ParallelepipedGenerationParameters(
                1, 10000, 0.2f, null, 1f * (1 / (MathF.PI / 6f)),
                3f * (1 / (MathF.PI / 6f)), 1f, 3, 1.1f)
        ], NanoSystemsKind: ParticleKind.Parallelepiped);
        
        await service.RunSeriesGeneration(options);
    }
}   