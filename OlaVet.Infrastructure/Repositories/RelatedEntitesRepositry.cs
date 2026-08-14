// =============================================
// COMBINED REPOSITORIES (Handle multiple related entities)
// =============================================

using Microsoft.EntityFrameworkCore;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Interfaces;
using OlaVet.Infrastructure.Data;

namespace OlaVet.Infrastructure.Repositories;

public class ReviewRepository : IReviewRepository
{
    private readonly OlaVetDbContext _context;
    
    public ReviewRepository(OlaVetDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<VetReview>> GetVetReviewsAsync(int vetId)
    {
        return await _context.VetReviews
            .Include(r => r.PetOwner)
            .Where(r => r.VetId == vetId)
            .OrderByDescending(r => r.ReviewDateTime)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<LabReview>> GetLabReviewsAsync(int labId)
    {
        return await _context.LabReviews
            .Include(r => r.PetOwner)
            .Where(r => r.LabId == labId)
            .OrderByDescending(r => r.ReviewDateTime)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<StoreReview>> GetStoreReviewsAsync(int storeId)
    {
        return await _context.StoreReviews
            .Include(r => r.PetOwner)
            .Where(r => r.StoreId == storeId)
            .OrderByDescending(r => r.ReviewDateTime)
            .ToListAsync();
    }
    
    public async Task<double> GetAverageRatingAsync(string entityType, int entityId)
    {
        return entityType.ToLower() switch
        {
            "vet" => await _context.VetReviews
                .Where(r => r.VetId == entityId)
                .AverageAsync(r => (double?)r.Rating) ?? 0,
                
            "lab" => await _context.LabReviews
                .Where(r => r.LabId == entityId)
                .AverageAsync(r => (double?)r.Rating) ?? 0,
                
            "store" => await _context.StoreReviews
                .Where(r => r.StoreId == entityId)
                .AverageAsync(r => (double?)r.Rating) ?? 0,
                
            _ => throw new ArgumentException($"Unknown entity type: {entityType}")
        };
    }
    
    public async Task<IEnumerable<VetReview>> GetVetReviewsByOwnerAsync(int ownerId)
    {
        return await _context.VetReviews
            .Include(r => r.Vet)
            .Where(r => r.PetOwnerId == ownerId)
            .OrderByDescending(r => r.ReviewDateTime)
            .ToListAsync();
    }
    
    public async Task<VetReview?> GetVetReviewByAppointmentAsync(int appointmentId)
    {
        return await _context.VetReviews
            .Include(r => r.PetOwner)
            .Include(r => r.Vet)
            .FirstOrDefaultAsync(r => r.VetAppointmentId == appointmentId);
    }
    
    public async Task<IEnumerable<LabReview>> GetLabReviewsByOwnerAsync(int ownerId)
    {
        return await _context.LabReviews
            .Include(r => r.Lab)
            .Where(r => r.PetOwnerId == ownerId)
            .OrderByDescending(r => r.ReviewDateTime)
            .ToListAsync();
    }
    
    public async Task<LabReview?> GetLabReviewByAppointmentAsync(int appointmentId)
    {
        return await _context.LabReviews
            .Include(r => r.PetOwner)
            .Include(r => r.Lab)
            .FirstOrDefaultAsync(r => r.LabAppointmentId == appointmentId);
    }
    
    public async Task<IEnumerable<StoreReview>> GetStoreReviewsByOwnerAsync(int ownerId)
    {
        return await _context.StoreReviews
            .Include(r => r.Store)
            .Where(r => r.PetOwnerId == ownerId)
            .OrderByDescending(r => r.ReviewDateTime)
            .ToListAsync();
    }
    
    public async Task<StoreReview?> GetStoreReviewByOrderAsync(int orderId)
    {
        return await _context.StoreReviews
            .Include(r => r.PetOwner)
            .Include(r => r.Store)
            .FirstOrDefaultAsync(r => r.MedicineOrderId == orderId);
    }
    
    public async Task<int> GetReviewCountAsync(string entityType, int entityId)
    {
        return entityType.ToLower() switch
        {
            "vet" => await _context.VetReviews.CountAsync(r => r.VetId == entityId),
            "lab" => await _context.LabReviews.CountAsync(r => r.LabId == entityId),
            "store" => await _context.StoreReviews.CountAsync(r => r.StoreId == entityId),
            _ => throw new ArgumentException($"Unknown entity type: {entityType}")
        };
    }
    
    public async Task<RatingDistribution> GetRatingDistributionAsync(string entityType, int entityId)
    {
        var ratings = entityType.ToLower() switch
        {
            "vet" => await _context.VetReviews.Where(r => r.VetId == entityId).Select(r => r.Rating).ToListAsync(),
            "lab" => await _context.LabReviews.Where(r => r.LabId == entityId).Select(r => r.Rating).ToListAsync(),
            "store" => await _context.StoreReviews.Where(r => r.StoreId == entityId).Select(r => r.Rating).ToListAsync(),
            _ => throw new ArgumentException($"Unknown entity type: {entityType}")
        };
        
        return new RatingDistribution
        {
            OneStar = ratings.Count(r => r == 1),
            TwoStar = ratings.Count(r => r == 2),
            ThreeStar = ratings.Count(r => r == 3),
            FourStar = ratings.Count(r => r == 4),
            FiveStar = ratings.Count(r => r == 5)
        };
    }
    
    public async Task<IEnumerable<RecentReview>> GetRecentReviewsAsync(int count = 10)
    {
        var vetReviews = await _context.VetReviews
            .AsNoTracking()
            .OrderByDescending(r => r.ReviewDateTime)
            .Take(count)
            .Select(r => new RecentReview
            {
                ReviewType = "Vet",
                ReviewId = r.VetReviewId,
                EntityId = r.VetId,
                EntityName = r.Vet.VetName,
                Rating = r.Rating,
                Comments = r.Comments,
                ReviewDateTime = r.ReviewDateTime,
                OwnerName = r.PetOwner.OwnerName
            })
            .ToListAsync();
            
        var labReviews = await _context.LabReviews
            .AsNoTracking()
            .OrderByDescending(r => r.ReviewDateTime)
            .Take(count)
            .Select(r => new RecentReview
            {
                ReviewType = "Lab",
                ReviewId = r.LabReviewId,
                EntityId = r.LabId,
                EntityName = r.Lab.LabName,
                Rating = r.Rating,
                Comments = r.Comments,
                ReviewDateTime = r.ReviewDateTime,
                OwnerName = r.PetOwner.OwnerName
            })
            .ToListAsync();
            
        var storeReviews = await _context.StoreReviews
            .AsNoTracking()
            .OrderByDescending(r => r.ReviewDateTime)
            .Take(count)
            .Select(r => new RecentReview
            {
                ReviewType = "Store",
                ReviewId = r.StoreReviewId,
                EntityId = r.StoreId,
                EntityName = r.Store.StoreName,
                Rating = r.Rating,
                Comments = r.Comments,
                ReviewDateTime = r.ReviewDateTime,
                OwnerName = r.PetOwner.OwnerName
            })
            .ToListAsync();
            
        return vetReviews.Concat(labReviews).Concat(storeReviews)
            .OrderByDescending(r => r.ReviewDateTime)
            .Take(count);
    }
}

// =============================================
// PAYMENT REPOSITORY
// =============================================

public class PaymentRepository : IPaymentRepository
{
    private readonly OlaVetDbContext _context;
    
    public PaymentRepository(OlaVetDbContext context)
    {
        _context = context;
    }
    
    public async Task<IEnumerable<VetPayment>> GetVetPaymentsAsync(int vetId)
    {
        return await _context.VetPayments
            .Include(p => p.PetOwner)
            .Include(p => p.VetAppointment)
            .Where(p => p.VetId == vetId)
            .OrderByDescending(p => p.PaymentDateTime)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<VetPayment>> GetVetPaymentsByOwnerAsync(int ownerId)
    {
        return await _context.VetPayments
            .Include(p => p.Vet)
            .Include(p => p.VetAppointment)
            .Where(p => p.PetOwnerId == ownerId)
            .OrderByDescending(p => p.PaymentDateTime)
            .ToListAsync();
    }
    
    public async Task<VetPayment?> GetVetPaymentByAppointmentAsync(int appointmentId)
    {
        return await _context.VetPayments
            .Include(p => p.PetOwner)
            .Include(p => p.Vet)
            .FirstOrDefaultAsync(p => p.VetAppointmentId == appointmentId);
    }
    
    public async Task<decimal> GetVetRevenueAsync(int vetId, DateTime startDate, DateTime endDate)
    {
        return await _context.VetPayments
            .Where(p => p.VetId == vetId && 
                       p.PaymentDateTime >= startDate && 
                       p.PaymentDateTime <= endDate)
            .SumAsync(p => p.Amount);
    }
    
    public async Task<IEnumerable<LabPayment>> GetLabPaymentsAsync(int labId)
    {
        return await _context.LabPayments
            .Include(p => p.PetOwner)
            .Include(p => p.LabAppointment)
            .Where(p => p.LabId == labId)
            .OrderByDescending(p => p.PaymentDateTime)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<LabPayment>> GetLabPaymentsByOwnerAsync(int ownerId)
    {
        return await _context.LabPayments
            .Include(p => p.Lab)
            .Include(p => p.LabAppointment)
            .Where(p => p.PetOwnerId == ownerId)
            .OrderByDescending(p => p.PaymentDateTime)
            .ToListAsync();
    }
    
    public async Task<LabPayment?> GetLabPaymentByAppointmentAsync(int appointmentId)
    {
        return await _context.LabPayments
            .Include(p => p.PetOwner)
            .Include(p => p.Lab)
            .FirstOrDefaultAsync(p => p.LabAppointmentId == appointmentId);
    }
    
    public async Task<decimal> GetLabRevenueAsync(int labId, DateTime startDate, DateTime endDate)
    {
        return await _context.LabPayments
            .Where(p => p.LabId == labId && 
                       p.PaymentDateTime >= startDate && 
                       p.PaymentDateTime <= endDate)
            .SumAsync(p => p.Amount);
    }
    
    public async Task<IEnumerable<StorePayment>> GetStorePaymentsAsync(int storeId)
    {
        return await _context.StorePayments
            .Include(p => p.PetOwner)
            .Include(p => p.MedicineOrder)
            .Where(p => p.StoreId == storeId)
            .OrderByDescending(p => p.PaymentDateTime)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<StorePayment>> GetStorePaymentsByOwnerAsync(int ownerId)
    {
        return await _context.StorePayments
            .Include(p => p.Store)
            .Include(p => p.MedicineOrder)
            .Where(p => p.PetOwnerId == ownerId)
            .OrderByDescending(p => p.PaymentDateTime)
            .ToListAsync();
    }
    
    public async Task<StorePayment?> GetStorePaymentByOrderAsync(int orderId)
    {
        return await _context.StorePayments
            .Include(p => p.PetOwner)
            .Include(p => p.Store)
            .FirstOrDefaultAsync(p => p.MedicineOrderId == orderId);
    }
    
    public async Task<decimal> GetStoreRevenueAsync(int storeId, DateTime startDate, DateTime endDate)
    {
        return await _context.StorePayments
            .Where(p => p.StoreId == storeId && 
                       p.PaymentDateTime >= startDate && 
                       p.PaymentDateTime <= endDate)
            .SumAsync(p => p.Amount);
    }
    
    public async Task<OwnerPaymentSummary> GetOwnerPaymentSummaryAsync(int ownerId)
    {
        // Use server-side aggregation instead of loading all records to memory
        var vetSummary = await _context.VetPayments
            .Where(p => p.PetOwnerId == ownerId)
            .GroupBy(p => 1)
            .Select(g => new { Total = g.Sum(p => p.Amount), Count = g.Count() })
            .FirstOrDefaultAsync();
            
        var labSummary = await _context.LabPayments
            .Where(p => p.PetOwnerId == ownerId)
            .GroupBy(p => 1)
            .Select(g => new { Total = g.Sum(p => p.Amount), Count = g.Count() })
            .FirstOrDefaultAsync();
            
        var storeSummary = await _context.StorePayments
            .Where(p => p.PetOwnerId == ownerId)
            .GroupBy(p => 1)
            .Select(g => new { Total = g.Sum(p => p.Amount), Count = g.Count() })
            .FirstOrDefaultAsync();
            
        return new OwnerPaymentSummary
        {
            OwnerId = ownerId,
            TotalVetPayments = vetSummary?.Total ?? 0,
            TotalLabPayments = labSummary?.Total ?? 0,
            TotalStorePayments = storeSummary?.Total ?? 0,
            VetPaymentCount = vetSummary?.Count ?? 0,
            LabPaymentCount = labSummary?.Count ?? 0,
            StorePaymentCount = storeSummary?.Count ?? 0
        };
    }
    
    public async Task<PaymentStatistics> GetPaymentStatisticsAsync(DateTime startDate, DateTime endDate)
    {
        var vetRevenue = await _context.VetPayments
            .Where(p => p.PaymentDateTime >= startDate && p.PaymentDateTime <= endDate)
            .SumAsync(p => p.Amount);
            
        var labRevenue = await _context.LabPayments
            .Where(p => p.PaymentDateTime >= startDate && p.PaymentDateTime <= endDate)
            .SumAsync(p => p.Amount);
            
        var storeRevenue = await _context.StorePayments
            .Where(p => p.PaymentDateTime >= startDate && p.PaymentDateTime <= endDate)
            .SumAsync(p => p.Amount);
            
        var vetCount = await _context.VetPayments
            .Where(p => p.PaymentDateTime >= startDate && p.PaymentDateTime <= endDate)
            .CountAsync();
            
        var labCount = await _context.LabPayments
            .Where(p => p.PaymentDateTime >= startDate && p.PaymentDateTime <= endDate)
            .CountAsync();
            
        var storeCount = await _context.StorePayments
            .Where(p => p.PaymentDateTime >= startDate && p.PaymentDateTime <= endDate)
            .CountAsync();
            
        var totalRevenue = vetRevenue + labRevenue + storeRevenue;
        var totalCount = vetCount + labCount + storeCount;
            
        return new PaymentStatistics
        {
            TotalRevenue = totalRevenue,
            VetRevenue = vetRevenue,
            LabRevenue = labRevenue,
            StoreRevenue = storeRevenue,
            TotalTransactions = totalCount,
            AverageTransactionAmount = totalCount > 0 ? totalRevenue / totalCount : 0
        };
    }
}
