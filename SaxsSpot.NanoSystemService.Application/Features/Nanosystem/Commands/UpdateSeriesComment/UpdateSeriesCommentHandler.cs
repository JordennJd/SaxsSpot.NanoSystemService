using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using SaxsSpot.NanoSystemService.Application.Interfaces;

namespace SaxsSpot.NanoSystemService.Application.Features.Nanosystem.Commands.UpdateSeriesComment;

public class UpdateSeriesCommentHandler(
    INanoSystemSeriesStorage seriesStorage,
    ILogger<UpdateSeriesCommentHandler> logger) : IRequestHandler<UpdateSeriesCommentCommand, FluentResults.Result<Unit>>
{
    public async Task<FluentResults.Result<Unit>> Handle(UpdateSeriesCommentCommand request, CancellationToken cancellationToken)
    {
        var series = await seriesStorage.FirstOrDefaultAsync(x => x.Id == request.SeriesId);
        if (series is null)
        {
            logger.LogWarning("Series {SeriesId} not found for comment update", request.SeriesId);
            return FluentResults.Result.Fail("Series not found");
        }

        series.Comment = request.Comment;
        await seriesStorage.UpdateOrInsertAsync(series);
        return FluentResults.Result.Ok();
    }
}
