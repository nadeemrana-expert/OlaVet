// =============================================
// Order / Store / Medicine models — maps to OlaVet.Application.DTOs.Order
// =============================================

export interface MedicineOrder {
  medicineOrderId: number;
  petOwnerId: number;
  ownerName: string;
  storeId: number;
  storeName: string;
  orderDate: string;
  status: string;
  totalAmount: number;
  deliveryAddress?: string;
  items: OrderItem[];
}

export interface OrderItem {
  orderDetailId: number;
  medicineId: number;
  medicineName: string;
  quantity: number;
  unitPrice: number;
  subtotal: number;
}

export interface CreateOrderRequest {
  petOwnerId: number;
  storeId: number;
  items: CreateOrderItem[];
  deliveryAddress?: string;
  useWallet: boolean;
}

export interface CreateOrderItem {
  medicineId: number;
  quantity: number;
}

export interface UpdateOrderStatusRequest {
  statusId: number;
  notes?: string;
}

export interface Medicine {
  medicineId: number;
  medicineName: string;
  description?: string;
  medicineType: string;
  unitPrice: number;
  stockQuantity: number;
  isAvailable: boolean;
}

export interface Store {
  storeId: number;
  storeName: string;
  storeAddress: string;
  contactNumber?: string;
  openingTime?: string;
  closingTime?: string;
  isActive: boolean;
  averageRating?: number;
  reviewCount?: number;
}

export interface StoreWithInventory extends Store {
  inventory: InventoryItem[];
}

export interface InventoryItem {
  inventoryId: number;
  medicineId: number;
  medicineName: string;
  price: number;
  quantity: number;
  lastRestocked?: string;
}
