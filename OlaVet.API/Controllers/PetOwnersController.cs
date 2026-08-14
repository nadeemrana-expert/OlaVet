// =============================================
// File: OlaVet.API/Controllers/PetOwnersController.cs
// Pet Owners API Controller
// =============================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OlaVet.API.Extensions;
using OlaVet.API.Security;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Interfaces;

namespace OlaVet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PetOwnersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PetOwnersController> _logger;

    public PetOwnersController(IUnitOfWork unitOfWork, ILogger<PetOwnersController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get all pet owners with pagination
    /// </summary>
    [HttpGet]
    [HasPermission("petowners.read", "vets.read")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _unitOfWork.PetOwners.GetPagedAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Get pet owner by ID
    /// </summary>
    [HttpGet("{id}")]
    [HasPermission("petowners.read")]
    [ResourceOwner("petOwnerId")]
    public async Task<IActionResult> GetById(int id)
    {
        var owner = await _unitOfWork.PetOwners.GetByIdAsync(id);
        
        if (owner == null)
            return NotFound(new { Message = $"Pet owner with ID {id} not found" });
            
        return Ok(owner);
    }

    /// <summary>
    /// Get pet owner with pets
    /// </summary>
    [HttpGet("{id}/with-pets")]
    [HasPermission("petowners.read")]
    [ResourceOwner("petOwnerId")]
    public async Task<IActionResult> GetWithPets(int id)
    {
        var owner = await _unitOfWork.PetOwners.GetWithPetsAsync(id);
        
        if (owner == null)
            return NotFound(new { Message = $"Pet owner with ID {id} not found" });
            
        return Ok(new
        {
            owner.PetOwnerId,
            owner.OwnerName,
            owner.Email,
            owner.ContactNumber,
            owner.Wallet,
            Pets = owner.Pets.Select(p => new
            {
                p.PetId,
                p.Name,
                p.Species,
                p.Breed,
                p.Age
            })
        });
    }

    /// <summary>
    /// Search pet owners
    /// </summary>
    [HttpGet("search")]
    [HasPermission("petowners.read", "vets.read")]
    public async Task<IActionResult> Search([FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return BadRequest(new { Message = "Search term is required" });
            
        var owners = await _unitOfWork.PetOwners.SearchAsync(term);
        return Ok(owners);
    }

    /// <summary>
    /// Create new pet owner
    /// </summary>
    [HttpPost]
    [HasPermission("petowners.create")]
    public async Task<IActionResult> Create([FromBody] CreatePetOwnerRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
            
        var owner = new PetOwner
        {
            OwnerName = request.OwnerName,
            Email = request.Email,
            ContactNumber = request.ContactNumber,
            HomeAddress = request.HomeAddress,
            Age = request.Age,
            Gender = request.Gender,
            Wallet = request.InitialWalletBalance
        };
        
        await _unitOfWork.PetOwners.AddAsync(owner);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("Created pet owner: {OwnerId} - {OwnerName}", owner.PetOwnerId, owner.OwnerName);
        
        return CreatedAtAction(nameof(GetById), new { id = owner.PetOwnerId }, owner);
    }

    /// <summary>
    /// Update pet owner
    /// </summary>
    [HttpPut("{id}")]
    [HasPermission("petowners.update")]
    [ResourceOwner("petOwnerId")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePetOwnerRequest request)
    {
        var owner = await _unitOfWork.PetOwners.GetByIdAsync(id);
        
        if (owner == null)
            return NotFound(new { Message = $"Pet owner with ID {id} not found" });
            
        owner.OwnerName = request.OwnerName ?? owner.OwnerName;
        owner.Email = request.Email ?? owner.Email;
        owner.ContactNumber = request.ContactNumber ?? owner.ContactNumber;
        owner.HomeAddress = request.HomeAddress ?? owner.HomeAddress;
        owner.Age = request.Age ?? owner.Age;
        owner.Gender = request.Gender ?? owner.Gender;
        
        _unitOfWork.PetOwners.Update(owner);
        await _unitOfWork.SaveChangesAsync();
        
        return Ok(owner);
    }

    /// <summary>
    /// Add funds to wallet
    /// </summary>
    [HttpPost("{id}/add-funds")]
    [HasPermission("petowners.update")]
    [ResourceOwner("petOwnerId")]
    public async Task<IActionResult> AddFunds(int id, [FromBody] AddFundsRequest request)
    {
        var owner = await _unitOfWork.PetOwners.GetByIdAsync(id);
        
        if (owner == null)
            return NotFound(new { Message = $"Pet owner with ID {id} not found" });
            
        if (request.Amount <= 0)
            return BadRequest(new { Message = "Amount must be greater than 0" });
            
        owner.Wallet += request.Amount;
        
        _unitOfWork.PetOwners.Update(owner);
        await _unitOfWork.SaveChangesAsync();
        
        return Ok(new { owner.Wallet, Message = $"Added {request.Amount:C} to wallet" });
    }

    /// <summary>
    /// Delete pet owner (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [HasPermission("petowners.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var owner = await _unitOfWork.PetOwners.GetByIdAsync(id);
        
        if (owner == null)
            return NotFound(new { Message = $"Pet owner with ID {id} not found" });
            
        _unitOfWork.PetOwners.SoftDelete(owner);
        await _unitOfWork.SaveChangesAsync();
        
        return NoContent();
    }

    /// <summary>
    /// Get payment summary for pet owner
    /// </summary>
    [HttpGet("{id}/payment-summary")]
    [HasPermission("petowners.read")]
    [ResourceOwner("petOwnerId")]
    public async Task<IActionResult> GetPaymentSummary(int id)
    {
        var owner = await _unitOfWork.PetOwners.GetByIdAsync(id);
        
        if (owner == null)
            return NotFound(new { Message = $"Pet owner with ID {id} not found" });
            
        var summary = await _unitOfWork.Payments.GetOwnerPaymentSummaryAsync(id);
        return Ok(summary);
    }
}

// =============================================
// REQUEST MODELS
// =============================================

public class CreatePetOwnerRequest
{
    public required string OwnerName { get; set; }
    public required string Email { get; set; }
    public required string ContactNumber { get; set; }
    public string? HomeAddress { get; set; }
    public int? Age { get; set; }
    public string? Gender { get; set; }
    public decimal InitialWalletBalance { get; set; } = 0;
}

public class UpdatePetOwnerRequest
{
    public string? OwnerName { get; set; }
    public string? Email { get; set; }
    public string? ContactNumber { get; set; }
    public string? HomeAddress { get; set; }
    public int? Age { get; set; }
    public string? Gender { get; set; }
}

public class AddFundsRequest
{
    public decimal Amount { get; set; }
}
