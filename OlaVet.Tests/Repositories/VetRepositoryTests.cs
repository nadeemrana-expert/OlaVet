// =============================================
// File: OlaVet.Tests/Repositories/VetRepositoryTests.cs
// Unit tests for VetRepository
// =============================================

using FluentAssertions;
using OlaVet.Infrastructure.Repositories;
using OlaVet.Domain.Entities;

namespace OlaVet.Tests.Repositories;

public class VetRepositoryTests : RepositoryTestBase
{
    private readonly VetRepository _repository;

    public VetRepositoryTests()
    {
        _repository = new VetRepository(Context);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnVet_WhenExists()
    {
        // Arrange
        var vet = CreateTestVet("Dr. Ahmed Ali");

        // Act
        var result = await _repository.GetByIdAsync(vet.VetId);

        // Assert
        result.Should().NotBeNull();
        result!.VetName.Should().Be("Dr. Ahmed Ali");
    }

    [Fact]
    public async Task AddAsync_ShouldAddNewVet()
    {
        // Arrange
        var vet = new Vet
        {
            VetName = "Dr. New Vet",
            Specialization = "Surgery",
            Fee = 3000,
            ContactNumber = "+92-321-9999999",
            ClinicLocation = "New Clinic"
        };

        // Act
        await _repository.AddAsync(vet);
        await Context.SaveChangesAsync();

        // Assert
        var result = await _repository.GetByIdAsync(vet.VetId);
        result.Should().NotBeNull();
        result!.Specialization.Should().Be("Surgery");
    }

    [Fact]
    public async Task GetBySpecializationAsync_ShouldReturnMatchingVets()
    {
        // Arrange
        var vet1 = new Vet
        {
            VetName = "Dr. Vet 1",
            Specialization = "Surgery",
            ContactNumber = "+92-321-1111111",
            Fee = 2000
        };
        var vet2 = new Vet
        {
            VetName = "Dr. Vet 2",
            Specialization = "Surgery",
            ContactNumber = "+92-321-2222222",
            Fee = 2500
        };
        var vet3 = new Vet
        {
            VetName = "Dr. Vet 3",
            Specialization = "Dermatology",
            ContactNumber = "+92-321-3333333",
            Fee = 1800
        };
        
        Context.Vets.AddRange(vet1, vet2, vet3);
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetBySpecializationAsync("Surgery");

        // Assert
        result.Should().HaveCount(2);
        result.All(v => v.Specialization == "Surgery").Should().BeTrue();
    }

    [Fact]
    public async Task SearchAsync_ShouldFindVetsByName()
    {
        // Arrange
        Context.Vets.AddRange(
            new Vet { VetName = "Dr. Ahmed Khan", ContactNumber = "1", Fee = 1000 },
            new Vet { VetName = "Dr. Sara Ahmed", ContactNumber = "2", Fee = 1000 },
            new Vet { VetName = "Dr. Ali Raza", ContactNumber = "3", Fee = 1000 }
        );
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.SearchAsync("Ahmed");

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetWithDetailsAsync_ShouldIncludeRelatedData()
    {
        // Arrange
        var vet = CreateTestVet("Dr. Detail Test");
        
        // Add qualifications
        Context.EducationQualifications.Add(new EducationQualification
        {
            VetId = vet.VetId,
            QualificationName = "DVM",
            Institute = "University of Veterinary Sciences"
        });
        
        // Add services
        Context.Services.Add(new Service
        {
            VetId = vet.VetId,
            ServiceType = "Vaccination",
            ServiceFee = 500
        });
        
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetWithDetailsAsync(vet.VetId);

        // Assert
        result.Should().NotBeNull();
        result!.EducationQualifications.Should().HaveCount(1);
        result.Services.Should().HaveCount(1);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnCorrectPage()
    {
        // Arrange
        for (int i = 1; i <= 25; i++)
        {
            Context.Vets.Add(new Vet
            {
                VetName = $"Dr. Vet {i}",
                ContactNumber = $"+92-321-000000{i}",
                Fee = 1000 + i * 100
            });
        }
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetPagedAsync(2, 10);

        // Assert
        result.Items.Should().HaveCount(10);
        result.Page.Should().Be(2);
        result.TotalCount.Should().Be(25);
    }
}
