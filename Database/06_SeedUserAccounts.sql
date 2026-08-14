-- =============================================
-- 06_SeedUserAccounts.sql
-- Creates ApplicationUser accounts for seeded domain entities
-- so they can log in to the system.
--
-- Uses a SINGLE BCrypt hash for default password "User@123!"
-- (Generated with work factor 12)
-- 
-- Creates accounts for:
--   - 50 PetOwners (first 50) with role PetOwner
--   - 20 Vets (first 20) with role Vet
--   - 10 Labs (first 10) with role LabTechnician
--   - 10 Stores (first 10) with role StoreManager
-- =============================================

USE OlaVet;
GO

PRINT '=== Seeding User Accounts for Domain Entities ===';

-- BCrypt hash for "User@123!" with work factor 12
DECLARE @defaultHash NVARCHAR(200) = '$2a$12$G3gnud079/6HUPRnoQCDvu9xN2Te1OOTzCq6zVg5r.9XGYNtojS6q';
DECLARE @now DATETIME2 = GETUTCDATE();

-- Get Role IDs
DECLARE @petOwnerRoleId INT, @vetRoleId INT, @labTechRoleId INT, @storeManagerRoleId INT;
SELECT @petOwnerRoleId = RoleId FROM Role WHERE Name = 'PetOwner';
SELECT @vetRoleId = RoleId FROM Role WHERE Name = 'Vet';
SELECT @labTechRoleId = RoleId FROM Role WHERE Name = 'LabTechnician';
SELECT @storeManagerRoleId = RoleId FROM Role WHERE Name = 'StoreManager';

PRINT 'Role IDs - PetOwner: ' + CAST(@petOwnerRoleId AS VARCHAR) 
    + ', Vet: ' + CAST(@vetRoleId AS VARCHAR)
    + ', LabTechnician: ' + CAST(@labTechRoleId AS VARCHAR)
    + ', StoreManager: ' + CAST(@storeManagerRoleId AS VARCHAR);

-- =============================================
-- PET OWNER ACCOUNTS (first 50)
-- Login: [their email from PetOwner table] / User@123!
-- =============================================

PRINT 'Creating PetOwner user accounts...';

INSERT INTO ApplicationUser (Email, PasswordHash, FirstName, LastName, PhoneNumber, 
    EmailConfirmed, IsLockedOut, FailedLoginAttempts, TwoFactorEnabled, 
    GdprConsentGiven, GdprConsentDate, PetOwnerId, VetId, IsActive, CreatedDate, ModifiedDate)
SELECT TOP 50
    po.Email,
    @defaultHash,
    LEFT(po.OwnerName, CHARINDEX(' ', po.OwnerName + ' ') - 1),  -- FirstName
    CASE 
        WHEN CHARINDEX(' ', po.OwnerName) > 0 
        THEN SUBSTRING(po.OwnerName, CHARINDEX(' ', po.OwnerName) + 1, 100) 
        ELSE '' 
    END,  -- LastName
    po.ContactNumber,
    1,  -- EmailConfirmed
    0,  -- IsLockedOut
    0,  -- FailedLoginAttempts
    0,  -- TwoFactorEnabled
    1,  -- GdprConsentGiven
    @now,  -- GdprConsentDate
    po.PetOwnerId,  -- Link to PetOwner
    NULL,  -- VetId
    1,  -- IsActive
    @now,
    @now
FROM PetOwner po
WHERE po.IsActive = 1
    AND NOT EXISTS (
        SELECT 1 FROM ApplicationUser au WHERE au.Email = po.Email
    )
ORDER BY po.PetOwnerId;

DECLARE @petOwnerCount INT = @@ROWCOUNT;
PRINT 'Created ' + CAST(@petOwnerCount AS VARCHAR) + ' PetOwner accounts';

-- Assign PetOwner role
INSERT INTO UserRole (UserId, RoleId, AssignedDate)
SELECT au.UserId, @petOwnerRoleId, @now
FROM ApplicationUser au
INNER JOIN PetOwner po ON au.PetOwnerId = po.PetOwnerId
WHERE NOT EXISTS (
    SELECT 1 FROM UserRole ur WHERE ur.UserId = au.UserId AND ur.RoleId = @petOwnerRoleId
);

-- =============================================
-- VET ACCOUNTS (first 20)
-- Login: [their email from Vet table] / User@123!
-- =============================================

PRINT 'Creating Vet user accounts...';

INSERT INTO ApplicationUser (Email, PasswordHash, FirstName, LastName, PhoneNumber,
    EmailConfirmed, IsLockedOut, FailedLoginAttempts, TwoFactorEnabled,
    GdprConsentGiven, GdprConsentDate, PetOwnerId, VetId, IsActive, CreatedDate, ModifiedDate)
SELECT TOP 20
    v.Email,
    @defaultHash,
    LEFT(v.VetName, CHARINDEX(' ', v.VetName + ' ') - 1),
    CASE 
        WHEN CHARINDEX(' ', v.VetName) > 0 
        THEN SUBSTRING(v.VetName, CHARINDEX(' ', v.VetName) + 1, 100) 
        ELSE '' 
    END,
    v.ContactNumber,
    1, 0, 0, 0, 1, @now,
    NULL,  -- PetOwnerId
    v.VetId,  -- Link to Vet
    1, @now, @now
FROM Vet v
WHERE v.IsActive = 1
    AND v.Email IS NOT NULL
    AND NOT EXISTS (
        SELECT 1 FROM ApplicationUser au WHERE au.Email = v.Email
    )
ORDER BY v.VetId;

