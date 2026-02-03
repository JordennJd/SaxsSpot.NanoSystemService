using System.Text.Json;
using FluentResults;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.AnalyzeModels;
using RadialAnalysisLayerResult = SaxsSpot.NanoSystemGeneration.Contracts.Models.AnalyzeModels.RadialAnalysisLayerResult;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationZones;
using SaxsSpot.NanoSystemGeneration.Engine.Services;
using SaxsSpot.NanoSystemService.Application.Interfaces;
using SaxsSpot.NanoSystemService.Contracts.Services;
using SaxsSpot.NanoSystemService.Domain;
using SaxsSpot.Shared.ProgressTrackerClient.Contracts.Services;
using JobModels = SaxsSpot.Shared.ProgressTrackerClient.Contracts.Models ;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunRadialAnalysis;

public class RunRadialAnalysisHandler(
    INanoSystemStorage nanosystemStorage, 
    INanoSystemObjectStorage nanosystemObjectStorage,
    IServiceScopeFactory scopeFactory,
    ILogger<RunRadialAnalysisHandler> logger
    ) 
    : IRequestHandler<RunRadialAnalysisCommand, IResult<Guid>>
{
    private const string JobType = "RunRadialAnalysis";

    public async Task<IResult<Guid>> Handle(RunRadialAnalysisCommand request, CancellationToken cancellationToken)
    {
        var operationGuid = Guid.NewGuid();
        
        try
        {
            logger.LogInformation(
                "Starting radial analysis for nanosystem {NanosystemId} with operation id {OperationId}. LayerCount: {LayerCount}, PointCount: {PointCount}",
                request.NanosystemId, operationGuid, request.LayerCount, request.PointCount);
            
        var nanosystem = await nanosystemStorage.FirstOrDefaultAsync(x => x.Id == request.NanosystemId);
        if (nanosystem == null)
        {
                logger.LogWarning("Nanosystem with ID {NanosystemId} not found for operation {OperationId}", 
                    request.NanosystemId, operationGuid);
            throw new ArgumentException($"Nanosystem with ID {request.NanosystemId} does not exist.");
        }
        
            logger.LogDebug("Loading nanosystem object {ObjectId} for nanosystem {NanosystemId}", 
                nanosystem.ObjectId, nanosystem.Id);
            
        var nanosystemObject = nanosystemObjectStorage.Load(nanosystem.ObjectId, cancellationToken);
            var inputDate = DateTime.UtcNow;
            
        _ = Task.Run(async () =>
        {
            using var scope = scopeFactory.CreateScope();

            var radialAnalysisLayerStorage = scope.ServiceProvider.GetRequiredService<IRadialAnalysisLayerStorage>();
            var radialAnalysisStorage = scope.ServiceProvider.GetRequiredService<IRadialAnalysisStorage>();
                var jobService = scope.ServiceProvider.GetService<IJobServiceClient>();
                
                logger.LogDebug("Creating job for radial analysis operation {OperationId}", operationGuid);

            var result = await jobService!.CreateJobAsync(new JobModels.CreateJobQuery(operationGuid.ToString(), JobType,
                "radial analysis started", JsonSerializer.Serialize(request)));
            
            if (result.IsSuccessful is false)
            {
                    logger.LogError("Failed to create job for operation {OperationId}. Error: {ErrorMessage}", 
                        operationGuid, result.ErrorMessage);
                throw new InvalidOperationException(
                    $"Operation not started with id {operationGuid} with error on remote server {result.ErrorMessage}");
            }

                logger.LogInformation("Job created successfully for operation {OperationId}", operationGuid);
                
                var startDate = DateTime.UtcNow;
            try
            {
                    logger.LogDebug("Starting job for operation {OperationId}", operationGuid);
                    
                result = await jobService.StartJobAsync(new JobModels.StartJobQuery(operationGuid.ToString()));
                if (result.IsSuccessful is false)
                {
                        logger.LogError("Failed to start job for operation {OperationId}. Error: {ErrorMessage}", 
                            operationGuid, result.ErrorMessage);
                    throw new InvalidOperationException(
                        $"Operation not started with id {operationGuid} with error on remote server {result.ErrorMessage}");
                }

                    logger.LogInformation("Job started successfully for operation {OperationId}. Starting analysis for particle kind {ParticleKind}", 
                        operationGuid, nanosystem.ParticleKind);
                
                ICollection<RadialAnalysisLayerResult> analysisLayers;
                if (nanosystem.ParticleKind == ParticleKind.Parallelepiped)
                {
                        logger.LogDebug("Running analysis for Parallelepiped particles. Operation {OperationId}", operationGuid);
                    analysisLayers = NanosystemAnalyzer.GetNanosystemAnalyzeWithLayers(nanosystemObject
                            .ToBlockingEnumerable()
                            .Select(x => (Parallelepiped)x).ToList(),
                        new GenerationZone(nanosystem.GlobalSize, nanosystem.GenerationZoneForm), request.LayerCount,
                        request.PointCount);
                }
                else 
                {
                        logger.LogDebug("Running analysis for Sphere particles. Operation {OperationId}", operationGuid);
                    analysisLayers = NanosystemAnalyzer.GetNanosystemAnalyzeWithLayers(nanosystemObject
                            .ToBlockingEnumerable()
                            .Select(x => (Sphere)x).ToList(),
                        new GenerationZone(nanosystem.GlobalSize, nanosystem.GenerationZoneForm), request.LayerCount,
                        request.PointCount);
                }

                    logger.LogInformation("Analysis completed for operation {OperationId}. Generated {AnalysisCount} zones", 
                        operationGuid, analysisLayers.Count);

                    logger.LogDebug("Saving analysis layers to DB for operation {OperationId}", operationGuid);

                await radialAnalysisStorage.UpdateOrInsertAsync(new RadialAnalysis()
                {
                    Id = operationGuid,
                    NanosystemId = nanosystem.Id,
                    ObjectId = Guid.Empty,
                    LayerCount = request.LayerCount,
                    PointCount = request.PointCount,
                    InputDate = inputDate,
                    StartDate = startDate,
                    EndDate = DateTime.UtcNow,
                });

                var layerEntities = analysisLayers.Select(l => new RadialAnalysisLayer
                {
                    Id = Guid.NewGuid(),
                    RadialAnalysisId = operationGuid,
                    NanosystemId = nanosystem.Id,
                    LayerIndex = l.ZoneIndex,
                    LayerFrom = l.LayerFrom,
                    LayerTo = l.LayerTo,
                    NumericalConcentration = l.NumericalConcentration,
                    PointCount = l.PointCount,
                }).ToList();
                await radialAnalysisLayerStorage.AddRangeAsync(layerEntities, cancellationToken);

                    var endDate = DateTime.UtcNow;
                    var duration = endDate - startDate;
                
                    logger.LogInformation(
                        "Radial analysis completed successfully for operation {OperationId}. Duration: {Duration}ms. Zones: {AnalysisCount}",
                        operationGuid, duration.TotalMilliseconds, analysisLayers.Count);
                    
                await jobService.CompleteJobAsync(new JobModels.CompleteJobQuery(operationGuid.ToString(),
                    "radial analysis completed"));
            }
            catch (Exception e)
            {
                    var endDate = DateTime.UtcNow;
                    var duration = endDate - startDate;
                    
                    logger.LogError(e, 
                        "Error occurred during radial analysis for operation {OperationId} after {Duration}ms. Error: {ErrorMessage}", 
                        operationGuid, duration.TotalMilliseconds, e.Message);
                    
                await jobService.CompleteJobAsync(new JobModels.CompleteJobQuery(operationGuid.ToString(),
                    e.Message, true));
            }
        }, cancellationToken);
        
            logger.LogInformation("Radial analysis task started for operation {OperationId}", operationGuid);
        return FluentResults.Result.Ok(operationGuid);
        }
        catch (OperationCanceledException ex)
        {
            logger.LogInformation("Radial analysis operation {OperationId} was cancelled", operationGuid);
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to start radial analysis operation {OperationId} for nanosystem {NanosystemId}", 
                operationGuid, request.NanosystemId);
            throw;
        }
    }
}