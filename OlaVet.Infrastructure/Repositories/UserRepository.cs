// =============================================
// File: OlaVet.Infrastructure/Repositories/UserRepository.cs
// User repository implementation
// =============================================

using Microsoft.EntityFrameworkCore;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Interfaces;
using OlaVet.Infrastructure.Data;

namespace OlaVet.Infrastructure.Repositories;

/// <summary>
/// Repository for user-related database operations
/// </summary>
public class UserRepository : IUserRepository
{
    private readonly OlaVetDbContext _context;
    
    public UserRepository(OlaVetDbContext context)
    {
        _context = context;
    }
    
    public async Task<ApplicationUser?> GetByIdAsync(int id)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.UserId == id && u.IsActive);
    }
    
    public async Task<ApplicationUser> AddAsync(ApplicationUser user)
    {
        await _context.Users.AddAsync(user);
        return user;
    }
    
    public void Update(ApplicationUser user)
    {
        _context.Users.Update(user);
    }
    
    public async Task<ApplicationUser?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.Email.ToLower() == email.ToLower() && u.IsActive);
    }
    
    public async Task<ApplicationUser?> GetByIdWithRolesAsync(int userId)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .FirstOrDefaultAsync(u => u.UserId == userId && u.IsActive);
    }
    
    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _context.Users
            .AnyAsync(u => u.Email.ToLower() == email.ToLower());
    }
    
    public async Task<ApplicationUser?> GetByRefreshTokenAsync(string refreshToken)
    {
        return await _context.Users
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                    .ThenInclude(r => r.RolePermissions)
                        .ThenInclude(rp => rp.Permission)
            .Include(u => u.RefreshTokens)
            .FirstOrDefaultAsync(u => u.RefreshTokens.Any(rt => rt.Token == refreshToken));
    }
}
