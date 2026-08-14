// =============================================
// File: OlaVet.Domain/Interfaces/IRepository.cs
// Generic repository interface - base for all repositories
// =============================================

using System.Linq.Expressions;
using OlaVet.Domain.Common;

namespace OlaVet.Domain.Interfaces;

/// <summary>
/// Generic repository interface providing common CRUD operations
/// T = Entity type (PetOwner, Vet, Pet, etc.)
/// </summary>
public interface IRepository<T> where T : class
{
    // =============================================
    // QUERY METHODS
    // =============================================
    
    /// <summary>
    /// Get entity by primary key
    /// </summary>
    Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Get entity by primary key with related entities loaded
    /// </summary>
    Task<T?> GetByIdWithIncludesAsync(
        int id, 
        params Expression<Func<T, object>>[] includes);
    
    /// <summary>
    /// Get all entities
    /// </summary>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Find entities matching a condition
    /// </summary>
    Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Find entities with related entities loaded
    /// </summary>
    Task<IEnumerable<T>> FindWithIncludesAsync(
        Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includes);
    
    /// <summary>
    /// Get first entity matching condition or null
    /// </summary>
    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Check if any entity matches condition
    /// </summary>
    Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Count entities matching condition
    /// </summary>
    Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default);
    
    // =============================================
    // PAGINATION
    // =============================================
    
    /// <summary>
    /// Get paginated results
    /// </summary>
    Task<PagedResult<T>> GetPagedAsync(
        int page,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, object>>? orderBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default);
    
    // =============================================
    // COMMAND METHODS
    // =============================================
    
    /// <summary>
    /// Add new entity
    /// </summary>
    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Add multiple entities
    /// </summary>
    Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update existing entity
    /// </summary>
    void Update(T entity);
    
    /// <summary>
    /// Update multiple entities
    /// </summary>
    void UpdateRange(IEnumerable<T> entities);
    
    /// <summary>
    /// Delete entity
    /// </summary>
    void Remove(T entity);
    
    /// <summary>
    /// Delete multiple entities
    /// </summary>
    void RemoveRange(IEnumerable<T> entities);
    
    /// <summary>
    /// Soft delete (set IsActive = false)
    /// </summary>
    void SoftDelete(T entity);
}
