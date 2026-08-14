// =============================================
// File: OlaVet.API/Controllers/LabsController.cs
// Laboratories API Controller
// =============================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OlaVet.API.Security;
using OlaVet.Domain.Interfaces;

namespace OlaVet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class LabsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<LabsController> _logger;

    public LabsController(IUnitOfWork unitOfWork, ILogger<LabsController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get all labs with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _unitOfWork.Labs.GetPagedAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Get lab by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var lab = await _unitOfWork.Labs.GetByIdAsync(id);
        
        if (lab == null)
            return NotFound(new { Message = $"Lab with ID {id} not found" });
            
        return Ok(lab);
    }

    /// <summary>
    /// Get lab with appointments
    /// </summary>
    [HttpGet("{id}/appointments")]
    public async Task<IActionResult> GetWithAppointments(int id)
    {
        var lab = await _unitOfWork.Labs.GetWithAppointmentsAsync(id);
        
        if (lab == null)
            return NotFound(new { Message = $"Lab with ID {id} not found" });
            
        return Ok(new
        {
            lab.LabId,
            lab.LabName,
            lab.LabAddress,
            lab.Specialization,
            TotalAppointments = lab.LabAppointments.Count,
            RecentAppointments = lab.LabAppointments
                .OrderByDescending(a => a.AppointmentDateTime)
                .Take(10)
                .Select(a => new
                {
                    a.LabAppointmentId,
                    a.AppointmentDateTime,
                    a.Notes
                })
        });
    }

    /// <summary>
    /// Get labs with ratings
    /// </summary>
    [HttpGet("with-ratings")]
    public async Task<IActionResult> GetWithRatings()
    {
        var labs = await _unitOfWork.Labs.GetLabsWithRatingsAsync();
        return Ok(labs);
    }

    /// <summary>
    /// Get top rated labs
    /// </summary>
    [HttpGet("top-rated")]
    public async Task<IActionResult> GetTopRated([FromQuery] int count = 10)
    {
        var labs = await _unitOfWork.Labs.GetTopRatedAsync(count);
        return Ok(labs);
    }

    /// <summary>
    /// Search labs
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return BadRequest(new { Message = "Search term is required" });
            
        var labs = await _unitOfWork.Labs.SearchAsync(term);
        return Ok(labs);
    }

    /// <summary>
    /// Get labs by specialization
    /// </summary>
    [HttpGet("specialization/{specialization}")]
    public async Task<IActionResult> GetBySpecialization(string specialization)
    {
        var labs = await _unitOfWork.Labs.GetBySpecializationAsync(specialization);
        return Ok(labs);
    }

    /// <summary>
    /// Get lab reviews
    /// </summary>
    [HttpGet("{id}/reviews")]
    public async Task<IActionResult> GetReviews(int id)
    {
        var reviews = await _unitOfWork.Reviews.GetLabReviewsAsync(id);
        return Ok(reviews.Select(r => new
        {
            r.LabReviewId,
            r.Rating,
            r.Comments,
            r.ReviewDateTime,
            OwnerName = r.PetOwner?.OwnerName
        }));
    }

    /// <summary>
    /// Get lab rating distribution
    /// </summary>
    [HttpGet("{id}/rating-distribution")]
    public async Task<IActionResult> GetRatingDistribution(int id)
    {
        var distribution = await _unitOfWork.Reviews.GetRatingDistributionAsync("lab", id);
        return Ok(distribution);
    }
}
