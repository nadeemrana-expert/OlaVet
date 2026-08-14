// =============================================
// File: OlaVet.API/Middleware/GlobalExceptionMiddleware.cs
// Global Exception Handling Middleware
// Catches unhandled exceptions and returns proper JSON errors
// =============================================

using System.Net;
using System.Text.Json;

namespace OlaVet.API.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly IHostEnvironment _env;

    public GlobalExceptionMiddleware(
        RequestDelegate next,
        ILogger<GlobalExceptionMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _env = env;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception for {Method} {Path}", 
                context.Request.Method, context.Request.Path);

            context.Response.ContentType = "application/json";

            var (statusCode, message) = ex switch
            {
                ArgumentNullException => (StatusCodes.Status400BadRequest, "A required value was missing."),
                ArgumentException argEx => (StatusCodes.Status400BadRequest, argEx.Message),
                KeyNotFoundException => (StatusCodes.Status404NotFound, "The requested resource was not found."),
                UnauthorizedAccessException => (StatusCodes.Status403Forbidden, "Access denied."),
                InvalidOperationException ioe => (StatusCodes.Status400BadRequest, ioe.Message),
                _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred.")
            };

            context.Response.StatusCode = statusCode;

            var response = _env.IsDevelopment()
                ? new
                {
                    error = message,
                    detail = ex.Message,
                    stackTrace = ex.StackTrace,
                    innerError = ex.InnerException?.Message
                }
                : (object)new
                {
                    error = message
                };

            var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
