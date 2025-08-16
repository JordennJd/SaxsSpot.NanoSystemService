namespace SaxsSpot.NanoSystemService.Host.Result;

public class ResultDto<T>
{
    public T Result { get; set; }
    
    public bool IsSuccess { get; set; }

    public IEnumerable<ErrorDto> Errors { get; set; }

    public ResultDto(bool isSuccess, IEnumerable<ErrorDto> errors, T result)
    {
        Result = result;
        IsSuccess = isSuccess;
        Errors = errors;
    }
}

public class ErrorDto
{
    public string Message { get; set; }

    public string Code { get; set; }

    public ErrorDto(string message, string code)
    {
        Message = message;
        Code = code;
    }
}