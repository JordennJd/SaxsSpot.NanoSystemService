using FluentResults;
using MediatR;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.DownloadRadialAnalysis;

public record DownloadRadialAnalysisQuery(Guid Id) : IRequest<IResult<Stream>>;
