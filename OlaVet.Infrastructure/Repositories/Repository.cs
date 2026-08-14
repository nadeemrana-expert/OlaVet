// =============================================
// File: OlaVet.Infrastructure/Repositories/Repository.cs
// Generic repository implementation - the workhorse!
// =============================================

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OlaVet.Domain.Common;
using OlaVet.Domain.Interfaces;
using OlaVet.Infrastructure.Data;

namespace OlaVet.Infrastructure.Repositories;

/// <summary>
/// Generic repository implementation using EF Core
/// Provides common CRUD operations for any entity
/// </summary>
public class Repository<T> : IRepository<T> where T : class
{
    protected readonly OlaVetDbContext _context;
    protected readonly DbSet<T> _dbSet;
    
    public Repository(OlaVetDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }
    
    // =============================================
    // QUERY METHODS
    // =============================================
    
    public virtual async Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _dbSet.FindAsync(new object[] { id }, cancellationToken);
    }
    
    public virtual async Task<T?> GetByIdWithIncludesAsync(
        int id, 
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        
        // Apply all includes
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        // Find by ID
        // Note: FindAsync doesn't work with Include, so we use FirstOrDefaultAsync
        var keyProperty = _context.Model.FindEntityType(typeof(T))
            ?.FindPrimaryKey()?.Properties[0].Name;
        
        if (keyProperty == null)
            throw new InvalidOperationException($"No primary key found for {typeof(T).Name}");
        
        var parameter = Expression.Parameter(typeof(T));
        var property = Expression.Property(parameter, keyProperty);
        var constant = Expression.Constant(id);
        var equals = Expression.Equal(property, constant);
        var lambda = Expression.Lambda<Func<T, bool>>(equals, parameter);
        
        return await query.FirstOrDefaultAsync(lambda);
    }
    
    public virtual async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }
    
    public virtual async Task<IEnumerable<T>> FindAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.Where(predicate).ToListAsync(cancellationToken);
    }
    
    public virtual async Task<IEnumerable<T>> FindWithIncludesAsync(
        Expression<Func<T, bool>> predicate,
        params Expression<Func<T, object>>[] includes)
    {
        IQueryable<T> query = _dbSet;
        
        // Apply includes
        foreach (var include in includes)
        {
            query = query.Include(include);
        }
        
        return await query.Where(predicate).ToListAsync();
    }
    
    public virtual async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.FirstOrDefaultAsync(predicate, cancellationToken);
    }
    
    public virtual async Task<bool> AnyAsync(
        Expression<Func<T, bool>> predicate,
        CancellationToken cancellationToken = default)
    {
        return await _dbSet.AnyAsync(predicate, cancellationToken);
    }
    
    public virtual async Task<int> CountAsync(
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default)
    {
        if (predicate == null)
            return await _dbSet.CountAsync(cancellationToken);
        
        return await _dbSet.CountAsync(predicate, cancellationToken);
    }
    
    // =============================================
    // PAGINATION
    // =============================================
    
    public virtual async Task<PagedResult<T>> GetPagedAsync(
        int page,
        int pageSize,
        Expression<Func<T, bool>>? predicate = null,
        Expression<Func<T, object>>? orderBy = null,
        bool ascending = true,
        CancellationToken cancellationToken = default)
    {
        // Validation
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100; // Max page size
        
        IQueryable<T> query = _dbSet;
        
        // Apply filter
        if (predicate != null)
        {
            query = query.Where(predicate);
        }
        
        // Get total count
        var totalCount = await query.CountAsync(cancellationToken);
        
        // Apply ordering (default to primary key if none specified)
        if (orderBy != null)
        {
            query = ascending 
                ? query.OrderBy(orderBy) 
                : query.OrderByDescending(orderBy);
        }
        else
        {
            // Default ordering by primary key to prevent EF Core warnings
            var keyProperty = _context.Model.FindEntityType(typeof(T))
                ?.FindPrimaryKey()?.Properties[0].Name;
            
            if (keyProperty != null)
            {
                var parameter = Expression.Parameter(typeof(T));
                var property = Expression.Property(parameter, keyProperty);
                var converted = Expression.Convert(property, typeof(object));
                var defaultOrder = Expression.Lambda<Func<T, object>>(converted, parameter);
                query = ascending ? query.OrderBy(defaultOrder) : query.OrderByDescending(defaultOrder);
            }
        }
        
        // Apply pagination
        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
        
        return new PagedResult<T>
        {
            Items = items,
            TotalCount = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }
    
    // =============================================
    // COMMAND METHODS
    // =============================================
    
    public virtual async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddAsync(entity, cancellationToken);
        return entity;
    }
    
    public virtual async Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default)
    {
        await _dbSet.AddRangeAsync(entities, cancellationToken);
    }
    
    public virtual void Update(T entity)
    {
        _dbSet.Update(entity);
    }
    
    public virtual void UpdateRange(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
    }
    
    public virtual void Remove(T entity)
    {
        _dbSet.Remove(entity);
    }
    
    public virtual void RemoveRange(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
    }
    
    public virtual void SoftDelete(T entity)
    {
        // Check if entity has IsActive property
        var isActiveProperty = typeof(T).GetProperty("IsActive");
        
        if (isActiveProperty != null && isActiveProperty.PropertyType == typeof(bool))
        {
            isActiveProperty.SetValue(entity, false);
            Update(entity);
        }
        else
        {
            throw new InvalidOperationException(
                $"Entity {typeof(T).Name} does not have an IsActive property");
        }
    }
}
