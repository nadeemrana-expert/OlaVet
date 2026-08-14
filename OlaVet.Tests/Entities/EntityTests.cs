// =============================================
// File: OlaVet.Tests/Entities/EntityTests.cs
// Unit tests for Domain Entities
// =============================================

using FluentAssertions;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Common;

namespace OlaVet.Tests.Entities;

public class EntityTests
{
    [Fact]
    public void PetOwner_ShouldInheritFromBaseEntity()
    {
        // Arrange
        var owner = new PetOwner();

        // Assert
        owner.Should().BeAssignableTo<BaseEntity>();
    }

    [Fact]
    public void Pet_ShouldHaveCorrectDefaultValues()
    {
        // Arrange & Act
        var pet = new Pet();

        // Assert
        pet.IsActive.Should().BeTrue();
    }

    [Fact]
    public void Vet_ShouldHaveCorrectDefaultValues()
    {
        // Arrange & Act
        var vet = new Vet();

        // Assert
        vet.IsActive.Should().BeTrue();
        vet.Fee.Should().Be(0);
    }

    [Fact]
    public void MedicineOrderDetail_Subtotal_ShouldCalculateCorrectly()
    {
        // Arrange
        var orderDetail = new MedicineOrderDetail
        {
            Quantity = 5,
            UnitPrice = 100m
        };

        // Assert
        orderDetail.Subtotal.Should().Be(500m);
    }

    [Fact]
    public void PetOwner_Collections_ShouldBeInitialized()
    {
        // Arrange & Act
        var owner = new PetOwner();

        // Assert
        owner.Pets.Should().NotBeNull();
        owner.VetAppointments.Should().NotBeNull();
        owner.LabAppointments.Should().NotBeNull();
        owner.MedicineOrders.Should().NotBeNull();
    }

    [Fact]
    public void Vet_Collections_ShouldBeInitialized()
    {
        // Arrange & Act
        var vet = new Vet();

        // Assert
        vet.EducationQualifications.Should().NotBeNull();
        vet.Services.Should().NotBeNull();
        vet.Availabilities.Should().NotBeNull();
        vet.VetAppointments.Should().NotBeNull();
    }

    [Fact]
    public void Store_Collections_ShouldBeInitialized()
    {
        // Arrange & Act
        var store = new Store();

        // Assert
        store.Inventories.Should().NotBeNull();
        store.MedicineOrders.Should().NotBeNull();
    }

    [Fact]
    public void BaseEntity_ShouldHaveAuditProperties()
    {
        // Arrange & Act
        var owner = new PetOwner();

        // Assert
        owner.CreatedDate.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
        owner.IsActive.Should().BeTrue();
        owner.ModifiedDate.Should().BeNull();
    }

    [Fact]
    public void SoftDelete_ShouldSetIsActiveToFalse()
    {
        // Arrange
        var owner = new PetOwner();

        // Act
        owner.IsActive = false;

        // Assert
        owner.IsActive.Should().BeFalse();
    }
}

