using FluentResults;
using MediatR;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.DeleteSeries;

public record DeleteSeriesCommand(Guid SeriesId) : IRequest<FluentResults.Result<Unit>>;
