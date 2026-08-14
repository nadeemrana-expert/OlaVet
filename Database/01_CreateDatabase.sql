-- =============================================
-- OlaVet Database Creation Script
-- Version: 1.0
-- Description: Creates OlaVet database schema
-- =============================================

-- Create Database (run on master)
USE master;
GO

IF NOT EXISTS (SELECT * FROM sys.databases WHERE name = 'OlaVet')
BEGIN
    CREATE DATABASE OlaVet;
END;
GO

USE OlaVet;
GO

-- =============================================
-- LOOKUP TABLES
-- =============================================

-- Medicine Types
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MedicineType')
BEGIN
    CREATE TABLE MedicineType (
        MedicineTypeId INT IDENTITY(1,1) PRIMARY KEY,
        TypeName NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500),
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2
    );
END;
GO

-- Record Types
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RecordType')
BEGIN
    CREATE TABLE RecordType (
        RecordTypeId INT IDENTITY(1,1) PRIMARY KEY,
        TypeName NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500),
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2
    );
END;
GO

-- Vet Appointment Types
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'VetAppointmentType')
BEGIN
    CREATE TABLE VetAppointmentType (
        VetAppointmentTypeId INT IDENTITY(1,1) PRIMARY KEY,
        TypeName NVARCHAR(100) NOT NULL,
        Description NVARCHAR(500),
        DefaultDuration INT NOT NULL DEFAULT 30,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2
    );
END;
GO

-- Status Types
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'StatusType')
BEGIN
    CREATE TABLE StatusType (
        StatusTypeId INT IDENTITY(1,1) PRIMARY KEY,
        StatusName NVARCHAR(100) NOT NULL,
        AppliesTo NVARCHAR(100),
        Description NVARCHAR(500),
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2
    );
END;
GO

-- =============================================
-- CORE TABLES
-- =============================================

-- Pet Owners
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'PetOwner')
BEGIN
    CREATE TABLE PetOwner (
        PetOwnerId INT IDENTITY(1,1) PRIMARY KEY,
        OwnerName NVARCHAR(200) NOT NULL,
        Email NVARCHAR(255) NOT NULL,
        ContactNumber NVARCHAR(50) NOT NULL,
        HomeAddress NVARCHAR(500),
        Age INT,
        Gender NVARCHAR(20),
        Wallet DECIMAL(18,2) NOT NULL DEFAULT 0,
        RegistrationDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        IsActive BIT NOT NULL DEFAULT 1,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2,
        DeletedDate DATETIME2,
        CONSTRAINT UQ_PetOwner_Email UNIQUE (Email)
    );
    CREATE NONCLUSTERED INDEX IX_PetOwner_Email ON PetOwner(Email);
    CREATE NONCLUSTERED INDEX IX_PetOwner_IsActive ON PetOwner(IsActive) WHERE IsDeleted = 0;
END;
GO

-- Veterinarians
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Vet')
BEGIN
    CREATE TABLE Vet (
        VetId INT IDENTITY(1,1) PRIMARY KEY,
        VetName NVARCHAR(200) NOT NULL,
        Specialization NVARCHAR(200),
        ClinicLocation NVARCHAR(500),
        Fee DECIMAL(18,2) NOT NULL DEFAULT 0,
        ContactNumber NVARCHAR(50) NOT NULL,
        Email NVARCHAR(255),
        YearsOfExperience INT,
        LicenseNumber NVARCHAR(100),
        IsActive BIT NOT NULL DEFAULT 1,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2,
        DeletedDate DATETIME2
    );
    CREATE NONCLUSTERED INDEX IX_Vet_Specialization ON Vet(Specialization);
    CREATE NONCLUSTERED INDEX IX_Vet_IsActive ON Vet(IsActive) WHERE IsDeleted = 0;
END;
GO

