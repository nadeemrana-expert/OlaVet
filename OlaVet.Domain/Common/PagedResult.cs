// =============================================
// File: OlaVet.Domain/Common/PagedResult.cs
// Pagination result wrapper for API responses
// =============================================

namespace OlaVet.Domain.Common;

/// <summary>
/// Represents a paginated result set
/// Used for returning paged data from repository queries
/// </summary>
/// <typeparam name="T">The type of items in the result</typeparam>
public class PagedResult<T>
{
    /// <summary>
    /// Default constructor for serialization
    /// </summary>
    public PagedResult() { }
    
    /// <summary>
    /// Constructor with parameters
    /// </summary>
    public PagedResult(IEnumerable<T> items, int totalCount, int page, int pageSize)
    {
        Items = items;
        TotalCount = totalCount;
        Page = page;
        PageSize = pageSize;
    }
    
    /// <summary>
    /// The items in the current page
    /// </summary>
    public IEnumerable<T> Items { get; set; } = Enumerable.Empty<T>();
    
    /// <summary>
    /// Total count of all items (across all pages)
    /// </summary>
    public int TotalCount { get; set; }
    
    /// <summary>
    /// Current page number (1-based)
    /// </summary>
    public int Page { get; set; }
    
    /// <summary>
    /// Number of items per page
    /// </summary>
    public int PageSize { get; set; }
    
    /// <summary>
    /// Total number of pages
    /// </summary>
    public int TotalPages => PageSize > 0 ? (int)Math.Ceiling(TotalCount / (double)PageSize) : 0;
    
    /// <summary>
    /// Whether there is a previous page
    /// </summary>
    public bool HasPrevious => Page > 1;
    
    /// <summary>
    /// Whether there is a next page
    /// </summary>
    public bool HasNext => Page < TotalPages;
    
    /// <summary>
    /// First item index (1-based)
    /// </summary>
    public int FirstItemIndex => TotalCount > 0 ? (Page - 1) * PageSize + 1 : 0;
    
    /// <summary>
    /// Last item index in current page
    /// </summary>
    public int LastItemIndex => Math.Min(Page * PageSize, TotalCount);
}
