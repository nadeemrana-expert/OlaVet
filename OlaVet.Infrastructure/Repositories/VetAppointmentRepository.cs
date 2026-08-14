
// =============================================
// File: OlaVet.Infrastructure/Repositories/VetAppointmentRepository.cs
// Most complex repository - handles appointments
// =============================================

using Microsoft.EntityFrameworkCore;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Interfaces;
using OlaVet.Infrastructure.Data;

namespace OlaVet.Infrastructure.Repositories;

public class VetAppointmentRepository : Repository<VetAppointment>, IVetAppointmentRepository
{
    public VetAppointmentRepository(OlaVetDbContext context) : base(context)
    {
    }
    
    public async Task<VetAppointment?> GetWithDetailsAsync(int appointmentId)
    {
        return await _dbSet
            .Include(a => a.Pet)
                .ThenInclude(p => p.PetOwner)
            .Include(a => a.Vet)
            .Include(a => a.VetAppointmentType)
            .Include(a => a.StatusType)
            .Include(a => a.VetPayment)
            .Include(a => a.VetReview)
            .FirstOrDefaultAsync(a => a.VetAppointmentId == appointmentId);
    }
    
    public async Task<IEnumerable<VetAppointment>> GetByVetAndDateAsync(int vetId, DateTime date)
    {
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);
        
        return await _dbSet
            .Include(a => a.Pet)
            .Include(a => a.PetOwner)
            .Include(a => a.StatusType)
            .Where(a => a.VetId == vetId &&
                       a.AppointmentDateTime >= startOfDay &&
                       a.AppointmentDateTime < endOfDay)
            .OrderBy(a => a.AppointmentDateTime)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<VetAppointment>> GetByOwnerIdAsync(int ownerId)
    {
        return await _dbSet
            .Include(a => a.Pet)
            .Include(a => a.Vet)
            .Include(a => a.StatusType)
            .Where(a => a.PetOwnerId == ownerId)
            .OrderByDescending(a => a.AppointmentDateTime)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<VetAppointment>> GetUpcomingAsync(int days = 7)
    {
        var now = DateTime.UtcNow;
        var futureDate = now.AddDays(days);
        
        return await _dbSet
            .Include(a => a.Pet)
            .Include(a => a.PetOwner)
            .Include(a => a.Vet)
            .Include(a => a.StatusType)
            .Where(a => a.AppointmentDateTime >= now &&
                       a.AppointmentDateTime <= futureDate &&
                       a.StatusType.StatusName == "Scheduled")
            .OrderBy(a => a.AppointmentDateTime)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<VetAppointment>> GetPetHistoryAsync(int petId)
    {
        return await _dbSet
            .Include(a => a.Vet)
            .Include(a => a.StatusType)
            .Include(a => a.VetReview)
            .Where(a => a.PetId == petId)
            .OrderByDescending(a => a.AppointmentDateTime)
            .ToListAsync();
    }
    
    public async Task<bool> IsTimeSlotAvailableAsync(int vetId, DateTime dateTime)
    {
        // Check if there's already an appointment at this exact time
        var hasConflict = await _dbSet
            .AnyAsync(a => a.VetId == vetId &&
                          a.AppointmentDateTime == dateTime &&
                          a.StatusType.StatusName != "Cancelled");
        
        return !hasConflict;
    }
    
    public async Task<IEnumerable<DateTime>> GetAvailableTimeSlotsAsync(int vetId, DateTime date)
    {
        // Get vet's availability for this day
        var dayOfWeek = date.DayOfWeek.ToString();
        
        var availability = await _context.VetAvailabilities
            .FirstOrDefaultAsync(a => a.VetId == vetId &&
                                    a.DayOfWeek == dayOfWeek &&
                                    a.IsAvailable);
        
        if (availability == null)
            return Enumerable.Empty<DateTime>();
        
        // Get existing appointments for this day
        var startOfDay = date.Date;
        var endOfDay = startOfDay.AddDays(1);
        
        var existingAppointments = await _dbSet
            .Where(a => a.VetId == vetId &&
                       a.AppointmentDateTime >= startOfDay &&
                       a.AppointmentDateTime < endOfDay &&
                       a.StatusType.StatusName != "Cancelled")
            .Select(a => a.AppointmentDateTime)
            .ToListAsync();
        
        // Generate time slots
        var slots = new List<DateTime>();
        var currentTime = startOfDay.Add(availability.StartTime);
        var endTime = startOfDay.Add(availability.EndTime);
        
        while (currentTime < endTime)
        {
            // Check if slot is not already booked
            if (!existingAppointments.Contains(currentTime))
            {
                slots.Add(currentTime);
            }
            
            currentTime = currentTime.AddMinutes(availability.SlotDurationMinutes);
        }
        
        return slots;
    }
    
    public async Task<IEnumerable<VetAppointment>> GetRequiringFollowUpAsync()
    {
        var twoWeeksAgo = DateTime.UtcNow.AddDays(-14);
        
        return await _dbSet
            .Include(a => a.Pet)
            .Include(a => a.PetOwner)
            .Include(a => a.Vet)
            .Where(a => a.CompletedDate != null &&
                       a.CompletedDate < twoWeeksAgo &&
                       a.Notes != null &&
                       a.Notes.ToLower().Contains("follow"))
            .OrderBy(a => a.CompletedDate)
            .ToListAsync();
    }
}