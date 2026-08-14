-- =============================================
-- File: Database/04_SeedSecurityData.sql
-- Seeds roles and permissions for RBAC
-- Run AFTER 03_CreateSecurityTables.sql
-- =============================================

USE OlaVet;
GO

-- =============================================
-- 1. SEED ROLES
-- =============================================
PRINT 'Seeding roles...';

IF NOT EXISTS (SELECT 1 FROM Role WHERE Name = 'Admin')
    INSERT INTO Role (Name, [Description]) VALUES ('Admin', 'Full system administrator');
    
IF NOT EXISTS (SELECT 1 FROM Role WHERE Name = 'Vet')
    INSERT INTO Role (Name, [Description]) VALUES ('Vet', 'Veterinarian user');
    
IF NOT EXISTS (SELECT 1 FROM Role WHERE Name = 'PetOwner')
    INSERT INTO Role (Name, [Description]) VALUES ('PetOwner', 'Pet owner / customer');
    
IF NOT EXISTS (SELECT 1 FROM Role WHERE Name = 'LabTechnician')
    INSERT INTO Role (Name, [Description]) VALUES ('LabTechnician', 'Laboratory technician');
    
IF NOT EXISTS (SELECT 1 FROM Role WHERE Name = 'StoreManager')
    INSERT INTO Role (Name, [Description]) VALUES ('StoreManager', 'Store / pharmacy manager');

GO

-- =============================================
-- 2. SEED PERMISSIONS
-- =============================================
PRINT 'Seeding permissions...';

-- Pet Owner Permissions
INSERT INTO Permission (Name, [Description], Category) VALUES 
    ('petowners.read', 'View pet owner profiles', 'PetOwners'),
    ('petowners.create', 'Create pet owner profiles', 'PetOwners'),
    ('petowners.update', 'Update pet owner profiles', 'PetOwners'),
    ('petowners.delete', 'Delete pet owner profiles', 'PetOwners');

-- Pet Permissions
INSERT INTO Permission (Name, [Description], Category) VALUES 
    ('pets.read', 'View pet information', 'Pets'),
    ('pets.create', 'Register new pets', 'Pets'),
    ('pets.update', 'Update pet information', 'Pets'),
    ('pets.delete', 'Remove pets', 'Pets');

-- Vet Permissions
INSERT INTO Permission (Name, [Description], Category) VALUES 
    ('vets.read', 'View vet profiles', 'Vets'),
    ('vets.create', 'Create vet profiles', 'Vets'),
    ('vets.update', 'Update vet profiles', 'Vets'),
    ('vets.delete', 'Delete vet profiles', 'Vets'),
    ('vets.manage', 'Full vet management', 'Vets');

-- Appointment Permissions
INSERT INTO Permission (Name, [Description], Category) VALUES 
    ('appointments.read', 'View appointments', 'Appointments'),
    ('appointments.create', 'Book appointments', 'Appointments'),
    ('appointments.update', 'Modify appointments', 'Appointments'),
    ('appointments.cancel', 'Cancel appointments', 'Appointments');

-- Order Permissions
INSERT INTO Permission (Name, [Description], Category) VALUES 
    ('orders.read', 'View orders', 'Orders'),
    ('orders.create', 'Place orders', 'Orders'),
    ('orders.update', 'Modify orders', 'Orders'),
    ('orders.cancel', 'Cancel orders', 'Orders');

-- Review Permissions
INSERT INTO Permission (Name, [Description], Category) VALUES 
    ('reviews.read', 'View reviews', 'Reviews'),
    ('reviews.create', 'Write reviews', 'Reviews'),
    ('reviews.update', 'Edit own reviews', 'Reviews'),
    ('reviews.delete', 'Delete reviews', 'Reviews');

-- Lab Permissions
INSERT INTO Permission (Name, [Description], Category) VALUES 
    ('labs.read', 'View lab information', 'Labs'),
    ('labs.create', 'Create lab records', 'Labs'),
    ('labs.update', 'Update lab records', 'Labs'),
    ('labs.delete', 'Delete lab records', 'Labs');

-- Store Permissions
INSERT INTO Permission (Name, [Description], Category) VALUES 
    ('stores.read', 'View store information', 'Stores'),
    ('stores.create', 'Create store listings', 'Stores'),
    ('stores.update', 'Update store listings', 'Stores'),
    ('stores.delete', 'Delete store listings', 'Stores');

