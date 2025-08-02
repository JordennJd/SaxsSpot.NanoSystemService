using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace SaxsSpot.NanoSystemService.Host.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        context.Response.ContentType = "application/json";
        var problemDetails = new ProblemDetails();

        switch (ex)
        {
            // 1. Обрабатываем ValidationException (FluentValidation)
            case ValidationException validationEx:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Validation Error";
                problemDetails.Extensions["errors"] = validationEx.Errors
                    .GroupBy(e => e.PropertyName)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Select(e => e.ErrorMessage).ToArray()
                    );
                break;
            
            case KeyNotFoundException keyNotFoundException:
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                problemDetails.Title = "Validation Error";
                // problemDetails.Extensions["errors"] = keyNotFoundException.;
                break;
            
            // 3. Общий случай (500)
            default:
                _logger.LogError(ex, "Unhandled exception");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                problemDetails.Title = "Internal Server Error";
                if (context.RequestServices.GetService<IWebHostEnvironment>()?.IsDevelopment() == true)
                {
                    problemDetails.Extensions["trace"] = ex.StackTrace;
                }
                break;
        }

        problemDetails.Status = context.Response.StatusCode;
        problemDetails.Detail = ex.Message;
        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}