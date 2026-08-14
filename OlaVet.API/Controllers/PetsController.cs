// =============================================
// File: OlaVet.API/Controllers/PetsController.cs
// Pets API Controller
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
public class PetsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<PetsController> _logger;

    public PetsController(IUnitOfWork unitOfWork, ILogger<PetsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get all pets with pagination
    /// </summary>
    [HttpGet]
    [HasPermission("pets.read")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        // PetOwner sees only their own pets
        if (User.IsPetOwner())
        {
            var ownerId = User.GetPetOwnerId();
            if (ownerId == null) return Forbid();
            var myPets = await _unitOfWork.Pets.GetByOwnerIdAsync(ownerId.Value);
            return Ok(myPets);
        }

        // Admin, Vet see all
        var result = await _unitOfWork.Pets.GetPagedAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Get pet by ID
    /// </summary>
    [HttpGet("{id}")]
    [HasPermission("pets.read")]
    public async Task<IActionResult> GetById(int id)
    {
        var pet = await _unitOfWork.Pets.GetByIdAsync(id);
        
        if (pet == null)
            return NotFound(new { Message = $"Pet with ID {id} not found" });

        // PetOwner can only see their own pets
        if (User.IsPetOwner() && pet.PetOwnerId != User.GetPetOwnerId())
            return Forbid();
            
        return Ok(pet);
    }

    /// <summary>
    /// Get pet with owner
    /// </summary>
    [HttpGet("{id}/with-owner")]
    [HasPermission("pets.read")]
    public async Task<IActionResult> GetWithOwner(int id)
    {
        var pet = await _unitOfWork.Pets.GetWithOwnerAsync(id);
        
        if (pet == null)
            return NotFound(new { Message = $"Pet with ID {id} not found" });

        // PetOwner can only see their own pets
        if (User.IsPetOwner() && pet.PetOwnerId != User.GetPetOwnerId())
            return Forbid();
            
        return Ok(new
        {
            pet.PetId,
            pet.Name,
            pet.Species,
            pet.Breed,
            pet.Age,
            pet.PetWeight,
            pet.Color,
            pet.Gender,
            Owner = pet.PetOwner == null ? null : new
            {
                pet.PetOwner.PetOwnerId,
                pet.PetOwner.OwnerName,
                pet.PetOwner.Email,
                pet.PetOwner.ContactNumber
            }
        });
    }

    /// <summary>
    /// Get pet with medical history
    /// </summary>
    [HttpGet("{id}/medical-history")]
    [HasPermission("pets.read")]
    public async Task<IActionResult> GetMedicalHistory(int id)
    {
        var pet = await _unitOfWork.Pets.GetWithMedicalHistoryAsync(id);
        
        if (pet == null)
            return NotFound(new { Message = $"Pet with ID {id} not found" });

        // PetOwner can only see their own pet's records
        if (User.IsPetOwner() && pet.PetOwnerId != User.GetPetOwnerId())
            return Forbid();
            
        return Ok(new
        {
            pet.PetId,
            pet.Name,
            pet.Species,
            MedicalRecords = (pet.MedicalRecords ?? Enumerable.Empty<OlaVet.Domain.Entities.MedicalRecord>())
                .OrderByDescending(r => r.RecordDate).Select(r => new
            {
                r.RecordId,
                r.RecordDate,
                RecordType = r.RecordType?.TypeName,
                r.Diagnosis,
                r.TreatmentDescription,
                VetName = r.Vet?.VetName
            })
        });
    }

    /// <summary>
    /// Get pets by owner
    /// </summary>
    [HttpGet("owner/{ownerId}")]
    [HasPermission("pets.read")]
    public async Task<IActionResult> GetByOwner(int ownerId)
    {
        // PetOwner can only see their own pets
        if (User.IsPetOwner() && User.GetPetOwnerId() != ownerId)
            return Forbid();

        var pets = await _unitOfWork.Pets.GetByOwnerIdAsync(ownerId);
        return Ok(pets);
    }

    /// <summary>
    /// Get pets by species
    /// </summary>
    [HttpGet("species/{species}")]
    [HasPermission("pets.read")]
    public async Task<IActionResult> GetBySpecies(string species)
    {
        var pets = await _unitOfWork.Pets.GetBySpeciesAsync(species);
        return Ok(pets);
    }

    /// <summary>
    /// Get pets due for checkup
    /// </summary>
    [HttpGet("due-for-checkup")]
    [HasPermission("pets.read", "vets.read")]
    public async Task<IActionResult> GetDueForCheckup([FromQuery] int daysThreshold = 180)
    {
        var pets = await _unitOfWork.Pets.GetDueForCheckupAsync(daysThreshold);
        return Ok(pets);
    }

    /// <summary>
    /// Create new pet
    /// </summary>
    [HttpPost]
    [HasPermission("pets.create")]
    public async Task<IActionResult> Create([FromBody] CreatePetRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // PetOwner can only create pets for themselves
        if (User.IsPetOwner())
        {
            var ownerId = User.GetPetOwnerId();
            if (ownerId == null || request.PetOwnerId != ownerId)
                return Forbid();
        }
            
        // Verify owner exists
        var owner = await _unitOfWork.PetOwners.GetByIdAsync(request.PetOwnerId);
        if (owner == null)
            return BadRequest(new { Message = $"Pet owner with ID {request.PetOwnerId} not found" });
            
        var pet = new Pet
        {
            PetOwnerId = request.PetOwnerId,
            Name = request.Name,
            Species = request.Species,
            Breed = request.Breed,
            Age = request.Age,
            PetWeight = request.Weight,
            Color = request.Color,
            Gender = request.Gender,
            DateOfBirth = request.DateOfBirth
        };
        
        await _unitOfWork.Pets.AddAsync(pet);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("Created pet: {PetId} - {PetName} for owner {OwnerId}", 
            pet.PetId, pet.Name, pet.PetOwnerId);
        
        return CreatedAtAction(nameof(GetById), new { id = pet.PetId }, pet);
    }

    /// <summary>
    /// Update pet
    /// </summary>
    [HttpPut("{id}")]
    [HasPermission("pets.update")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdatePetRequest request)
    {
        var pet = await _unitOfWork.Pets.GetByIdAsync(id);
        
        if (pet == null)
            return NotFound(new { Message = $"Pet with ID {id} not found" });

        // PetOwner can only update their own pets
        if (User.IsPetOwner() && pet.PetOwnerId != User.GetPetOwnerId())
            return Forbid();
            
        pet.Name = request.Name ?? pet.Name;
        pet.Species = request.Species ?? pet.Species;
        pet.Breed = request.Breed ?? pet.Breed;
        pet.Age = request.Age ?? pet.Age;
        pet.PetWeight = request.Weight ?? pet.PetWeight;
        pet.Color = request.Color ?? pet.Color;
        pet.Gender = request.Gender ?? pet.Gender;
        
        _unitOfWork.Pets.Update(pet);
        await _unitOfWork.SaveChangesAsync();
        
        return Ok(pet);
    }

    /// <summary>
    /// Delete pet (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [HasPermission("pets.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var pet = await _unitOfWork.Pets.GetByIdAsync(id);
        
        if (pet == null)
            return NotFound(new { Message = $"Pet with ID {id} not found" });
            
        _unitOfWork.Pets.SoftDelete(pet);
        await _unitOfWork.SaveChangesAsync();
        
        return NoContent();
    }
}

// =============================================
// REQUEST MODELS
// =============================================

public class CreatePetRequest
{
    public int PetOwnerId { get; set; }
    public required string Name { get; set; }
    public required string Species { get; set; }
    public string? Breed { get; set; }
    public int? Age { get; set; }
    public decimal? Weight { get; set; }
    public string? Color { get; set; }
    public string? Gender { get; set; }
    public DateTime? DateOfBirth { get; set; }
}

public class UpdatePetRequest
{
    public string? Name { get; set; }
    public string? Species { get; set; }
    public string? Breed { get; set; }
    public int? Age { get; set; }
    public decimal? Weight { get; set; }
    public string? Color { get; set; }
    public string? Gender { get; set; }
}