-- Pets
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Pet')
BEGIN
    CREATE TABLE Pet (
        PetId INT IDENTITY(1,1) PRIMARY KEY,
        PetOwnerId INT NOT NULL,
        Name NVARCHAR(200) NOT NULL,
        Species NVARCHAR(100) NOT NULL,
        Breed NVARCHAR(100),
        Age INT,
        PetWeight DECIMAL(10,2),
        Color NVARCHAR(50),
        Gender NVARCHAR(20),
        DateOfBirth DATE,
        RegistrationDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        IsActive BIT NOT NULL DEFAULT 1,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2,
        DeletedDate DATETIME2,
        CONSTRAINT FK_Pet_PetOwner FOREIGN KEY (PetOwnerId) REFERENCES PetOwner(PetOwnerId)
    );
    CREATE NONCLUSTERED INDEX IX_Pet_PetOwnerId ON Pet(PetOwnerId);
    CREATE NONCLUSTERED INDEX IX_Pet_Species ON Pet(Species);
END;
GO

-- Labs
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Lab')
BEGIN
    CREATE TABLE Lab (
        LabId INT IDENTITY(1,1) PRIMARY KEY,
        LabName NVARCHAR(200) NOT NULL,
        LabAddress NVARCHAR(500),
        ContactNumber NVARCHAR(50) NOT NULL,
        Email NVARCHAR(255),
        Specialization NVARCHAR(200),
        IsActive BIT NOT NULL DEFAULT 1,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2,
        DeletedDate DATETIME2
    );
END;
GO

-- Lab Tests
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LabTest')
BEGIN
    CREATE TABLE LabTest (
        LabTestId INT IDENTITY(1,1) PRIMARY KEY,
        LabTestName NVARCHAR(200) NOT NULL,
        Description NVARCHAR(500),
        TestFee DECIMAL(18,2) NOT NULL DEFAULT 0,
        TurnaroundTimeHours INT,
        IsActive BIT NOT NULL DEFAULT 1,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2,
        DeletedDate DATETIME2
    );
END;
GO

-- Stores (Pharmacies)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Store')
BEGIN
    CREATE TABLE Store (
        StoreId INT IDENTITY(1,1) PRIMARY KEY,
        StoreName NVARCHAR(200) NOT NULL,
        StoreAddress NVARCHAR(500),
        ContactNumber NVARCHAR(50) NOT NULL,
        Email NVARCHAR(255),
        OpeningTime TIME,
        ClosingTime TIME,
        IsActive BIT NOT NULL DEFAULT 1,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2,
        DeletedDate DATETIME2
    );
END;
GO

-- Medicines
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Medicine')
BEGIN
    CREATE TABLE Medicine (
        MedicineId INT IDENTITY(1,1) PRIMARY KEY,
        MedicineTypeId INT NOT NULL,
        MedicineName NVARCHAR(200) NOT NULL,
        Description NVARCHAR(500),
        Manufacturer NVARCHAR(200),
        DosageInstructions NVARCHAR(500),
        Price DECIMAL(18,2) NOT NULL DEFAULT 0,
        RequiresPrescription BIT NOT NULL DEFAULT 0,
        IsActive BIT NOT NULL DEFAULT 1,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2,
        DeletedDate DATETIME2,
        CONSTRAINT FK_Medicine_MedicineType FOREIGN KEY (MedicineTypeId) REFERENCES MedicineType(MedicineTypeId)
    );
    CREATE NONCLUSTERED INDEX IX_Medicine_MedicineTypeId ON Medicine(MedicineTypeId);
END;
GO

-- =============================================
-- SUPPORTING TABLES
-- =============================================

-- Education Qualifications
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'EducationQualification')
BEGIN
    CREATE TABLE EducationQualification (
        QualificationId INT IDENTITY(1,1) PRIMARY KEY,
        VetId INT NOT NULL,
        QualificationName NVARCHAR(200) NOT NULL,
        Institute NVARCHAR(300),
        YearOfDegree INT,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2,
        CONSTRAINT FK_EducationQualification_Vet FOREIGN KEY (VetId) REFERENCES Vet(VetId)
    );
    CREATE NONCLUSTERED INDEX IX_EducationQualification_VetId ON EducationQualification(VetId);
END;
GO

