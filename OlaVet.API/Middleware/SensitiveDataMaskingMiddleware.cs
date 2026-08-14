// =============================================
// File: OlaVet.API/Middleware/SensitiveDataMaskingMiddleware.cs
// Masks sensitive data in logs
// =============================================

using System.Text.RegularExpressions;

namespace OlaVet.API.Middleware;

/// <summary>
/// Middleware to mask sensitive data (passwords, tokens, SSN, credit cards)
/// in request/response logging. Ensures GDPR and data protection compliance.
/// </summary>
public partial class SensitiveDataMaskingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<SensitiveDataMaskingMiddleware> _logger;

    public SensitiveDataMaskingMiddleware(RequestDelegate next, ILogger<SensitiveDataMaskingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log sanitized request info
        var sanitizedPath = context.Request.Path.ToString();
        var sanitizedQuery = MaskSensitiveQueryParams(context.Request.QueryString.ToString());
        
        _logger.LogInformation("Request: {Method} {Path}{Query} from {IP}",
            context.Request.Method,
            sanitizedPath,
            sanitizedQuery,
            context.Connection.RemoteIpAddress);

        await _next(context);

        _logger.LogInformation("Response: {StatusCode} for {Method} {Path}",
            context.Response.StatusCode,
            context.Request.Method,
            sanitizedPath);
    }

    /// <summary>
    /// Mask sensitive query parameters
    /// </summary>
    private static string MaskSensitiveQueryParams(string queryString)
    {
        if (string.IsNullOrEmpty(queryString)) return queryString;
        
        // Mask common sensitive parameter names
        var sensitiveParams = new[] { "password", "token", "secret", "key", "authorization", "creditcard", "ssn" };
        
        foreach (var param in sensitiveParams)
        {
            var pattern = $@"({param}=)([^&]*)";
            queryString = Regex.Replace(queryString, pattern, $"$1***MASKED***", RegexOptions.IgnoreCase);
        }
        
        return queryString;
    }
}

/// <summary>
/// Custom log enricher that masks sensitive fields in structured logs
/// </summary>
public static class SensitiveDataMasker
{
    private static readonly string[] SensitiveFields = 
    [
        "password", "passwordHash", "token", "refreshToken", "accessToken",
        "secret", "creditCard", "ssn", "socialSecurity", "apiKey"
    ];
    
    /// <summary>
    /// Mask a value if the field name suggests it's sensitive
    /// </summary>
    public static string MaskIfSensitive(string fieldName, string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        
        if (SensitiveFields.Any(f => fieldName.Contains(f, StringComparison.OrdinalIgnoreCase)))
        {
            return value.Length > 4 
                ? string.Concat(value.AsSpan(0, 2), "***", value.AsSpan(value.Length - 2)) 
                : "***";
        }
        
        return value;
    }
    
    /// <summary>
    /// Mask email address for logging
    /// </summary>
    public static string MaskEmail(string email)
    {
        var parts = email.Split('@');
        if (parts.Length != 2) return "***";
        
        var local = parts[0].Length > 1 
            ? parts[0][0] + new string('*', Math.Min(parts[0].Length - 1, 5)) 
            : "*";
        
        return $"{local}@{parts[1]}";
    }
    
    /// <summary>
    /// Mask phone number for logging
    /// </summary>
    public static string MaskPhone(string phone)
    {
        if (string.IsNullOrEmpty(phone) || phone.Length < 4) return "***";
        return new string('*', phone.Length - 4) + phone[^4..];
    }
}

public static class SensitiveDataMaskingMiddlewareExtensions
{
    public static IApplicationBuilder UseSensitiveDataMasking(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<SensitiveDataMaskingMiddleware>();
    }
}
