using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemService.Application.Interfaces;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetGenerationMetrics;

public class GetGenerationMetricsHandler(
    IParticleGenerationMetricsStorage particleGenerationMetricsStorage,
    ILogger<GetGenerationMetricsHandler> logger)
    : IRequestHandler<GetGenerationMetricsQuery, Result<IEnumerable<GenerationMetricsDto>>>
{
    public async Task<Result<IEnumerable<GenerationMetricsDto>>> Handle(GetGenerationMetricsQuery request, CancellationToken cancellationToken)
    {
        try
        {
            var metrics = await particleGenerationMetricsStorage.WhereAsync(x =>
                x.NanosystemId == request.NanosystemId &&
                (request.IndexRanges == null || request.IndexRanges.Any(i => i.FromIndex <= x.ParticleIndex && i.ToIndex >= x.ParticleIndex)));
            
            var result = metrics.Select(m => new GenerationMetricsDto
            {
                Id = m.Id,
                NanosystemId = m.NanosystemId,
                ParticleIndex = m.ParticleIndex,
                TotalAttempts = m.TotalAttempts,
                PositiveAttempts = m.PositiveAttempts,
                TotalChangePositionAttempts = m.TotalChangePositionAttempts,
                GenerationTimeMs = m.GenerationTimeMs,
                Volume = m.Volume,
                Diameter = m.Diameter,
                ParticlesCheckedForIntersection = m.ParticlesCheckedForIntersection,
                OutOfZoneAttempts = m.OutOfZoneAttempts,
                FirstNodeIntersectionFindTimes = m.FirstNodeIntersectionFindTimes,
                TotalNeighborsNodesCheckedCount = m.TotalNeighborsNodesCheckedCount,
                IsInterCenterDistanceMoreThenDiagonalCheckTimesPositive = m.IsInterCenterDistanceMoreThenDiagonalCheckTimesPositive,
                IsInterCenterDistanceMoreThenDiagonalCheckTimesTotal = m.IsInterCenterDistanceMoreThenDiagonalCheckTimesTotal,
                IsInterCenterDistanceLessThenSidesCheckTimesPositive = m.IsInterCenterDistanceLessThenSidesCheckTimesPositive,
                IsInterCenterDistanceLessThenSidesCheckTimesTotal = m.IsInterCenterDistanceLessThenSidesCheckTimesTotal,
                ElementaryIntersectCheckOnlyBordersNewTransformationTimesPositive = m.ElementaryIntersectCheckOnlyBordersNewTransformationTimesPositive,
                ElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal = m.ElementaryIntersectCheckOnlyBordersNewTransformationTimesTotal,
                ElementaryIntersectCheckOnlyBordersOldTransformationTimesPositive = m.ElementaryIntersectCheckOnlyBordersOldTransformationTimesPositive,
                ElementaryIntersectCheckOnlyBordersOldTransformationTimesTotal = m.ElementaryIntersectCheckOnlyBordersOldTransformationTimesTotal,
                BackRotateMatrixReused = m.BackRotateMatrixReused,
                SATCheckTimesPositive = m.SATCheckTimesPositive,
                SATCheckTimesTotal = m.SATCheckTimesTotal
            })
            .OrderBy(dto => dto.ParticleIndex)
            .ToList();
            
            return FluentResults.Result.Ok(result.AsEnumerable());
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error getting generation metrics for nanosystem {NanosystemId}", request.NanosystemId);
            return FluentResults.Result.Fail($"Error getting generation metrics: {ex.Message}");
        }
    }
}