-- Services
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Services')
BEGIN
    CREATE TABLE Services (
        ServiceId INT IDENTITY(1,1) PRIMARY KEY,
        VetId INT NOT NULL,
        ServiceType NVARCHAR(200) NOT NULL,
        ServiceDescription NVARCHAR(500),
        ServiceFee DECIMAL(18,2) NOT NULL DEFAULT 0,
        DurationMinutes INT,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2,
        CONSTRAINT FK_Services_Vet FOREIGN KEY (VetId) REFERENCES Vet(VetId)
    );
    CREATE NONCLUSTERED INDEX IX_Services_VetId ON Services(VetId);
END;
GO

-- Vet Availability
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'VetAvailability')
BEGIN
    CREATE TABLE VetAvailability (
        VetAvailabilityId INT IDENTITY(1,1) PRIMARY KEY,
        VetId INT NOT NULL,
        DayOfWeek NVARCHAR(20) NOT NULL,
        StartTime TIME NOT NULL,
        EndTime TIME NOT NULL,
        IsAvailable BIT NOT NULL DEFAULT 1,
        SlotDurationMinutes INT NOT NULL DEFAULT 30,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2,
        CONSTRAINT FK_VetAvailability_Vet FOREIGN KEY (VetId) REFERENCES Vet(VetId)
    );
    CREATE NONCLUSTERED INDEX IX_VetAvailability_VetId ON VetAvailability(VetId);
END;
GO

-- Inventory
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Inventory')
BEGIN
    CREATE TABLE Inventory (
        InventoryId INT IDENTITY(1,1) PRIMARY KEY,
        StoreId INT NOT NULL,
        MedicineId INT NOT NULL,
        Quantity INT NOT NULL DEFAULT 0,
        ReorderLevel INT NOT NULL DEFAULT 10,
        LastRestocked DATETIME2,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2,
        CONSTRAINT FK_Inventory_Store FOREIGN KEY (StoreId) REFERENCES Store(StoreId),
        CONSTRAINT FK_Inventory_Medicine FOREIGN KEY (MedicineId) REFERENCES Medicine(MedicineId),
        CONSTRAINT UQ_Inventory_Store_Medicine UNIQUE (StoreId, MedicineId)
    );
    CREATE NONCLUSTERED INDEX IX_Inventory_StoreId ON Inventory(StoreId);
    CREATE NONCLUSTERED INDEX IX_Inventory_MedicineId ON Inventory(MedicineId);
END;
GO

-- =============================================
-- TRANSACTIONAL TABLES
-- =============================================

-- Vet Appointments
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'VetAppointment')
BEGIN
    CREATE TABLE VetAppointment (
        VetAppointmentId INT IDENTITY(1,1) PRIMARY KEY,
        PetId INT NOT NULL,
        PetOwnerId INT NOT NULL,
        VetId INT NOT NULL,
        VetAppointmentTypeId INT NOT NULL,
        AppointmentDateTime DATETIME2 NOT NULL,
        StatusTypeId INT NOT NULL,
        Reason NVARCHAR(500),
        Notes NVARCHAR(1000),
        CompletedDate DATETIME2,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2,
        CONSTRAINT FK_VetAppointment_Pet FOREIGN KEY (PetId) REFERENCES Pet(PetId),
        CONSTRAINT FK_VetAppointment_PetOwner FOREIGN KEY (PetOwnerId) REFERENCES PetOwner(PetOwnerId),
        CONSTRAINT FK_VetAppointment_Vet FOREIGN KEY (VetId) REFERENCES Vet(VetId),
        CONSTRAINT FK_VetAppointment_Type FOREIGN KEY (VetAppointmentTypeId) REFERENCES VetAppointmentType(VetAppointmentTypeId),
        CONSTRAINT FK_VetAppointment_Status FOREIGN KEY (StatusTypeId) REFERENCES StatusType(StatusTypeId)
    );
    CREATE NONCLUSTERED INDEX IX_VetAppointment_PetId ON VetAppointment(PetId);
    CREATE NONCLUSTERED INDEX IX_VetAppointment_VetId ON VetAppointment(VetId);
    CREATE NONCLUSTERED INDEX IX_VetAppointment_DateTime ON VetAppointment(AppointmentDateTime);
END;
GO

