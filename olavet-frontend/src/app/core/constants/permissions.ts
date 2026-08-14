// =============================================
// Permission constants — mirrors backend Permissions.cs
// =============================================

export const Permissions = {
  // Pet Owners
  PetOwnersRead: 'petowners.read',
  PetOwnersCreate: 'petowners.create',
  PetOwnersUpdate: 'petowners.update',
  PetOwnersDelete: 'petowners.delete',

  // Pets
  PetsRead: 'pets.read',
  PetsCreate: 'pets.create',
  PetsUpdate: 'pets.update',
  PetsDelete: 'pets.delete',

  // Vets
  VetsRead: 'vets.read',
  VetsCreate: 'vets.create',
  VetsUpdate: 'vets.update',
  VetsDelete: 'vets.delete',
  VetsManage: 'vets.manage',

  // Appointments
  AppointmentsRead: 'appointments.read',
  AppointmentsCreate: 'appointments.create',
  AppointmentsUpdate: 'appointments.update',
  AppointmentsCancel: 'appointments.cancel',

  // Orders
  OrdersRead: 'orders.read',
  OrdersCreate: 'orders.create',
  OrdersUpdate: 'orders.update',
  OrdersCancel: 'orders.cancel',

  // Reviews
  ReviewsRead: 'reviews.read',
  ReviewsCreate: 'reviews.create',
  ReviewsUpdate: 'reviews.update',
  ReviewsDelete: 'reviews.delete',

  // Admin
  AdminFullAccess: 'admin.full',
  AdminUserManagement: 'admin.users',
  AdminRoleManagement: 'admin.roles',
  AdminReports: 'admin.reports',

  // Labs
  LabsRead: 'labs.read',
  LabsCreate: 'labs.create',
  LabsUpdate: 'labs.update',
  LabsDelete: 'labs.delete',

  // Stores
  StoresRead: 'stores.read',
  StoresCreate: 'stores.create',
  StoresUpdate: 'stores.update',
  StoresDelete: 'stores.delete',
} as const;

export const RoleNames = {
  Admin: 'Admin',
  Vet: 'Vet',
  PetOwner: 'PetOwner',
  LabTechnician: 'LabTechnician',
  StoreManager: 'StoreManager',
} as const;
