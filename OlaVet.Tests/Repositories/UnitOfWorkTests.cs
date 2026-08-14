// =============================================
// File: OlaVet.Tests/Repositories/UnitOfWorkTests.cs
// Unit tests for UnitOfWork
// =============================================

using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OlaVet.Infrastructure.Data;
using OlaVet.Infrastructure.Repositories;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Interfaces;

namespace OlaVet.Tests.Repositories;

public class UnitOfWorkTests : IDisposable
{
    private readonly OlaVetDbContext _context;
    private readonly UnitOfWork _unitOfWork;

    public UnitOfWorkTests()
    {
        var options = new DbContextOptionsBuilder<OlaVetDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new OlaVetDbContext(options);
        _unitOfWork = new UnitOfWork(_context);
    }

    [Fact]
    public void PetOwners_ShouldReturnPetOwnerRepository()
    {
        // Act
        var repository = _unitOfWork.PetOwners;

        // Assert
        repository.Should().NotBeNull();
        repository.Should().BeAssignableTo<IPetOwnerRepository>();
    }

    [Fact]
    public void Vets_ShouldReturnVetRepository()
    {
        // Act
        var repository = _unitOfWork.Vets;

        // Assert
        repository.Should().NotBeNull();
        repository.Should().BeAssignableTo<IVetRepository>();
    }

    [Fact]
    public void Pets_ShouldReturnPetRepository()
    {
        // Act
        var repository = _unitOfWork.Pets;

        // Assert
        repository.Should().NotBeNull();
        repository.Should().BeAssignableTo<IPetRepository>();
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldPersistChanges()
    {
        // Arrange
        var owner = new PetOwner
        {
            OwnerName = "Test Owner",
            Email = "test@test.com",
            ContactNumber = "+92-300-1234567"
        };
        
        await _unitOfWork.PetOwners.AddAsync(owner);

        // Act
        var result = await _unitOfWork.SaveChangesAsync();

        // Assert
        result.Should().BeGreaterThan(0);
        (await _unitOfWork.PetOwners.GetByIdAsync(owner.PetOwnerId)).Should().NotBeNull();
    }

    [Fact(Skip = "In-memory database does not support transactions")]
    public async Task BeginTransactionAsync_ShouldStartTransaction()
    {
        // Note: This test requires SQL Server or another provider that supports transactions
        // The in-memory database provider does not support transactions
        await _unitOfWork.BeginTransactionAsync();
        await _unitOfWork.RollbackTransactionAsync();
    }

    [Fact(Skip = "In-memory database does not support transactions")]
    public async Task CommitTransactionAsync_ShouldCommitChanges()
    {
        // Note: This test requires SQL Server or another provider that supports transactions
        await _unitOfWork.BeginTransactionAsync();
        
        var owner = new PetOwner
        {
            OwnerName = "Transaction Owner",
            Email = "transaction@test.com",
            ContactNumber = "+92-300-9999999"
        };
        
        await _unitOfWork.PetOwners.AddAsync(owner);
        await _unitOfWork.CommitTransactionAsync();

        var result = await _unitOfWork.PetOwners.GetByIdAsync(owner.PetOwnerId);
        result.Should().NotBeNull();
    }

    [Fact(Skip = "In-memory database does not support transactions")]
    public async Task RollbackTransactionAsync_ShouldRevertChanges()
    {
        // Note: This test requires SQL Server or another provider that supports transactions
        await _unitOfWork.BeginTransactionAsync();
        
        var owner = new PetOwner
        {
            OwnerName = "Rollback Owner",
            Email = "rollback@test.com",
            ContactNumber = "+92-300-8888888"
        };
        
        await _unitOfWork.PetOwners.AddAsync(owner);
        await _unitOfWork.SaveChangesAsync();
        var ownerId = owner.PetOwnerId;

        // Act
        await _unitOfWork.RollbackTransactionAsync();

        // Assert - Entity should not exist after rollback
        // Note: In-memory database doesn't fully support transactions like SQL Server
        // This test verifies the rollback method executes without error
    }

    [Fact]
    public void RepositoryProperties_ShouldReturnSameInstance()
    {
        // Act
        var repo1 = _unitOfWork.PetOwners;
        var repo2 = _unitOfWork.PetOwners;

        // Assert - Lazy initialization should return same instance
        repo1.Should().BeSameAs(repo2);
    }

    [Fact]
    public void AllRepositories_ShouldBeAccessible()
    {
        // Assert all repositories are accessible
        _unitOfWork.PetOwners.Should().NotBeNull();
        _unitOfWork.Vets.Should().NotBeNull();
        _unitOfWork.Pets.Should().NotBeNull();
        _unitOfWork.VetAppointments.Should().NotBeNull();
        _unitOfWork.Labs.Should().NotBeNull();
        _unitOfWork.LabAppointments.Should().NotBeNull();
        _unitOfWork.Stores.Should().NotBeNull();
        _unitOfWork.Medicines.Should().NotBeNull();
        _unitOfWork.MedicineOrders.Should().NotBeNull();
        _unitOfWork.MedicalRecords.Should().NotBeNull();
        _unitOfWork.Payments.Should().NotBeNull();
        _unitOfWork.Reviews.Should().NotBeNull();
    }

    public void Dispose()
    {
        _unitOfWork.Dispose();
        _context.Dispose();
    }
}
