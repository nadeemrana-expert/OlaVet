// =============================================
// File: OlaVet.Application/DependencyInjection.cs
// Extension method to register Application layer services
// =============================================

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using OlaVet.Application.Mappings;
using OlaVet.Application.Services.Implementations;
using OlaVet.Application.Services.Interfaces;

namespace OlaVet.Application;

/// <summary>
/// Extension methods for registering Application layer services
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Add all Application layer services to the DI container
    /// </summary>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register AutoMapper - scan for all profiles in this assembly
        services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
        
        // Register FluentValidation validators
        services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);
        
        // Register Services
        services.AddScoped<IPetOwnerService, PetOwnerService>();
        services.AddScoped<IVetService, VetService>();
        services.AddScoped<IPetService, PetService>();
        services.AddScoped<IAppointmentService, AppointmentService>();
        services.AddScoped<IOrderService, OrderService>();
        services.AddScoped<IReviewService, ReviewService>();
        
        // Register Auth Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IFileUploadService, FileUploadService>();
        
        return services;
    }
}
