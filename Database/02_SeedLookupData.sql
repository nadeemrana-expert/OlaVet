-- =============================================
-- OlaVet Lookup Data Seeding Script
-- Version: 1.0
-- Description: Seeds lookup tables with initial data
-- =============================================

USE OlaVet;
GO

-- =============================================
-- MEDICINE TYPES
-- =============================================
IF NOT EXISTS (SELECT 1 FROM MedicineType)
BEGIN
    INSERT INTO MedicineType (TypeName, Description)
    VALUES 
        ('Antibiotic', 'Medicines used to treat bacterial infections'),
        ('Antiparasitic', 'Medicines for treating parasites like worms and ticks'),
        ('Anti-inflammatory', 'Medicines to reduce inflammation and pain'),
        ('Vaccine', 'Preventive medicines for disease immunization'),
        ('Supplement', 'Nutritional supplements and vitamins'),
        ('Painkiller', 'Medicines for pain relief'),
        ('Antifungal', 'Medicines for treating fungal infections'),
        ('Sedative', 'Medicines for calming and sedation'),
        ('Antiseptic', 'Medicines for preventing infection'),
        ('Antihistamine', 'Medicines for treating allergies');
    
    PRINT 'Medicine Types seeded: 10 rows';
END;
GO

-- =============================================
-- RECORD TYPES
-- =============================================
IF NOT EXISTS (SELECT 1 FROM RecordType)
BEGIN
    INSERT INTO RecordType (TypeName, Description)
    VALUES 
        ('Checkup', 'Regular health examination'),
        ('Vaccination', 'Vaccine administration record'),
        ('Surgery', 'Surgical procedure record'),
        ('Treatment', 'Medical treatment record'),
        ('Diagnosis', 'Diagnostic findings record'),
        ('Emergency', 'Emergency visit record'),
        ('Follow-up', 'Follow-up appointment record'),
        ('Lab Result', 'Laboratory test results');
    
    PRINT 'Record Types seeded: 8 rows';
END;
GO

-- =============================================
-- VET APPOINTMENT TYPES
-- =============================================
IF NOT EXISTS (SELECT 1 FROM VetAppointmentType)
BEGIN
    INSERT INTO VetAppointmentType (TypeName, Description, DefaultDuration)
    VALUES 
        ('Clinic Visit', 'In-person visit at the veterinary clinic', 30),
        ('Video Call', 'Online video consultation', 20),
        ('Home Visit', 'Veterinarian visits the pet at home', 45),
        ('Emergency', 'Emergency consultation', 60);
    
    PRINT 'Vet Appointment Types seeded: 4 rows';
END;
GO

-- =============================================
-- STATUS TYPES
-- =============================================
IF NOT EXISTS (SELECT 1 FROM StatusType)
BEGIN
    INSERT INTO StatusType (StatusName, AppliesTo, Description)
    VALUES 
        -- Appointment Statuses (1-6)
        ('Scheduled', 'Appointment', 'Appointment is scheduled and pending'),
        ('Confirmed', 'Appointment', 'Appointment is confirmed by both parties'),
        ('Completed', 'Appointment', 'Appointment has been completed'),
        ('Cancelled', 'Appointment', 'Appointment was cancelled'),
        ('NoShow', 'Appointment', 'Patient did not show up for appointment'),
        ('Rescheduled', 'Appointment', 'Appointment has been rescheduled'),
        
        -- Order Statuses (7-12)
        ('Processing', 'Order', 'Order is being processed'),
        ('Shipped', 'Order', 'Order has been shipped'),
        ('Delivered', 'Order', 'Order has been delivered'),
        ('Returned', 'Order', 'Order has been returned'),
        ('Refunded', 'Order', 'Order has been refunded'),
        ('OnHold', 'Order', 'Order is on hold'),
        
        -- Payment Statuses (13-16)
        ('Pending', 'Payment', 'Payment is pending'),
        ('Paid', 'Payment', 'Payment has been received'),
        ('Failed', 'Payment', 'Payment has failed'),
        ('Refunded', 'Payment', 'Payment has been refunded');
    
    PRINT 'Status Types seeded: 16 rows';
END;
GO

PRINT 'All lookup data seeded successfully!'
GO
