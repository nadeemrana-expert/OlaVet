-- =============================================
-- 05_PerformanceIndexes.sql
-- Performance optimization indexes for 2.5M+ records
-- Run AFTER all other scripts
-- =============================================

USE OlaVet;
GO

PRINT '=== Adding Performance Indexes ===';

-- =============================================
-- PAYMENT TABLE INDEXES (Missing FK indexes)
-- PaymentDateTime index exists, but FK columns have NO indexes
-- =============================================

-- VetPayment: Add FK indexes
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_VetPayment_VetId')
    CREATE NONCLUSTERED INDEX IX_VetPayment_VetId 
    ON VetPayment(VetId)
    INCLUDE (Amount, PaymentDateTime);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_VetPayment_PetOwnerId')
    CREATE NONCLUSTERED INDEX IX_VetPayment_PetOwnerId 
    ON VetPayment(PetOwnerId)
    INCLUDE (Amount, PaymentDateTime);
GO

-- LabPayment: Add FK indexes
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LabPayment_LabId')
    CREATE NONCLUSTERED INDEX IX_LabPayment_LabId 
    ON LabPayment(LabId)
    INCLUDE (Amount, PaymentDateTime);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LabPayment_PetOwnerId')
    CREATE NONCLUSTERED INDEX IX_LabPayment_PetOwnerId 
    ON LabPayment(PetOwnerId)
    INCLUDE (Amount, PaymentDateTime);
GO

-- StorePayment: Add FK indexes
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_StorePayment_StoreId')
    CREATE NONCLUSTERED INDEX IX_StorePayment_StoreId 
    ON StorePayment(StoreId)
    INCLUDE (Amount, PaymentDateTime);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_StorePayment_PetOwnerId')
    CREATE NONCLUSTERED INDEX IX_StorePayment_PetOwnerId 
    ON StorePayment(PetOwnerId)
    INCLUDE (Amount, PaymentDateTime);
GO

-- =============================================
-- REVIEW TABLE INDEXES (Add PetOwnerId + composite)
-- =============================================

-- VetReview: Add PetOwnerId index + composite for rating queries
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_VetReview_PetOwnerId')
    CREATE NONCLUSTERED INDEX IX_VetReview_PetOwnerId 
    ON VetReview(PetOwnerId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_VetReview_VetId_Rating')
    CREATE NONCLUSTERED INDEX IX_VetReview_VetId_Rating
    ON VetReview(VetId, Rating)
    INCLUDE (Comments, ReviewDateTime, PetOwnerId);
GO

-- LabReview: Add PetOwnerId index + composite
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LabReview_PetOwnerId')
    CREATE NONCLUSTERED INDEX IX_LabReview_PetOwnerId 
    ON LabReview(PetOwnerId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LabReview_LabId_Rating')
    CREATE NONCLUSTERED INDEX IX_LabReview_LabId_Rating 
    ON LabReview(LabId, Rating)
    INCLUDE (Comments, ReviewDateTime, PetOwnerId);
GO

-- Fix the copy-paste naming bug
IF EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LabReview_StoreId')
BEGIN
    EXEC sp_rename N'LabReview.IX_LabReview_StoreId', N'IX_LabReview_LabId', N'INDEX';
    PRINT 'Fixed: Renamed IX_LabReview_StoreId -> IX_LabReview_LabId';
END
GO

-- StoreReview: Add PetOwnerId index + composite
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_StoreReview_PetOwnerId')
    CREATE NONCLUSTERED INDEX IX_StoreReview_PetOwnerId 
    ON StoreReview(PetOwnerId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_StoreReview_StoreId_Rating')
    CREATE NONCLUSTERED INDEX IX_StoreReview_StoreId_Rating 
    ON StoreReview(StoreId, Rating)
    INCLUDE (Comments, ReviewDateTime, PetOwnerId);
GO

-- =============================================
-- REVIEW DATETIME DESCENDING INDEXES (for ORDER BY DESC)
-- The existing indexes are ASC, but queries always ORDER BY DESC
-- =============================================

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_VetReview_ReviewDateTime_DESC')
    CREATE NONCLUSTERED INDEX IX_VetReview_ReviewDateTime_DESC
    ON VetReview(ReviewDateTime DESC)
    INCLUDE (VetId, Rating, Comments, PetOwnerId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LabReview_ReviewDateTime_DESC')
    CREATE NONCLUSTERED INDEX IX_LabReview_ReviewDateTime_DESC
    ON LabReview(ReviewDateTime DESC)
    INCLUDE (LabId, Rating, Comments, PetOwnerId);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_StoreReview_ReviewDateTime_DESC')
    CREATE NONCLUSTERED INDEX IX_StoreReview_ReviewDateTime_DESC
    ON StoreReview(ReviewDateTime DESC)
    INCLUDE (StoreId, Rating, Comments, PetOwnerId);
GO

-- =============================================
-- MEDICINE ORDER: Add StoreId index
-- =============================================

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MedicineOrder_StoreId')
    CREATE NONCLUSTERED INDEX IX_MedicineOrder_StoreId 
    ON MedicineOrder(StoreId);
GO

-- =============================================
-- INVENTORY: Composite for stock lookups
-- =============================================

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Inventory_StoreId_MedicineId')
    CREATE NONCLUSTERED INDEX IX_Inventory_StoreId_MedicineId 
    ON Inventory(StoreId, MedicineId)
    INCLUDE (Quantity, LastRestocked);
GO

-- =============================================
-- MEDICAL RECORDS: Add missing FK indexes
-- =============================================

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MedicalRecord_PetId')
    CREATE NONCLUSTERED INDEX IX_MedicalRecord_PetId
    ON MedicalRecord(PetId, RecordDate DESC);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MedicalRecord_PetOwnerId')
    CREATE NONCLUSTERED INDEX IX_MedicalRecord_PetOwnerId
    ON MedicalRecord(PetOwnerId, RecordDate DESC);
GO

IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_MedicalRecord_VetId')
    CREATE NONCLUSTERED INDEX IX_MedicalRecord_VetId
    ON MedicalRecord(VetId, RecordDate DESC);
GO

-- =============================================
-- APPOINTMENT TABLES: Covering indexes for dashboard
-- =============================================

-- VetAppointment: Covering index for status-based counts
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_VetAppointment_StatusTypeId_Covering')
    CREATE NONCLUSTERED INDEX IX_VetAppointment_StatusTypeId_Covering
    ON VetAppointment(StatusTypeId)
    INCLUDE (AppointmentDateTime, VetId, PetOwnerId);
GO

-- LabAppointment: Covering index for status-based counts
IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_LabAppointment_StatusTypeId_Covering')
    CREATE NONCLUSTERED INDEX IX_LabAppointment_StatusTypeId_Covering
    ON LabAppointment(StatusTypeId)
    INCLUDE (AppointmentDateTime, LabId, PetOwnerId);
GO

PRINT '=== Performance Indexes Added Successfully ===';
GO
