// =============================================
// File: OlaVet.Tests/Controllers/PetOwnersControllerTests.cs
// Unit tests for PetOwnersController
// =============================================

using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using OlaVet.API.Controllers;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Interfaces;
using OlaVet.Domain.Common;

namespace OlaVet.Tests.Controllers;

public class PetOwnersControllerTests
{
    private readonly Mock<IUnitOfWork> _mockUnitOfWork;
    private readonly Mock<ILogger<PetOwnersController>> _mockLogger;
    private readonly PetOwnersController _controller;

    public PetOwnersControllerTests()
    {
        _mockUnitOfWork = new Mock<IUnitOfWork>();
        _mockLogger = new Mock<ILogger<PetOwnersController>>();
        _controller = new PetOwnersController(_mockUnitOfWork.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task GetById_ShouldReturnOk_WhenOwnerExists()
    {
        // Arrange
        var owner = new PetOwner 
        { 
            PetOwnerId = 1, 
            OwnerName = "Test Owner",
            Email = "test@test.com",
            ContactNumber = "+92-300-1234567"
        };
        
        _mockUnitOfWork.Setup(u => u.PetOwners.GetByIdAsync(1))
            .ReturnsAsync(owner);

        // Act
        var result = await _controller.GetById(1);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedOwner = okResult.Value.Should().BeOfType<PetOwner>().Subject;
        returnedOwner.OwnerName.Should().Be("Test Owner");
    }

    [Fact]
    public async Task GetById_ShouldReturnNotFound_WhenOwnerDoesNotExist()
    {
        // Arrange
        _mockUnitOfWork.Setup(u => u.PetOwners.GetByIdAsync(999))
            .ReturnsAsync((PetOwner?)null);

        // Act
        var result = await _controller.GetById(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task GetAll_ShouldReturnPaginatedResults()
    {
        // Arrange
        var pagedResult = new PagedResult<PetOwner>
        {
            Items = new List<PetOwner>
            {
                new() { PetOwnerId = 1, OwnerName = "Owner 1", Email = "o1@test.com", ContactNumber = "1" },
                new() { PetOwnerId = 2, OwnerName = "Owner 2", Email = "o2@test.com", ContactNumber = "2" }
            },
            Page = 1,
            PageSize = 10,
            TotalCount = 2
        };
        
        _mockUnitOfWork.Setup(u => u.PetOwners.GetPagedAsync(1, 10))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _controller.GetAll(1, 10);

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedResult = okResult.Value.Should().BeOfType<PagedResult<PetOwner>>().Subject;
        returnedResult.Items.Should().HaveCount(2);
    }

    [Fact]
    public async Task Create_ShouldReturnCreatedAtAction()
    {
        // Arrange
        var request = new CreatePetOwnerRequest
        {
            OwnerName = "New Owner",
            Email = "new@test.com",
            ContactNumber = "+92-300-9999999"
        };
        
        _mockUnitOfWork.Setup(u => u.PetOwners.AddAsync(It.IsAny<PetOwner>(), default))
            .ReturnsAsync((PetOwner o, CancellationToken _) => o);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        var result = await _controller.Create(request);

        // Assert
        var createdResult = result.Should().BeOfType<CreatedAtActionResult>().Subject;
        createdResult.ActionName.Should().Be(nameof(PetOwnersController.GetById));
    }

    [Fact]
    public async Task Delete_ShouldReturnNoContent_WhenOwnerExists()
    {
        // Arrange
        var owner = new PetOwner 
        { 
            PetOwnerId = 1, 
            OwnerName = "To Delete",
            Email = "delete@test.com",
            ContactNumber = "+92-300-1111111"
        };
        
        _mockUnitOfWork.Setup(u => u.PetOwners.GetByIdAsync(1))
            .ReturnsAsync(owner);
        _mockUnitOfWork.Setup(u => u.SaveChangesAsync(default))
            .ReturnsAsync(1);

        // Act
        var result = await _controller.Delete(1);

        // Assert
        result.Should().BeOfType<NoContentResult>();
        _mockUnitOfWork.Verify(u => u.PetOwners.SoftDelete(owner), Times.Once);
    }

    [Fact]
    public async Task Delete_ShouldReturnNotFound_WhenOwnerDoesNotExist()
    {
        // Arrange
        _mockUnitOfWork.Setup(u => u.PetOwners.GetByIdAsync(999))
            .ReturnsAsync((PetOwner?)null);

        // Act
        var result = await _controller.Delete(999);

        // Assert
        result.Should().BeOfType<NotFoundObjectResult>();
    }

    [Fact]
    public async Task Search_ShouldReturnBadRequest_WhenTermIsEmpty()
    {
        // Act
        var result = await _controller.Search("");

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task Search_ShouldReturnResults_WhenTermIsProvided()
    {
        // Arrange
        var owners = new List<PetOwner>
        {
            new() { PetOwnerId = 1, OwnerName = "Ahmed Khan", Email = "a@test.com", ContactNumber = "1" },
            new() { PetOwnerId = 2, OwnerName = "Sara Ahmed", Email = "s@test.com", ContactNumber = "2" }
        };
        
        _mockUnitOfWork.Setup(u => u.PetOwners.SearchAsync("Ahmed"))
            .ReturnsAsync(owners);

        // Act
        var result = await _controller.Search("Ahmed");

        // Assert
        var okResult = result.Should().BeOfType<OkObjectResult>().Subject;
        var returnedOwners = okResult.Value.Should().BeAssignableTo<IEnumerable<PetOwner>>().Subject;
        returnedOwners.Should().HaveCount(2);
    }

    [Fact]
    public async Task AddFunds_ShouldReturnBadRequest_WhenAmountIsNegative()
    {
        // Arrange
        var owner = new PetOwner 
        { 
            PetOwnerId = 1, 
            OwnerName = "Test",
            Email = "t@test.com",
            ContactNumber = "1",
            Wallet = 1000
        };
        
        _mockUnitOfWork.Setup(u => u.PetOwners.GetByIdAsync(1))
            .ReturnsAsync(owner);
        
        var request = new AddFundsRequest { Amount = -100 };

        // Act
        var result = await _controller.AddFunds(1, request);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
    }
}