-- Lab Appointments
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LabAppointment')
BEGIN
    CREATE TABLE LabAppointment (
        LabAppointmentId INT IDENTITY(1,1) PRIMARY KEY,
        PetId INT NOT NULL,
        PetOwnerId INT NOT NULL,
        LabId INT NOT NULL,
        AppointmentDateTime DATETIME2 NOT NULL,
        StatusTypeId INT NOT NULL,
        Notes NVARCHAR(1000),
        CompletedDate DATETIME2,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2,
        CONSTRAINT FK_LabAppointment_Pet FOREIGN KEY (PetId) REFERENCES Pet(PetId),
        CONSTRAINT FK_LabAppointment_PetOwner FOREIGN KEY (PetOwnerId) REFERENCES PetOwner(PetOwnerId),
        CONSTRAINT FK_LabAppointment_Lab FOREIGN KEY (LabId) REFERENCES Lab(LabId),
        CONSTRAINT FK_LabAppointment_Status FOREIGN KEY (StatusTypeId) REFERENCES StatusType(StatusTypeId)
    );
    CREATE NONCLUSTERED INDEX IX_LabAppointment_LabId ON LabAppointment(LabId);
    CREATE NONCLUSTERED INDEX IX_LabAppointment_DateTime ON LabAppointment(AppointmentDateTime);
END;
GO

-- Lab Appointment Tests
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LabAppointmentTest')
BEGIN
    CREATE TABLE LabAppointmentTest (
        LabAppointmentTestId INT IDENTITY(1,1) PRIMARY KEY,
        LabAppointmentId INT NOT NULL,
        LabTestId INT NOT NULL,
        TestResult NVARCHAR(MAX),
        ResultDate DATETIME2,
        ResultFile NVARCHAR(500),
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2,
        CONSTRAINT FK_LabAppointmentTest_LabAppointment FOREIGN KEY (LabAppointmentId) REFERENCES LabAppointment(LabAppointmentId),
        CONSTRAINT FK_LabAppointmentTest_LabTest FOREIGN KEY (LabTestId) REFERENCES LabTest(LabTestId)
    );
    CREATE NONCLUSTERED INDEX IX_LabAppointmentTest_LabAppointmentId ON LabAppointmentTest(LabAppointmentId);
END;
GO

-- Medical Records
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MedicalRecord')
BEGIN
    CREATE TABLE MedicalRecord (
        RecordId INT IDENTITY(1,1) PRIMARY KEY,
        PetId INT NOT NULL,
        PetOwnerId INT NOT NULL,
        RecordTypeId INT NOT NULL,
        RecordDate DATETIME2 NOT NULL,
        Diagnosis NVARCHAR(1000),
        TreatmentDescription NVARCHAR(2000),
        VetId INT,
        AttachmentPath NVARCHAR(500),
        IsActive BIT NOT NULL DEFAULT 1,
        IsDeleted BIT NOT NULL DEFAULT 0,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2,
        DeletedDate DATETIME2,
        CONSTRAINT FK_MedicalRecord_Pet FOREIGN KEY (PetId) REFERENCES Pet(PetId),
        CONSTRAINT FK_MedicalRecord_PetOwner FOREIGN KEY (PetOwnerId) REFERENCES PetOwner(PetOwnerId),
        CONSTRAINT FK_MedicalRecord_RecordType FOREIGN KEY (RecordTypeId) REFERENCES RecordType(RecordTypeId),
        CONSTRAINT FK_MedicalRecord_Vet FOREIGN KEY (VetId) REFERENCES Vet(VetId)
    );
    CREATE NONCLUSTERED INDEX IX_MedicalRecord_PetId ON MedicalRecord(PetId);
    CREATE NONCLUSTERED INDEX IX_MedicalRecord_RecordDate ON MedicalRecord(RecordDate);
END;
GO

