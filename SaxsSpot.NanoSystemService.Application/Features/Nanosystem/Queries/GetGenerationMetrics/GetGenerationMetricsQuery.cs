using FluentResults;
using MediatR;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.GetGenerationMetrics;

public record IndexRange(int FromIndex, int ToIndex);

public record GetGenerationMetricsQuery(Guid NanosystemId, IList<IndexRange>? IndexRanges = null) 
    : IRequest<Result<IEnumerable<GenerationMetricsDto>>>;