-- Admin Permissions
INSERT INTO Permission (Name, [Description], Category) VALUES 
    ('admin.full', 'Full admin access - bypasses all checks', 'Admin'),
    ('admin.users', 'User management', 'Admin'),
    ('admin.roles', 'Role management', 'Admin'),
    ('admin.reports', 'View reports and analytics', 'Admin');

GO

-- =============================================
-- 3. ASSIGN PERMISSIONS TO ROLES
-- =============================================
PRINT 'Assigning permissions to roles...';

-- ADMIN: Gets ALL permissions
INSERT INTO RolePermission (RoleId, PermissionId)
SELECT r.RoleId, p.PermissionId 
FROM Role r, Permission p 
WHERE r.Name = 'Admin';

-- VET: Relevant vet permissions
INSERT INTO RolePermission (RoleId, PermissionId)
SELECT r.RoleId, p.PermissionId 
FROM Role r, Permission p 
WHERE r.Name = 'Vet' 
AND p.Name IN (
    'vets.read', 'vets.update',
    'pets.read',
    'appointments.read', 'appointments.update',
    'reviews.read',
    'labs.read'
);

-- PET OWNER: Relevant pet owner permissions
INSERT INTO RolePermission (RoleId, PermissionId)
SELECT r.RoleId, p.PermissionId 
FROM Role r, Permission p 
WHERE r.Name = 'PetOwner' 
AND p.Name IN (
    'petowners.read', 'petowners.update',
    'pets.read', 'pets.create', 'pets.update',
    'vets.read',
    'appointments.read', 'appointments.create', 'appointments.cancel',
    'orders.read', 'orders.create', 'orders.cancel',
    'reviews.read', 'reviews.create', 'reviews.update',
    'labs.read',
    'stores.read'
);

-- LAB TECHNICIAN
INSERT INTO RolePermission (RoleId, PermissionId)
SELECT r.RoleId, p.PermissionId 
FROM Role r, Permission p 
WHERE r.Name = 'LabTechnician' 
AND p.Name IN (
    'labs.read', 'labs.create', 'labs.update',
    'pets.read',
    'appointments.read',
    'reviews.read'
);

-- STORE MANAGER
INSERT INTO RolePermission (RoleId, PermissionId)
SELECT r.RoleId, p.PermissionId 
FROM Role r, Permission p 
WHERE r.Name = 'StoreManager' 
AND p.Name IN (
    'stores.read', 'stores.create', 'stores.update', 'stores.delete',
    'orders.read', 'orders.update',
    'reviews.read'
);

GO

-- =============================================
-- 4. CREATE DEFAULT ADMIN USER
-- Password: Admin@123! (BCrypt hashed)
-- =============================================
PRINT 'Creating default admin user...';

IF NOT EXISTS (SELECT 1 FROM ApplicationUser WHERE Email = 'admin@olavet.com')
BEGIN
    INSERT INTO ApplicationUser (
        Email, 
        PasswordHash, 
        FirstName, 
        LastName, 
        EmailConfirmed, 
        GdprConsentGiven, 
        GdprConsentDate
    ) VALUES (
        'admin@olavet.com',
        -- BCrypt hash of 'Admin@123!' (work factor 12)
        '$2a$12$vBMWoTaEsMQGxUmEQ4WN8.LIanjO6PKoHO6tVy2sacJ8dwSUe6zYK',
        'System',
        'Admin',
        1,
        1,
        GETUTCDATE()
    );
    
    -- Assign Admin role
    INSERT INTO UserRole (UserId, RoleId)
    SELECT u.UserId, r.RoleId
    FROM ApplicationUser u, Role r
    WHERE u.Email = 'admin@olavet.com' AND r.Name = 'Admin';
    
    PRINT 'Default admin user created: admin@olavet.com';
END

GO

PRINT '========================================';
PRINT 'Security data seeded successfully!';
PRINT '========================================';
PRINT '';
PRINT 'Default Admin Credentials:';
PRINT '  Email: admin@olavet.com';
PRINT '  Password: Admin@123!';
PRINT '  (Change immediately in production!)';
PRINT '========================================';
