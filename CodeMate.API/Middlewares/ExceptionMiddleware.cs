using System.Text.Json;
using CodeMate.Contracts.Common.ApiResponse;
using CodeMate.Shared.Exceptions;

namespace CodeMate.API.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
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
        catch (AppException ex)
        {
            _logger.LogWarning(ex, "Handled application exception");
            await WriteResponseAsync(context, ex.StatusCode, ex.Message,
                ex is ValidationException v ? v.Errors : null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception");
            await WriteResponseAsync(context, 500, "An unexpected error occurred.", null);
        }
    }

    private static async Task WriteResponseAsync(
        HttpContext context, int statusCode, string message, IDictionary<string, string[]>? errors)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        var response = new ErrorResponse { Message = message, Errors = errors };
        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}