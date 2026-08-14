// =============================================
// SIMPLE REPOSITORY IMPLEMENTATIONS
// (For entities that don't need custom methods)
// =============================================

using Microsoft.EntityFrameworkCore;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Interfaces;
using OlaVet.Infrastructure.Data;

namespace OlaVet.Infrastructure.Repositories;

public class LabRepository : Repository<Lab>, ILabRepository
{
    public LabRepository(OlaVetDbContext context) : base(context) { }
    
    public async Task<Lab?> GetWithAppointmentsAsync(int labId)
    {
        return await _context.Labs
            .Include(l => l.LabAppointments)
            .FirstOrDefaultAsync(l => l.LabId == labId);
    }
    
    public async Task<IEnumerable<Lab>> SearchAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        return await _context.Labs
            .Where(l => l.LabName.ToLower().Contains(term) ||
                       (l.LabAddress != null && l.LabAddress.ToLower().Contains(term)) ||
                       (l.Specialization != null && l.Specialization.ToLower().Contains(term)))
            .Take(50)
            .AsNoTracking()
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Lab>> GetBySpecializationAsync(string specialization)
    {
        return await _context.Labs
            .Where(l => l.Specialization != null && l.Specialization.Contains(specialization))
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Lab>> GetTopRatedAsync(int count = 10)
    {
        var topLabIds = await _context.LabReviews
            .GroupBy(r => r.LabId)
            .Select(g => new { LabId = g.Key, AvgRating = g.Average(r => r.Rating) })
            .OrderByDescending(x => x.AvgRating)
            .Take(count)
            .Select(x => x.LabId)
            .ToListAsync();
            
        return await _context.Labs
            .Where(l => topLabIds.Contains(l.LabId))
            .AsNoTracking()
            .ToListAsync();
    }
    
    public async Task<IEnumerable<LabWithRating>> GetLabsWithRatingsAsync()
    {
        return await _context.Labs
            .AsNoTracking()
            .Select(l => new LabWithRating
            {
                LabId = l.LabId,
                LabName = l.LabName,
                LabAddress = l.LabAddress,
                Specialization = l.Specialization,
                ContactNumber = l.ContactNumber,
                AverageRating = l.LabReviews.Any() ? l.LabReviews.Average(r => r.Rating) : 0,
                ReviewCount = l.LabReviews.Count,
                AppointmentCount = l.LabAppointments.Count
            })
            .ToListAsync();
    }
}

public class LabAppointmentRepository : Repository<LabAppointment>, ILabAppointmentRepository
{
    public LabAppointmentRepository(OlaVetDbContext context) : base(context) { }
    
    public async Task<LabAppointment?> GetWithDetailsAsync(int appointmentId)
    {
        return await _context.LabAppointments
            .Include(a => a.Pet)
            .Include(a => a.PetOwner)
            .Include(a => a.Lab)
            .Include(a => a.StatusType)
            .Include(a => a.LabAppointmentTests)
                .ThenInclude(t => t.LabTest)
            .FirstOrDefaultAsync(a => a.LabAppointmentId == appointmentId);
    }
    
