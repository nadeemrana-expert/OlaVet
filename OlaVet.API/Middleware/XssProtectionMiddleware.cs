// =============================================
// File: OlaVet.API/Middleware/XssProtectionMiddleware.cs
// XSS protection middleware - sanitizes inputs
// =============================================

using System.Text.RegularExpressions;

namespace OlaVet.API.Middleware;

/// <summary>
/// Middleware to detect and block XSS attacks in request bodies and query strings.
/// Also adds security headers to all responses.
/// </summary>
public partial class XssProtectionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<XssProtectionMiddleware> _logger;

    // Dangerous patterns that indicate XSS attempts
    [GeneratedRegex(@"<script[\s>]|javascript:|on\w+\s*=|<\s*iframe|<\s*object|<\s*embed|<\s*form|<\s*img[^>]+onerror|eval\s*\(|expression\s*\(", RegexOptions.IgnoreCase | RegexOptions.Compiled)]
    private static partial Regex XssPattern();

    public XssProtectionMiddleware(RequestDelegate next, ILogger<XssProtectionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // 1. Check query string parameters for XSS
        foreach (var param in context.Request.Query)
        {
            if (ContainsXss(param.Value.ToString()))
            {
                _logger.LogWarning("XSS attempt detected in query parameter '{Key}' from IP {IP}",
                    param.Key, context.Connection.RemoteIpAddress);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { error = "Potentially dangerous content detected in request" });
                return;
            }
        }

        // 2. Check request body for XSS (for POST/PUT/PATCH)
        if (context.Request.ContentLength > 0 && 
            context.Request.ContentType?.Contains("json", StringComparison.OrdinalIgnoreCase) == true)
        {
            context.Request.EnableBuffering();
            
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            context.Request.Body.Position = 0;

            if (ContainsXss(body))
            {
                _logger.LogWarning("XSS attempt detected in request body from IP {IP}",
                    context.Connection.RemoteIpAddress);
                context.Response.StatusCode = StatusCodes.Status400BadRequest;
                await context.Response.WriteAsJsonAsync(new { error = "Potentially dangerous content detected in request" });
                return;
            }
        }

        // 3. Add security headers to response
        context.Response.Headers["X-Content-Type-Options"] = "nosniff";
        context.Response.Headers["X-Frame-Options"] = "DENY";
        context.Response.Headers["X-XSS-Protection"] = "1; mode=block";
        context.Response.Headers["Referrer-Policy"] = "strict-origin-when-cross-origin";
        context.Response.Headers["Permissions-Policy"] = "camera=(), microphone=(), geolocation=()";

        // Only set CSP on non-API responses (API responses are JSON, CSP is for HTML pages)
        if (!context.Request.Path.StartsWithSegments("/api"))
        {
            context.Response.Headers["Content-Security-Policy"] = "default-src 'self'; script-src 'self'; style-src 'self' 'unsafe-inline'";
        }

        await _next(context);
    }

    private static bool ContainsXss(string input)
    {
        if (string.IsNullOrEmpty(input)) return false;
        return XssPattern().IsMatch(input);
    }
}

/// <summary>
/// Extension method for XSS protection middleware
/// </summary>
public static class XssProtectionMiddlewareExtensions
{
    public static IApplicationBuilder UseXssProtection(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<XssProtectionMiddleware>();
    }
}
