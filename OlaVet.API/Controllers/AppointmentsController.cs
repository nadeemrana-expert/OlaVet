// =============================================
// File: OlaVet.API/Controllers/AppointmentsController.cs
// Appointments API Controller
// =============================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlaVet.API.Extensions;
using OlaVet.API.Security;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Interfaces;
using OlaVet.Infrastructure.Data;

namespace OlaVet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class AppointmentsController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly OlaVetDbContext _context;
    private readonly ILogger<AppointmentsController> _logger;

    public AppointmentsController(IUnitOfWork unitOfWork, OlaVetDbContext context, ILogger<AppointmentsController> logger)
    {
        _unitOfWork = unitOfWork;
        _context = context;
        _logger = logger;
    }

    // =============================================
    // VET APPOINTMENTS
    // =============================================

    /// <summary>
    /// Get all vet appointments with pagination
    /// </summary>
    [HttpGet("vet")]
    [HasPermission("appointments.read")]
    public async Task<IActionResult> GetVetAppointments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (User.IsPetOwner())
        {
            var ownerId = User.GetPetOwnerId();
            if (ownerId == null) return Forbid();
            var appointments = await _unitOfWork.VetAppointments.GetByOwnerIdAsync(ownerId.Value);
            return Ok(appointments);
        }

        if (User.IsVet())
        {
            var vetId = User.GetVetId();
            if (vetId == null) return Forbid();
            var appointments = await _context.VetAppointments
                .Where(a => a.VetId == vetId)
                .OrderByDescending(a => a.AppointmentDateTime)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return Ok(appointments);
        }

        // Admin sees all
        var result = await _unitOfWork.VetAppointments.GetPagedAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Get vet appointment by ID
    /// </summary>
    [HttpGet("vet/{id}")]
    [HasPermission("appointments.read")]
    public async Task<IActionResult> GetVetAppointment(int id)
    {
        var appointment = await _unitOfWork.VetAppointments.GetWithDetailsAsync(id);
        
        if (appointment == null)
            return NotFound(new { Message = $"Vet appointment with ID {id} not found" });

        // PetOwner can only see their own appointments
        if (User.IsPetOwner() && appointment.PetOwnerId != User.GetPetOwnerId())
            return Forbid();
        // Vet can only see their own appointments
        if (User.IsVet() && appointment.VetId != User.GetVetId())
            return Forbid();
            
        return Ok(new
        {
            appointment.VetAppointmentId,
            appointment.AppointmentDateTime,
            appointment.Reason,
            appointment.Notes,
            Status = appointment.StatusType?.StatusName,
            AppointmentType = appointment.VetAppointmentType?.TypeName,
            Pet = appointment.Pet == null ? null : new { appointment.Pet.PetId, appointment.Pet.Name, appointment.Pet.Species },
            Owner = appointment.PetOwner == null ? null : new { appointment.PetOwner.PetOwnerId, appointment.PetOwner.OwnerName },
            Vet = appointment.Vet == null ? null : new { appointment.Vet.VetId, appointment.Vet.VetName, appointment.Vet.Fee }
        });
    }

    /// <summary>
    /// Get vet appointments by owner
    /// </summary>
    [HttpGet("vet/owner/{ownerId}")]
    [HasPermission("appointments.read")]
    public async Task<IActionResult> GetByOwner(int ownerId)
    {
        // PetOwner can only see their own appointments
        if (User.IsPetOwner() && User.GetPetOwnerId() != ownerId)
            return Forbid();

        var appointments = await _unitOfWork.VetAppointments.GetByOwnerIdAsync(ownerId);
        return Ok(appointments);
    }

    /// <summary>
    /// Get vet appointments by vet and date
    /// </summary>
    [HttpGet("vet/schedule")]
    [HasPermission("appointments.read")]
    public async Task<IActionResult> GetVetSchedule(
        [FromQuery] int vetId,
        [FromQuery] DateTime date)
    {
        // Vet can only see their own schedule
        if (User.IsVet() && User.GetVetId() != vetId)
            return Forbid();

        var appointments = await _unitOfWork.VetAppointments.GetByVetAndDateAsync(vetId, date);
        return Ok(appointments);
    }

    /// <summary>
    /// Get upcoming vet appointments
    /// </summary>
    [HttpGet("vet/upcoming")]
    [HasPermission("appointments.read")]
    public async Task<IActionResult> GetUpcoming([FromQuery] int days = 7)
    {
        if (User.IsPetOwner())
        {
            var ownerId = User.GetPetOwnerId();
            if (ownerId == null) return Forbid();
            var cutoff = DateTime.UtcNow.AddDays(days);
            var myAppts = await _context.VetAppointments
                .Where(a => a.PetOwnerId == ownerId && a.AppointmentDateTime >= DateTime.UtcNow && a.AppointmentDateTime <= cutoff)
                .OrderBy(a => a.AppointmentDateTime)
                .ToListAsync();
            return Ok(myAppts);
        }

        if (User.IsVet())
        {
            var vetId = User.GetVetId();
            if (vetId == null) return Forbid();
            var cutoff = DateTime.UtcNow.AddDays(days);
            var myAppts = await _context.VetAppointments
                .Where(a => a.VetId == vetId && a.AppointmentDateTime >= DateTime.UtcNow && a.AppointmentDateTime <= cutoff)
                .OrderBy(a => a.AppointmentDateTime)
                .ToListAsync();
            return Ok(myAppts);
        }

        // Admin sees all
        var appointments = await _unitOfWork.VetAppointments.GetUpcomingAsync(days);
        return Ok(appointments);
    }

    /// <summary>
    /// Get pet appointment history
    /// </summary>
    [HttpGet("vet/pet/{petId}/history")]
    [HasPermission("appointments.read")]
    public async Task<IActionResult> GetPetHistory(int petId)
    {
        // PetOwner can only see their own pet's history
        if (User.IsPetOwner())
        {
            var pet = await _unitOfWork.Pets.GetByIdAsync(petId);
            if (pet == null || pet.PetOwnerId != User.GetPetOwnerId())
                return Forbid();
        }

        var appointments = await _unitOfWork.VetAppointments.GetPetHistoryAsync(petId);
        return Ok(appointments);
    }

    /// <summary>
    /// Get available time slots for vet
    /// </summary>
    [HttpGet("vet/available-slots")]
    [HasPermission("appointments.read", "appointments.create")]
    public async Task<IActionResult> GetAvailableSlots(
        [FromQuery] int vetId,
        [FromQuery] DateTime date)
    {
        var slots = await _unitOfWork.VetAppointments.GetAvailableTimeSlotsAsync(vetId, date);
        return Ok(slots);
    }

    /// <summary>
    /// Create vet appointment
    /// </summary>
    [HttpPost("vet")]
    [HasPermission("appointments.create")]
    public async Task<IActionResult> CreateVetAppointment([FromBody] CreateVetAppointmentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // PetOwner can only book for themselves
        if (User.IsPetOwner())
        {
            var ownerId = User.GetPetOwnerId();
            if (ownerId == null || request.PetOwnerId != ownerId)
                return Forbid();
        }
            
        // Check time slot availability
        var isAvailable = await _unitOfWork.VetAppointments
            .IsTimeSlotAvailableAsync(request.VetId, request.AppointmentDateTime);
            
        if (!isAvailable)
            return BadRequest(new { Message = "The selected time slot is not available" });
            
        var appointment = new VetAppointment
        {
            PetId = request.PetId,
            PetOwnerId = request.PetOwnerId,
            VetId = request.VetId,
            VetAppointmentTypeId = request.AppointmentTypeId,
            StatusTypeId = 1, // Scheduled
            AppointmentDateTime = request.AppointmentDateTime,
            Reason = request.Reason,
            Notes = request.Notes
        };
        
        await _unitOfWork.VetAppointments.AddAsync(appointment);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("Created vet appointment: {AppointmentId}", appointment.VetAppointmentId);
        
        return CreatedAtAction(nameof(GetVetAppointment), new { id = appointment.VetAppointmentId }, appointment);
    }

    // =============================================
    // LAB APPOINTMENTS
    // =============================================

    /// <summary>
    /// Get all lab appointments with pagination
    /// </summary>
    [HttpGet("lab")]
    [HasPermission("appointments.read")]
    public async Task<IActionResult> GetLabAppointments(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        if (User.IsPetOwner())
        {
            var ownerId = User.GetPetOwnerId();
            if (ownerId == null) return Forbid();
            var appointments = await _unitOfWork.LabAppointments.GetByOwnerIdAsync(ownerId.Value);
            return Ok(appointments);
        }

        // Admin, LabTechnician see all
        var result = await _unitOfWork.LabAppointments.GetPagedAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Get lab appointment by ID
    /// </summary>
    [HttpGet("lab/{id}")]
    [HasPermission("appointments.read")]
    public async Task<IActionResult> GetLabAppointment(int id)
    {
        var appointment = await _unitOfWork.LabAppointments.GetWithDetailsAsync(id);
        
        if (appointment == null)
            return NotFound(new { Message = $"Lab appointment with ID {id} not found" });

        // PetOwner can only see their own appointments
        if (User.IsPetOwner() && appointment.PetOwnerId != User.GetPetOwnerId())
            return Forbid();
            
        return Ok(new
        {
            appointment.LabAppointmentId,
            appointment.AppointmentDateTime,
            appointment.Notes,
            Status = appointment.StatusType?.StatusName,
            Pet = appointment.Pet == null ? null : new { appointment.Pet.PetId, appointment.Pet.Name },
            Owner = appointment.PetOwner == null ? null : new { appointment.PetOwner.PetOwnerId, appointment.PetOwner.OwnerName },
            Lab = appointment.Lab == null ? null : new { appointment.Lab.LabId, appointment.Lab.LabName },
            Tests = appointment.LabAppointmentTests?.Select(t => new
            {
                t.LabAppointmentTestId,
                TestName = t.LabTest?.LabTestName,
                t.TestResult,
                t.ResultDate
            }) ?? Enumerable.Empty<object>()
        });
    }

    /// <summary>
    /// Get lab appointments by owner
    /// </summary>
    [HttpGet("lab/owner/{ownerId}")]
    [HasPermission("appointments.read")]
    public async Task<IActionResult> GetLabByOwner(int ownerId)
    {
        // PetOwner can only see their own
        if (User.IsPetOwner() && User.GetPetOwnerId() != ownerId)
            return Forbid();

        var appointments = await _unitOfWork.LabAppointments.GetByOwnerIdAsync(ownerId);
        return Ok(appointments);
    }

    /// <summary>
    /// Get upcoming lab appointments
    /// </summary>
    [HttpGet("lab/upcoming")]
    [HasPermission("appointments.read")]
    public async Task<IActionResult> GetLabUpcoming([FromQuery] int days = 7)
    {
        var appointments = await _unitOfWork.LabAppointments.GetUpcomingAsync(days);
        return Ok(appointments);
    }

    /// <summary>
    /// Create lab appointment
    /// </summary>
    [HttpPost("lab")]
    [HasPermission("appointments.create")]
    public async Task<IActionResult> CreateLabAppointment([FromBody] CreateLabAppointmentRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // PetOwner can only book for themselves
        if (User.IsPetOwner())
        {
            var ownerId = User.GetPetOwnerId();
            if (ownerId == null || request.PetOwnerId != ownerId)
                return Forbid();
        }
            
        var appointment = new LabAppointment
        {
            PetId = request.PetId,
            PetOwnerId = request.PetOwnerId,
            LabId = request.LabId,
            StatusTypeId = 1, // Scheduled
            AppointmentDateTime = request.AppointmentDateTime,
            Notes = request.Notes
        };
        
        await _unitOfWork.LabAppointments.AddAsync(appointment);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("Created lab appointment: {AppointmentId}", appointment.LabAppointmentId);
        
        return CreatedAtAction(nameof(GetLabAppointment), new { id = appointment.LabAppointmentId }, appointment);
    }
}

// =============================================
// REQUEST MODELS
// =============================================

public class CreateVetAppointmentRequest
{
    public int PetId { get; set; }
    public int PetOwnerId { get; set; }
    public int VetId { get; set; }
    public int AppointmentTypeId { get; set; } // 1 = Clinic, 2 = Video
    public DateTime AppointmentDateTime { get; set; }
    public string? Reason { get; set; }
    public string? Notes { get; set; }
}

public class CreateLabAppointmentRequest
{
    public int PetId { get; set; }
    public int PetOwnerId { get; set; }
    public int LabId { get; set; }
    public DateTime AppointmentDateTime { get; set; }
    public string? Notes { get; set; }
    public List<int>? TestIds { get; set; }
}
