// =============================================
// File: OlaVet.Tests/Repositories/RepositoryTestBase.cs
// Base class for repository tests with in-memory database
// =============================================

using Microsoft.EntityFrameworkCore;
using OlaVet.Infrastructure.Data;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Entities.Lookups;

namespace OlaVet.Tests.Repositories;

public abstract class RepositoryTestBase : IDisposable
{
    protected readonly OlaVetDbContext Context;

    protected RepositoryTestBase()
    {
        var options = new DbContextOptionsBuilder<OlaVetDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        Context = new OlaVetDbContext(options);
        SeedBaseData();
    }

    private void SeedBaseData()
    {
        // Seed lookup tables
        Context.StatusTypes.AddRange(
            new StatusType { StatusTypeId = 1, StatusName = "Scheduled", AppliesTo = "Appointment" },
            new StatusType { StatusTypeId = 2, StatusName = "Confirmed", AppliesTo = "Appointment" },
            new StatusType { StatusTypeId = 3, StatusName = "Completed", AppliesTo = "Appointment" },
            new StatusType { StatusTypeId = 4, StatusName = "Cancelled", AppliesTo = "Appointment" }
        );

        Context.VetAppointmentTypes.AddRange(
            new VetAppointmentType { VetAppointmentTypeId = 1, TypeName = "Clinic Visit" },
            new VetAppointmentType { VetAppointmentTypeId = 2, TypeName = "Video Call" }
        );

        Context.MedicineTypes.AddRange(
            new MedicineType { MedicineTypeId = 1, TypeName = "Antibiotic" },
            new MedicineType { MedicineTypeId = 2, TypeName = "Antiparasitic" }
        );

        Context.RecordTypes.AddRange(
            new RecordType { RecordTypeId = 1, TypeName = "Checkup" },
            new RecordType { RecordTypeId = 2, TypeName = "Vaccination" }
        );

        Context.SaveChanges();
    }

    protected PetOwner CreateTestPetOwner(string name = "Test Owner")
    {
        var owner = new PetOwner
        {
            OwnerName = name,
            Email = $"{name.Replace(" ", "").ToLower()}@test.com",
            ContactNumber = "+92-300-1234567",
            HomeAddress = "Test Address, Lahore",
            Age = 30,
            Gender = "Male",
            Wallet = 5000
        };
        Context.PetOwners.Add(owner);
        Context.SaveChanges();
        return owner;
    }

    protected Vet CreateTestVet(string name = "Dr. Test Vet")
    {
        var vet = new Vet
        {
            VetName = name,
            Specialization = "General",
            ClinicLocation = "Test Clinic, Lahore",
            Fee = 2000,
            ContactNumber = "+92-321-1234567",
            Email = "testvet@olavet.com",
            YearsOfExperience = 5,
            LicenseNumber = "VET-2020-123"
        };
        Context.Vets.Add(vet);
        Context.SaveChanges();
        return vet;
    }

    protected Pet CreateTestPet(int ownerId, string name = "Buddy")
    {
        var pet = new Pet
        {
            PetOwnerId = ownerId,
            Name = name,
            Species = "Dog",
            Breed = "Labrador",
            Age = 3,
            PetWeight = 25.5m,
            Color = "Golden",
            Gender = "Male"
        };
        Context.Pets.Add(pet);
        Context.SaveChanges();
        return pet;
    }

    public void Dispose()
    {
        Context.Dispose();
    }
}
