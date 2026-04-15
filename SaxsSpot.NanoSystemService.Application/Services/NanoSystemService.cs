using AutoMapper;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.AnalyzeModels;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemGeneration.Engine.Services;
using RadialAnalysisLayerResult = SaxsSpot.NanoSystemGeneration.Contracts.Models.AnalyzeModels.RadialAnalysisLayerResult;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Services;

using Contracts.Services;
using Domain;

public class NanoSystemService(
    INanoSystemObjectStorage objectStorage,
    INanoSystemStorage storage,
    INanoSystemSeriesStorage seriesStorage,
    IRadialAnalysisLayerStorage radialAnalysisLayerStorage,
    IRadialAnalysisStorage radialAnalysisStorage,
    IGenerationMetricsStorage generationMetricsStorage,
    IParticleGenerationMetricsStorage particleGenerationMetricsStorage,
    IMapper mapper)
    : INanoSystemService
{
    public async Task RunSeriesGeneration(MassGenerateNanoSystemOptions options,
        CancellationToken cancellationToken = default, EventHandler<int>? progressHandler = null)
    {
        var generationParams = options.Options;

        var series = new NanosystemSeries
        {
            Id = Guid.NewGuid(),
            ParticleKind = options.NanoSystemsKind,
            ExcessFrom = generationParams.Min(x => x.Excess),
            ExcessTo = generationParams.Max(x => x.Excess),
            KFrom = generationParams.Min(x => x.K),
            KTo = generationParams.Max(x => x.K),
            ThetaFrom = generationParams.Min(x => x.Theta),
            ThetaTo = generationParams.Max(x => x.Theta),
            CreatedAt = DateTime.UtcNow,
        };
        await seriesStorage.UpdateOrInsertAsync(series);

        var handled = 0;
        foreach (var option in generationParams)
        {
            switch (option.GetParticleKind())
            {
                case ParticleKind.Parallelepiped:
                    await RunGeneration(mapper.Map<ParallelepipedGenerationParameters>(option), seriesId: series.Id, cancellationToken: cancellationToken);
                    break;
                case ParticleKind.Sphere:
                    await RunGeneration(mapper.Map<SphereGenerationParameters>(option), seriesId: series.Id, cancellationToken: cancellationToken);
                    break;
            }
            
            var generatedSystems = (await storage.WhereAsync(x => x.SeriesId == series.Id))
                .ToList();

            if (generationParams.Any())
            {
                series.MinParticleSizeFrom = generatedSystems.Min(x => x.MinParticleSize);
                series.MinParticleSizeTo = generatedSystems.Max(x => x.MinParticleSize);
                series.MaxParticleSizeFrom = generatedSystems.Min(x => x.MaxParticleSize);
                series.MaxParticleSizeTo = generatedSystems.Max(x => x.MaxParticleSize);
                series.GlobalSizeFrom = generatedSystems.Min(x => x.GlobalSize);
                series.GlobalSizeTo = generatedSystems.Max(x => x.GlobalSize);
                series.ParticleCountFrom = generatedSystems.Min(x => x.ParticleCount);
                series.ParticleCountTo = generatedSystems.Max(x => x.ParticleCount);
                series.NumericalConcentrationFrom = generatedSystems.Min(x => x.NumericalConcentration);
                series.NumericalConcentrationTo = generatedSystems.Max(x => x.NumericalConcentration);
            }

            await seriesStorage.UpdateOrInsertAsync(series);
            handled++;
            progressHandler?.Invoke(this, handled);
        }
    }
    
    public async Task RunGeneration(ParticleGenerationParameters options, EventHandler<float>? progressHandler = null,
        CancellationToken cancellationToken = default, Guid seriesId = default, int analysisZoneCount = 20,
        int analysisVectorCount = 5_000_000, Func<Task>? onAnalysisStarted = null, EventHandler<float>? analysisProgressHandler = null,
        bool needMetrics = false)
    {
        var systemObjectGuid = Guid.NewGuid();
        
        var generator = new NanoSystemGenerator(options);
        var generationStartDate = DateTime.Now.ToUniversalTime();
        var system = await generator.GenerateSystem();
        var progress = new Progress<float>();

        if (progressHandler is not null)
        {
            progress.ProgressChanged += progressHandler;
        }
        
        var distributeParticles = await generator.DistributeParticles(progress, cancellationToken);
        var generationZone = await generator.GetGenerationZone();
        var generationEndDate = DateTime.Now.ToUniversalTime();
        var nanosystemId = Guid.NewGuid();
        
        double avgByFiveZone = 0;
        
        // Perform analysis only if zoneCount and pointCount are greater than 0
        if (analysisZoneCount > 0 && analysisVectorCount > 0)
        {
            // Notify that analysis is starting
            if (onAnalysisStarted != null)
            {
                await onAnalysisStarted();
            }

            var analysisProgress = new Progress<float>();
            if (analysisProgressHandler is not null)
            {
                analysisProgress.ProgressChanged += analysisProgressHandler;
            }

            var analysisStartDate = DateTime.Now;
            ICollection<RadialAnalysisLayerResult> analysisLayers = options.GetParticleKind() == ParticleKind.Parallelepiped
                ? NanosystemAnalyzer.GetNanosystemAnalyzeWithLayers(distributeParticles.Select(x => (Parallelepiped)x).ToList(), generationZone, analysisZoneCount, analysisVectorCount, analysisProgress)
                : NanosystemAnalyzer.GetNanosystemAnalyzeWithLayers(distributeParticles.Select(x => (Sphere)x).ToList(), generationZone, analysisZoneCount, analysisVectorCount, analysisProgress);
            var analysisEndDate = DateTime.Now;

            avgByFiveZone = analysisLayers.Take(5).Average(x => x.NumericalConcentration);

            var radialAnalysisId = Guid.NewGuid();
            await radialAnalysisStorage.UpdateOrInsertAsync(new RadialAnalysis()
            {
                Id = radialAnalysisId,
                NanosystemId = nanosystemId,
                ObjectId = Guid.Empty,
                LayerCount = analysisZoneCount,
                PointCount = analysisVectorCount,
                InputDate = DateTime.Now.ToUniversalTime(),
                StartDate = analysisStartDate.ToUniversalTime(),
                EndDate = analysisEndDate.ToUniversalTime(),
            });
            var layerEntities = analysisLayers.Select(l => new RadialAnalysisLayer
            {
                Id = Guid.NewGuid(),
                RadialAnalysisId = radialAnalysisId,
                NanosystemId = nanosystemId,
                LayerIndex = l.ZoneIndex,
                LayerFrom = l.LayerFrom,
                LayerTo = l.LayerTo,
                NumericalConcentration = l.NumericalConcentration,
                PointCount = l.PointCount,
            }).ToList();
            await radialAnalysisLayerStorage.AddRangeAsync(layerEntities);
        }
        else
        {
            avgByFiveZone = distributeParticles.Sum(x => x.GetVolume())/(await generator.GetGenerationZone()).GetVolume();
        }
        
        var entity = new Nanosystem
        {
            Id = nanosystemId,
            ParticleKind = options.GetParticleKind(),
            ParticleCount = distributeParticles.Count,
            NumericalConcentration = avgByFiveZone,
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
            GenerationEnd = generationEndDate,
            InputDate = DateTime.Now.ToUniversalTime()
        };
        
        await storage.UpdateOrInsertAsync(entity);
        await objectStorage.Save(distributeParticles, systemObjectGuid);
        
        // Save particle generation metrics if needed
        if (needMetrics && generator.GenerationInfo != null)
        {
            var particleInfos = generator.GenerationInfo.GetParticleInfos();
            var particleMetrics = particleInfos.Select(particleInfo => new ParticleGenerationMetrics
            {
                Id = Guid.NewGuid(),
                NanosystemId = nanosystemId,
                ParticleIndex = particleInfo.ParticleIndex,
                TotalAttempts = particleInfo.TotalAttempts,
                PositiveAttempts = particleInfo.PositiveAttempts,
                TotalChangePositionAttempts = particleInfo.TotalChangePositionAttempts,
                GenerationTimeMs = (long)particleInfo.GenerationTime.TotalMilliseconds,
                Volume = particleInfo.Volume,
                Diameter = particleInfo.Diameter,
                ParticlesCheckedForIntersection = particleInfo.ParticlesCheckedForIntersection,
                OutOfZoneAttempts = particleInfo.OutOfZoneAttempts,
                FirstNodeIntersectionFindTimes = particleInfo.FirstNodeIntersectionFindTimes,
                TotalNeighborsNodesCheckedCount = particleInfo.TotalNeighborsNodesCheckedCount,
                IsInterCenterDistanceMoreThenDiagonalCheckTimesPositive = particleInfo.IsInterCenterDistanceMoreThenDiagonalCheckTimesPositive,
                IsInterCenterDistanceMoreThenDiagonalCheckTimesTotal = particleInfo.IsInterCenterDistanceMoreThenDiagonalCheckTimesTotal,
                IsInterCenterDistanceLessThenSidesCheckTimesPositive = particleInfo.IsInterCenterDistanceLessThenSidesCheckTimesPositive,
                IsInterCenterDistanceLessThenSidesCheckTimesTotal = particleInfo.IsInterCenterDistanceLessThenSidesCheckTimesTotal,
                ElementaryIntersectCheckOnlyBordersNewTransformationTimesPositive = particleInfo.ElementaryIntersectCheckOnlyBordersNewTransformationTimesPositive,
                ElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal = particleInfo.ElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal,
                ElementaryIntersectCheckOnlyBordersOldTransformationTimesPositive = particleInfo.ElementaryIntersectCheckOnlyBordersOldTransformationTimesPositive,
                ElementaryIntersectCheckOnlyBordersOldTransformationTimesTotal = particleInfo.ElementaryIntersectCheckOnlyBordersOldTransformationTimesTotal,
                BackRotateMatrixReused = particleInfo.BackRotateMatrixReused,
                SATCheckTimesPositive = particleInfo.SATCheckTimesPositive,
                SATCheckTimesTotal = particleInfo.SATCheckTimesTotal,
                InputDate = DateTime.Now.ToUniversalTime()
            }).ToList();
            
            await particleGenerationMetricsStorage.UpdateOrInsertAsync(particleMetrics);
        }
        
        if (seriesId != default)
        {
            var existingSeriesList = (await seriesStorage.WhereAsync(x => x.Id == seriesId)).ToList();
            var existingSeries = existingSeriesList.FirstOrDefault();
            
            if (existingSeries == null)
            {
                // Create new series with initial parameters from current generation
                var newSeries = new NanosystemSeries
                {
                    Id = seriesId,
                    ParticleKind = options.GetParticleKind(),
                    ExcessFrom = options.Excess,
                    ExcessTo = options.Excess,
                    KFrom = options.K,
                    KTo = options.K,
                    ThetaFrom = options.Theta,
                    ThetaTo = options.Theta,
                    CreatedAt = DateTime.UtcNow,
                };
                await seriesStorage.UpdateOrInsertAsync(newSeries);
                existingSeries = newSeries;
            }
            
            var generatedSystems = (await storage.WhereAsync(x => x.SeriesId == seriesId)).ToList();
            if (generatedSystems.Any())
            {
                existingSeries.MinParticleSizeFrom = generatedSystems.Min(x => x.MinParticleSize);
                existingSeries.MinParticleSizeTo = generatedSystems.Max(x => x.MinParticleSize);
                existingSeries.MaxParticleSizeFrom = generatedSystems.Min(x => x.MaxParticleSize);
                existingSeries.MaxParticleSizeTo = generatedSystems.Max(x => x.MaxParticleSize);
                existingSeries.GlobalSizeFrom = generatedSystems.Min(x => x.GlobalSize);
                existingSeries.GlobalSizeTo = generatedSystems.Max(x => x.GlobalSize);
                existingSeries.ParticleCountFrom = generatedSystems.Min(x => x.ParticleCount);
                existingSeries.ParticleCountTo = generatedSystems.Max(x => x.ParticleCount);
                existingSeries.NumericalConcentrationFrom = generatedSystems.Min(x => x.NumericalConcentration);
                existingSeries.ExcessFrom = generatedSystems.Min(x => x.Excess);
                existingSeries.ExcessTo = generatedSystems.Max(x => x.Excess);
                existingSeries.NumericalConcentrationTo = generatedSystems.Max(x => x.NumericalConcentration);
                await seriesStorage.UpdateOrInsertAsync(existingSeries);
            }
        }
    }
}