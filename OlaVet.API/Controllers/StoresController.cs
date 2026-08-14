// =============================================
// File: OlaVet.API/Controllers/StoresController.cs
// Stores (Pharmacies) API Controller
// =============================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using OlaVet.API.Extensions;
using OlaVet.API.Security;
using OlaVet.Domain.Entities;
using OlaVet.Domain.Interfaces;

namespace OlaVet.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StoresController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StoresController> _logger;

    public StoresController(IUnitOfWork unitOfWork, ILogger<StoresController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get all stores with pagination
    /// </summary>
    [HttpGet]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        var result = await _unitOfWork.Stores.GetPagedAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Get store by ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var store = await _unitOfWork.Stores.GetByIdAsync(id);
        
        if (store == null)
            return NotFound(new { Message = $"Store with ID {id} not found" });
            
        return Ok(store);
    }

    /// <summary>
    /// Get store with inventory
    /// </summary>
    [HttpGet("{id}/inventory")]
    public async Task<IActionResult> GetWithInventory(int id)
    {
        var store = await _unitOfWork.Stores.GetWithInventoryAsync(id);
        
        if (store == null)
            return NotFound(new { Message = $"Store with ID {id} not found" });
            
        return Ok(new
        {
            store.StoreId,
            store.StoreName,
            store.StoreAddress,
            OpeningTime = store.OpeningTime?.ToString(@"hh\:mm"),
            ClosingTime = store.ClosingTime?.ToString(@"hh\:mm"),
            Inventory = (store.Inventories ?? Enumerable.Empty<OlaVet.Domain.Entities.Inventory>()).Select(i => new
            {
                i.InventoryId,
                i.MedicineId,
                MedicineName = i.Medicine?.MedicineName,
                Price = i.Medicine?.Price ?? 0,
                i.Quantity,
                i.LastRestocked
            })
        });
    }

    /// <summary>
    /// Get stores with ratings
    /// </summary>
    [HttpGet("with-ratings")]
    public async Task<IActionResult> GetWithRatings()
    {
        var stores = await _unitOfWork.Stores.GetStoresWithRatingsAsync();
        return Ok(stores);
    }

    /// <summary>
    /// Get top rated stores
    /// </summary>
    [HttpGet("top-rated")]
    public async Task<IActionResult> GetTopRated([FromQuery] int count = 10)
    {
        var stores = await _unitOfWork.Stores.GetTopRatedAsync(count);
        return Ok(stores);
    }

    /// <summary>
    /// Search stores
    /// </summary>
    [HttpGet("search")]
    public async Task<IActionResult> Search([FromQuery] string term)
    {
        if (string.IsNullOrWhiteSpace(term))
            return BadRequest(new { Message = "Search term is required" });
            
        var stores = await _unitOfWork.Stores.SearchAsync(term);
        return Ok(stores);
    }

    /// <summary>
    /// Get stores with specific medicine
    /// </summary>
    [HttpGet("with-medicine/{medicineId}")]
    public async Task<IActionResult> GetStoresWithMedicine(int medicineId)
    {
        var stores = await _unitOfWork.Stores.GetStoresWithMedicineAsync(medicineId);
        return Ok(stores);
    }

    /// <summary>
    /// Get currently open stores
    /// </summary>
    [HttpGet("open")]
    public async Task<IActionResult> GetOpenStores()
    {
        var stores = await _unitOfWork.Stores.GetOpenStoresAsync();
        return Ok(stores);
    }

    /// <summary>
    /// Get store reviews
    /// </summary>
    [HttpGet("{id}/reviews")]
    public async Task<IActionResult> GetReviews(int id)
    {
        var reviews = await _unitOfWork.Reviews.GetStoreReviewsAsync(id);
        return Ok(reviews.Select(r => new
        {
            r.StoreReviewId,
            r.Rating,
            r.Comments,
            r.ReviewDateTime,
            OwnerName = r.PetOwner?.OwnerName
        }));
    }
}

// =============================================
// MEDICINE ORDERS CONTROLLER
// =============================================

