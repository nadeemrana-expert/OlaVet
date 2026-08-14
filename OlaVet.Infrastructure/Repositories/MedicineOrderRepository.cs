
// =============================================
// File: OlaVet.Infrastructure/Repositories/MedicineOrderRepository.cs
// =============================================

using Microsoft.EntityFrameworkCore;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Interfaces;
using OlaVet.Infrastructure.Data;

namespace OlaVet.Infrastructure.Repositories;

public class MedicineOrderRepository : Repository<MedicineOrder>, IMedicineOrderRepository
{
    public MedicineOrderRepository(OlaVetDbContext context) : base(context)
    {
    }
    
    public async Task<MedicineOrder?> GetWithDetailsAsync(int orderId)
    {
        return await _dbSet
            .Include(o => o.MedicineOrderDetails)
                .ThenInclude(d => d.Medicine)
            .Include(o => o.PetOwner)
            .Include(o => o.Store)
            .Include(o => o.StatusType)
            .FirstOrDefaultAsync(o => o.MedicineOrderId == orderId);
    }
    
    public async Task<IEnumerable<MedicineOrder>> GetByOwnerIdAsync(int ownerId)
    {
        return await _dbSet
            .Include(o => o.Store)
            .Include(o => o.StatusType)
            .Where(o => o.PetOwnerId == ownerId)
            .OrderByDescending(o => o.OrderDateTime)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<MedicineOrder>> GetPendingOrdersAsync()
    {
        return await _dbSet
            .Include(o => o.PetOwner)
            .Include(o => o.Store)
            .Where(o => o.StatusType.StatusName == "Pending" ||
                       o.StatusType.StatusName == "Processing")
            .OrderBy(o => o.OrderDateTime)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<MedicineOrder>> GetByStatusAsync(string status)
    {
        return await _dbSet
            .Include(o => o.PetOwner)
            .Include(o => o.Store)
            .Include(o => o.StatusType)
            .Where(o => o.StatusType.StatusName.ToLower() == status.ToLower())
            .OrderByDescending(o => o.OrderDateTime)
            .ToListAsync();
    }
    
    public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Where(o => o.OrderDateTime >= startDate &&
                       o.OrderDateTime <= endDate &&
                       o.StatusType.StatusName == "Delivered")
            .SumAsync(o => o.TotalAmount);
    }
}