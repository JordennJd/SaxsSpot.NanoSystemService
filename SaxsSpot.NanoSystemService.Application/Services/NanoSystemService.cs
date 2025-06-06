using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemGeneration.Engine.Services;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Services;

using Contracts.Services;
using Domain;
using Storage.Contracts;

public class NanoSystemService : INanoSystemService
{
    private readonly INanoSystemStorage _storage;
    private readonly INanoSystemObjectStorage _objectStorage;

    public NanoSystemService(INanoSystemObjectStorage objectStorage, INanoSystemStorage storage)
    {
        _objectStorage = objectStorage;
        _storage = storage;
    }

    public async Task RunSeriesGeneration(MassGenerateNanoSystemOptions options)
    {
        var generationParams = options.Options;

        var series = new NanosystemSeries
        {
            ParticleKind = options.NanoSystemsKind,
            ExcessFrom = generationParams.Min(x => x.Excess),
            ExcessTo = generationParams.Max(x => x.Excess),
            KFrom = generationParams.Min(x => x.K),
            KTo = generationParams.Max(x => x.K),
            ThetaFrom = generationParams.Min(x => x.Theta),
            ThetaTo = generationParams.Max(x => x.Theta),
        };

        foreach (var option in generationParams)
        {
            await RunGeneration(option, series.Id);
        }

        var generatedSystems = (await _storage.WhereAsync(x => x.SeriesId == series.Id))
            .ToList();

        if (generationParams?.Any() is true)
        {
            series.MinParticleSizeFrom = generatedSystems.Min(x => x.MinParticleSize);
            series.MinParticleSizeTo = generatedSystems.Max(x => x.MinParticleSize);
            series.GlobalSizeFrom = generatedSystems.Min(x => x.GlobalSize);
            series.GlobalSizeTo = generatedSystems.Max(x => x.GlobalSize);
            series.ParticleCountFrom = generatedSystems.Min(x => x.ParticleCount);
            series.ParticleCountTo = generatedSystems.Max(x => x.ParticleCount);
            series.NumericalConcentrationFrom = generatedSystems.Min(x => x.NumericalConcentration);
            series.NumericalConcentrationTo = generatedSystems.Max(x => x.NumericalConcentration);
        }
    }
    
    public async Task RunGeneration(ParticleGenerationParameters options, Guid seriesId = default)
    {
        var generationStartDate = DateTime.Now.ToUniversalTime();
        var systemObjectGuid = Guid.NewGuid();
        
        var generator = new NanoSystemGenerator(options);
        var system = await generator.GenerateSystem();
        var sumOfVolumes = system.Sum(x => x.GetVolume());
        var progress = new Progress<float>();
        progress.ProgressChanged += ProgressHandler;
        var distributeParticles = await generator?.DistributeParticles(progress, CancellationToken.None);
        
        var generationZone = await generator.GetGenerationZone();
        var entity = new Nanosystem()
        {
            ParticleKind = options.GetParticleKind(),
            ParticleCount = distributeParticles.Count,
            NumericalConcentration = distributeParticles.Sum(x => x.GetVolume()) / generationZone.GetVolume(),
            GlobalSize = generationZone.GlobalSize,
            GenerationZoneForm = generationZone.GenerationZoneForm,
            GenerationZoneVolume = generationZone.GetVolume(),
            K = options.K,
            Theta = options.Theta,
            GenerationStart = generationStartDate,
            Excess = options.Excess,
            SeriesId = seriesId,
            UserId = 0,
            MinParticleSize = system.MinBy(x => x.GetParticleSize())!.GetParticleSize(),
            MaxParticleSize = system.MaxBy(x => x.GetParticleSize())!.GetParticleSize(),
            ObjectId = systemObjectGuid,
            GenerationEnd = DateTime.Now.ToUniversalTime(),
        };
        
        await _storage.UpdateOrInsertAsync(entity);
        await _objectStorage.Save(distributeParticles, systemObjectGuid);
    }

    private static void ProgressHandler<TEventArgs>(object? sender, TEventArgs e)
    {
        if (e is float progress)
        {
            Console.WriteLine($"Progress: {progress}");
        }
    }
}