// =============================================
// File: OlaVet.Infrastructure/DependencyInjection.cs
// Extension method to register Infrastructure layer services
// =============================================

using Microsoft.Extensions.DependencyInjection;
using OlaVet.Application.Services.Interfaces;
using OlaVet.Domain.Interfaces;
using OlaVet.Infrastructure.Repositories;
using OlaVet.Infrastructure.Security;

namespace OlaVet.Infrastructure;

/// <summary>
/// Extension methods for registering Infrastructure layer services
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Add all Infrastructure layer services to the DI container
    /// </summary>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        
        // Individual repositories (if needed outside UoW)
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
        
        // Security services
        services.AddSingleton<IPasswordHasher, PasswordHasher>();
        services.AddSingleton<ITokenService, TokenService>();
        
        return services;
    }
}
