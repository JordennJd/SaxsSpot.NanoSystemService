using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemService.Application.Interfaces;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.DeleteSeries;

public class DeleteSeriesHandler(
    INanoSystemSeriesStorage seriesStorage,
    INanoSystemStorage nanosystemStorage,
    INanoSystemObjectStorage nanosystemObjectStorage,
    IRadialAnalysisStorage radialAnalysisStorage,
    IRadialAnalysisLayerStorage radialAnalysisLayerStorage,
    IScatteringCalculationStorage scatteringCalculationStorage,
    IScatteringResultObjectStorage scatteringResultObjectStorage,
    IGenerationMetricsStorage generationMetricsStorage,
    IParticleGenerationMetricsStorage particleGenerationMetricsStorage,
    ILogger<DeleteSeriesHandler> logger) : IRequestHandler<DeleteSeriesCommand, FluentResults.Result<Unit>>
{
    public async Task<Result<Unit>> Handle(DeleteSeriesCommand request, CancellationToken cancellationToken)
    {
        var series = await seriesStorage.FirstOrDefaultAsync(x => x.Id == request.SeriesId);
        if (series == null)
        {
            logger.LogWarning("Series {SeriesId} not found", request.SeriesId);
            return FluentResults.Result.Fail("Series not found");
        }

        try
        {
            logger.LogInformation("Starting cascade delete for series {SeriesId}", request.SeriesId);

            // Get all nanosystems in the series
            var nanosystems = await nanosystemStorage.WhereAsync(x => x.SeriesId == request.SeriesId);
            var nanosystemList = nanosystems.ToList();
            logger.LogInformation("Found {Count} nanosystems in series {SeriesId}", nanosystemList.Count, request.SeriesId);

            // Delete each nanosystem and all its related data
            foreach (var nanosystem in nanosystemList)
            {
                logger.LogDebug("Deleting nanosystem {NanosystemId} from series {SeriesId}", nanosystem.Id, request.SeriesId);

                // Delete radial analysis layers
                var radialAnalyses = await radialAnalysisStorage.WhereAsync(x => x.NanosystemId == nanosystem.Id);
                foreach (var analysis in radialAnalyses)
                {
                    var layers = await radialAnalysisLayerStorage.WhereAsync(x => x.RadialAnalysisId == analysis.Id);
                    await radialAnalysisLayerStorage.DeleteRangeAsync(layers);
                }

                // Delete radial analyses
                await radialAnalysisStorage.DeleteRangeAsync(radialAnalyses);

                var scatteringCalculations = await scatteringCalculationStorage.WhereAsync(x => x.NanosystemId == nanosystem.Id);
                foreach (var calculation in scatteringCalculations)
                {
                    if (calculation.ObjectId != Guid.Empty)
                    {
                        try
                        {
                            await scatteringResultObjectStorage.Delete(calculation.ObjectId);
                        }
                        catch (Exception ex)
                        {
                            logger.LogWarning(ex, "Failed to delete scattering result object {ObjectId}, continuing", calculation.ObjectId);
                        }
                    }
                }

                await scatteringCalculationStorage.DeleteRangeAsync(scatteringCalculations);

                // Delete generation metrics
                var generationMetrics = await generationMetricsStorage.WhereAsync(x => x.NanosystemId == nanosystem.Id);
                await generationMetricsStorage.DeleteRangeAsync(generationMetrics);

                // Delete particle generation metrics
                var particleMetrics = await particleGenerationMetricsStorage.WhereAsync(x => x.NanosystemId == nanosystem.Id);
                await particleGenerationMetricsStorage.DeleteRangeAsync(particleMetrics);

                // Delete object from object storage
                if (nanosystem.ObjectId != Guid.Empty)
                {
                    try
                    {
                        await nanosystemObjectStorage.Delete(nanosystem.ObjectId);
                    }
                    catch (Exception ex)
                    {
                        logger.LogWarning(ex, "Failed to delete object {ObjectId} from object storage, continuing", nanosystem.ObjectId);
                    }
                }

                // Delete nanosystem entity
                await nanosystemStorage.DeleteAsync(nanosystem);
            }

            // Delete series entity
            await seriesStorage.DeleteAsync(series);
            logger.LogInformation("Successfully deleted series {SeriesId} and all {Count} nanosystems with related data", request.SeriesId, nanosystemList.Count);

            return FluentResults.Result.Ok();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error deleting series {SeriesId}", request.SeriesId);
            return FluentResults.Result.Fail($"Failed to delete series: {ex.Message}");
        }
    }
}
