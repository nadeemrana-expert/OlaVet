// =============================================
// File: OlaVet.Domain/Interfaces/IUserRepository.cs
// Repository interface for user operations
// =============================================

using OlaVet.Domain.Entities;

namespace OlaVet.Domain.Interfaces;

/// <summary>
/// Repository for user-related database operations
/// </summary>
public interface IUserRepository
{
    Task<ApplicationUser?> GetByIdAsync(int id);
    Task<ApplicationUser?> GetByEmailAsync(string email);
    Task<ApplicationUser?> GetByIdWithRolesAsync(int userId);
    Task<bool> EmailExistsAsync(string email);
    Task<ApplicationUser?> GetByRefreshTokenAsync(string refreshToken);
    Task<ApplicationUser> AddAsync(ApplicationUser user);
    void Update(ApplicationUser user);
}
