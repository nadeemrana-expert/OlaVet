// =============================================
// File: OlaVet.Domain/Interfaces/IMedicineOrderRepository.cs
// =============================================

using OlaVet.Domain.Entities;

namespace OlaVet.Domain.Interfaces;

public interface IMedicineOrderRepository : IRepository<MedicineOrder>
{
    /// <summary>
    /// Get order with all line items
    /// </summary>
    Task<MedicineOrder?> GetWithDetailsAsync(int orderId);
    
    /// <summary>
    /// Get orders by owner
    /// </summary>
    Task<IEnumerable<MedicineOrder>> GetByOwnerIdAsync(int ownerId);
    
    /// <summary>
    /// Get pending orders
    /// </summary>
    Task<IEnumerable<MedicineOrder>> GetPendingOrdersAsync();
    
    /// <summary>
    /// Get orders by status
    /// </summary>
    Task<IEnumerable<MedicineOrder>> GetByStatusAsync(string status);
    
    /// <summary>
    /// Calculate total revenue for date range
    /// </summary>
    Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate);
}