-- Medicine Orders
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MedicineOrder')
BEGIN
    CREATE TABLE MedicineOrder (
        MedicineOrderId INT IDENTITY(1,1) PRIMARY KEY,
        PetOwnerId INT NOT NULL,
        StoreId INT NOT NULL,
        OrderDateTime DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        StatusTypeId INT NOT NULL,
        TotalAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        DeliveryAddress NVARCHAR(500),
        DeliveredDate DATETIME2,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2,
        CONSTRAINT FK_MedicineOrder_PetOwner FOREIGN KEY (PetOwnerId) REFERENCES PetOwner(PetOwnerId),
        CONSTRAINT FK_MedicineOrder_Store FOREIGN KEY (StoreId) REFERENCES Store(StoreId),
        CONSTRAINT FK_MedicineOrder_Status FOREIGN KEY (StatusTypeId) REFERENCES StatusType(StatusTypeId)
    );
    CREATE NONCLUSTERED INDEX IX_MedicineOrder_PetOwnerId ON MedicineOrder(PetOwnerId);
    CREATE NONCLUSTERED INDEX IX_MedicineOrder_StoreId ON MedicineOrder(StoreId);
END;
GO

-- Medicine Order Details
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'MedicineOrderDetails')
BEGIN
    CREATE TABLE MedicineOrderDetails (
        OrderDetailId INT IDENTITY(1,1) PRIMARY KEY,
        MedicineOrderId INT NOT NULL,
        MedicineId INT NOT NULL,
        Quantity INT NOT NULL,
        UnitPrice DECIMAL(18,2) NOT NULL,
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2,
        CONSTRAINT FK_MedicineOrderDetails_Order FOREIGN KEY (MedicineOrderId) REFERENCES MedicineOrder(MedicineOrderId),
        CONSTRAINT FK_MedicineOrderDetails_Medicine FOREIGN KEY (MedicineId) REFERENCES Medicine(MedicineId)
    );
    CREATE NONCLUSTERED INDEX IX_MedicineOrderDetails_OrderId ON MedicineOrderDetails(MedicineOrderId);
END;
GO

-- =============================================
-- PAYMENT TABLES
-- =============================================

-- Vet Payments
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'VetPayment')
BEGIN
    CREATE TABLE VetPayment (
        VetPaymentId INT IDENTITY(1,1) PRIMARY KEY,
        VetAppointmentId INT NOT NULL,
        PetOwnerId INT NOT NULL,
        VetId INT NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        PaymentDateTime DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        PaymentMethod NVARCHAR(50) NOT NULL,
        TransactionId NVARCHAR(100),
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2,
        CONSTRAINT FK_VetPayment_VetAppointment FOREIGN KEY (VetAppointmentId) REFERENCES VetAppointment(VetAppointmentId),
        CONSTRAINT FK_VetPayment_PetOwner FOREIGN KEY (PetOwnerId) REFERENCES PetOwner(PetOwnerId),
        CONSTRAINT FK_VetPayment_Vet FOREIGN KEY (VetId) REFERENCES Vet(VetId)
    );
    CREATE NONCLUSTERED INDEX IX_VetPayment_PaymentDate ON VetPayment(PaymentDateTime);
END;
GO

-- Lab Payments
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LabPayment')
BEGIN
    CREATE TABLE LabPayment (
        LabPaymentId INT IDENTITY(1,1) PRIMARY KEY,
        LabAppointmentId INT NOT NULL,
        PetOwnerId INT NOT NULL,
        LabId INT NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        PaymentDateTime DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        PaymentMethod NVARCHAR(50) NOT NULL,
        TransactionId NVARCHAR(100),
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2,
        CONSTRAINT FK_LabPayment_LabAppointment FOREIGN KEY (LabAppointmentId) REFERENCES LabAppointment(LabAppointmentId),
        CONSTRAINT FK_LabPayment_PetOwner FOREIGN KEY (PetOwnerId) REFERENCES PetOwner(PetOwnerId),
        CONSTRAINT FK_LabPayment_Lab FOREIGN KEY (LabId) REFERENCES Lab(LabId)
    );
    CREATE NONCLUSTERED INDEX IX_LabPayment_PaymentDate ON LabPayment(PaymentDateTime);
END;
GO

