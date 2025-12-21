using System.Text.Json;
using FluentResults;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using SaxsSpot.NanoSystemGeneration.Contracts.Models;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.AnalyzeModels;
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
    INanoSystemStorage nanosystemStorage, INanoSystemObjectStorage nanosystemObjectStorage,
    IServiceScopeFactory scopeFactory
    ) 
    : IRequestHandler<RunRadialAnalysisCommand, IResult<Guid>>
{
    private const string JobType = "RunRadialAnalysis";

    public async Task<IResult<Guid>> Handle(RunRadialAnalysisCommand request, CancellationToken cancellationToken)
    {
        var nanosystem = await nanosystemStorage.FirstOrDefaultAsync(x => x.Id == request.NanosystemId);
        if (nanosystem == null)
        {
            throw new ArgumentException($"Nanosystem with ID {request.NanosystemId} does not exist.");
        }
        
        var nanosystemObject = nanosystemObjectStorage.Load(nanosystem.ObjectId, cancellationToken);
        var operationGuid = Guid.NewGuid();
        var inputDate = DateTime.UtcNow;
        _ = Task.Run(async () =>
        {
            using var scope = scopeFactory.CreateScope();

            var radialAnalysisObjectStorage = scope.ServiceProvider.GetService<IRadialAnalysisObjectStorage>();
            var radialAnalysisStorage = scope.ServiceProvider.GetService<IRadialAnalysisStorage>();

            var jobService = scope.ServiceProvider.GetService<IJobServiceClient>();
            var result = await jobService!.CreateJobAsync(new JobModels.CreateJobQuery(operationGuid.ToString(), JobType,
                "radial analysis started", JsonSerializer.Serialize(request)));
            
            if (result.IsSuccessful is false)
            {
                throw new InvalidOperationException(
                    $"Operation not started with id {operationGuid} with error on remote server {result.ErrorMessage}");
            }

            var startDate = DateTime.UtcNow;
            try
            {
                result = await jobService.StartJobAsync(new JobModels.StartJobQuery(operationGuid.ToString()));
                if (result.IsSuccessful is false)
                {
                    throw new InvalidOperationException(
                        $"Operation not started with id {operationGuid} with error on remote server {result.ErrorMessage}");
                }
                
                ICollection<ZoneConcentrationAnalyze> analysis;
                if (nanosystem.ParticleKind == ParticleKind.Parallelepiped)
                {
                    analysis = NanosystemAnalyzer.GetNanosystemAnalyze(nanosystemObject
                            .ToBlockingEnumerable()
                            .Select(x => (Parallelepiped)x).ToList(),
                        new GenerationZone(nanosystem.GlobalSize, nanosystem.GenerationZoneForm), request.LayerCount,
                        request.PointCount);
                }
                else 
                {
                    analysis = NanosystemAnalyzer.GetNanosystemAnalyze(nanosystemObject
                            .ToBlockingEnumerable()
                            .Select(x => (Sphere)x).ToList(),
                        new GenerationZone(nanosystem.GlobalSize, nanosystem.GenerationZoneForm), request.LayerCount,
                        request.PointCount);

                }

                var objectId = Guid.NewGuid();
                await radialAnalysisObjectStorage.Save(analysis, objectId);

                var endDate = DateTime.UtcNow;
                await radialAnalysisStorage.UpdateOrInsertAsync(new RadialAnalysis()
                {
                    Id = operationGuid,
                    NanosystemId = nanosystem.Id,
                    ObjectId = objectId,
                    LayerCount = request.LayerCount,
                    PointCount = request.PointCount,
                    InputDate = inputDate,
                    StartDate = startDate,
                    EndDate = endDate,
                });
                
                await jobService.CompleteJobAsync(new JobModels.CompleteJobQuery(operationGuid.ToString(),
                    "radial analysis completed"));
            }
            catch (Exception e)
            {
                var endDate = DateTime.UtcNow;
                await radialAnalysisStorage.UpdateOrInsertAsync(new RadialAnalysis()
                {
                    Id = operationGuid,
                    NanosystemId = nanosystem.Id,
                    ObjectId = Guid.Empty, // Will be set if analysis was successful
                    LayerCount = request.LayerCount,
                    PointCount = request.PointCount,
                    InputDate = inputDate,
                    StartDate = startDate,
                    EndDate = endDate,
                });
                
                await jobService.CompleteJobAsync(new JobModels.CompleteJobQuery(operationGuid.ToString(),
                    e.Message, true));
            }
        }, cancellationToken);
        
        return FluentResults.Result.Ok(operationGuid);
    }
}