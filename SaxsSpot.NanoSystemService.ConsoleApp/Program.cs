using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemService.Application.Extensions;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunGeneration;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.Get;
using SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetNanosystemGenerationOptions;
using SaxsSpot.NanoSystemService.Contracts.Services;
using SaxsSpot.NanoSystemService.Application.Services;
using SaxsSpot.NanoSystemService.Storage;
using SaxsSpot.NanoSystemService.Storage.DbContexts;
using Timer = System.Timers.Timer;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = new ConfigurationBuilder()
            .AddJsonFile("appsettings.json");
        
        IConfiguration configuration = builder.Build();

        var serviceProvider = new ServiceCollection()
            .AddScoped((_) => configuration)
            .AddApplication(configuration)
            .BuildServiceProvider();
        
        using var scope = serviceProvider.CreateScope();
        var service = scope.ServiceProvider.GetService<INanoSystemService>();
        
        var mediator = scope.ServiceProvider.GetService<IMediator>();
        // var result = mediator.Send(new GetNanosystemQuery(Guid.Parse("01974708-3f83-731d-883c-c0b5377542af")));
        var result = await mediator.Send(new GetNanosystemGenerationOptionsQuery
        {
            Count = 10,
            ParticleKind = ParticleKind.Parallelepiped,
            GlobalSizeFrom = null,
            GlobalSizeTo = null,
            NumericalConcentrationFrom = 0.2f,
            NumericalConcentrationTo = 0.2f,
            EpsilonFrom = 1,
            EpsilonTo = 1,
            ParticleCountFrom = 10000,
            ParticleCountTo = 10000,
            ExcessFrom = 1,
            ExcessTo = 1.2f,
            KFrom = 3,
            KTo = 3,
            ThetaFrom = 1,
            ThetaTo = 1,
            MinParticleSizeFrom = 1,
            MinParticleSizeTo = 1,
            MaxParticleSizeFrom = 3,
            MaxParticleSizeTo = 3,
        });

        var cts = new CancellationTokenSource();
        
        await mediator.Send(new RunGenerationCommand(result.Value.Options[0], Guid.NewGuid()), cts.Token);
    }
}   