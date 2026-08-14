using System.Text;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using OlaVet.Application;
using OlaVet.Application.Security;
using OlaVet.Infrastructure;
using OlaVet.Infrastructure.Data;
using OlaVet.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

// =============================================
// CONFIGURATION - Bind settings from appsettings.json
// =============================================
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection(JwtSettings.SectionName));
builder.Services.Configure<FileUploadSettings>(builder.Configuration.GetSection(FileUploadSettings.SectionName));

var jwtSettings = builder.Configuration.GetSection(JwtSettings.SectionName).Get<JwtSettings>()!;

// =============================================
// Add DbContext
// =============================================
builder.Services.AddDbContext<OlaVetDbContext>(options =>
{
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            // Retry on transient failures
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,
                maxRetryDelay: TimeSpan.FromSeconds(30),
                errorNumbersToAdd: null);

            // Command timeout (seconds)
            sqlOptions.CommandTimeout(60);

            // Use query splitting for better performance
            sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
        });

    // Development settings
    if (builder.Environment.IsDevelopment())
    {
        options.EnableSensitiveDataLogging(); // Show parameter values in logs
        options.EnableDetailedErrors(); // Detailed error messages
    }
});

// =============================================
// AUTHENTICATION - JWT Bearer
// =============================================
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey)),
        ClockSkew = TimeSpan.Zero // No clock skew tolerance (strict expiry)
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            if (context.Exception is SecurityTokenExpiredException)
            {
                context.Response.Headers["Token-Expired"] = "true";
            }
            return Task.CompletedTask;
        }
    };
});

// =============================================
// AUTHORIZATION - Role-based + Policy-based
// =============================================
builder.Services.AddAuthorization(options =>
{
    // Role-based policies
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(RoleNames.Admin));
    options.AddPolicy("VetOnly", policy => policy.RequireRole(RoleNames.Vet, RoleNames.Admin));
    options.AddPolicy("PetOwnerOnly", policy => policy.RequireRole(RoleNames.PetOwner, RoleNames.Admin));
    
    // Permission-based policies
    options.AddPolicy("CanManageVets", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("permission", Permissions.VetsManage) ||
            context.User.HasClaim("permission", Permissions.AdminFullAccess)));
    
    options.AddPolicy("CanManageUsers", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim("permission", Permissions.AdminUserManagement) ||
            context.User.HasClaim("permission", Permissions.AdminFullAccess)));
});

// =============================================
// RATE LIMITING - Prevent abuse
// =============================================
builder.Services.AddRateLimiter(options =>
{
    // Global rate limit: 100 requests per minute per IP
    options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "anonymous",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                AutoReplenishment = true,
                PermitLimit = 100,
                Window = TimeSpan.FromMinutes(1)
            }));
    
    // Stricter rate limit for auth endpoints (login/register)
    options.AddFixedWindowLimiter("AuthRateLimit", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromMinutes(5);
        opt.AutoReplenishment = true;
    });
    
    // Rate limit for file uploads
    options.AddFixedWindowLimiter("FileUploadRateLimit", opt =>
    {
        opt.PermitLimit = 20;
        opt.Window = TimeSpan.FromMinutes(10);
        opt.AutoReplenishment = true;
    });
    
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    
    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.ContentType = "application/json";
        await context.HttpContext.Response.WriteAsJsonAsync(
            new { error = "Too many requests. Please try again later." },
            cancellationToken);
    };
});

// =============================================
// CORS - Cross-Origin Resource Sharing
// =============================================
builder.Services.AddCors(options =>
{
    // Development: Allow all origins
    options.AddPolicy("Development", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
    
    // Production: Restrict to specific origins
    options.AddPolicy("Production", policy =>
    {
        var allowedOrigins = builder.Configuration
            .GetSection("CorsSettings:AllowedOrigins")
            .Get<string[]>() ?? ["https://olavet.com"];
        
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials()
              .SetPreflightMaxAge(TimeSpan.FromMinutes(10));
    });
});

// =============================================
// Other Services
// =============================================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = 
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddEndpointsApiExplorer();

// Swagger with JWT support
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "OlaVet API",
        Version = "v1",
        Description = "Veterinary Management System API with JWT Authentication"
    });
    
    // Add JWT authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token. Example: eyJhbGciOiJIUzI1NiIs..."
    });
    
    options.AddSecurityRequirement(_ => new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecuritySchemeReference("Bearer"),
            new List<string>()
        }
    });
});

// =============================================
// DI Registration - Application & Infrastructure layers
// =============================================
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices();

// =============================================
// HSTS - HTTP Strict Transport Security
// =============================================
if (!builder.Environment.IsDevelopment())
{
    builder.Services.AddHsts(options =>
    {
        options.Preload = true;
        options.IncludeSubDomains = true;
        options.MaxAge = TimeSpan.FromDays(365);
    });
}

var app = builder.Build();

// =============================================
// MIDDLEWARE PIPELINE (order matters!)
// =============================================

// 0. Global exception handler (must be first to catch all errors)
app.UseGlobalExceptionHandler();

// 1. Exception handling
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHsts();
}

// 2. HTTPS Redirection
app.UseHttpsRedirection();

// 3. Sensitive data masking in logs
app.UseSensitiveDataMasking();

// 4. XSS Protection & security headers
app.UseXssProtection();

// 5. CORS
app.UseCors(app.Environment.IsDevelopment() ? "Development" : "Production");

// 6. Rate limiting
app.UseRateLimiter();

// 7. Authentication & Authorization (must be in this order)
app.UseAuthentication();
app.UseAuthorization();

// 8. Map controllers
app.MapControllers();

app.Run();