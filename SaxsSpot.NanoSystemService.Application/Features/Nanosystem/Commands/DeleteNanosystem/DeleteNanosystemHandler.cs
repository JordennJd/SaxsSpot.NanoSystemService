using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemService.Application.Interfaces;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.DeleteNanosystem;

public class DeleteNanosystemHandler(
    INanoSystemStorage nanosystemStorage,
    INanoSystemObjectStorage nanosystemObjectStorage,
    IRadialAnalysisStorage radialAnalysisStorage,
    IRadialAnalysisLayerStorage radialAnalysisLayerStorage,
    IGenerationMetricsStorage generationMetricsStorage,
    IParticleGenerationMetricsStorage particleGenerationMetricsStorage,
    ILogger<DeleteNanosystemHandler> logger) : IRequestHandler<DeleteNanosystemCommand, FluentResults.Result<Unit>>
{
    private const string RequiredPassword = "Pass123$";

    public async Task<FluentResults.Result<Unit>> Handle(DeleteNanosystemCommand request, CancellationToken cancellationToken)
    {
        if (request.Password != RequiredPassword)
        {
            logger.LogWarning("Invalid password provided for deleting nanosystem {NanosystemId}", request.NanosystemId);
            return FluentResults.Result.Fail("Invalid password");
        }

        var nanosystem = await nanosystemStorage.FirstOrDefaultAsync(x => x.Id == request.NanosystemId);
        if (nanosystem == null)
        {
            logger.LogWarning("Nanosystem {NanosystemId} not found", request.NanosystemId);
            return FluentResults.Result.Fail("Nanosystem not found");
        }

        try
        {
            logger.LogInformation("Starting cascade delete for nanosystem {NanosystemId}", request.NanosystemId);

            // Delete radial analysis layers
            var radialAnalyses = await radialAnalysisStorage.WhereAsync(x => x.NanosystemId == request.NanosystemId);
            foreach (var analysis in radialAnalyses)
            {
                var layers = await radialAnalysisLayerStorage.WhereAsync(x => x.RadialAnalysisId == analysis.Id);
                await radialAnalysisLayerStorage.DeleteRangeAsync(layers);
                logger.LogDebug("Deleted {Count} radial analysis layers for analysis {AnalysisId}", layers.Count(), analysis.Id);
            }

            // Delete radial analyses
            await radialAnalysisStorage.DeleteRangeAsync(radialAnalyses);
            logger.LogDebug("Deleted {Count} radial analyses for nanosystem {NanosystemId}", radialAnalyses.Count(), request.NanosystemId);

            // Delete generation metrics
            var generationMetrics = await generationMetricsStorage.WhereAsync(x => x.NanosystemId == request.NanosystemId);
            await generationMetricsStorage.DeleteRangeAsync(generationMetrics);
            logger.LogDebug("Deleted {Count} generation metrics for nanosystem {NanosystemId}", generationMetrics.Count(), request.NanosystemId);

            // Delete particle generation metrics
            var particleMetrics = await particleGenerationMetricsStorage.WhereAsync(x => x.NanosystemId == request.NanosystemId);
            await particleGenerationMetricsStorage.DeleteRangeAsync(particleMetrics);
            logger.LogDebug("Deleted {Count} particle generation metrics for nanosystem {NanosystemId}", particleMetrics.Count(), request.NanosystemId);

            // Delete object from object storage
            if (nanosystem.ObjectId != Guid.Empty)
            {
                try
                {
                    await nanosystemObjectStorage.Delete(nanosystem.ObjectId);
                    logger.LogDebug("Deleted object {ObjectId} from object storage", nanosystem.ObjectId);
                }
                catch (Exception ex)
                {
                    logger.LogWarning(ex, "Failed to delete object {ObjectId} from object storage, continuing", nanosystem.ObjectId);
                }
            }

            // Delete nanosystem entity
            await nanosystemStorage.DeleteAsync(nanosystem);
            logger.LogInformation("Successfully deleted nanosystem {NanosystemId} and all related data", request.NanosystemId);

            return FluentResults.Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting nanosystem {NanosystemId}", request.NanosystemId);
            return FluentResults.Result.Fail($"Failed to delete nanosystem: {ex.Message}");
        }
    }
}
