// =============================================
// File: OlaVet.Application/Security/Permissions.cs
// Constants for all permission names
// =============================================

namespace OlaVet.Application.Security;

/// <summary>
/// Static class containing all permission constants
/// Used for authorization checks throughout the application
/// </summary>
public static class Permissions
{
    // =============================================
    // PET OWNER PERMISSIONS
    // =============================================
    public const string PetOwnersRead = "petowners.read";
    public const string PetOwnersCreate = "petowners.create";
    public const string PetOwnersUpdate = "petowners.update";
    public const string PetOwnersDelete = "petowners.delete";
    
    // =============================================
    // PET PERMISSIONS
    // =============================================
    public const string PetsRead = "pets.read";
    public const string PetsCreate = "pets.create";
    public const string PetsUpdate = "pets.update";
    public const string PetsDelete = "pets.delete";
    
    // =============================================
    // VET PERMISSIONS
    // =============================================
    public const string VetsRead = "vets.read";
    public const string VetsCreate = "vets.create";
    public const string VetsUpdate = "vets.update";
    public const string VetsDelete = "vets.delete";
    public const string VetsManage = "vets.manage";
    
    // =============================================
    // APPOINTMENT PERMISSIONS
    // =============================================
    public const string AppointmentsRead = "appointments.read";
    public const string AppointmentsCreate = "appointments.create";
    public const string AppointmentsUpdate = "appointments.update";
    public const string AppointmentsCancel = "appointments.cancel";
    
    // =============================================
    // ORDER PERMISSIONS
    // =============================================
    public const string OrdersRead = "orders.read";
    public const string OrdersCreate = "orders.create";
    public const string OrdersUpdate = "orders.update";
    public const string OrdersCancel = "orders.cancel";
    
    // =============================================
    // REVIEW PERMISSIONS
    // =============================================
    public const string ReviewsRead = "reviews.read";
    public const string ReviewsCreate = "reviews.create";
    public const string ReviewsUpdate = "reviews.update";
    public const string ReviewsDelete = "reviews.delete";
    
    // =============================================
    // ADMIN PERMISSIONS
    // =============================================
    public const string AdminFullAccess = "admin.full";
    public const string AdminUserManagement = "admin.users";
    public const string AdminRoleManagement = "admin.roles";
    public const string AdminReports = "admin.reports";
    
    // =============================================
    // LAB PERMISSIONS
    // =============================================
    public const string LabsRead = "labs.read";
    public const string LabsCreate = "labs.create";
    public const string LabsUpdate = "labs.update";
    public const string LabsDelete = "labs.delete";
    
    // =============================================
    // STORE PERMISSIONS
    // =============================================
    public const string StoresRead = "stores.read";
    public const string StoresCreate = "stores.create";
    public const string StoresUpdate = "stores.update";
    public const string StoresDelete = "stores.delete";
}

/// <summary>
/// Predefined role names
/// </summary>
public static class RoleNames
{
    public const string Admin = "Admin";
    public const string Vet = "Vet";
    public const string PetOwner = "PetOwner";
    public const string LabTechnician = "LabTechnician";
    public const string StoreManager = "StoreManager";
}
