// =============================================
// File: OlaVet.Domain/Interfaces/IRoleRepository.cs
// Repository interface for role operations
// =============================================

using OlaVet.Domain.Entities;

namespace OlaVet.Domain.Interfaces;

/// <summary>
/// Repository for role-related database operations
/// </summary>
public interface IRoleRepository
{
    Task<Role?> GetByIdAsync(int id);
    Task<Role?> GetByNameAsync(string name);
    Task<Role?> GetByIdWithPermissionsAsync(int roleId);
    Task<IEnumerable<Permission>> GetPermissionsForRoleAsync(int roleId);
    Task<Role> AddAsync(Role role);
}