[ApiController]
[Route("api/orders")]
[Authorize]
public class MedicineOrdersController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<MedicineOrdersController> _logger;

    public MedicineOrdersController(IUnitOfWork unitOfWork, ILogger<MedicineOrdersController> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    /// <summary>
    /// Get all orders with pagination
    /// </summary>
    [HttpGet]
    [HasPermission("orders.read")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10)
    {
        // PetOwner sees only their own orders
        if (User.IsPetOwner())
        {
            var ownerId = User.GetPetOwnerId();
            if (ownerId == null) return Forbid();
            var myOrders = await _unitOfWork.MedicineOrders.GetByOwnerIdAsync(ownerId.Value);
            return Ok(myOrders);
        }

        // Admin, StoreManager see all
        var result = await _unitOfWork.MedicineOrders.GetPagedAsync(page, pageSize);
        return Ok(result);
    }

    /// <summary>
    /// Get order by ID
    /// </summary>
    [HttpGet("{id}")]
    [HasPermission("orders.read")]
    public async Task<IActionResult> GetById(int id)
    {
        var order = await _unitOfWork.MedicineOrders.GetWithDetailsAsync(id);
        
        if (order == null)
            return NotFound(new { Message = $"Order with ID {id} not found" });

        // PetOwner can only see their own orders
        if (User.IsPetOwner() && order.PetOwnerId != User.GetPetOwnerId())
            return Forbid();
            
        return Ok(new
        {
            order.MedicineOrderId,
            order.OrderDateTime,
            order.TotalAmount,
            order.DeliveryAddress,
            Status = order.StatusType?.StatusName,
            Store = order.Store == null ? null : new { order.Store.StoreId, order.Store.StoreName },
            Owner = order.PetOwner == null ? null : new { order.PetOwner.PetOwnerId, order.PetOwner.OwnerName },
            Items = (order.MedicineOrderDetails ?? Enumerable.Empty<OlaVet.Domain.Entities.MedicineOrderDetail>()).Select(d => new
            {
                d.OrderDetailId,
                d.MedicineId,
                MedicineName = d.Medicine?.MedicineName,
                d.Quantity,
                d.UnitPrice,
                d.Subtotal
            })
        });
    }

    /// <summary>
    /// Get orders by owner
    /// </summary>
    [HttpGet("owner/{ownerId}")]
    [HasPermission("orders.read")]
    public async Task<IActionResult> GetByOwner(int ownerId)
    {
        // PetOwner can only see their own orders
        if (User.IsPetOwner() && User.GetPetOwnerId() != ownerId)
            return Forbid();

        var orders = await _unitOfWork.MedicineOrders.GetByOwnerIdAsync(ownerId);
        return Ok(orders);
    }

    /// <summary>
    /// Get pending orders
    /// </summary>
    [HttpGet("pending")]
    [HasPermission("orders.read", "stores.read")]
    public async Task<IActionResult> GetPending()
    {
        var orders = await _unitOfWork.MedicineOrders.GetPendingOrdersAsync();
        return Ok(orders);
    }

    /// <summary>
    /// Create new order
    /// </summary>
    [HttpPost]
    [HasPermission("orders.create")]
    public async Task<IActionResult> Create([FromBody] CreateMedicineOrderRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // PetOwner can only create orders for themselves
        if (User.IsPetOwner())
        {
            var ownerId = User.GetPetOwnerId();
            if (ownerId == null || request.PetOwnerId != ownerId)
                return Forbid();
        }
            
        var order = new MedicineOrder
        {
            PetOwnerId = request.PetOwnerId,
            StoreId = request.StoreId,
            StatusTypeId = 7, // Pending
            DeliveryAddress = request.DeliveryAddress,
            TotalAmount = 0 // Will be calculated
        };
        
        await _unitOfWork.MedicineOrders.AddAsync(order);
        await _unitOfWork.SaveChangesAsync();
        
        _logger.LogInformation("Created order: {OrderId}", order.MedicineOrderId);
        
        return CreatedAtAction(nameof(GetById), new { id = order.MedicineOrderId }, order);
    }
}

// =============================================
// REQUEST MODELS
// =============================================

public class CreateMedicineOrderRequest
{
    public int PetOwnerId { get; set; }
    public int StoreId { get; set; }
    public string? DeliveryAddress { get; set; }
    public List<OrderItemRequest>? Items { get; set; }
}

public class OrderItemRequest
{
    public int MedicineId { get; set; }
    public int Quantity { get; set; }
}