-- Store Payments
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'StorePayment')
BEGIN
    CREATE TABLE StorePayment (
        StorePaymentId INT IDENTITY(1,1) PRIMARY KEY,
        MedicineOrderId INT NOT NULL,
        PetOwnerId INT NOT NULL,
        StoreId INT NOT NULL,
        Amount DECIMAL(18,2) NOT NULL,
        PaymentDateTime DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        PaymentMethod NVARCHAR(50) NOT NULL,
        TransactionId NVARCHAR(100),
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2,
        CONSTRAINT FK_StorePayment_MedicineOrder FOREIGN KEY (MedicineOrderId) REFERENCES MedicineOrder(MedicineOrderId),
        CONSTRAINT FK_StorePayment_PetOwner FOREIGN KEY (PetOwnerId) REFERENCES PetOwner(PetOwnerId),
        CONSTRAINT FK_StorePayment_Store FOREIGN KEY (StoreId) REFERENCES Store(StoreId)
    );
    CREATE NONCLUSTERED INDEX IX_StorePayment_PaymentDate ON StorePayment(PaymentDateTime);
END;
GO

-- =============================================
-- REVIEW TABLES
-- =============================================

-- Vet Reviews
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'VetReview')
BEGIN
    CREATE TABLE VetReview (
        VetReviewId INT IDENTITY(1,1) PRIMARY KEY,
        VetAppointmentId INT NOT NULL,
        PetOwnerId INT NOT NULL,
        VetId INT NOT NULL,
        Rating INT NOT NULL,
        Comments NVARCHAR(1000),
        ReviewDateTime DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2,
        CONSTRAINT FK_VetReview_VetAppointment FOREIGN KEY (VetAppointmentId) REFERENCES VetAppointment(VetAppointmentId),
        CONSTRAINT FK_VetReview_PetOwner FOREIGN KEY (PetOwnerId) REFERENCES PetOwner(PetOwnerId),
        CONSTRAINT FK_VetReview_Vet FOREIGN KEY (VetId) REFERENCES Vet(VetId),
        CONSTRAINT CK_VetReview_Rating CHECK (Rating >= 1 AND Rating <= 5)
    );
END;
GO

-- Lab Reviews
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'LabReview')
BEGIN
    CREATE TABLE LabReview (
        LabReviewId INT IDENTITY(1,1) PRIMARY KEY,
        LabAppointmentId INT NOT NULL,
        PetOwnerId INT NOT NULL,
        LabId INT NOT NULL,
        Rating INT NOT NULL,
        Comments NVARCHAR(1000),
        ReviewDateTime DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2,
        CONSTRAINT FK_LabReview_LabAppointment FOREIGN KEY (LabAppointmentId) REFERENCES LabAppointment(LabAppointmentId),
        CONSTRAINT FK_LabReview_PetOwner FOREIGN KEY (PetOwnerId) REFERENCES PetOwner(PetOwnerId),
        CONSTRAINT FK_LabReview_Lab FOREIGN KEY (LabId) REFERENCES Lab(LabId),
        CONSTRAINT CK_LabReview_Rating CHECK (Rating >= 1 AND Rating <= 5)
    );
END;
GO

-- Store Reviews
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'StoreReview')
BEGIN
    CREATE TABLE StoreReview (
        StoreReviewId INT IDENTITY(1,1) PRIMARY KEY,
        MedicineOrderId INT NOT NULL,
        PetOwnerId INT NOT NULL,
        StoreId INT NOT NULL,
        Rating INT NOT NULL,
        Comments NVARCHAR(1000),
        ReviewDateTime DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        CreatedDate DATETIME2 NOT NULL DEFAULT GETUTCDATE(),
        ModifiedDate DATETIME2,
        CONSTRAINT FK_StoreReview_MedicineOrder FOREIGN KEY (MedicineOrderId) REFERENCES MedicineOrder(MedicineOrderId),
        CONSTRAINT FK_StoreReview_PetOwner FOREIGN KEY (PetOwnerId) REFERENCES PetOwner(PetOwnerId),
        CONSTRAINT FK_StoreReview_Store FOREIGN KEY (StoreId) REFERENCES Store(StoreId),
        CONSTRAINT CK_StoreReview_Rating CHECK (Rating >= 1 AND Rating <= 5)
    );
END;
GO

PRINT 'OlaVet database schema created successfully!'
GO
