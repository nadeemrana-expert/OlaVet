// =============================================
// File: OlaVet.Application/Exceptions/ApplicationException.cs
// Custom exceptions for the application layer
// =============================================

namespace OlaVet.Application.Exceptions;

/// <summary>
/// Base exception for application layer
/// </summary>
public class AppException : Exception
{
    public AppException(string message) : base(message) { }
    public AppException(string message, Exception inner) : base(message, inner) { }
}

/// <summary>
/// Thrown when a requested entity is not found
/// </summary>
public class NotFoundException : AppException
{
    public string EntityName { get; }
    public object EntityId { get; }
    
    public NotFoundException(string entityName, object id) 
        : base($"{entityName} with ID {id} was not found")
    {
        EntityName = entityName;
        EntityId = id;
    }
}

/// <summary>
/// Thrown when validation fails
/// </summary>
public class ValidationException : AppException
{
    public Dictionary<string, string[]> Errors { get; }
    
    public ValidationException(Dictionary<string, string[]> errors) 
        : base("One or more validation errors occurred")
    {
        Errors = errors;
    }
    
    public ValidationException(string propertyName, string error)
        : base($"Validation failed for {propertyName}: {error}")
    {
        Errors = new Dictionary<string, string[]>
        {
            { propertyName, new[] { error } }
        };
    }
}

/// <summary>
/// Thrown when business rule validation fails
/// </summary>
public class BusinessRuleException : AppException
{
    public string RuleName { get; }
    
    public BusinessRuleException(string ruleName, string message) 
        : base(message)
    {
        RuleName = ruleName;
    }
}

/// <summary>
/// Thrown when user has insufficient funds
/// </summary>
public class InsufficientFundsException : BusinessRuleException
{
    public decimal Required { get; }
    public decimal Available { get; }
    
    public InsufficientFundsException(decimal required, decimal available)
        : base("InsufficientFunds", $"Insufficient funds. Required: {required:C}, Available: {available:C}")
    {
        Required = required;
        Available = available;
    }
}

/// <summary>
/// Thrown when a conflicting operation is attempted
/// </summary>
public class ConflictException : AppException
{
    public ConflictException(string message) : base(message) { }
}

/// <summary>
/// Thrown when user is not authorized for an operation
/// </summary>
public class UnauthorizedException : AppException
{
    public UnauthorizedException(string message = "You are not authorized to perform this action") 
        : base(message) { }
}
