// =============================================
// File: OlaVet.API/Controllers/VetsController.cs
// Veterinarians API Controller
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
public class VetsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<VetsController> _logger;

    public VetsController(IUnitOfWork unitOfWork, ILogger<VetsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get all vets with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _unitOfWork.Vets.GetPagedAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Get vet by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var vet = await _unitOfWork.Vets.GetByIdAsync(id);
        
        if (vet == null)
            return NotFound(new { Message = $"Vet with ID {id} not found" });
            
        return Ok(vet);
    }

    /// <summary>
    /// Get vet with full details
    /// </summary>
    [HttpGet("{id}/details")]
    public async Task<IActionResult> GetWithDetails(int id)
    {
        var vet = await _unitOfWork.Vets.GetWithDetailsAsync(id);
        
        if (vet == null)
            return NotFound(new { Message = $"Vet with ID {id} not found" });
            
        return Ok(new
        {
            vet.VetId,
            vet.VetName,
            vet.Specialization,
            vet.ClinicLocation,
            vet.Fee,
            vet.ContactNumber,
            vet.Email,
            vet.YearsOfExperience,
            vet.LicenseNumber,
            Qualifications = vet.EducationQualifications.Select(q => new
            {
                q.QualificationName,
                q.Institute,
                q.YearOfDegree
            }),
            Services = vet.Services.Select(s => new
            {
                s.ServiceType,
                s.ServiceDescription,
                s.ServiceFee
            }),
            Availabilities = vet.Availabilities.Where(a => a.IsAvailable).Select(a => new
            {
                a.DayOfWeek,
                StartTime = a.StartTime.ToString(@"hh\:mm"),
                EndTime = a.EndTime.ToString(@"hh\:mm"),
                a.SlotDurationMinutes
            })
        });
    }

    /// <summary>
    /// Get vets with ratings
    /// </summary>
    [HttpGet("with-ratings")]
    public async Task<IActionResult> GetWithRatings()
    {
        var vets = await _unitOfWork.Vets.GetVetsWithRatingsAsync();
        return Ok(vets);
    }

    /// <summary>
    /// Get top rated vets
    /// </summary>
    [HttpGet("top-rated")]
    public async Task<IActionResult> GetTopRated([FromQuery] int count = 10)
    {
        var vets = await _unitOfWork.Vets.GetTopRatedAsync(count);
        return Ok(vets);
    }

    /// <summary>
    /// Search vets
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return BadRequest(new { Message = "Search term is required" });
            
        var vets = await _unitOfWork.Vets.SearchAsync(term);
        return Ok(vets);
    }

    /// <summary>
    /// Get vets by specialization
    /// </summary>
    [HttpGet("specialization/{specialization}")]
    public async Task<IActionResult> GetBySpecialization(string specialization)
    {
        var vets = await _unitOfWork.Vets.GetBySpecializationAsync(specialization);
        return Ok(vets);
    }

    /// <summary>
    /// Get available vets for date/time
    /// </summary>
    [HttpGet("available")]
    public async Task<IActionResult> GetAvailable([FromQuery] DateTime dateTime)
    {
        var vets = await _unitOfWork.Vets.GetAvailableVetsAsync(dateTime);
        return Ok(vets);
    }

    /// <summary>
    /// Get vet reviews
    /// </summary>
    [HttpGet("{id}/reviews")]
    public async Task<IActionResult> GetReviews(int id)
    {
        var reviews = await _unitOfWork.Reviews.GetVetReviewsAsync(id);
        return Ok(reviews.Select(r => new
        {
            r.VetReviewId,
            r.Rating,
            r.Comments,
            r.ReviewDateTime,
            OwnerName = r.PetOwner?.OwnerName
        }));
    }

    /// <summary>
    /// Get vet rating distribution
    /// </summary>
    [HttpGet("{id}/rating-distribution")]
    public async Task<IActionResult> GetRatingDistribution(int id)
    {
        var distribution = await _unitOfWork.Reviews.GetRatingDistributionAsync("vet", id);
        return Ok(distribution);
    }

    /// <summary>
    /// Create new vet
    /// </summary>
    [HttpPost]
    [HasPermission("vets.create")]
    public async Task<IActionResult> Create([FromBody] CreateVetRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);
            
        var vet = new Vet
        {
            VetName = request.VetName,
            Specialization = request.Specialization,
            ClinicLocation = request.ClinicLocation,
            Fee = request.Fee,
            ContactNumber = request.ContactNumber,
            Email = request.Email,
            YearsOfExperience = request.YearsOfExperience,
            LicenseNumber = request.LicenseNumber
        };
        
        await _unitOfWork.Vets.AddAsync(vet);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("Created vet: {VetId} - {VetName}", vet.VetId, vet.VetName);
        
        return CreatedAtAction(nameof(GetById), new { id = vet.VetId }, vet);
    }

    /// <summary>
    /// Update vet
    /// </summary>
    [HttpPut("{id}")]
    [HasPermission("vets.update")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateVetRequest request)
    {
        // Vet can only update their own profile
        if (User.IsVet() && User.GetVetId() != id)
            return Forbid();

        var vet = await _unitOfWork.Vets.GetByIdAsync(id);
        
        if (vet == null)
            return NotFound(new { Message = $"Vet with ID {id} not found" });
            
        vet.VetName = request.VetName ?? vet.VetName;
        vet.Specialization = request.Specialization ?? vet.Specialization;
        vet.ClinicLocation = request.ClinicLocation ?? vet.ClinicLocation;
        vet.Fee = request.Fee ?? vet.Fee;
        vet.ContactNumber = request.ContactNumber ?? vet.ContactNumber;
        vet.Email = request.Email ?? vet.Email;
        
        _unitOfWork.Vets.Update(vet);
        await _unitOfWork.SaveChangesAsync();
        
        return Ok(vet);
    }

    /// <summary>
    /// Delete vet (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [HasPermission("vets.delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var vet = await _unitOfWork.Vets.GetByIdAsync(id);
        
        if (vet == null)
            return NotFound(new { Message = $"Vet with ID {id} not found" });
            
        _unitOfWork.Vets.SoftDelete(vet);
        await _unitOfWork.SaveChangesAsync();
        
        return NoContent();
    }
}

// =============================================
// REQUEST MODELS
// =============================================

public class CreateVetRequest
{
    public required string VetName { get; set; }
    public string? Specialization { get; set; }
    public string? ClinicLocation { get; set; }
    public decimal Fee { get; set; }
    public required string ContactNumber { get; set; }
    public string? Email { get; set; }
    public int? YearsOfExperience { get; set; }
    public string? LicenseNumber { get; set; }
}

public class UpdateVetRequest
{
    public string? VetName { get; set; }
    public string? Specialization { get; set; }
    public string? ClinicLocation { get; set; }
    public decimal? Fee { get; set; }
    public string? ContactNumber { get; set; }
    public string? Email { get; set; }
}
