// =============================================
// File: OlaVet.Application/Common/Result.cs
// Generic result wrapper for service operations
// =============================================

namespace OlaVet.Application.Common;

/// <summary>
/// Result wrapper for operation outcomes
/// Provides a clean way to return success/failure with data or errors
/// </summary>
public class Result<T>
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public T? Data { get; private set; }
    public string? Error { get; private set; }
    public List<string> Errors { get; private set; } = new();
    
    private Result() { }
    
    /// <summary>
    /// Create a successful result with data
    /// </summary>
    public static Result<T> Success(T data)
    {
        return new Result<T>
        {
            IsSuccess = true,
            Data = data
        };
    }
    
    /// <summary>
    /// Create a failure result with error message
    /// </summary>
    public static Result<T> Failure(string error)
    {
        return new Result<T>
        {
            IsSuccess = false,
            Error = error,
            Errors = new List<string> { error }
        };
    }
    
    /// <summary>
    /// Create a failure result with multiple errors
    /// </summary>
    public static Result<T> Failure(IEnumerable<string> errors)
    {
        var errorList = errors.ToList();
        return new Result<T>
        {
            IsSuccess = false,
            Error = errorList.FirstOrDefault(),
            Errors = errorList
        };
    }
}

/// <summary>
/// Non-generic result for operations that don't return data
/// </summary>
public class Result
{
    public bool IsSuccess { get; private set; }
    public bool IsFailure => !IsSuccess;
    public string? Error { get; private set; }
    public List<string> Errors { get; private set; } = new();
    
    private Result() { }
    
    public static Result Success()
    {
        return new Result { IsSuccess = true };
    }
    
    public static Result Failure(string error)
    {
        return new Result
        {
            IsSuccess = false,
            Error = error,
            Errors = new List<string> { error }
        };
    }
    
    public static Result Failure(IEnumerable<string> errors)
    {
        var errorList = errors.ToList();
        return new Result
        {
            IsSuccess = false,
            Error = errorList.FirstOrDefault(),
            Errors = errorList
        };
    }
}
