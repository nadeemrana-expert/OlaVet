// =============================================
// File: OlaVet.Tests/Repositories/PetRepositoryTests.cs
// Unit tests for PetRepository
// =============================================

using FluentAssertions;
using OlaVet.Infrastructure.Repositories;
using OlaVet.Domain.Entities;

namespace OlaVet.Tests.Repositories;

public class PetRepositoryTests : RepositoryTestBase
{
    private readonly PetRepository _repository;

    public PetRepositoryTests()
    {
        _repository = new PetRepository(Context);
    }

    [Fact]
    public async Task GetByIdAsync_ShouldReturnPet_WhenExists()
    {
        // Arrange
        var owner = CreateTestPetOwner();
        var pet = CreateTestPet(owner.PetOwnerId, "Max");

        // Act
        var result = await _repository.GetByIdAsync(pet.PetId);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Max");
    }

    [Fact]
    public async Task GetByOwnerIdAsync_ShouldReturnAllPetsForOwner()
    {
        // Arrange
        var owner = CreateTestPetOwner("Multi Pet Owner");
        CreateTestPet(owner.PetOwnerId, "Pet 1");
        CreateTestPet(owner.PetOwnerId, "Pet 2");
        CreateTestPet(owner.PetOwnerId, "Pet 3");
        
        var anotherOwner = CreateTestPetOwner("Single Pet Owner");
        CreateTestPet(anotherOwner.PetOwnerId, "Other Pet");

        // Act
        var result = await _repository.GetByOwnerIdAsync(owner.PetOwnerId);

        // Assert
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task GetBySpeciesAsync_ShouldReturnPetsOfSpecies()
    {
        // Arrange
        var owner = CreateTestPetOwner();
        
        Context.Pets.AddRange(
            new Pet { PetOwnerId = owner.PetOwnerId, Name = "Dog 1", Species = "Dog", Breed = "Labrador" },
            new Pet { PetOwnerId = owner.PetOwnerId, Name = "Dog 2", Species = "Dog", Breed = "German Shepherd" },
            new Pet { PetOwnerId = owner.PetOwnerId, Name = "Cat 1", Species = "Cat", Breed = "Persian" }
        );
        await Context.SaveChangesAsync();

        // Act
        var result = await _repository.GetBySpeciesAsync("Dog");

        // Assert
        result.Should().HaveCount(2);
        result.All(p => p.Species == "Dog").Should().BeTrue();
    }

    [Fact]
    public async Task GetWithOwnerAsync_ShouldIncludeOwnerDetails()
    {
        // Arrange
        var owner = CreateTestPetOwner("Pet Owner Name");
        var pet = CreateTestPet(owner.PetOwnerId, "Pet With Owner");

        // Act
        var result = await _repository.GetWithOwnerAsync(pet.PetId);

        // Assert
        result.Should().NotBeNull();
        result!.PetOwner.Should().NotBeNull();
        result.PetOwner.OwnerName.Should().Be("Pet Owner Name");
    }

    [Fact]
    public async Task AddAsync_ShouldAddNewPet()
    {
        // Arrange
        var owner = CreateTestPetOwner();
        var pet = new Pet
        {
            PetOwnerId = owner.PetOwnerId,
            Name = "New Pet",
            Species = "Cat",
            Breed = "Siamese",
            Age = 2
        };

        // Act
        await _repository.AddAsync(pet);
        await Context.SaveChangesAsync();

        // Assert
        var result = await _repository.GetByIdAsync(pet.PetId);
        result.Should().NotBeNull();
        result!.Species.Should().Be("Cat");
    }

    [Fact]
    public async Task Update_ShouldModifyExistingPet()
    {
        // Arrange
        var owner = CreateTestPetOwner();
        var pet = CreateTestPet(owner.PetOwnerId, "Original Name");

        // Act
        pet.Name = "Updated Name";
        pet.Age = 5;
        _repository.Update(pet);
        await Context.SaveChangesAsync();

        // Assert
        var result = await _repository.GetByIdAsync(pet.PetId);
        result!.Name.Should().Be("Updated Name");
        result.Age.Should().Be(5);
    }

    [Fact]
    public async Task SoftDelete_ShouldMarkPetAsInactive()
    {
        // Arrange
        var owner = CreateTestPetOwner();
        var pet = CreateTestPet(owner.PetOwnerId, "To Delete");

        // Act
        _repository.SoftDelete(pet);
        await Context.SaveChangesAsync();

        // Assert
        var result = await _repository.GetByIdAsync(pet.PetId);
        result!.IsActive.Should().BeFalse();
    }
}
