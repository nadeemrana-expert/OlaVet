// =============================================
// File: OlaVet.Application/Services/Implementations/OrderService.cs
// Service implementation for Order business logic
// =============================================

using AutoMapper;
using FluentValidation;
using OlaVet.Application.Common;
using OlaVet.Application.DTOs.Order;
using OlaVet.Application.Exceptions;
using OlaVet.Application.Services.Interfaces;
using OlaVet.Domain.Common;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Interfaces;

namespace OlaVet.Application.Services.Implementations;

public class OrderService : IOrderService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IValidator<CreateOrderDto> _createOrderValidator;
    
    private const int StatusPending = 1;
    private const int StatusProcessing = 2;
    private const int StatusShipped = 3;
    private const int StatusDelivered = 4;
    private const int StatusCancelled = 5;
    
    public OrderService(
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IValidator<CreateOrderDto> createOrderValidator)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createOrderValidator = createOrderValidator;
    }
    
    // =============================================
    // ORDERS
    // =============================================
    
    public async Task<Result<MedicineOrderDto>> GetOrderByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.MedicineOrders.GetWithDetailsAsync(id);
        
        if (order == null)
            return Result<MedicineOrderDto>.Failure($"Order with ID {id} not found");
        
        return Result<MedicineOrderDto>.Success(_mapper.Map<MedicineOrderDto>(order));
    }
    
    public async Task<Result<PagedResult<MedicineOrderDto>>> GetOrdersAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.MedicineOrders.GetPagedAsync(page, pageSize, null, null, true, cancellationToken);
        
        var dtos = new PagedResult<MedicineOrderDto>(
            _mapper.Map<IEnumerable<MedicineOrderDto>>(result.Items),
            result.TotalCount,
            result.Page,
            result.PageSize
        );
        
        return Result<PagedResult<MedicineOrderDto>>.Success(dtos);
    }
    
    public async Task<Result<IEnumerable<MedicineOrderDto>>> GetOrdersByOwnerAsync(int ownerId, CancellationToken cancellationToken = default)
    {
        var orders = await _unitOfWork.MedicineOrders.GetByOwnerIdAsync(ownerId);
        return Result<IEnumerable<MedicineOrderDto>>.Success(_mapper.Map<IEnumerable<MedicineOrderDto>>(orders));
    }
    
    public async Task<Result<IEnumerable<MedicineOrderDto>>> GetOrdersByStoreAsync(int storeId, CancellationToken cancellationToken = default)
    {
        // Note: IMedicineOrderRepository doesn't have GetByStoreIdAsync
        // We'll get all pending orders and filter by store
        var orders = await _unitOfWork.MedicineOrders.GetPendingOrdersAsync();
        var storeOrders = orders.Where(o => o.StoreId == storeId);
        return Result<IEnumerable<MedicineOrderDto>>.Success(_mapper.Map<IEnumerable<MedicineOrderDto>>(storeOrders));
    }
    
    public async Task<Result<MedicineOrderDto>> CreateOrderAsync(CreateOrderDto dto, CancellationToken cancellationToken = default)
    {
        // Validate
        var validationResult = await _createOrderValidator.ValidateAsync(dto, cancellationToken);
        if (!validationResult.IsValid)
            return Result<MedicineOrderDto>.Failure(validationResult.Errors.Select(e => e.ErrorMessage));
        
        // Verify owner exists
        var owner = await _unitOfWork.PetOwners.GetByIdAsync(dto.PetOwnerId, cancellationToken);
        if (owner == null)
            return Result<MedicineOrderDto>.Failure($"Pet owner with ID {dto.PetOwnerId} not found");
        
        // Verify store exists
        var store = await _unitOfWork.Stores.GetByIdAsync(dto.StoreId, cancellationToken);
        if (store == null)
            return Result<MedicineOrderDto>.Failure($"Store with ID {dto.StoreId} not found");
        
        // Calculate total and verify medicines exist
        decimal totalAmount = 0;
        var orderDetails = new List<MedicineOrderDetail>();
        
        foreach (var item in dto.Items)
        {
            var medicine = await _unitOfWork.Medicines.GetByIdAsync(item.MedicineId, cancellationToken);
            if (medicine == null)
                return Result<MedicineOrderDto>.Failure($"Medicine with ID {item.MedicineId} not found");
            
            var subtotal = medicine.Price * item.Quantity;
            totalAmount += subtotal;
            
            orderDetails.Add(new MedicineOrderDetail
            {
                MedicineId = item.MedicineId,
                Quantity = item.Quantity,
                UnitPrice = medicine.Price
            });
        }
        
        // Check funds if using wallet
        if (dto.UseWallet && owner.Wallet < totalAmount)
            throw new InsufficientFundsException(totalAmount, owner.Wallet);
        
        // Create order
        var order = new MedicineOrder
        {
            PetOwnerId = dto.PetOwnerId,
            StoreId = dto.StoreId,
            OrderDateTime = DateTime.UtcNow,
            StatusTypeId = StatusPending,
            TotalAmount = totalAmount,
            DeliveryAddress = dto.DeliveryAddress ?? owner.HomeAddress,
            MedicineOrderDetails = orderDetails
        };
        
        await _unitOfWork.MedicineOrders.AddAsync(order, cancellationToken);
        
        // Deduct from wallet if using wallet
        if (dto.UseWallet)
        {
            owner.Wallet -= totalAmount;
            _unitOfWork.PetOwners.Update(owner);
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result<MedicineOrderDto>.Success(_mapper.Map<MedicineOrderDto>(order));
    }
    
    public async Task<Result<MedicineOrderDto>> UpdateOrderStatusAsync(int id, UpdateOrderStatusDto dto, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.MedicineOrders.GetByIdAsync(id, cancellationToken);
        if (order == null)
            return Result<MedicineOrderDto>.Failure($"Order with ID {id} not found");
        
        order.StatusTypeId = dto.StatusId;
        
        _unitOfWork.MedicineOrders.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        var updated = await _unitOfWork.MedicineOrders.GetWithDetailsAsync(id);
        
        return Result<MedicineOrderDto>.Success(_mapper.Map<MedicineOrderDto>(updated));
    }
    
    public async Task<Result> CancelOrderAsync(int id, CancellationToken cancellationToken = default)
    {
        var order = await _unitOfWork.MedicineOrders.GetByIdAsync(id, cancellationToken);
        if (order == null)
            return Result.Failure($"Order with ID {id} not found");
        
        if (order.StatusTypeId == StatusDelivered)
            return Result.Failure("Cannot cancel a delivered order");
        
        if (order.StatusTypeId == StatusCancelled)
            return Result.Failure("Order is already cancelled");
        
        // Refund to wallet
        var owner = await _unitOfWork.PetOwners.GetByIdAsync(order.PetOwnerId, cancellationToken);
        if (owner != null)
        {
            owner.Wallet += order.TotalAmount;
            _unitOfWork.PetOwners.Update(owner);
        }
        
        order.StatusTypeId = StatusCancelled;
        
        _unitOfWork.MedicineOrders.Update(order);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        
        return Result.Success();
    }
    
    // =============================================
    // STORES
    // =============================================
    
    public async Task<Result<StoreDto>> GetStoreByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        var store = await _unitOfWork.Stores.GetByIdAsync(id, cancellationToken);
        
        if (store == null)
            return Result<StoreDto>.Failure($"Store with ID {id} not found");
        
        return Result<StoreDto>.Success(_mapper.Map<StoreDto>(store));
    }
    
    public async Task<Result<PagedResult<StoreDto>>> GetStoresAsync(int page, int pageSize, CancellationToken cancellationToken = default)
    {
        var result = await _unitOfWork.Stores.GetPagedAsync(page, pageSize, null, null, true, cancellationToken);
        
        var dtos = new PagedResult<StoreDto>(
            _mapper.Map<IEnumerable<StoreDto>>(result.Items),
            result.TotalCount,
            result.Page,
            result.PageSize
        );
        
        return Result<PagedResult<StoreDto>>.Success(dtos);
    }
    
    public async Task<Result<IEnumerable<StoreDto>>> GetTopRatedStoresAsync(int count, CancellationToken cancellationToken = default)
    {
        var stores = await _unitOfWork.Stores.GetTopRatedAsync(count);
        return Result<IEnumerable<StoreDto>>.Success(_mapper.Map<IEnumerable<StoreDto>>(stores));
    }
    
    // =============================================
    // MEDICINES
    // =============================================
    
    public async Task<Result<IEnumerable<MedicineDto>>> GetMedicinesByStoreAsync(int storeId, CancellationToken cancellationToken = default)
    {
        var medicines = await _unitOfWork.Medicines.GetInStockAtStoreAsync(storeId);
        return Result<IEnumerable<MedicineDto>>.Success(_mapper.Map<IEnumerable<MedicineDto>>(medicines));
    }
    
    public async Task<Result<IEnumerable<MedicineDto>>> SearchMedicinesAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(searchTerm))
            return Result<IEnumerable<MedicineDto>>.Failure("Search term is required");
        
        var medicines = await _unitOfWork.Medicines.SearchAsync(searchTerm);
        return Result<IEnumerable<MedicineDto>>.Success(_mapper.Map<IEnumerable<MedicineDto>>(medicines));
    }
    
    public async Task<Result<bool>> CheckStockAvailabilityAsync(int storeId, int medicineId, int quantity, CancellationToken cancellationToken = default)
    {
        // IMedicineRepository doesn't have CheckStockAsync
        // We'll check if the medicine is in stock at the store
        var medicines = await _unitOfWork.Medicines.GetInStockAtStoreAsync(storeId);
        var medicine = medicines.FirstOrDefault(m => m.MedicineId == medicineId);
        
        return Result<bool>.Success(medicine != null);
    }
}
