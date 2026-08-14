// =============================================
// File: OlaVet.API/Controllers/DashboardController.cs
// Dashboard API Controller - Statistics & Reports
// =============================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OlaVet.API.Extensions;
using OlaVet.API.Security;
using OlaVet.Domain.Interfaces;
using OlaVet.Infrastructure.Data;

namespace OlaVet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DashboardController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly OlaVetDbContext _context;
    private readonly ILogger<DashboardController> _logger;

    public DashboardController(
        IUnitOfWork unitOfWork, 
        OlaVetDbContext context,
        ILogger<DashboardController> logger)
    {
        _unitOfWork = unitOfWork;
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Get role-based dashboard statistics
    /// </summary>
    [HttpGet("stats")]
    public async Task<IActionResult> GetStats()
    {
        if (User.IsAdmin())
            return Ok(await GetAdminStatsAsync());
        if (User.IsPetOwner())
            return Ok(await GetPetOwnerStatsAsync());
        if (User.IsVet())
            return Ok(await GetVetStatsAsync());
        if (User.IsLabTechnician())
            return Ok(await GetLabTechStatsAsync());
        if (User.IsStoreManager())
            return Ok(await GetStoreManagerStatsAsync());
        return Forbid();
    }

    private async Task<object> GetAdminStatsAsync()
    {
        var petOwners = await _context.PetOwners.CountAsync(o => o.IsActive);
        var vets = await _context.Vets.CountAsync(v => v.IsActive);
        var pets = await _context.Pets.CountAsync(p => p.IsActive);
        var labs = await _context.Labs.CountAsync(l => l.IsActive);
        var stores = await _context.Stores.CountAsync(s => s.IsActive);
        var vetAppts = await _context.VetAppointments.CountAsync();
        var labAppts = await _context.LabAppointments.CountAsync();
        var orders = await _context.MedicineOrders.CountAsync();
        var meds = await _context.Medicines.CountAsync(m => m.IsActive);

        return new
        {
            Role = "Admin",
            PetOwners = petOwners,
            Vets = vets,
            Pets = pets,
            Labs = labs,
            Stores = stores,
            VetAppointments = vetAppts,
            LabAppointments = labAppts,
            MedicineOrders = orders,
            Medicines = meds
        };
    }

    private async Task<object> GetPetOwnerStatsAsync()
    {
        var ownerId = User.GetPetOwnerId();
        if (ownerId == null) return new { Role = "PetOwner" };

        var myPets = await _context.Pets.CountAsync(p => p.IsActive && p.PetOwnerId == ownerId);
        var myVetAppts = await _context.VetAppointments.CountAsync(a => a.PetOwnerId == ownerId);
        var myLabAppts = await _context.LabAppointments.CountAsync(a => a.PetOwnerId == ownerId);
        var myOrders = await _context.MedicineOrders.CountAsync(o => o.PetOwnerId == ownerId);
        var upcomingAppts = await _context.VetAppointments
            .CountAsync(a => a.PetOwnerId == ownerId && a.AppointmentDateTime >= DateTime.UtcNow);
        var wallet = await _context.PetOwners
            .Where(o => o.PetOwnerId == ownerId)
            .Select(o => o.Wallet)
            .FirstOrDefaultAsync();

        return new
        {
            Role = "PetOwner",
            Pets = myPets,
            VetAppointments = myVetAppts,
            LabAppointments = myLabAppts,
            MedicineOrders = myOrders,
            UpcomingAppointments = upcomingAppts,
            WalletBalance = wallet
        };
    }

    private async Task<object> GetVetStatsAsync()
    {
        var vetId = User.GetVetId();
        if (vetId == null) return new { Role = "Vet" };

        var today = DateTime.UtcNow.Date;
        var todayAppts = await _context.VetAppointments
            .CountAsync(a => a.VetId == vetId && a.AppointmentDateTime.Date == today);
        var weekAppts = await _context.VetAppointments
            .CountAsync(a => a.VetId == vetId && a.AppointmentDateTime >= today.AddDays(-7));
        var totalAppts = await _context.VetAppointments.CountAsync(a => a.VetId == vetId);
        var totalPatients = await _context.VetAppointments
            .Where(a => a.VetId == vetId)
            .Select(a => a.PetOwnerId)
            .Distinct()
            .CountAsync();

        return new
        {
            Role = "Vet",
            TodayAppointments = todayAppts,
            WeekAppointments = weekAppts,
            VetAppointments = totalAppts,
            TotalPatients = totalPatients
        };
    }

    private async Task<object> GetLabTechStatsAsync()
    {
        var statusTypes = await _context.Set<OlaVet.Domain.Entities.Lookups.StatusType>()
            .AsNoTracking()
            .Where(s => s.StatusName == "Completed" || s.StatusName == "Scheduled")
            .ToDictionaryAsync(s => s.StatusName, s => s.StatusTypeId);

        var scheduledId = statusTypes.GetValueOrDefault("Scheduled");
        var completedId = statusTypes.GetValueOrDefault("Completed");

        return new
        {
            Role = "LabTechnician",
            LabAppointments = await _context.LabAppointments.CountAsync(),
            PendingTests = await _context.LabAppointments.CountAsync(a => a.StatusTypeId == scheduledId),
            CompletedTests = await _context.LabAppointments.CountAsync(a => a.StatusTypeId == completedId)
        };
    }

    private async Task<object> GetStoreManagerStatsAsync()
    {
        var statusTypes = await _context.Set<OlaVet.Domain.Entities.Lookups.StatusType>()
            .AsNoTracking()
            .Where(s => s.StatusName == "Pending")
            .ToDictionaryAsync(s => s.StatusName, s => s.StatusTypeId);

        var pendingId = statusTypes.GetValueOrDefault("Pending");

        return new
        {
            Role = "StoreManager",
            MedicineOrders = await _context.MedicineOrders.CountAsync(),
            PendingOrders = await _context.MedicineOrders.CountAsync(o => o.StatusTypeId == pendingId),
            Medicines = await _context.Medicines.CountAsync(m => m.IsActive)
        };
    }

    /// <summary>
    /// Get payment statistics for date range
    /// </summary>
    [HttpGet("payments")]
    [HasPermission("admin.full")]
    public async Task<IActionResult> GetPaymentStats(
        [FromQuery] DateTime? startDate = null,
        [FromQuery] DateTime? endDate = null)
    {
        var start = startDate ?? DateTime.UtcNow.AddMonths(-1);
        var end = endDate ?? DateTime.UtcNow;
        
        var stats = await _unitOfWork.Payments.GetPaymentStatisticsAsync(start, end);
        
        return Ok(new
        {
            Period = new { StartDate = start, EndDate = end },
            Statistics = stats
        });
    }

    /// <summary>
    /// Get recent reviews
    /// </summary>
    [HttpGet("recent-reviews")]
    public async Task<IActionResult> GetRecentReviews([FromQuery] int count = 10)
    {
        var reviews = await _unitOfWork.Reviews.GetRecentReviewsAsync(count);
        return Ok(reviews);
    }

    /// <summary>
    /// Get appointment statistics
    /// </summary>
    [HttpGet("appointments")]
    public async Task<IActionResult> GetAppointmentStats()
    {
        var today = DateTime.UtcNow.Date;
        var weekAgo = today.AddDays(-7);
        var monthAgo = today.AddMonths(-1);
        
        var statusTypes = await _context.Set<OlaVet.Domain.Entities.Lookups.StatusType>()
            .AsNoTracking()
            .Where(s => s.StatusName == "Completed" || s.StatusName == "Scheduled" || s.StatusName == "Cancelled")
            .ToDictionaryAsync(s => s.StatusName, s => s.StatusTypeId);
        
        var completedId = statusTypes.GetValueOrDefault("Completed");
        var scheduledId = statusTypes.GetValueOrDefault("Scheduled");
        var cancelledId = statusTypes.GetValueOrDefault("Cancelled");

        // Apply role-based filtering
        IQueryable<OlaVet.Domain.Entities.VetAppointment> vetQuery = _context.VetAppointments;
        IQueryable<OlaVet.Domain.Entities.LabAppointment> labQuery = _context.LabAppointments;

        if (User.IsPetOwner())
        {
            var ownerId = User.GetPetOwnerId();
            if (ownerId != null)
            {
                vetQuery = vetQuery.Where(a => a.PetOwnerId == ownerId);
                labQuery = labQuery.Where(a => a.PetOwnerId == ownerId);
            }
        }
        else if (User.IsVet())
        {
            var vetId = User.GetVetId();
            if (vetId != null)
                vetQuery = vetQuery.Where(a => a.VetId == vetId);
        }
        
        var vetStats = new
        {
            Today = await vetQuery.CountAsync(a => a.AppointmentDateTime.Date == today),
            ThisWeek = await vetQuery.CountAsync(a => a.AppointmentDateTime >= weekAgo),
            ThisMonth = await vetQuery.CountAsync(a => a.AppointmentDateTime >= monthAgo),
            Completed = await vetQuery.CountAsync(a => a.StatusTypeId == completedId),
            Scheduled = await vetQuery.CountAsync(a => a.StatusTypeId == scheduledId),
            Cancelled = await vetQuery.CountAsync(a => a.StatusTypeId == cancelledId)
        };
        
        var labStats = new
        {
            Today = await labQuery.CountAsync(a => a.AppointmentDateTime.Date == today),
            ThisWeek = await labQuery.CountAsync(a => a.AppointmentDateTime >= weekAgo),
            ThisMonth = await labQuery.CountAsync(a => a.AppointmentDateTime >= monthAgo),
            Completed = await labQuery.CountAsync(a => a.StatusTypeId == completedId),
            Scheduled = await labQuery.CountAsync(a => a.StatusTypeId == scheduledId)
        };
        
        return Ok(new { VetAppointments = vetStats, LabAppointments = labStats });
    }

    /// <summary>
    /// Get top performers (vets, labs, stores)
    /// </summary>
    [HttpGet("top-performers")]
    [HasPermission("admin.full", "admin.reports")]
    public async Task<IActionResult> GetTopPerformers([FromQuery] int count = 5)
    {
        var topVets = await _unitOfWork.Vets.GetTopRatedAsync(count);
        var topLabs = await _unitOfWork.Labs.GetTopRatedAsync(count);
        var topStores = await _unitOfWork.Stores.GetTopRatedAsync(count);
        
        return Ok(new
        {
            TopVets = topVets.Select(v => new { v.VetId, v.VetName, v.Specialization }),
            TopLabs = topLabs.Select(l => new { l.LabId, l.LabName, l.Specialization }),
            TopStores = topStores.Select(s => new { s.StoreId, s.StoreName, s.StoreAddress })
        });
    }

    /// <summary>
    /// Get species distribution
    /// </summary>
    [HttpGet("species-distribution")]
    [HasPermission("admin.full", "pets.read")]
    public async Task<IActionResult> GetSpeciesDistribution()
    {
        var distribution = await _context.Pets
            .Where(p => p.IsActive)
            .GroupBy(p => p.Species)
            .Select(g => new { Species = g.Key, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .ToListAsync();
            
        return Ok(distribution);
    }

    /// <summary>
    /// Get revenue trend
    /// </summary>
    [HttpGet("revenue-trend")]
    [HasPermission("admin.full")]
    public async Task<IActionResult> GetRevenueTrend([FromQuery] int days = 30)
    {
        var startDate = DateTime.UtcNow.AddDays(-days).Date;
        
        var vetRevenue = await _context.VetPayments
            .Where(p => p.PaymentDateTime >= startDate)
            .GroupBy(p => p.PaymentDateTime.Date)
            .Select(g => new { Date = g.Key, Amount = g.Sum(p => p.Amount) })
            .ToListAsync();
            
        var labRevenue = await _context.LabPayments
            .Where(p => p.PaymentDateTime >= startDate)
            .GroupBy(p => p.PaymentDateTime.Date)
            .Select(g => new { Date = g.Key, Amount = g.Sum(p => p.Amount) })
            .ToListAsync();
            
        var storeRevenue = await _context.StorePayments
            .Where(p => p.PaymentDateTime >= startDate)
            .GroupBy(p => p.PaymentDateTime.Date)
            .Select(g => new { Date = g.Key, Amount = g.Sum(p => p.Amount) })
            .ToListAsync();
            
        return Ok(new
        {
            VetRevenue = vetRevenue,
            LabRevenue = labRevenue,
            StoreRevenue = storeRevenue
        });
    }
}