    public async Task<IEnumerable<LabAppointment>> GetByLabAndDateAsync(int labId, DateTime date)
    {
        return await _context.LabAppointments
            .Include(a => a.Pet)
            .Include(a => a.PetOwner)
            .Where(a => a.LabId == labId && a.AppointmentDateTime.Date == date.Date)
            .OrderBy(a => a.AppointmentDateTime)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<LabAppointment>> GetByOwnerIdAsync(int ownerId)
    {
        return await _context.LabAppointments
            .Include(a => a.Lab)
            .Include(a => a.Pet)
            .Include(a => a.StatusType)
            .Where(a => a.PetOwnerId == ownerId)
            .OrderByDescending(a => a.AppointmentDateTime)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<LabAppointment>> GetByPetIdAsync(int petId)
    {
        return await _context.LabAppointments
            .Include(a => a.Lab)
            .Include(a => a.StatusType)
            .Where(a => a.PetId == petId)
            .OrderByDescending(a => a.AppointmentDateTime)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<LabAppointment>> GetUpcomingAsync(int days = 7)
    {
        var endDate = DateTime.UtcNow.AddDays(days);
        return await _context.LabAppointments
            .Include(a => a.Pet)
            .Include(a => a.PetOwner)
            .Include(a => a.Lab)
            .Where(a => a.AppointmentDateTime >= DateTime.UtcNow && 
                       a.AppointmentDateTime <= endDate &&
                       a.StatusType.StatusName != "Cancelled")
            .OrderBy(a => a.AppointmentDateTime)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<LabAppointment>> GetCompletedWithResultsAsync()
    {
        return await _context.LabAppointments
            .Include(a => a.LabAppointmentTests)
                .ThenInclude(t => t.LabTest)
            .Where(a => a.StatusType.StatusName == "Completed" &&
                       a.LabAppointmentTests.Any(t => t.TestResult != null))
            .ToListAsync();
    }
    
    public async Task<IEnumerable<LabAppointment>> GetAwaitingResultsAsync()
    {
        return await _context.LabAppointments
            .Include(a => a.Pet)
            .Include(a => a.Lab)
            .Include(a => a.LabAppointmentTests)
            .Where(a => a.StatusType.StatusName == "Completed" &&
                       a.LabAppointmentTests.Any(t => t.TestResult == null))
            .ToListAsync();
    }
}

public class StoreRepository : Repository<Store>, IStoreRepository
{
    public StoreRepository(OlaVetDbContext context) : base(context) { }
    
    public async Task<Store?> GetWithInventoryAsync(int storeId)
    {
        return await _context.Stores
            .Include(s => s.Inventories)
                .ThenInclude(i => i.Medicine)
            .FirstOrDefaultAsync(s => s.StoreId == storeId);
    }
    
    public async Task<IEnumerable<Store>> SearchAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        return await _context.Stores
            .Where(s => s.StoreName.ToLower().Contains(term) ||
                       (s.StoreAddress != null && s.StoreAddress.ToLower().Contains(term)))
            .Take(50)
            .AsNoTracking()
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Store>> GetStoresWithMedicineAsync(int medicineId)
    {
        return await _context.Stores
            .Where(s => s.Inventories.Any(i => i.MedicineId == medicineId && i.Quantity > 0))
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Store>> GetTopRatedAsync(int count = 10)
    {
        var topStoreIds = await _context.StoreReviews
            .GroupBy(r => r.StoreId)
            .Select(g => new { StoreId = g.Key, AvgRating = g.Average(r => r.Rating) })
            .OrderByDescending(x => x.AvgRating)
            .Take(count)
            .Select(x => x.StoreId)
            .ToListAsync();
            
        return await _context.Stores
            .Where(s => topStoreIds.Contains(s.StoreId))
            .AsNoTracking()
            .ToListAsync();
    }
    
    public async Task<IEnumerable<StoreWithRating>> GetStoresWithRatingsAsync()
    {
        return await _context.Stores
            .AsNoTracking()
            .Select(s => new StoreWithRating
            {
                StoreId = s.StoreId,
                StoreName = s.StoreName,
                StoreAddress = s.StoreAddress,
                ContactNumber = s.ContactNumber,
                OpeningTime = s.OpeningTime,
                ClosingTime = s.ClosingTime,
                AverageRating = s.StoreReviews.Any() ? s.StoreReviews.Average(r => r.Rating) : 0,
                ReviewCount = s.StoreReviews.Count,
                OrderCount = s.MedicineOrders.Count
            })
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Store>> GetOpenStoresAsync()
    {
        var currentTime = DateTime.UtcNow.TimeOfDay;
        return await _context.Stores
            .Where(s => s.OpeningTime.HasValue && s.ClosingTime.HasValue &&
                       s.OpeningTime.Value <= currentTime && s.ClosingTime.Value >= currentTime)
            .ToListAsync();
    }
}

public class MedicineRepository : Repository<Medicine>, IMedicineRepository
{
    public MedicineRepository(OlaVetDbContext context) : base(context) { }
    
    public async Task<Medicine?> GetWithDetailsAsync(int medicineId)
    {
        return await _context.Medicines
            .Include(m => m.MedicineType)
            .Include(m => m.Inventories)
                .ThenInclude(i => i.Store)
            .FirstOrDefaultAsync(m => m.MedicineId == medicineId);
    }
    
    public async Task<IEnumerable<Medicine>> SearchAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        return await _context.Medicines
            .Include(m => m.MedicineType)
            .Where(m => m.MedicineName.ToLower().Contains(term) ||
                       (m.Manufacturer != null && m.Manufacturer.ToLower().Contains(term)))
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Medicine>> GetByTypeAsync(int medicineTypeId)
    {
        return await _context.Medicines
            .Where(m => m.MedicineTypeId == medicineTypeId)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Medicine>> GetInStockAtStoreAsync(int storeId)
    {
        return await _context.Medicines
            .Where(m => m.Inventories.Any(i => i.StoreId == storeId && i.Quantity > 0))
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Medicine>> GetPrescriptionMedicinesAsync()
    {
        return await _context.Medicines
            .Where(m => m.RequiresPrescription)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Medicine>> GetOtcMedicinesAsync()
    {
        return await _context.Medicines
            .Where(m => !m.RequiresPrescription)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<MedicineWithStock>> GetLowStockAsync(int threshold = 10)
    {
        return await _context.Medicines
            .Select(m => new MedicineWithStock
            {
                MedicineId = m.MedicineId,
                MedicineName = m.MedicineName,
                MG = m.MG,
                Price = m.Price,
                Manufacturer = m.Manufacturer,
                RequiresPrescription = m.RequiresPrescription,
                TotalStock = m.Inventories.Sum(i => i.Quantity),
                StoreCount = m.Inventories.Count(i => i.Quantity > 0)
            })
            .Where(m => m.TotalStock < threshold)
            .ToListAsync();
    }
}

public class MedicalRecordRepository : Repository<MedicalRecord>, IMedicalRecordRepository
{
    public MedicalRecordRepository(OlaVetDbContext context) : base(context) { }
    
    public async Task<MedicalRecord?> GetWithDetailsAsync(int recordId)
    {
        return await _context.MedicalRecords
            .Include(r => r.Pet)
            .Include(r => r.PetOwner)
            .Include(r => r.RecordType)
            .Include(r => r.Vet)
            .FirstOrDefaultAsync(r => r.RecordId == recordId);
    }
    
    public async Task<IEnumerable<MedicalRecord>> GetByPetIdAsync(int petId)
    {
        return await _context.MedicalRecords
            .Include(r => r.RecordType)
            .Include(r => r.Vet)
            .Where(r => r.PetId == petId)
            .OrderByDescending(r => r.RecordDate)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<MedicalRecord>> GetByOwnerIdAsync(int ownerId)
    {
        return await _context.MedicalRecords
            .Include(r => r.Pet)
            .Include(r => r.RecordType)
            .Where(r => r.PetOwnerId == ownerId)
            .OrderByDescending(r => r.RecordDate)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<MedicalRecord>> GetByTypeAsync(int recordTypeId)
    {
        return await _context.MedicalRecords
            .Include(r => r.Pet)
            .Where(r => r.RecordTypeId == recordTypeId)
            .OrderByDescending(r => r.RecordDate)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<MedicalRecord>> GetByVetIdAsync(int vetId)
    {
        return await _context.MedicalRecords
            .Include(r => r.Pet)
            .Include(r => r.PetOwner)
            .Where(r => r.VetId == vetId)
            .OrderByDescending(r => r.RecordDate)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<MedicalRecord>> GetByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _context.MedicalRecords
            .Include(r => r.Pet)
            .Include(r => r.RecordType)
            .Where(r => r.RecordDate >= startDate && r.RecordDate <= endDate)
            .OrderByDescending(r => r.RecordDate)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<MedicalRecord>> SearchByDiagnosisAsync(string searchTerm)
    {
        var term = searchTerm.ToLower();
        return await _context.MedicalRecords
            .Include(r => r.Pet)
            .Where(r => r.Diagnosis != null && r.Diagnosis.ToLower().Contains(term))
            .OrderByDescending(r => r.RecordDate)
            .ToListAsync();
    }
}