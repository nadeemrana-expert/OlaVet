// =============================================
// Dashboard models — maps to DashboardController responses
// =============================================

export interface DashboardStats {
  role: string;
  // Admin fields
  petOwners?: number;
  vets?: number;
  pets?: number;
  labs?: number;
  stores?: number;
  vetAppointments?: number;
  labAppointments?: number;
  medicineOrders?: number;
  medicines?: number;
  // PetOwner fields
  upcomingAppointments?: number;
  walletBalance?: number;
  // Vet fields
  todayAppointments?: number;
  weekAppointments?: number;
  totalPatients?: number;
  // LabTechnician fields
  pendingTests?: number;
  completedTests?: number;
  // StoreManager fields
  pendingOrders?: number;
}

export interface AppointmentStats {
  vetAppointments: {
    today: number;
    thisWeek: number;
    thisMonth: number;
    completed: number;
    scheduled: number;
    cancelled: number;
  };
  labAppointments: {
    today: number;
    thisWeek: number;
    thisMonth: number;
    completed: number;
    scheduled: number;
  };
}

export interface TopPerformers {
  topVets: { vetId: number; vetName: string; specialization: string }[];
  topLabs: { labId: number; labName: string; specialization: string }[];
  topStores: { storeId: number; storeName: string; storeAddress: string }[];
}

export interface SpeciesDistribution {
  species: string;
  count: number;
}

export interface RevenueTrend {
  vetRevenue: { date: string; amount: number }[];
  labRevenue: { date: string; amount: number }[];
  storeRevenue: { date: string; amount: number }[];
}

export interface PaymentStats {
  period: { startDate: string; endDate: string };
  statistics: Record<string, unknown>;
}