DECLARE @vetCount INT = @@ROWCOUNT;
PRINT 'Created ' + CAST(@vetCount AS VARCHAR) + ' Vet accounts';

-- Assign Vet role
INSERT INTO UserRole (UserId, RoleId, AssignedDate)
SELECT au.UserId, @vetRoleId, @now
FROM ApplicationUser au
INNER JOIN Vet v ON au.VetId = v.VetId
WHERE NOT EXISTS (
    SELECT 1 FROM UserRole ur WHERE ur.UserId = au.UserId AND ur.RoleId = @vetRoleId
);

-- =============================================
-- LAB TECHNICIAN ACCOUNTS (first 10 labs)
-- Login: lab[labId]@olavet.com / User@123!
-- =============================================

PRINT 'Creating LabTechnician user accounts...';

-- Labs don't have emails, so we generate them
INSERT INTO ApplicationUser (Email, PasswordHash, FirstName, LastName, PhoneNumber,
    EmailConfirmed, IsLockedOut, FailedLoginAttempts, TwoFactorEnabled,
    GdprConsentGiven, GdprConsentDate, PetOwnerId, VetId, IsActive, CreatedDate, ModifiedDate)
SELECT TOP 10
    'lab' + CAST(l.LabId AS VARCHAR) + '@olavet.com',
    @defaultHash,
    'Lab Tech',
    l.LabName,
    l.ContactNumber,
    1, 0, 0, 0, 1, @now,
    NULL, NULL,
    1, @now, @now
FROM Lab l
WHERE l.IsActive = 1
    AND NOT EXISTS (
        SELECT 1 FROM ApplicationUser au WHERE au.Email = 'lab' + CAST(l.LabId AS VARCHAR) + '@olavet.com'
    )
ORDER BY l.LabId;

DECLARE @labCount INT = @@ROWCOUNT;
PRINT 'Created ' + CAST(@labCount AS VARCHAR) + ' LabTechnician accounts';

-- Assign LabTechnician role
INSERT INTO UserRole (UserId, RoleId, AssignedDate)
SELECT au.UserId, @labTechRoleId, @now
FROM ApplicationUser au
WHERE au.Email LIKE 'lab%@olavet.com'
    AND NOT EXISTS (
        SELECT 1 FROM UserRole ur WHERE ur.UserId = au.UserId AND ur.RoleId = @labTechRoleId
    );

-- =============================================
-- STORE MANAGER ACCOUNTS (first 10 stores)
-- Login: store[storeId]@olavet.com / User@123!
-- =============================================

PRINT 'Creating StoreManager user accounts...';

INSERT INTO ApplicationUser (Email, PasswordHash, FirstName, LastName, PhoneNumber,
    EmailConfirmed, IsLockedOut, FailedLoginAttempts, TwoFactorEnabled,
    GdprConsentGiven, GdprConsentDate, PetOwnerId, VetId, IsActive, CreatedDate, ModifiedDate)
SELECT TOP 10
    'store' + CAST(s.StoreId AS VARCHAR) + '@olavet.com',
    @defaultHash,
    'Store Manager',
    s.StoreName,
    s.ContactNumber,
    1, 0, 0, 0, 1, @now,
    NULL, NULL,
    1, @now, @now
FROM Store s
WHERE s.IsActive = 1
    AND NOT EXISTS (
        SELECT 1 FROM ApplicationUser au WHERE au.Email = 'store' + CAST(s.StoreId AS VARCHAR) + '@olavet.com'
    )
ORDER BY s.StoreId;

DECLARE @storeCount INT = @@ROWCOUNT;
PRINT 'Created ' + CAST(@storeCount AS VARCHAR) + ' StoreManager accounts';

-- Assign StoreManager role
INSERT INTO UserRole (UserId, RoleId, AssignedDate)
SELECT au.UserId, @storeManagerRoleId, @now
FROM ApplicationUser au
WHERE au.Email LIKE 'store%@olavet.com'
    AND NOT EXISTS (
        SELECT 1 FROM UserRole ur WHERE ur.UserId = au.UserId AND ur.RoleId = @storeManagerRoleId
    );

-- =============================================
-- ASSIGN PERMISSIONS to new user roles
-- (RolePermissions already exist from 04_SeedSecurityData.sql)
-- =============================================

PRINT '';
PRINT '=== Summary ===';

DECLARE @totalUsers INT;
SELECT @totalUsers = COUNT(*) FROM ApplicationUser;
PRINT 'Total user accounts: ' + CAST(@totalUsers AS VARCHAR);
PRINT '';
PRINT 'Sample logins:';
PRINT '  Admin:        admin@olavet.com / Admin@123!';

-- Show first PetOwner login
DECLARE @samplePO NVARCHAR(200);
SELECT TOP 1 @samplePO = au.Email FROM ApplicationUser au 
INNER JOIN PetOwner po ON au.PetOwnerId = po.PetOwnerId
ORDER BY au.UserId;
IF @samplePO IS NOT NULL
    PRINT '  PetOwner:     ' + @samplePO + ' / User@123!';

-- Show first Vet login
DECLARE @sampleVet NVARCHAR(200);
SELECT TOP 1 @sampleVet = au.Email FROM ApplicationUser au 
INNER JOIN Vet v ON au.VetId = v.VetId
ORDER BY au.UserId;
IF @sampleVet IS NOT NULL
    PRINT '  Vet:          ' + @sampleVet + ' / User@123!';

PRINT '  LabTech:      lab1@olavet.com / User@123!';
PRINT '  StoreManager: store1@olavet.com / User@123!';
PRINT '';
PRINT '=== User Account Seeding Complete ===';
GO
