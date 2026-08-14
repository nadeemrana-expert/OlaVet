-- =============================================
-- File: Database/03_CreateSecurityTables.sql
-- Creates authentication & authorization tables
-- Run AFTER 01_CreateDatabase.sql
-- =============================================

USE OlaVet;
GO

-- =============================================
-- 1. APPLICATION USER TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'ApplicationUser')
BEGIN
    CREATE TABLE ApplicationUser (
        UserId          INT             IDENTITY(1,1) PRIMARY KEY,
        Email           NVARCHAR(256)   NOT NULL,
        PasswordHash    NVARCHAR(512)   NOT NULL,
        FirstName       NVARCHAR(100)   NOT NULL,
        LastName        NVARCHAR(100)   NOT NULL,
        PhoneNumber     NVARCHAR(20)    NULL,
        
        -- Account Status
        EmailConfirmed      BIT         NOT NULL DEFAULT 0,
        IsLockedOut         BIT         NOT NULL DEFAULT 0,
        LockoutEnd          DATETIME2   NULL,
        FailedLoginAttempts INT         NOT NULL DEFAULT 0,
        LastLoginDate       DATETIME2   NULL,
        
        -- MFA (future)
        TwoFactorEnabled    BIT         NOT NULL DEFAULT 0,
        TwoFactorSecret     NVARCHAR(512) NULL,
        
        -- GDPR
        GdprConsentGiven    BIT         NOT NULL DEFAULT 0,
        GdprConsentDate     DATETIME2   NULL,
        
        -- Linked entities
        PetOwnerId      INT         NULL,
        VetId            INT         NULL,
        
        -- Audit fields (from BaseEntity)
        CreatedDate     DATETIME2   NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate    DATETIME2   NULL,
        IsActive        BIT         NOT NULL DEFAULT 1,
        
        -- Foreign Keys
        CONSTRAINT FK_ApplicationUser_PetOwner FOREIGN KEY (PetOwnerId) REFERENCES PetOwner(PetOwnerId),
        CONSTRAINT FK_ApplicationUser_Vet FOREIGN KEY (VetId) REFERENCES Vet(VetId),
        
        -- Unique email
        CONSTRAINT UQ_ApplicationUser_Email UNIQUE (Email)
    );
    
    CREATE NONCLUSTERED INDEX IX_ApplicationUser_Email ON ApplicationUser(Email);
    PRINT 'Created ApplicationUser table';
END
GO

-- =============================================
-- 2. ROLE TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Role')
BEGIN
    CREATE TABLE Role (
        RoleId          INT             IDENTITY(1,1) PRIMARY KEY,
        Name            NVARCHAR(50)    NOT NULL,
        [Description]   NVARCHAR(200)   NULL,
        
        -- Audit
        CreatedDate     DATETIME2   NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate    DATETIME2   NULL,
        IsActive        BIT         NOT NULL DEFAULT 1,
        
        CONSTRAINT UQ_Role_Name UNIQUE (Name)
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX IX_Role_Name ON Role(Name);
    PRINT 'Created Role table';
END
GO

-- =============================================
-- 3. USER-ROLE JOIN TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'UserRole')
BEGIN
    CREATE TABLE UserRole (
        UserId          INT         NOT NULL,
        RoleId          INT         NOT NULL,
        AssignedDate    DATETIME2   NOT NULL DEFAULT GETUTCDATE(),
        
        CONSTRAINT PK_UserRole PRIMARY KEY (UserId, RoleId),
        CONSTRAINT FK_UserRole_User FOREIGN KEY (UserId) REFERENCES ApplicationUser(UserId) ON DELETE CASCADE,
        CONSTRAINT FK_UserRole_Role FOREIGN KEY (RoleId) REFERENCES Role(RoleId) ON DELETE CASCADE
    );
    PRINT 'Created UserRole table';
END
GO

-- =============================================
-- 4. PERMISSION TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Permission')
BEGIN
    CREATE TABLE Permission (
        PermissionId    INT             IDENTITY(1,1) PRIMARY KEY,
        Name            NVARCHAR(100)   NOT NULL,
        [Description]   NVARCHAR(300)   NULL,
        Category        NVARCHAR(50)    NULL,
        
        -- Audit
        CreatedDate     DATETIME2   NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate    DATETIME2   NULL,
        IsActive        BIT         NOT NULL DEFAULT 1,
        
        CONSTRAINT UQ_Permission_Name UNIQUE (Name)
    );
    
    CREATE UNIQUE NONCLUSTERED INDEX IX_Permission_Name ON Permission(Name);
    PRINT 'Created Permission table';
END
GO

-- =============================================
-- 5. ROLE-PERMISSION JOIN TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RolePermission')
BEGIN
    CREATE TABLE RolePermission (
        RoleId          INT     NOT NULL,
        PermissionId    INT     NOT NULL,
        
        CONSTRAINT PK_RolePermission PRIMARY KEY (RoleId, PermissionId),
        CONSTRAINT FK_RolePermission_Role FOREIGN KEY (RoleId) REFERENCES Role(RoleId) ON DELETE CASCADE,
        CONSTRAINT FK_RolePermission_Permission FOREIGN KEY (PermissionId) REFERENCES Permission(PermissionId) ON DELETE CASCADE
    );
    PRINT 'Created RolePermission table';
END
GO

-- =============================================
-- 6. REFRESH TOKEN TABLE
-- =============================================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RefreshToken')
BEGIN
    CREATE TABLE RefreshToken (
        RefreshTokenId  INT             IDENTITY(1,1) PRIMARY KEY,
        Token           NVARCHAR(512)   NOT NULL,
        ExpiresAt       DATETIME2       NOT NULL,
        CreatedAt       DATETIME2       NOT NULL DEFAULT GETUTCDATE(),
        CreatedByIp     NVARCHAR(50)    NULL,
        RevokedAt       DATETIME2       NULL,
        RevokedByIp     NVARCHAR(50)    NULL,
        ReplacedByToken NVARCHAR(512)   NULL,
        RevokeReason    NVARCHAR(256)   NULL,
        UserId          INT             NOT NULL,
        
        CONSTRAINT FK_RefreshToken_User FOREIGN KEY (UserId) REFERENCES ApplicationUser(UserId) ON DELETE CASCADE
    );
    
    CREATE NONCLUSTERED INDEX IX_RefreshToken_Token ON RefreshToken(Token);
    CREATE NONCLUSTERED INDEX IX_RefreshToken_UserId ON RefreshToken(UserId);
    PRINT 'Created RefreshToken table';
END
GO

PRINT '========================================';
PRINT 'Security tables created successfully!';
PRINT '========================================';
