// =============================================
// File: OlaVet.Infrastructure/Repositories/UnitOfWork.cs
// Unit of Work - coordinates all repositories and transactions
// =============================================

using Microsoft.EntityFrameworkCore.Storage;
using OlaVet.Domain.Interfaces;
using OlaVet.Infrastructure.Data;

namespace OlaVet.Infrastructure.Repositories;

/// <summary>
/// Unit of Work implementation - manages repositories and transactions
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly OlaVetDbContext _context;
    private IDbContextTransaction? _transaction;
    
    // =============================================
    // LAZY INITIALIZATION OF REPOSITORIES
    // =============================================
    // Repositories are created only when accessed (better performance)
    
    private IPetOwnerRepository? _petOwners;
    private IVetRepository? _vets;
    private IPetRepository? _pets;
    private IVetAppointmentRepository? _vetAppointments;
    private ILabRepository? _labs;
    private ILabAppointmentRepository? _labAppointments;
    private IStoreRepository? _stores;
    private IMedicineRepository? _medicines;
    private IMedicineOrderRepository? _medicineOrders;
    private IMedicalRecordRepository? _medicalRecords;
    private IPaymentRepository? _payments;
    private IReviewRepository? _reviews;
    private IUserRepository? _users;
    private IRoleRepository? _roles;
    
    public UnitOfWork(OlaVetDbContext context)
    {
        _context = context;
    }
    
    // =============================================
    // REPOSITORY PROPERTIES
    // =============================================
    // Lazy initialization pattern - create only when needed
    
    public IPetOwnerRepository PetOwners => 
        _petOwners ??= new PetOwnerRepository(_context);
    
    public IVetRepository Vets => 
        _vets ??= new VetRepository(_context);
    
    public IPetRepository Pets => 
        _pets ??= new PetRepository(_context);
    
    public IVetAppointmentRepository VetAppointments => 
        _vetAppointments ??= new VetAppointmentRepository(_context);
    
    public ILabRepository Labs => 
        _labs ??= new LabRepository(_context);
    
    public ILabAppointmentRepository LabAppointments => 
        _labAppointments ??= new LabAppointmentRepository(_context);
    
    public IStoreRepository Stores => 
        _stores ??= new StoreRepository(_context);
    
    public IMedicineRepository Medicines => 
        _medicines ??= new MedicineRepository(_context);
    
    public IMedicineOrderRepository MedicineOrders => 
        _medicineOrders ??= new MedicineOrderRepository(_context);
    
    public IMedicalRecordRepository MedicalRecords => 
        _medicalRecords ??= new MedicalRecordRepository(_context);
    
    public IPaymentRepository Payments => 
        _payments ??= new PaymentRepository(_context);
    
    public IReviewRepository Reviews => 
        _reviews ??= new ReviewRepository(_context);
    
    public IUserRepository Users => 
        _users ??= new UserRepository(_context);
    
    public IRoleRepository Roles => 
        _roles ??= new RoleRepository(_context);
    
    // =============================================
    // TRANSACTION METHODS
    // =============================================
    
    /// <summary>
    /// Save all changes to database
    /// This is the ONLY place where SaveChanges should be called
    /// </summary>
    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }
        catch (Exception ex)
        {
            // Log exception here (add logging in production)
            throw new Exception("Error saving changes to database", ex);
        }
    }
    
    /// <summary>
    /// Begin a new transaction
    /// </summary>
    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction != null)
        {
            throw new InvalidOperationException("Transaction already started");
        }
        
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }
    
    /// <summary>
    /// Commit current transaction
    /// </summary>
    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction to commit");
        }
        
        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
    
    /// <summary>
    /// Rollback current transaction
    /// </summary>
    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
        {
            throw new InvalidOperationException("No transaction to rollback");
        }
        
        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }
    
    // =============================================
    // DISPOSE PATTERN
    // =============================================
    
    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
