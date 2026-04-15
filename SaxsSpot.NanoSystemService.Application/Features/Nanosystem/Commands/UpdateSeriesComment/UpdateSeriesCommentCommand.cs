using FluentResults;
using MediatR;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.UpdateSeriesComment;

public record UpdateSeriesCommentCommand(Guid SeriesId, string? Comment) : IRequest<FluentResults.Result<Unit>>;
