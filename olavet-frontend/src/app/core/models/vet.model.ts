// =============================================
// Vet models — maps to OlaVet.Application.DTOs.Vet
// =============================================

export interface Vet {
  vetId: number;
  vetName: string;
  specialization: string;
  clinicLocation: string;
  fee: number;
  contactNumber: string;
  email?: string;
  yearsOfExperience?: number;
  licenseNumber?: string;
  isActive: boolean;
}

export interface VetWithRating extends Vet {
  averageRating: number;
  reviewCount: number;
}

export interface VetDetails extends VetWithRating {
  qualifications: Qualification[];
  services: VetService[];
  availability: Availability[];
}

export interface Qualification {
  educationQualificationId: number;
  degreeName: string;
  instituteName: string;
  yearObtained: number;
}

export interface VetService {
  serviceId: number;
  serviceName: string;
  description?: string;
  serviceFee?: number;
}

export interface Availability {
  vetAvailabilityId: number;
  dayOfWeek: string;
  startTime: string;
  endTime: string;
  isAvailable: boolean;
}

export interface CreateVetRequest {
  vetName: string;
  specialization: string;
  clinicLocation: string;
  fee: number;
  contactNumber: string;
  email?: string;
  yearsOfExperience?: number;
  licenseNumber?: string;
}

export interface UpdateVetRequest {
  vetName?: string;
  specialization?: string;
  clinicLocation?: string;
  fee?: number;
  contactNumber?: string;
  email?: string;
}

export interface SearchVetsParams {
  searchTerm?: string;
  specialization?: string;
  maxFee?: number;
  minRating?: number;
  isAvailableNow?: boolean;
  page?: number;
  pageSize?: number;
}
