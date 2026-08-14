// =============================================
// Lab models — maps to backend Lab entities
// =============================================

export interface Lab {
  labId: number;
  labName: string;
  labAddress: string;
  specialization?: string;
  contactNumber?: string;
  email?: string;
  isActive: boolean;
}

export interface LabWithRating extends Lab {
  averageRating: number;
  reviewCount: number;
}

export interface LabWithAppointments extends Lab {
  totalAppointments: number;
  recentAppointments: {
    labAppointmentId: number;
    appointmentDateTime: string;
    notes?: string;
  }[];
}
