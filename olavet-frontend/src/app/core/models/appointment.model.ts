// =============================================
// Appointment models — maps to OlaVet.Application.DTOs.Appointment
// =============================================

export interface VetAppointment {
  vetAppointmentId: number;
  petId: number;
  petName: string;
  vetId: number;
  vetName: string;
  appointmentDateTime: string;
  appointmentType: string;
  status: string;
  reason?: string;
  notes?: string;
  fee: number;
  createdDate: string;
  pet?: { petId: number; name: string; species: string };
  owner?: { petOwnerId: number; ownerName: string };
  vet?: { vetId: number; vetName: string; fee: number };
}

export interface LabAppointment {
  labAppointmentId: number;
  petId: number;
  petName: string;
  labId: number;
  labName: string;
  appointmentDateTime: string;
  status: string;
  totalAmount: number;
  notes?: string;
  tests: LabTest[];
  pet?: { petId: number; name: string };
  owner?: { petOwnerId: number; ownerName: string };
  lab?: { labId: number; labName: string };
}

export interface LabTest {
  labTestId: number;
  testName: string;
  testPrice: number;
  result?: string;
}

export interface CreateVetAppointmentRequest {
  petId: number;
  petOwnerId: number;
  vetId: number;
  appointmentTypeId: number;
  appointmentDateTime: string;
  reason?: string;
  notes?: string;
}

export interface CreateLabAppointmentRequest {
  petId: number;
  petOwnerId: number;
  labId: number;
  appointmentDateTime: string;
  notes?: string;
  testIds?: number[];
}

export interface UpdateAppointmentStatusRequest {
  statusId: number;
  notes?: string;
}

export interface TimeSlot {
  startTime: string;
  endTime: string;
  isAvailable: boolean;
}

export interface SearchAppointmentsParams {
  petId?: number;
  vetId?: number;
  labId?: number;
  ownerId?: number;
  fromDate?: string;
  toDate?: string;
  statusId?: number;
  page?: number;
  pageSize?: number;
}
