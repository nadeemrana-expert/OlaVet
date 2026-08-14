// =============================================
// File: OlaVet.Domain/Interfaces/IUnitOfWork.cs
// Unit of Work pattern - manages transactions
// =============================================

namespace OlaVet.Domain.Interfaces;

/// <summary>
/// Unit of Work pattern - coordinates repository operations within a transaction
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // =============================================
    // REPOSITORY PROPERTIES
    // =============================================
    // Access to all repositories through Unit of Work
    
    IPetOwnerRepository PetOwners { get; }
    IVetRepository Vets { get; }
    IPetRepository Pets { get; }
    IVetAppointmentRepository VetAppointments { get; }
    ILabRepository Labs { get; }
    ILabAppointmentRepository LabAppointments { get; }
    IStoreRepository Stores { get; }
    IMedicineRepository Medicines { get; }
    IMedicineOrderRepository MedicineOrders { get; }
    IMedicalRecordRepository MedicalRecords { get; }
    IPaymentRepository Payments { get; }
    IReviewRepository Reviews { get; }
    IUserRepository Users { get; }
    IRoleRepository Roles { get; }
    
    // =============================================
    // TRANSACTION METHODS
    // =============================================
    
    /// <summary>
    /// Save all changes to the database
    /// Returns number of affected rows
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Begin a new transaction
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Commit current transaction
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Rollback current transaction
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}