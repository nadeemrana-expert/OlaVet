using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlaVet.Infrastructure.Data;

namespace OlaVet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    private readonly OlaVetDbContext _context;

    public TestController(OlaVetDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Test database connection
    /// </summary>
    [HttpGet("connection")]
    public async Task<IActionResult> TestConnection()
    {
        try
        {
            // Try to connect and query
            var canConnect = await _context.Database.CanConnectAsync();

            if (!canConnect)
            {
                return StatusCode(500, new { Message = "Cannot connect to database" });
            }

            var petOwnerCount = await _context.PetOwners.CountAsync();
            var vetCount = await _context.Vets.CountAsync();
            var petCount = await _context.Pets.CountAsync();
            var appointmentCount = await _context.VetAppointments.CountAsync();

            return Ok(new
            {
                Status = "Connected",
                Database = _context.Database.GetDbConnection().Database,
                Statistics = new
                {
                    PetOwners = petOwnerCount,
                    Vets = vetCount,
                    Pets = petCount,
                    Appointments = appointmentCount
                }
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = "Database connection failed",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Test a simple query with relationships
    /// </summary>
    [HttpGet("test-query")]
    public async Task<IActionResult> TestQuery()
    {
        try
        {
            // Query with Include (eager loading)
            var ownerWithPets = await _context.PetOwners
                .Include(o => o.Pets)
                .FirstOrDefaultAsync();

            if (ownerWithPets == null)
            {
                return Ok(new { Message = "No data found" });
            }

            return Ok(new
            {
                Owner = new
                {
                    ownerWithPets.OwnerName,
                    ownerWithPets.Email,
                    PetCount = ownerWithPets.Pets.Count
                },
                Pets = ownerWithPets.Pets.Select(p => new
                {
                    p.Name,
                    p.Species,
                    p.Breed,
                    p.Age
                })
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = "Query failed",
                Error = ex.Message
            });
        }
    }

    /// <summary>
    /// Test complex query with multiple relationships
    /// </summary>
    [HttpGet("test-complex-query")]
    public async Task<IActionResult> TestComplexQuery()
    {
        try
        {
            // Complex query joining multiple tables
            var appointmentDetails = await _context.VetAppointments
                .Include(a => a.Pet)
                    .ThenInclude(p => p.PetOwner)
                .Include(a => a.Vet)
                .Include(a => a.StatusType)
                .OrderByDescending(a => a.AppointmentDateTime)
                .Take(5)
                .Select(a => new
                {
                    AppointmentId = a.VetAppointmentId,
                    AppointmentDate = a.AppointmentDateTime,
                    Pet = new
                    {
                        a.Pet.Name,
                        a.Pet.Species,
                        Owner = a.Pet.PetOwner.OwnerName
                    },
                    Vet = new
                    {
                        a.Vet.VetName,
                        a.Vet.Specialization,
                        a.Vet.Fee
                    },
                    Status = a.StatusType.StatusName,
                    a.Reason
                })
                .ToListAsync();

            return Ok(new
            {
                Message = "Complex query successful",
                Count = appointmentDetails.Count,
                Appointments = appointmentDetails
            });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new
            {
                Message = "Complex query failed",
                Error = ex.Message,
                InnerError = ex.InnerException?.Message
            });
        }
    }
}