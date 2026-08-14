using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OlaVet.Infrastructure.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lab",
                columns: table => new
                {
                    LabId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    LabAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    WaitTime = table.Column<int>(type: "int", nullable: true),
                    Experience = table.Column<int>(type: "int", nullable: true),
                    ContactNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Discount = table.Column<decimal>(type: "decimal(5,2)", nullable: false, defaultValue: 0m),
                    Specialization = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lab", x => x.LabId);
                });

            migrationBuilder.CreateTable(
                name: "LabTest",
                columns: table => new
                {
                    LabTestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabTestName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    LabTestType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    LabTestDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TestFee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TurnaroundTimeHours = table.Column<int>(type: "int", nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabTest", x => x.LabTestId);
                });

            migrationBuilder.CreateTable(
                name: "MedicineType",
                columns: table => new
                {
                    MedicineTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineType", x => x.MedicineTypeId);
                });

            migrationBuilder.CreateTable(
                name: "PetOwner",
                columns: table => new
                {
                    PetOwnerId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    OwnerName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    HomeAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Age = table.Column<int>(type: "int", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    Wallet = table.Column<decimal>(type: "decimal(18,2)", nullable: false, defaultValue: 0m),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PetOwner", x => x.PetOwnerId);
                });

            migrationBuilder.CreateTable(
                name: "RecordType",
                columns: table => new
                {
                    RecordTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RecordType", x => x.RecordTypeId);
                });

            migrationBuilder.CreateTable(
                name: "StatusType",
                columns: table => new
                {
                    StatusTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StatusName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    AppliesTo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusType", x => x.StatusTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Store",
                columns: table => new
                {
                    StoreId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    StoreAddress = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Since = table.Column<DateTime>(type: "datetime2", nullable: true),
                    OpeningTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    ClosingTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    ContactNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Store", x => x.StoreId);
                });

            migrationBuilder.CreateTable(
                name: "Vet",
                columns: table => new
                {
                    VetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VetName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Specialization = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ClinicLocation = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Fee = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ContactNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    YearsOfExperience = table.Column<int>(type: "int", nullable: true),
                    LicenseNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vet", x => x.VetId);
                });

            migrationBuilder.CreateTable(
                name: "VetAppointmentType",
                columns: table => new
                {
                    VetAppointmentTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TypeName = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VetAppointmentType", x => x.VetAppointmentTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Medicine",
                columns: table => new
                {
                    MedicineId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicineName = table.Column<string>(type: "nvarchar(150)", maxLength: 150, nullable: false),
                    MG = table.Column<int>(type: "int", nullable: true),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MedicineTypeId = table.Column<int>(type: "int", nullable: true),
                    Manufacturer = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RequiresPrescription = table.Column<bool>(type: "bit", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Medicine", x => x.MedicineId);
                    table.ForeignKey(
                        name: "FK_Medicine_MedicineType_MedicineTypeId",
                        column: x => x.MedicineTypeId,
                        principalTable: "MedicineType",
                        principalColumn: "MedicineTypeId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Pet",
                columns: table => new
                {
                    PetId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PetOwnerId = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Species = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Breed = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Age = table.Column<int>(type: "int", nullable: true),
                    PetWeight = table.Column<decimal>(type: "decimal(8,2)", nullable: true),
                    Color = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    DateOfBirth = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RegistrationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pet", x => x.PetId);
                    table.ForeignKey(
                        name: "FK_Pet_PetOwner_PetOwnerId",
                        column: x => x.PetOwnerId,
                        principalTable: "PetOwner",
                        principalColumn: "PetOwnerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MedicineOrder",
                columns: table => new
                {
                    MedicineOrderId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PetOwnerId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    StatusTypeId = table.Column<int>(type: "int", nullable: false),
                    OrderDateTime = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DeliveryAddress = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DeliveredDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineOrder", x => x.MedicineOrderId);
                    table.ForeignKey(
                        name: "FK_MedicineOrder_PetOwner_PetOwnerId",
                        column: x => x.PetOwnerId,
                        principalTable: "PetOwner",
                        principalColumn: "PetOwnerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicineOrder_StatusType_StatusTypeId",
                        column: x => x.StatusTypeId,
                        principalTable: "StatusType",
                        principalColumn: "StatusTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicineOrder_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "EducationQualification",
                columns: table => new
                {
                    EducationId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VetId = table.Column<int>(type: "int", nullable: false),
                    QualificationName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Institute = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    YearOfDegree = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EducationQualification", x => x.EducationId);
                    table.ForeignKey(
                        name: "FK_EducationQualification_Vet_VetId",
                        column: x => x.VetId,
                        principalTable: "Vet",
                        principalColumn: "VetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Service",
                columns: table => new
                {
                    ServiceId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VetId = table.Column<int>(type: "int", nullable: false),
                    ServiceType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ServiceDescription = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ServiceFee = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Service", x => x.ServiceId);
                    table.ForeignKey(
                        name: "FK_Service_Vet_VetId",
                        column: x => x.VetId,
                        principalTable: "Vet",
                        principalColumn: "VetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VetAvailability",
                columns: table => new
                {
                    AvailabilityId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VetId = table.Column<int>(type: "int", nullable: false),
                    DayOfWeek = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    SlotDurationMinutes = table.Column<int>(type: "int", nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VetAvailability", x => x.AvailabilityId);
                    table.ForeignKey(
                        name: "FK_VetAvailability_Vet_VetId",
                        column: x => x.VetId,
                        principalTable: "Vet",
                        principalColumn: "VetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    InventoryId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    MedicineId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    LastRestocked = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.InventoryId);
                    table.ForeignKey(
                        name: "FK_Inventory_Medicine_MedicineId",
                        column: x => x.MedicineId,
                        principalTable: "Medicine",
                        principalColumn: "MedicineId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Inventory_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabAppointment",
                columns: table => new
                {
                    LabAppointmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PetId = table.Column<int>(type: "int", nullable: false),
                    PetOwnerId = table.Column<int>(type: "int", nullable: false),
                    LabId = table.Column<int>(type: "int", nullable: false),
                    StatusTypeId = table.Column<int>(type: "int", nullable: false),
                    AppointmentDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabAppointment", x => x.LabAppointmentId);
                    table.ForeignKey(
                        name: "FK_LabAppointment_Lab_LabId",
                        column: x => x.LabId,
                        principalTable: "Lab",
                        principalColumn: "LabId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabAppointment_PetOwner_PetOwnerId",
                        column: x => x.PetOwnerId,
                        principalTable: "PetOwner",
                        principalColumn: "PetOwnerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabAppointment_Pet_PetId",
                        column: x => x.PetId,
                        principalTable: "Pet",
                        principalColumn: "PetId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LabAppointment_StatusType_StatusTypeId",
                        column: x => x.StatusTypeId,
                        principalTable: "StatusType",
                        principalColumn: "StatusTypeId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicalRecord",
                columns: table => new
                {
                    RecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PetId = table.Column<int>(type: "int", nullable: false),
                    PetOwnerId = table.Column<int>(type: "int", nullable: false),
                    RecordTypeId = table.Column<int>(type: "int", nullable: false),
                    VetId = table.Column<int>(type: "int", nullable: true),
                    RecordDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    Diagnosis = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    TreatmentDescription = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    AttachmentPath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicalRecord", x => x.RecordId);
                    table.ForeignKey(
                        name: "FK_MedicalRecord_PetOwner_PetOwnerId",
                        column: x => x.PetOwnerId,
                        principalTable: "PetOwner",
                        principalColumn: "PetOwnerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicalRecord_Pet_PetId",
                        column: x => x.PetId,
                        principalTable: "Pet",
                        principalColumn: "PetId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicalRecord_RecordType_RecordTypeId",
                        column: x => x.RecordTypeId,
                        principalTable: "RecordType",
                        principalColumn: "RecordTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MedicalRecord_Vet_VetId",
                        column: x => x.VetId,
                        principalTable: "Vet",
                        principalColumn: "VetId",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "VetAppointment",
                columns: table => new
                {
                    VetAppointmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PetId = table.Column<int>(type: "int", nullable: false),
                    PetOwnerId = table.Column<int>(type: "int", nullable: false),
                    VetId = table.Column<int>(type: "int", nullable: false),
                    VetAppointmentTypeId = table.Column<int>(type: "int", nullable: false),
                    StatusTypeId = table.Column<int>(type: "int", nullable: false),
                    AppointmentDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Notes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VetAppointment", x => x.VetAppointmentId);
                    table.ForeignKey(
                        name: "FK_VetAppointment_PetOwner_PetOwnerId",
                        column: x => x.PetOwnerId,
                        principalTable: "PetOwner",
                        principalColumn: "PetOwnerId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VetAppointment_Pet_PetId",
                        column: x => x.PetId,
                        principalTable: "Pet",
                        principalColumn: "PetId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VetAppointment_StatusType_StatusTypeId",
                        column: x => x.StatusTypeId,
                        principalTable: "StatusType",
                        principalColumn: "StatusTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VetAppointment_VetAppointmentType_VetAppointmentTypeId",
                        column: x => x.VetAppointmentTypeId,
                        principalTable: "VetAppointmentType",
                        principalColumn: "VetAppointmentTypeId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_VetAppointment_Vet_VetId",
                        column: x => x.VetId,
                        principalTable: "Vet",
                        principalColumn: "VetId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MedicineOrderDetails",
                columns: table => new
                {
                    OrderDetailId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicineOrderId = table.Column<int>(type: "int", nullable: false),
                    MedicineId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    UnitPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Subtotal = table.Column<decimal>(type: "decimal(18,2)", nullable: false, computedColumnSql: "[Quantity] * [UnitPrice]", stored: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MedicineOrderDetails", x => x.OrderDetailId);
                    table.ForeignKey(
                        name: "FK_MedicineOrderDetails_MedicineOrder_MedicineOrderId",
                        column: x => x.MedicineOrderId,
                        principalTable: "MedicineOrder",
                        principalColumn: "MedicineOrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MedicineOrderDetails_Medicine_MedicineId",
                        column: x => x.MedicineId,
                        principalTable: "Medicine",
                        principalColumn: "MedicineId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StorePayment",
                columns: table => new
                {
                    StorePaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicineOrderId = table.Column<int>(type: "int", nullable: false),
                    PetOwnerId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StorePayment", x => x.StorePaymentId);
                    table.ForeignKey(
                        name: "FK_StorePayment_MedicineOrder_MedicineOrderId",
                        column: x => x.MedicineOrderId,
                        principalTable: "MedicineOrder",
                        principalColumn: "MedicineOrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StorePayment_PetOwner_PetOwnerId",
                        column: x => x.PetOwnerId,
                        principalTable: "PetOwner",
                        principalColumn: "PetOwnerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StorePayment_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StoreReview",
                columns: table => new
                {
                    StoreReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MedicineOrderId = table.Column<int>(type: "int", nullable: false),
                    PetOwnerId = table.Column<int>(type: "int", nullable: false),
                    StoreId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StoreReview", x => x.StoreReviewId);
                    table.CheckConstraint("CK_StoreReview_Rating", "[Rating] >= 1 AND [Rating] <= 5");
                    table.ForeignKey(
                        name: "FK_StoreReview_MedicineOrder_MedicineOrderId",
                        column: x => x.MedicineOrderId,
                        principalTable: "MedicineOrder",
                        principalColumn: "MedicineOrderId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreReview_PetOwner_PetOwnerId",
                        column: x => x.PetOwnerId,
                        principalTable: "PetOwner",
                        principalColumn: "PetOwnerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StoreReview_Store_StoreId",
                        column: x => x.StoreId,
                        principalTable: "Store",
                        principalColumn: "StoreId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabAppointmentTest",
                columns: table => new
                {
                    LabAppointmentTestId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabAppointmentId = table.Column<int>(type: "int", nullable: false),
                    LabTestId = table.Column<int>(type: "int", nullable: false),
                    TestResult = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    ResultDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ResultFile = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabAppointmentTest", x => x.LabAppointmentTestId);
                    table.ForeignKey(
                        name: "FK_LabAppointmentTest_LabAppointment_LabAppointmentId",
                        column: x => x.LabAppointmentId,
                        principalTable: "LabAppointment",
                        principalColumn: "LabAppointmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabAppointmentTest_LabTest_LabTestId",
                        column: x => x.LabTestId,
                        principalTable: "LabTest",
                        principalColumn: "LabTestId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "LabPayment",
                columns: table => new
                {
                    LabPaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabAppointmentId = table.Column<int>(type: "int", nullable: false),
                    PetOwnerId = table.Column<int>(type: "int", nullable: false),
                    LabId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabPayment", x => x.LabPaymentId);
                    table.ForeignKey(
                        name: "FK_LabPayment_LabAppointment_LabAppointmentId",
                        column: x => x.LabAppointmentId,
                        principalTable: "LabAppointment",
                        principalColumn: "LabAppointmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabPayment_Lab_LabId",
                        column: x => x.LabId,
                        principalTable: "Lab",
                        principalColumn: "LabId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabPayment_PetOwner_PetOwnerId",
                        column: x => x.PetOwnerId,
                        principalTable: "PetOwner",
                        principalColumn: "PetOwnerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LabReview",
                columns: table => new
                {
                    LabReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LabAppointmentId = table.Column<int>(type: "int", nullable: false),
                    PetOwnerId = table.Column<int>(type: "int", nullable: false),
                    LabId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LabReview", x => x.LabReviewId);
                    table.CheckConstraint("CK_LabReview_Rating", "[Rating] >= 1 AND [Rating] <= 5");
                    table.ForeignKey(
                        name: "FK_LabReview_LabAppointment_LabAppointmentId",
                        column: x => x.LabAppointmentId,
                        principalTable: "LabAppointment",
                        principalColumn: "LabAppointmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabReview_Lab_LabId",
                        column: x => x.LabId,
                        principalTable: "Lab",
                        principalColumn: "LabId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LabReview_PetOwner_PetOwnerId",
                        column: x => x.PetOwnerId,
                        principalTable: "PetOwner",
                        principalColumn: "PetOwnerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VetPayment",
                columns: table => new
                {
                    VetPaymentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VetAppointmentId = table.Column<int>(type: "int", nullable: false),
                    PetOwnerId = table.Column<int>(type: "int", nullable: false),
                    VetId = table.Column<int>(type: "int", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    PaymentDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PaymentMethod = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    TransactionId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VetPayment", x => x.VetPaymentId);
                    table.ForeignKey(
                        name: "FK_VetPayment_PetOwner_PetOwnerId",
                        column: x => x.PetOwnerId,
                        principalTable: "PetOwner",
                        principalColumn: "PetOwnerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VetPayment_VetAppointment_VetAppointmentId",
                        column: x => x.VetAppointmentId,
                        principalTable: "VetAppointment",
                        principalColumn: "VetAppointmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VetPayment_Vet_VetId",
                        column: x => x.VetId,
                        principalTable: "Vet",
                        principalColumn: "VetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "VetReview",
                columns: table => new
                {
                    VetReviewId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VetAppointmentId = table.Column<int>(type: "int", nullable: false),
                    PetOwnerId = table.Column<int>(type: "int", nullable: false),
                    VetId = table.Column<int>(type: "int", nullable: false),
                    Rating = table.Column<int>(type: "int", nullable: false),
                    Comments = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ReviewDateTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VetReview", x => x.VetReviewId);
                    table.CheckConstraint("CK_VetReview_Rating", "[Rating] >= 1 AND [Rating] <= 5");
                    table.ForeignKey(
                        name: "FK_VetReview_PetOwner_PetOwnerId",
                        column: x => x.PetOwnerId,
                        principalTable: "PetOwner",
                        principalColumn: "PetOwnerId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VetReview_VetAppointment_VetAppointmentId",
                        column: x => x.VetAppointmentId,
                        principalTable: "VetAppointment",
                        principalColumn: "VetAppointmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VetReview_Vet_VetId",
                        column: x => x.VetId,
                        principalTable: "Vet",
                        principalColumn: "VetId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EducationQualification_VetId",
                table: "EducationQualification",
                column: "VetId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_MedicineId",
                table: "Inventory",
                column: "MedicineId");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_Quantity",
                table: "Inventory",
                column: "Quantity");

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_Store_Medicine_Unique",
                table: "Inventory",
                columns: new[] { "StoreId", "MedicineId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Inventory_StoreId",
                table: "Inventory",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Lab_ContactNumber",
                table: "Lab",
                column: "ContactNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Lab_IsActive",
                table: "Lab",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Lab_IsActive_Specialization",
                table: "Lab",
                columns: new[] { "IsActive", "Specialization" });

            migrationBuilder.CreateIndex(
                name: "IX_LabAppointment_AppointmentDateTime",
                table: "LabAppointment",
                column: "AppointmentDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_LabAppointment_Lab_Status_Date_Covering",
                table: "LabAppointment",
                columns: new[] { "LabId", "StatusTypeId", "AppointmentDateTime" })
                .Annotation("SqlServer:Include", new[] { "PetId", "Notes" });

            migrationBuilder.CreateIndex(
                name: "IX_LabAppointment_Owner_DateTime",
                table: "LabAppointment",
                columns: new[] { "PetOwnerId", "AppointmentDateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_LabAppointment_PetId",
                table: "LabAppointment",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_LabAppointment_StatusTypeId",
                table: "LabAppointment",
                column: "StatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_LabAppointmentTest_AppointmentId",
                table: "LabAppointmentTest",
                column: "LabAppointmentId");

            migrationBuilder.CreateIndex(
                name: "IX_LabAppointmentTest_LabTestId",
                table: "LabAppointmentTest",
                column: "LabTestId");

            migrationBuilder.CreateIndex(
                name: "IX_LabPayment_LabAppointmentId",
                table: "LabPayment",
                column: "LabAppointmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabPayment_LabId",
                table: "LabPayment",
                column: "LabId");

            migrationBuilder.CreateIndex(
                name: "IX_LabPayment_PaymentDateTime",
                table: "LabPayment",
                column: "PaymentDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_LabPayment_PetOwnerId",
                table: "LabPayment",
                column: "PetOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_LabPayment_TransactionId",
                table: "LabPayment",
                column: "TransactionId",
                unique: true,
                filter: "[TransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_LabReview_LabAppointmentId",
                table: "LabReview",
                column: "LabAppointmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_LabReview_PetOwnerId",
                table: "LabReview",
                column: "PetOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_LabReview_Rating",
                table: "LabReview",
                column: "Rating");

            migrationBuilder.CreateIndex(
                name: "IX_LabReview_ReviewDateTime",
                table: "LabReview",
                column: "ReviewDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_LabReview_StoreId",
                table: "LabReview",
                column: "LabId");

            migrationBuilder.CreateIndex(
                name: "IX_LabTest_Name",
                table: "LabTest",
                column: "LabTestName");

            migrationBuilder.CreateIndex(
                name: "IX_LabTest_Type",
                table: "LabTest",
                column: "LabTestType");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecord_Pet_Date",
                table: "MedicalRecord",
                columns: new[] { "PetId", "RecordDate" });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecord_PetOwnerId",
                table: "MedicalRecord",
                column: "PetOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecord_RecordTypeId",
                table: "MedicalRecord",
                column: "RecordTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecord_Timeline_Covering",
                table: "MedicalRecord",
                columns: new[] { "PetId", "RecordTypeId" })
                .Annotation("SqlServer:Include", new[] { "RecordDate", "Diagnosis" });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecord_VetId",
                table: "MedicalRecord",
                column: "VetId");

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_IsActive",
                table: "Medicine",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_IsActive_MedicineTypeId",
                table: "Medicine",
                columns: new[] { "IsActive", "MedicineTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_MedicineName",
                table: "Medicine",
                column: "MedicineName");

            migrationBuilder.CreateIndex(
                name: "IX_Medicine_MedicineTypeId",
                table: "Medicine",
                column: "MedicineTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrder_OrderDateTime",
                table: "MedicineOrder",
                column: "OrderDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrder_PetOwnerId",
                table: "MedicineOrder",
                column: "PetOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrder_StatusTypeId",
                table: "MedicineOrder",
                column: "StatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrder_Store_Status_DateTime",
                table: "MedicineOrder",
                columns: new[] { "StoreId", "StatusTypeId", "OrderDateTime" });

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrderDetail_MedicineId",
                table: "MedicineOrderDetails",
                column: "MedicineId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineOrderDetail_MedicineOrderId",
                table: "MedicineOrderDetails",
                column: "MedicineOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicineType_TypeName",
                table: "MedicineType",
                column: "TypeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pet_PetOwnerId",
                table: "Pet",
                column: "PetOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Pet_PetOwnerId_IsActive",
                table: "Pet",
                columns: new[] { "PetOwnerId", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_Pet_RegistrationDate",
                table: "Pet",
                column: "RegistrationDate");

            migrationBuilder.CreateIndex(
                name: "IX_Pet_Species",
                table: "Pet",
                column: "Species");

            migrationBuilder.CreateIndex(
                name: "IX_PetOwner_ContactNumber",
                table: "PetOwner",
                column: "ContactNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PetOwner_Email",
                table: "PetOwner",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PetOwner_IsActive_RegistrationDate",
                table: "PetOwner",
                columns: new[] { "IsActive", "RegistrationDate" });

            migrationBuilder.CreateIndex(
                name: "IX_PetOwner_RegistrationDate",
                table: "PetOwner",
                column: "RegistrationDate");

            migrationBuilder.CreateIndex(
                name: "IX_RecordType_TypeName",
                table: "RecordType",
                column: "TypeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Service_Vet_Type",
                table: "Service",
                columns: new[] { "VetId", "ServiceType" });

            migrationBuilder.CreateIndex(
                name: "IX_StatusType_StatusName",
                table: "StatusType",
                column: "StatusName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ_StatusType_Name_AppliesTo",
                table: "StatusType",
                columns: new[] { "StatusName", "AppliesTo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Store_ContactNumber",
                table: "Store",
                column: "ContactNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Store_Name",
                table: "Store",
                column: "StoreName");

            migrationBuilder.CreateIndex(
                name: "IX_StorePayment_MedicineOrderId",
                table: "StorePayment",
                column: "MedicineOrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StorePayment_PaymentDateTime",
                table: "StorePayment",
                column: "PaymentDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_StorePayment_PetOwnerId",
                table: "StorePayment",
                column: "PetOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_StorePayment_StoreId",
                table: "StorePayment",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_StorePayment_TransactionId",
                table: "StorePayment",
                column: "TransactionId",
                unique: true,
                filter: "[TransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_StoreReview_MedicineOrderId",
                table: "StoreReview",
                column: "MedicineOrderId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StoreReview_PetOwnerId",
                table: "StoreReview",
                column: "PetOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_StoreReview_Rating",
                table: "StoreReview",
                column: "Rating");

            migrationBuilder.CreateIndex(
                name: "IX_StoreReview_ReviewDateTime",
                table: "StoreReview",
                column: "ReviewDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_StoreReview_StoreId",
                table: "StoreReview",
                column: "StoreId");

            migrationBuilder.CreateIndex(
                name: "IX_Vet_ContactNumber",
                table: "Vet",
                column: "ContactNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vet_IsActive_Specialization",
                table: "Vet",
                columns: new[] { "IsActive", "Specialization" });

            migrationBuilder.CreateIndex(
                name: "IX_Vet_Specialization",
                table: "Vet",
                column: "Specialization");

            migrationBuilder.CreateIndex(
                name: "IX_Vet_YearsOfExperience",
                table: "Vet",
                column: "YearsOfExperience");

            migrationBuilder.CreateIndex(
                name: "IX_VetAppointment_AppointmentDateTime",
                table: "VetAppointment",
                column: "AppointmentDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_VetAppointment_PetId",
                table: "VetAppointment",
                column: "PetId");

            migrationBuilder.CreateIndex(
                name: "IX_VetAppointment_PetOwnerId",
                table: "VetAppointment",
                column: "PetOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_VetAppointment_Status_DateTime_Covering",
                table: "VetAppointment",
                columns: new[] { "StatusTypeId", "AppointmentDateTime" })
                .Annotation("SqlServer:Include", new[] { "VetId", "PetId", "Reason" });

            migrationBuilder.CreateIndex(
                name: "IX_VetAppointment_StatusTypeId",
                table: "VetAppointment",
                column: "StatusTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VetAppointment_Vet_DateTime_Status",
                table: "VetAppointment",
                columns: new[] { "VetId", "AppointmentDateTime", "StatusTypeId" });

            migrationBuilder.CreateIndex(
                name: "IX_VetAppointment_VetAppointmentTypeId",
                table: "VetAppointment",
                column: "VetAppointmentTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_VetAppointment_VetId",
                table: "VetAppointment",
                column: "VetId");

            migrationBuilder.CreateIndex(
                name: "IX_VetAppointmentType_TypeName",
                table: "VetAppointmentType",
                column: "TypeName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VetAvailability_Vet_Day",
                table: "VetAvailability",
                columns: new[] { "VetId", "DayOfWeek" });

            migrationBuilder.CreateIndex(
                name: "IX_VetPayment_PaymentDateTime",
                table: "VetPayment",
                column: "PaymentDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_VetPayment_PetOwnerId",
                table: "VetPayment",
                column: "PetOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_VetPayment_TransactionId",
                table: "VetPayment",
                column: "TransactionId",
                unique: true,
                filter: "[TransactionId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_VetPayment_VetAppointmentId",
                table: "VetPayment",
                column: "VetAppointmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VetPayment_VetId",
                table: "VetPayment",
                column: "VetId");

            migrationBuilder.CreateIndex(
                name: "IX_VetReview_PetOwnerId",
                table: "VetReview",
                column: "PetOwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_VetReview_Rating",
                table: "VetReview",
                column: "Rating");

            migrationBuilder.CreateIndex(
                name: "IX_VetReview_ReviewDateTime",
                table: "VetReview",
                column: "ReviewDateTime");

            migrationBuilder.CreateIndex(
                name: "IX_VetReview_VetAppointmentId",
                table: "VetReview",
                column: "VetAppointmentId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_VetReview_VetId",
                table: "VetReview",
                column: "VetId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EducationQualification");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropTable(
                name: "LabAppointmentTest");

            migrationBuilder.DropTable(
                name: "LabPayment");

            migrationBuilder.DropTable(
                name: "LabReview");

            migrationBuilder.DropTable(
                name: "MedicalRecord");

            migrationBuilder.DropTable(
                name: "MedicineOrderDetails");

            migrationBuilder.DropTable(
                name: "Service");

            migrationBuilder.DropTable(
                name: "StorePayment");

            migrationBuilder.DropTable(
                name: "StoreReview");

            migrationBuilder.DropTable(
                name: "VetAvailability");

            migrationBuilder.DropTable(
                name: "VetPayment");

            migrationBuilder.DropTable(
                name: "VetReview");

            migrationBuilder.DropTable(
                name: "LabTest");

            migrationBuilder.DropTable(
                name: "LabAppointment");

            migrationBuilder.DropTable(
                name: "RecordType");

            migrationBuilder.DropTable(
                name: "Medicine");

            migrationBuilder.DropTable(
                name: "MedicineOrder");

            migrationBuilder.DropTable(
                name: "VetAppointment");

            migrationBuilder.DropTable(
                name: "Lab");

            migrationBuilder.DropTable(
                name: "MedicineType");

            migrationBuilder.DropTable(
                name: "Store");

            migrationBuilder.DropTable(
                name: "Pet");

            migrationBuilder.DropTable(
                name: "StatusType");

            migrationBuilder.DropTable(
                name: "VetAppointmentType");

            migrationBuilder.DropTable(
                name: "Vet");

            migrationBuilder.DropTable(
                name: "PetOwner");
        }
    }
}
