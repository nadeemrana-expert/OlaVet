// =============================================
// File: OlaVet.Infrastructure/Data/OlaVetDbContext.cs
// The heart of EF Core - manages all database operations
// =============================================

using Microsoft.EntityFrameworkCore;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Entities.Lookups;

namespace OlaVet.Infrastructure.Data;

/// <summary>
/// OlaVet Database Context - manages all entities and database operations
/// </summary>
public class OlaVetDbContext : DbContext
{
    // =============================================
    // CONSTRUCTOR
    // =============================================
    
    /// <summary>
    /// Constructor accepting DbContextOptions
    /// This allows dependency injection and configuration
    /// </summary>
    public OlaVetDbContext(DbContextOptions<OlaVetDbContext> options) : base(options)
    {
    }
    
    // =============================================
    // DBSET PROPERTIES (Tables)
    // =============================================
    // Each DbSet represents a table in the database
    // The property name determines the table name by convention
    
    #region Lookup Tables
    
    /// <summary>
    /// Medicine Types lookup table
    /// </summary>
    public DbSet<MedicineType> MedicineTypes => Set<MedicineType>();
    
    /// <summary>
    /// Record Types lookup table
    /// </summary>
    public DbSet<RecordType> RecordTypes => Set<RecordType>();
    
    /// <summary>
    /// Vet Appointment Types lookup table
    /// </summary>
    public DbSet<VetAppointmentType> VetAppointmentTypes => Set<VetAppointmentType>();
    
    /// <summary>
    /// Status Types lookup table
    /// </summary>
    public DbSet<StatusType> StatusTypes => Set<StatusType>();
    
    #endregion
    
    #region Core Entities
    
    /// <summary>
    /// Pet Owners (Customers)
    /// </summary>
    public DbSet<PetOwner> PetOwners => Set<PetOwner>();
    
    /// <summary>
    /// Veterinarians
    /// </summary>
    public DbSet<Vet> Vets => Set<Vet>();
    
    /// <summary>
    /// Pets (Patients)
    /// </summary>
    public DbSet<Pet> Pets => Set<Pet>();
    
    /// <summary>
    /// Laboratories
    /// </summary>
    public DbSet<Lab> Labs => Set<Lab>();
    
    /// <summary>
    /// Lab Tests catalog
    /// </summary>
    public DbSet<LabTest> LabTests => Set<LabTest>();
    
    /// <summary>
    /// Stores (Pharmacies)
    /// </summary>
    public DbSet<Store> Stores => Set<Store>();
    
    /// <summary>
    /// Medicine catalog
    /// </summary>
    public DbSet<Medicine> Medicines => Set<Medicine>();
    
    #endregion
    
    #region Supporting Entities
    
    /// <summary>
    /// Vet education qualifications
    /// </summary>
    public DbSet<EducationQualification> EducationQualifications => Set<EducationQualification>();
    
    /// <summary>
    /// Services offered by vets
    /// </summary>
    public DbSet<Service> Services => Set<Service>();
    
    /// <summary>
    /// Vet availability schedules
    /// </summary>
    public DbSet<VetAvailability> VetAvailabilities => Set<VetAvailability>();
    
    /// <summary>
    /// Store inventory (medicine stock)
    /// </summary>
    public DbSet<Inventory> Inventories => Set<Inventory>();
    
    #endregion
    
    #region Transactional Entities
    
    /// <summary>
    /// Vet appointments
    /// </summary>
    public DbSet<VetAppointment> VetAppointments => Set<VetAppointment>();
    
    /// <summary>
    /// Lab appointments
    /// </summary>
    public DbSet<LabAppointment> LabAppointments => Set<LabAppointment>();
    
    /// <summary>
    /// Lab appointment tests (tests within an appointment)
    /// </summary>
    public DbSet<LabAppointmentTest> LabAppointmentTests => Set<LabAppointmentTest>();
    
    /// <summary>
    /// Medicine orders (order headers)
    /// </summary>
    public DbSet<MedicineOrder> MedicineOrders => Set<MedicineOrder>();
    
    /// <summary>
    /// Medicine order details (order line items)
    /// </summary>
    public DbSet<MedicineOrderDetail> MedicineOrderDetails => Set<MedicineOrderDetail>();
    
    /// <summary>
    /// Medical records (pet medical history)
    /// </summary>
    public DbSet<MedicalRecord> MedicalRecords => Set<MedicalRecord>();
    
    #endregion
    
    #region Payment Entities
    
    /// <summary>
    /// Vet service payments
    /// </summary>
    public DbSet<VetPayment> VetPayments => Set<VetPayment>();
    
    /// <summary>
    /// Lab service payments
    /// </summary>
    public DbSet<LabPayment> LabPayments => Set<LabPayment>();
    
    /// <summary>
    /// Store/medicine order payments
    /// </summary>
    public DbSet<StorePayment> StorePayments => Set<StorePayment>();
    
    #endregion
    
    #region Review Entities
    
    /// <summary>
    /// Vet reviews
    /// </summary>
    public DbSet<VetReview> VetReviews => Set<VetReview>();
    
    /// <summary>
    /// Lab reviews
    /// </summary>
    public DbSet<LabReview> LabReviews => Set<LabReview>();
    
    /// <summary>
    /// Store reviews
    /// </summary>
    public DbSet<StoreReview> StoreReviews => Set<StoreReview>();
    
    #endregion
    
    #region Security / Auth Entities
    
    /// <summary>
    /// Application Users (authentication)
    /// </summary>
    public DbSet<ApplicationUser> Users => Set<ApplicationUser>();
    
    /// <summary>
    /// Roles for RBAC
    /// </summary>
    public DbSet<Role> Roles => Set<Role>();
    
    /// <summary>
    /// User-Role assignments
    /// </summary>
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    
    /// <summary>
    /// Permissions for granular access control
    /// </summary>
    public DbSet<Permission> Permissions => Set<Permission>();
    
    /// <summary>
    /// Role-Permission assignments
    /// </summary>
    public DbSet<RolePermission> RolePermissions => Set<RolePermission>();
    
    /// <summary>
    /// Refresh tokens for JWT rotation
    /// </summary>
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
    
    #endregion
    
    // =============================================
    // MODEL CONFIGURATION
    // =============================================
    
    /// <summary>
    /// Configure entity models using Fluent API
    /// This method is called once when the model is being created
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Apply all entity configurations from separate classes
        // This keeps DbContext clean and organizes configuration by entity
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(OlaVetDbContext).Assembly);
        
        // LEARNING NOTE: Why ApplyConfigurationsFromAssembly?
        // Instead of writing all configuration here (would be 1000+ lines),
        // we create separate configuration classes (next step).
        // This method automatically finds and applies them all.
    }
    
    // =============================================
    // OPTIONAL: Override SaveChanges for Audit Trail
    // =============================================
    
    /// <summary>
    /// Override SaveChanges to automatically set audit fields
    /// </summary>
    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }
    
    /// <summary>
    /// Override SaveChangesAsync to automatically set audit fields
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }
    
    /// <summary>
    /// Automatically update ModifiedDate for changed entities
    /// </summary>
    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Modified);
        
        foreach (var entry in entries)
        {
            // Check if entity has ModifiedDate property
            var modifiedDateProperty = entry.Properties
                .FirstOrDefault(p => p.Metadata.Name == nameof(Domain.Common.BaseEntity.ModifiedDate));
            
            if (modifiedDateProperty != null)
            {
                modifiedDateProperty.CurrentValue = DateTime.UtcNow;
            }
        }
    }
}
