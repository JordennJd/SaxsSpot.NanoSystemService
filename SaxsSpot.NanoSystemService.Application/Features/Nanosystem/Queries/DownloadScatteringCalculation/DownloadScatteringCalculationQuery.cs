using FluentResults;
using MediatR;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Queries.DownloadScatteringCalculation;

public record DownloadScatteringCalculationQuery(Guid Id) : IRequest<IResult<Stream>>;
