// =============================================
// File: OlaVet.Application/Services/Interfaces/IOrderService.cs
// Service interface for Medicine Order business logic
// =============================================

using OlaVet.Application.Common;
using OlaVet.Application.DTOs.Order;
using OlaVet.Domain.Common;

namespace OlaVet.Application.Services.Interfaces;

public interface IOrderService
{
    // Orders
    Task<Result<MedicineOrderDto>> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<MedicineOrderDto>>> GetOrdersAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<MedicineOrderDto>>> GetOrdersByOwnerAsync(int ownerId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<MedicineOrderDto>>> GetOrdersByStoreAsync(int storeId, CancellationToken cancellationToken = default);
    Task<Result<MedicineOrderDto>> CreateOrderAsync(CreateOrderDto dto, CancellationToken cancellationToken = default);
    Task<Result<MedicineOrderDto>> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto dto, CancellationToken cancellationToken = default);
    Task<Result> CancelOrderAsync(int id, CancellationToken cancellationToken = default);
    
    // Stores
    Task<Result<StoreDto>> GetStoreByIdAsync(int id, CancellationToken cancellationToken = default);
    Task<Result<PagedResult<StoreDto>>> GetStoresAsync(int page, int pageSize, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<StoreDto>>> GetTopRatedStoresAsync(int count, CancellationToken cancellationToken = default);
    
    // Medicines
    Task<Result<IEnumerable<MedicineDto>>> GetMedicinesByStoreAsync(int storeId, CancellationToken cancellationToken = default);
    Task<Result<IEnumerable<MedicineDto>>> SearchMedicinesAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<Result<bool>> CheckStockAvailabilityAsync(int storeId, int medicineId, int quantity, CancellationToken cancellationToken = default);
}
