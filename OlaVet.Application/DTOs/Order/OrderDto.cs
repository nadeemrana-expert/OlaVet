// =============================================
// File: OlaVet.Application/DTOs/Order/OrderDto.cs
// DTOs for Medicine Order operations
// =============================================

namespace OlaVet.Application.DTOs.Order;

/// <summary>
/// Medicine order response DTO
/// </summary>
public record MedicineOrderDto
{
    public int MedicineOrderId { get; init; }
    public int PetOwnerId { get; init; }
    public string OwnerName { get; init; } = string.Empty;
    public int StoreId { get; init; }
    public string StoreName { get; init; } = string.Empty;
    public DateTime OrderDate { get; init; }
    public string Status { get; init; } = string.Empty;
    public decimal TotalAmount { get; init; }
    public string? DeliveryAddress { get; init; }
    public List<OrderItemDto> Items { get; init; } = new();
}

/// <summary>
/// Order line item
/// </summary>
public record OrderItemDto
{
    public int OrderDetailId { get; init; }
    public int MedicineId { get; init; }
    public string MedicineName { get; init; } = string.Empty;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Subtotal { get; init; }
}

/// <summary>
/// Request for creating an order
/// </summary>
public record CreateOrderDto
{
    public int PetOwnerId { get; init; }
    public int StoreId { get; init; }
    public List<CreateOrderItemDto> Items { get; init; } = new();
    public string? DeliveryAddress { get; init; }
    public bool UseWallet { get; init; } = true;
}

/// <summary>
/// Item to add to order
/// </summary>
public record CreateOrderItemDto
{
    public int MedicineId { get; init; }
    public int Quantity { get; init; }
}

/// <summary>
/// Request to update order status
/// </summary>
public record UpdateOrderStatusDto
{
    public int StatusId { get; init; }
    public string? Notes { get; init; }
}

/// <summary>
/// Medicine info for browsing
/// </summary>
public record MedicineDto
{
    public int MedicineId { get; init; }
    public string MedicineName { get; init; } = string.Empty;
    public string? Description { get; init; }
    public string MedicineType { get; init; } = string.Empty;
    public decimal UnitPrice { get; init; }
    public int StockQuantity { get; init; }
    public bool IsAvailable { get; init; }
}

/// <summary>
/// Store info
/// </summary>
public record StoreDto
{
    public int StoreId { get; init; }
    public string StoreName { get; init; } = string.Empty;
    public string Location { get; init; } = string.Empty;
    public string? ContactNumber { get; init; }
    public bool IsActive { get; init; }
    public double? AverageRating { get; init; }
    public int ReviewCount { get; init; }
}
