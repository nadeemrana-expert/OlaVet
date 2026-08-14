// =============================================
// File: OlaVet.Tests/Repositories/PetOwnerRepositoryTests.cs
// Unit tests for PetOwnerRepository
// =============================================

using FluentAssertions;
using OlaVet.Infrastructure.Repositories;
using OlaVet.Domain.Entities;

namespace OlaVet.Tests.Repositories;

public class PetOwnerRepositoryTests : RepositoryTestBase
{
    private readonly PetOwnerRepository _repository;

    public PetOwnerRepositoryTests()
    {
        _repository = new PetOwnerRepository(Context);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnOwner_WhenExists()
    {
        // Arrange
        var owner = CreateTestPetOwner("John Doe");

        // Act
        var result = await _repository.GetByIdAsync(owner.PetOwnerId);

        // Assert
        result.Should().NotBeNull();
        result!.OwnerName.Should().Be("John Doe");
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnNull_WhenNotExists()
    {
        // Act
        var result = await _repository.GetByIdAsync(999);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task AddAsync_ShouldAddNewOwner()
    {
        // Arrange
        var owner = new PetOwner
        {
            OwnerName = "New Owner",
            Email = "new@test.com",
            ContactNumber = "+92-300-9999999"
        };

        // Act
        await _repository.AddAsync(owner);
        await Context.SaveChangesAsync();

        // Assert
        var result = await _repository.GetByIdAsync(owner.PetOwnerId);
        result.Should().NotBeNull();
        result!.OwnerName.Should().Be("New Owner");
    }

    [Fact]
    public async Task GetWithPetsAsync_ShouldReturnOwnerWithPets()
    {
        // Arrange
        var owner = CreateTestPetOwner("Pet Owner");
        CreateTestPet(owner.PetOwnerId, "Buddy");
        CreateTestPet(owner.PetOwnerId, "Max");

        // Act
        var result = await _repository.GetWithPetsAsync(owner.PetOwnerId);

        // Assert
        result.Should().NotBeNull();
        result!.Pets.Should().HaveCount(2);
    }

    [Fact]
    public async Task SearchAsync_ShouldFindOwnersByName()
    {
        // Arrange
        CreateTestPetOwner("Ahmed Khan");
        CreateTestPetOwner("Sara Ahmed");
        CreateTestPetOwner("Ali Raza");

        // Act
        var result = await _repository.SearchAsync("Ahmed");

        // Assert
        result.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetPagedAsync_ShouldReturnPaginatedResults()
    {
        // Arrange
        for (int i = 1; i <= 15; i++)
        {
            CreateTestPetOwner($"Owner {i}");
        }

        // Act
        var page1 = await _repository.GetPagedAsync(1, 10);
        var page2 = await _repository.GetPagedAsync(2, 10);

        // Assert
        page1.Items.Should().HaveCount(10);
        page2.Items.Should().HaveCount(5);
        page1.TotalCount.Should().Be(15);
        page1.TotalPages.Should().Be(2);
    }

    [Fact]
    public async Task SoftDelete_ShouldMarkAsInactive()
    {
        // Arrange
        var owner = CreateTestPetOwner("To Delete");

        // Act
        _repository.SoftDelete(owner);
        await Context.SaveChangesAsync();

        // Assert
        var result = await _repository.GetByIdAsync(owner.PetOwnerId);
        result!.IsActive.Should().BeFalse();
    }
}
