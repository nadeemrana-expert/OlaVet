// =============================================
// Pet models — maps to OlaVet.Application.DTOs.Pet
// =============================================

export interface Pet {
  petId: number;
  petOwnerId: number;
  name: string;
  species: string;
  breed?: string;
  age?: number;
  petWeight?: number;
  color?: string;
  gender?: string;
  dateOfBirth?: string;
  isActive: boolean;
  createdDate: string;
}

export interface PetWithOwner extends Pet {
  ownerName: string;
  ownerContactNumber: string;
}

export interface PetDetails extends PetWithOwner {
  medicalHistory: MedicalRecord[];
  recentAppointments: AppointmentSummary[];
  totalAppointments: number;
}

export interface MedicalRecord {
  medicalRecordId: number;
  recordDate: string;
  recordType: string;
  diagnosis?: string;
  treatment?: string;
  notes?: string;
  vetName?: string;
}

export interface AppointmentSummary {
  appointmentId: number;
  appointmentDate: string;
  appointmentType: string;
  status: string;
  vetName?: string;
  fee?: number;
}

export interface CreatePetRequest {
  petOwnerId: number;
  name: string;
  species: string;
  breed?: string;
  age?: number;
  petWeight?: number;
  color?: string;
  gender?: string;
  dateOfBirth?: string;
}

export interface UpdatePetRequest {
  name: string;
  species: string;
  breed?: string;
  age?: number;
  weight?: number;
  color?: string;
  gender?: string;
}

export interface SearchPetsParams {
  searchTerm?: string;
  species?: string;
  breed?: string;
  ownerId?: number;
  page?: number;
  pageSize?: number;
}
