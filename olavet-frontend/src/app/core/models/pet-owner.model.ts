// =============================================
// Pet Owner models — maps to OlaVet.Application.DTOs.PetOwner
// =============================================

export interface PetOwner {
  petOwnerId: number;
  ownerName: string;
  email: string;
  contactNumber: string;
  homeAddress?: string;
  age?: number;
  gender?: string;
  wallet: number;
  isActive: boolean;
  createdDate: string;
}

export interface PetOwnerDetails extends PetOwner {
  pets: PetSummary[];
  totalAppointments: number;
  totalSpent: number;
}

export interface PetSummary {
  petId: number;
  name: string;
  species: string;
  breed?: string;
  age?: number;
}

export interface CreatePetOwnerRequest {
  ownerName: string;
  email: string;
  contactNumber: string;
  homeAddress?: string;
  age?: number;
  gender?: string;
  initialWalletBalance: number;
}

export interface UpdatePetOwnerRequest {
  ownerName?: string;
  email?: string;
  contactNumber?: string;
  homeAddress?: string;
  age?: number;
  gender?: string;
}

export interface AddFundsRequest {
  amount: number;
  paymentMethod?: string;
  transactionReference?: string;
}

export interface OwnerPaymentSummary {
  totalVetPayments: number;
  totalLabPayments: number;
  totalStorePayments: number;
  grandTotal: number;
  totalTransactions: number;
}
