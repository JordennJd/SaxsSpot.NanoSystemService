using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemGeneration.Engine.Services;

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
    
    public async Task RunGeneration(ParticleGenerationParameters options)
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
            Id = 0,
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
            SeriesId = 0,
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