using FluentResults;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace SaxsSpot.NanoSystemService.Host.Result;

public static class ResultDtoExtensions
{
    public static ResultDto<T> ToResultDto<T>(this FluentResults.Result<T> result)
    {
        if (result.IsSuccess)
            return new ResultDto<T>(true, Enumerable.Empty<ErrorDto>(), result.ValueOrDefault);

        return new ResultDto<T>(false, TransformErrors(result.Errors), result.ValueOrDefault);
    }

    private static IEnumerable<ErrorDto> TransformErrors(IEnumerable<IError> errors)
    {
        
        return errors.Select(TransformError);
    }

    private static ErrorDto TransformError(IError error)
    {
        var errorCode = TransformErrorCode(error);

        return new ErrorDto(error.Message, errorCode);
    }

    private static string TransformErrorCode(IError error)
    {
        if (error.Metadata.TryGetValue("ErrorCode", out var errorCode))
            return errorCode as string;

        return "";
    }
}