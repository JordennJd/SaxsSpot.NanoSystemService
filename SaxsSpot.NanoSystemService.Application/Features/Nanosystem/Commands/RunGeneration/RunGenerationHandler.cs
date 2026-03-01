using AutoMapper;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.Enums;
using SaxsSpot.NanoSystemGeneration.Contracts.Models.GenerationParameters;
using SaxsSpot.NanoSystemService.Contracts.Services;
using SaxsSpot.Shared.ProgressTrackerClient.Contracts.Services;
using JobModels = SaxsSpot.Shared.ProgressTrackerClient.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.RunGeneration;

/// <summary>
/// Run single nanosystem generation
/// </summary>
public class RunGenerationHandler(
    INanoSystemService nanoSystemService,
    ILogger<RunGenerationHandler> logger,
    IMapper mapper,
    IJobServiceClient jobServiceClient) : IRequestHandler<RunGenerationCommand, Result<Guid>>
{
    public async Task<Result<Guid>> Handle(RunGenerationCommand request, CancellationToken cancellationToken)
    {
        var operationGuid = request.OperationId;

        try
        {
            var startResult =
                await jobServiceClient.StartJobAsync(new JobModels.StartJobQuery(operationGuid.ToString()));
            if (!startResult.IsSuccessful)
            {
                throw new InvalidOperationException(startResult.ErrorMessage);
            }

            logger.Log(LogLevel.Information, $"Run generation started with operation id: {operationGuid}, series id: {request.SeriesId}, zoneCount: {request.ZoneCount}, pointCount: {request.PointCount}, needAnalysis: {request.NeedAnalysis}");
            
            // Determine if analysis should be performed: needAnalysis must be true AND pointCount must be > 0
            var shouldPerformAnalysis = request.NeedAnalysis && request.PointCount > 0;
            var analysisZoneCount = shouldPerformAnalysis ? request.ZoneCount : 0;
            var analysisVectorCount = shouldPerformAnalysis ? request.PointCount : 0;
            
            // Create progress handler to update job with generation progress
            // Track last reported progress to update only every 0.5%
            var lastReportedProgress = -1.0f;
            EventHandler<float>? progressHandler = (sender, progress) =>
            {
                // Update only every 0.5% (0.005)
                var currentProgressPercent = progress;
                var progressDiff = Math.Abs(currentProgressPercent - (lastReportedProgress));
                
                if (progressDiff >= 0.5f || lastReportedProgress < 0)
                {
                    lastReportedProgress = progress;
                    var progressPercent = (int)Math.Round(currentProgressPercent);
                    
                    try
                    {
                        // Use Task.Run to avoid blocking the progress callback
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await jobServiceClient.ChangeJobMessageAsync(new JobModels.ChangeJobMessageQuery(
                                    operationGuid.ToString(),
                                    $"Generation in progress: {progressPercent}%"));
                            }
                            catch (Exception ex)
                            {
                                logger.LogWarning(ex, "Failed to update job progress for operation {OperationId}", operationGuid);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Failed to schedule job progress update for operation {OperationId}", operationGuid);
                    }
                }
            };
            
            // Create callback for when analysis starts (initial message)
            Func<Task>? onAnalysisStarted = null;
            EventHandler<float>? analysisProgressHandler = null;
            if (shouldPerformAnalysis)
            {
                onAnalysisStarted = async () =>
                {
                    try
                    {
                        await jobServiceClient.ChangeJobMessageAsync(new JobModels.ChangeJobMessageQuery(
                            operationGuid.ToString(),
                            "Analysis in progress: 0%"));
                        logger.LogInformation("Analysis started notification sent for operation {OperationId}", operationGuid);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Failed to notify analysis start for operation {OperationId}", operationGuid);
                    }
                };

                // Update job message with analysis progress (throttle to every 5%)
                var lastAnalysisProgress = -1;
                analysisProgressHandler = (sender, progress) =>
                {
                    var progressPercent = (int)Math.Round(progress);
                    if (Math.Abs(progressPercent - lastAnalysisProgress) < 5 && lastAnalysisProgress >= 0)
                        return;
                    lastAnalysisProgress = progressPercent;
                    try
                    {
                        _ = Task.Run(async () =>
                        {
                            try
                            {
                                await jobServiceClient.ChangeJobMessageAsync(new JobModels.ChangeJobMessageQuery(
                                    operationGuid.ToString(),
                                    $"Analysis in progress: {progressPercent}%"));
                            }
                            catch (Exception ex)
                            {
                                logger.LogWarning(ex, "Failed to update analysis progress for operation {OperationId}", operationGuid);
                            }
                        });
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Failed to schedule analysis progress update for operation {OperationId}", operationGuid);
                    }
                };
            }

            switch (request.Parameters.GetParticleKind())
            {
                case ParticleKind.Parallelepiped:
                    await nanoSystemService.RunGeneration(
                        mapper.Map<ParallelepipedGenerationParameters>(request.Parameters), 
                        progressHandler: progressHandler,
                        seriesId: request.SeriesId, 
                        cancellationToken: cancellationToken, 
                        analysisZoneCount: analysisZoneCount, 
                        analysisVectorCount: analysisVectorCount,
                        onAnalysisStarted: onAnalysisStarted,
                        analysisProgressHandler: analysisProgressHandler,
                        needMetrics: request.NeedMetrics);
                    break;
                case ParticleKind.Sphere:
                    await nanoSystemService.RunGeneration(
                        mapper.Map<SphereGenerationParameters>(request.Parameters), 
                        progressHandler: progressHandler,
                        seriesId: request.SeriesId, 
                        cancellationToken: cancellationToken, 
                        analysisZoneCount: analysisZoneCount, 
                        analysisVectorCount: analysisVectorCount,
                        onAnalysisStarted: onAnalysisStarted,
                        analysisProgressHandler: analysisProgressHandler,
                        needMetrics: request.NeedMetrics);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            var endResult =
                await jobServiceClient.CompleteJobAsync(new JobModels.CompleteJobQuery(operationGuid.ToString(),
                    "Generation completed"));

            return operationGuid;
        }
        catch (OperationCanceledException)
        {
            await jobServiceClient.CompleteJobAsync(new JobModels.CompleteJobQuery(operationGuid.ToString(),
                "Application was stopped. The task will be reprocessed when the handler is back up.", IsFailed: true));

            logger.LogInformation(
                "Run generation was canceled (application shutdown). OperationId: {OperationId}. The message will be redelivered and reprocessed when a consumer is available.",
                operationGuid);
            throw;
        }
        catch (Exception e)
        {
            await jobServiceClient.CompleteJobAsync(new JobModels.CompleteJobQuery(operationGuid.ToString(),
                $"Generation failed with error {e.Message}", IsFailed: true));
            throw;
        }
    }
}