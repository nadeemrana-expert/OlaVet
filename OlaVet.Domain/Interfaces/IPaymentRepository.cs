// =============================================
// File: OlaVet.Domain/Interfaces/IPaymentRepository.cs
// Combined payment repository interface
// =============================================

using OlaVet.Domain.Entities;

namespace OlaVet.Domain.Interfaces;

/// <summary>
/// Payment repository handling all payment types
/// </summary>
public interface IPaymentRepository
{
    // =============================================
    // VET PAYMENTS
    // =============================================
    
    /// <summary>
    /// Get all payments for a vet
    /// </summary>
    Task<IEnumerable<VetPayment>> GetVetPaymentsAsync(int vetId);
    
    /// <summary>
    /// Get vet payments by pet owner
    /// </summary>
    Task<IEnumerable<VetPayment>> GetVetPaymentsByOwnerAsync(int ownerId);
    
    /// <summary>
    /// Get vet payment by appointment
    /// </summary>
    Task<VetPayment?> GetVetPaymentByAppointmentAsync(int appointmentId);
    
    /// <summary>
    /// Get total vet revenue for date range
    /// </summary>
    Task<decimal> GetVetRevenueAsync(int vetId, DateTime startDate, DateTime endDate);
    
    // =============================================
    // LAB PAYMENTS
    // =============================================
    
    /// <summary>
    /// Get all payments for a lab
    /// </summary>
    Task<IEnumerable<LabPayment>> GetLabPaymentsAsync(int labId);
    
    /// <summary>
    /// Get lab payments by pet owner
    /// </summary>
    Task<IEnumerable<LabPayment>> GetLabPaymentsByOwnerAsync(int ownerId);
    
    /// <summary>
    /// Get lab payment by appointment
    /// </summary>
    Task<LabPayment?> GetLabPaymentByAppointmentAsync(int appointmentId);
    
    /// <summary>
    /// Get total lab revenue for date range
    /// </summary>
    Task<decimal> GetLabRevenueAsync(int labId, DateTime startDate, DateTime endDate);
    
    // =============================================
    // STORE PAYMENTS
    // =============================================
    
    /// <summary>
    /// Get all payments for a store
    /// </summary>
    Task<IEnumerable<StorePayment>> GetStorePaymentsAsync(int storeId);
    
    /// <summary>
    /// Get store payments by pet owner
    /// </summary>
    Task<IEnumerable<StorePayment>> GetStorePaymentsByOwnerAsync(int ownerId);
    
    /// <summary>
    /// Get store payment by order
    /// </summary>
    Task<StorePayment?> GetStorePaymentByOrderAsync(int orderId);
    
    /// <summary>
    /// Get total store revenue for date range
    /// </summary>
    Task<decimal> GetStoreRevenueAsync(int storeId, DateTime startDate, DateTime endDate);
    
    // =============================================
    // AGGREGATE METHODS
    // =============================================
    
    /// <summary>
    /// Get all payments for a pet owner
    /// </summary>
    Task<OwnerPaymentSummary> GetOwnerPaymentSummaryAsync(int ownerId);
    
    /// <summary>
    /// Get payment statistics
    /// </summary>
    Task<PaymentStatistics> GetPaymentStatisticsAsync(DateTime startDate, DateTime endDate);
}

/// <summary>
/// Summary of all payments by an owner
/// </summary>
public class OwnerPaymentSummary
{
    public int OwnerId { get; set; }
    public decimal TotalVetPayments { get; set; }
    public decimal TotalLabPayments { get; set; }
    public decimal TotalStorePayments { get; set; }
    public decimal GrandTotal => TotalVetPayments + TotalLabPayments + TotalStorePayments;
    public int VetPaymentCount { get; set; }
    public int LabPaymentCount { get; set; }
    public int StorePaymentCount { get; set; }
}

/// <summary>
/// Payment statistics for reporting
/// </summary>
public class PaymentStatistics
{
    public decimal TotalRevenue { get; set; }
    public decimal VetRevenue { get; set; }
    public decimal LabRevenue { get; set; }
    public decimal StoreRevenue { get; set; }
    public int TotalTransactions { get; set; }
    public decimal AverageTransactionAmount { get; set; }
}
