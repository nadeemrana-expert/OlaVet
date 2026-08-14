// =============================================
// File: OlaVet.Infrastructure/Repositories/RoleRepository.cs
// Role repository implementation
// =============================================

using Microsoft.EntityFrameworkCore;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Interfaces;
using OlaVet.Infrastructure.Data;

namespace OlaVet.Infrastructure.Repositories;

/// <summary>
/// Repository for role-related database operations
/// </summary>
public class RoleRepository : IRoleRepository
{
    private readonly OlaVetDbContext _context;
    
    public RoleRepository(OlaVetDbContext context)
    {
        _context = context;
    }
    
    public async Task<Role?> GetByIdAsync(int id)
    {
        return await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.RoleId == id && r.IsActive);
    }
    
    public async Task<Role> AddAsync(Role role)
    {
        await _context.Roles.AddAsync(role);
        return role;
    }
    
    public async Task<Role?> GetByNameAsync(string name)
    {
        return await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.Name.ToLower() == name.ToLower() && r.IsActive);
    }
    
    public async Task<Role?> GetByIdWithPermissionsAsync(int roleId)
    {
        return await _context.Roles
            .Include(r => r.RolePermissions)
                .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(r => r.RoleId == roleId && r.IsActive);
    }
    
    public async Task<IEnumerable<Permission>> GetPermissionsForRoleAsync(int roleId)
    {
        return await _context.RolePermissions
            .Where(rp => rp.RoleId == roleId)
            .Include(rp => rp.Permission)
            .Select(rp => rp.Permission)
            .ToListAsync();
    }
}
