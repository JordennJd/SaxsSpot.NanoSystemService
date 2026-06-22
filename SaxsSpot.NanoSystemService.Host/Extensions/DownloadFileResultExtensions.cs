using FluentResults;
using Microsoft.AspNetCore.Mvc;
using SaxsSpot.Shared.Contracts.Models;

namespace SaxsSpot.NanoSystemService.Host.Extensions;

public static class DownloadFileResultExtensions
{
    public static IActionResult ToDownloadFileResult(this IResult<Stream> result, string fileName)
    {
        if (result.IsFailed)
        {
            return new BadRequestObjectResult(result.ToResultDto());
        }

        if (result.Value is not { CanRead: true } stream)
        {
            return new BadRequestObjectResult(
                FluentResults.Result.Fail<Stream>("Download returned an empty stream").ToResultDto());
        }

        return new FileStreamResult(stream, "application/octet-stream")
        {
            FileDownloadName = fileName,
            EnableRangeProcessing = true,
        };
    }
}
