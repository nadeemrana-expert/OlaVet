
// =============================================
// File: OlaVet.Infrastructure/Repositories/VetRepository.cs
// Specific repository for Vet with custom queries
// =============================================

using Microsoft.EntityFrameworkCore;
using OlaVet.Domain.Common;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Interfaces;
using OlaVet.Infrastructure.Data;

namespace OlaVet.Infrastructure.Repositories;

public class VetRepository : Repository<Vet>, IVetRepository
{
    public VetRepository(OlaVetDbContext context) : base(context)
    {
    }
    
    public async Task<Vet?> GetWithDetailsAsync(int vetId)
    {
        return await _dbSet
            .Include(v => v.EducationQualifications)
            .Include(v => v.Services)
            .Include(v => v.Availabilities)
            .FirstOrDefaultAsync(v => v.VetId == vetId);
    }
    
    public async Task<IEnumerable<Vet>> SearchAsync(string searchTerm)
    {
        var lowerSearchTerm = searchTerm.ToLower();
        
        return await _dbSet
            .Where(v => v.IsActive &&
                       (v.VetName.ToLower().Contains(lowerSearchTerm) ||
                        (v.Specialization != null && 
                         v.Specialization.ToLower().Contains(lowerSearchTerm))))
            .OrderBy(v => v.VetName)
            .Take(50)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Vet>> GetBySpecializationAsync(string specialization)
    {
        return await _dbSet
            .Where(v => v.IsActive && 
                       v.Specialization != null &&
                       v.Specialization.ToLower() == specialization.ToLower())
            .OrderBy(v => v.Fee)
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Vet>> GetAvailableVetsAsync(DateTime dateTime)
    {
        var dayOfWeek = dateTime.DayOfWeek.ToString();
        var timeOfDay = dateTime.TimeOfDay;
        
        return await _dbSet
            .Include(v => v.Availabilities)
            .Where(v => v.IsActive &&
                       v.Availabilities.Any(a => 
                           a.DayOfWeek == dayOfWeek &&
                           a.IsAvailable &&
                           a.StartTime <= timeOfDay &&
                           a.EndTime >= timeOfDay))
            .ToListAsync();
    }
    
    public async Task<IEnumerable<Vet>> GetTopRatedAsync(int count = 10)
    {
        // Use projection to avoid loading full VetReview entities
        var topVetIds = await _context.VetReviews
            .GroupBy(r => r.VetId)
            .Select(g => new
            {
                VetId = g.Key,
                AvgRating = g.Average(r => r.Rating),
                ReviewCount = g.Count()
            })
            .OrderByDescending(x => x.AvgRating)
            .ThenByDescending(x => x.ReviewCount)
            .Take(count)
            .Select(x => x.VetId)
            .ToListAsync();
        
        return await _dbSet
            .Where(v => topVetIds.Contains(v.VetId) && v.IsActive)
            .AsNoTracking()
            .ToListAsync();
    }
    
    public async Task<IEnumerable<VetWithRating>> GetVetsWithRatingsAsync()
    {
        return await _dbSet
            .AsNoTracking()
            .Where(v => v.IsActive)
            .Select(v => new VetWithRating
            {
                VetId = v.VetId,
                VetName = v.VetName,
                Specialization = v.Specialization,
                Fee = v.Fee,
                AverageRating = v.VetReviews.Any() 
                    ? v.VetReviews.Average(r => r.Rating) 
                    : 0,
                ReviewCount = v.VetReviews.Count
            })
            .OrderByDescending(v => v.AverageRating)
            .ToListAsync();
    }
    
    public async Task<PagedResult<VetWithRating>> GetVetsWithRatingsAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbSet
            .AsNoTracking()
            .Where(v => v.IsActive)
            .Select(v => new VetWithRating
            {
                VetId = v.VetId,
                VetName = v.VetName,
                Specialization = v.Specialization,
                ClinicLocation = v.ClinicLocation,
                Fee = v.Fee,
                ContactNumber = v.ContactNumber,
                Email = v.Email,
                YearsOfExperience = v.YearsOfExperience,
                LicenseNumber = v.LicenseNumber,
                IsActive = v.IsActive,
                AverageRating = v.VetReviews.Any() 
                    ? v.VetReviews.Average(r => r.Rating) 
                    : 0,
                ReviewCount = v.VetReviews.Count
            })
            .OrderByDescending(v => v.AverageRating);
        
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        return new PagedResult<VetWithRating>(items, totalCount, page, pageSize);
    }
}