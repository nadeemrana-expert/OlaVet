
// =============================================
// File: OlaVet.Infrastructure/Repositories/PetRepository.cs
// =============================================

using Microsoft.EntityFrameworkCore;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Interfaces;
using OlaVet.Infrastructure.Data;

namespace OlaVet.Infrastructure.Repositories;

public class PetRepository : Repository<Pet>, IPetRepository
{
    public PetRepository(OlaVetDbContext context) : base(context)
    {
    }
    
    public async Task<Pet?> GetWithOwnerAsync(int petId)
    {
        return await _dbSet
            .Include(p => p.PetOwner)
            .FirstOrDefaultAsync(p => p.PetId == petId);
    }
    
    public async Task<Pet?> GetWithMedicalHistoryAsync(int petId)
    {
        return await _dbSet
            .Include(p => p.MedicalRecords.OrderByDescending(r => r.RecordDate))
            .Include(p => p.VetAppointments.OrderByDescending(a => a.AppointmentDateTime))
                .ThenInclude(a => a.Vet)
            .FirstOrDefaultAsync(p => p.PetId == petId);
    }
    
    public async Task<IEnumerable<Pet>> GetByOwnerIdAsync(int ownerId)
    {
        return await _dbSet
            .Where(p => p.PetOwnerId == ownerId && p.IsActive)
            .OrderBy(p => p.Name)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Pet>> GetBySpeciesAsync(string species)
    {
        return await _dbSet
            .Where(p => p.IsActive && p.Species.ToLower() == species.ToLower())
            .OrderBy(p => p.Name)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Pet>> GetDueForCheckupAsync(int daysThreshold = 180)
    {
        var thresholdDate = DateTime.UtcNow.AddDays(-daysThreshold);
        
        return await _dbSet
            .Include(p => p.PetOwner)
            .Where(p => p.IsActive &&
                       !p.VetAppointments.Any(a => a.AppointmentDateTime > thresholdDate))
            .OrderBy(p => p.PetOwner.OwnerName)
            .ThenBy(p => p.Name)
            .ToListAsync();
    }
}
