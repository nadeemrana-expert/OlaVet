// =============================================
// File: OlaVet.Infrastructure/Repositories/PetOwnerRepository.cs
// Specific repository for PetOwner with custom queries
// =============================================

using Microsoft.EntityFrameworkCore;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Interfaces;
using OlaVet.Infrastructure.Data;

namespace OlaVet.Infrastructure.Repositories;

public class PetOwnerRepository : Repository<PetOwner>, IPetOwnerRepository
{
    public PetOwnerRepository(OlaVetDbContext context) : base(context)
    {
    }
    
    public async Task<PetOwner?> GetWithPetsAsync(int ownerId)
    {
        return await _dbSet
            .Include(o => o.Pets)
            .FirstOrDefaultAsync(o => o.PetOwnerId == ownerId);
    }
    
    public async Task<PetOwner?> GetWithPetsAndAppointmentsAsync(int ownerId)
    {
        return await _dbSet
            .Include(o => o.Pets)
            .Include(o => o.VetAppointments)
                .ThenInclude(a => a.Vet)
            .Include(o => o.VetAppointments)
                .ThenInclude(a => a.StatusType)
            .FirstOrDefaultAsync(o => o.PetOwnerId == ownerId);
    }
    
    public async Task<IEnumerable<PetOwner>> SearchAsync(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        
        return await _dbSet
            .Where(o => o.IsActive &&
                       (o.OwnerName.ToLower().Contains(lowerSearchTerm) ||
                        o.Email.ToLower().Contains(lowerSearchTerm)))
            .OrderBy(o => o.OwnerName)
            .Take(50) // Limit results
            .ToListAsync();
    }
    
    public async Task<IEnumerable<PetOwner>> GetRegisteredBetweenAsync(
        DateTime startDate, 
        DateTime endDate)
    {
        return await _dbSet
            .Where(o => o.RegistrationDate >= startDate && 
                       o.RegistrationDate <= endDate)
            .OrderByDescending(o => o.RegistrationDate)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<PetOwner>> GetTopByWalletAsync(int count = 10)
    {
        return await _dbSet
            .Where(o => o.IsActive)
            .OrderByDescending(o => o.Wallet)
            .Take(count)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<PetOwner>> GetLowBalanceOwnersAsync(decimal threshold = 1000)
    {
        return await _dbSet
            .Where(o => o.IsActive && o.Wallet < threshold)
            .OrderBy(o => o.Wallet)
            .ToListAsync();
    }
}