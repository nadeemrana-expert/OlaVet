import { Routes } from '@angular/router';
import { authGuard, guestGuard, roleGuard } from './core/guards/auth.guard';
import { ShellComponent } from './layout/shell/shell.component';
import { RoleNames } from './core/constants/permissions';

export const routes: Routes = [
  // ── Public / Guest routes ──────────────────────────
  {
    path: 'auth',
    canActivate: [guestGuard],
    loadChildren: () =>
      import('./features/auth/auth.routes').then((m) => m.AUTH_ROUTES),
  },

  // ── Authenticated routes inside the shell layout ───
  {
    path: '',
    component: ShellComponent,
    canActivate: [authGuard],
    children: [
      { path: '', redirectTo: 'dashboard', pathMatch: 'full' },
      {
        path: 'dashboard',
        loadChildren: () =>
          import('./features/dashboard/dashboard.routes').then(
            (m) => m.dashboardRoutes
          ),
      },
      {
        path: 'pets',
        canActivate: [roleGuard(RoleNames.Admin, RoleNames.Vet, RoleNames.PetOwner)],
        loadChildren: () =>
          import('./features/pets/pets.routes').then((m) => m.petsRoutes),
      },
      {
        path: 'appointments',
        canActivate: [roleGuard(RoleNames.Admin, RoleNames.Vet, RoleNames.PetOwner, RoleNames.LabTechnician)],
        loadChildren: () =>
          import('./features/appointments/appointments.routes').then(
            (m) => m.appointmentsRoutes
          ),
      },
      {
        path: 'vets',
        canActivate: [roleGuard(RoleNames.Admin, RoleNames.PetOwner)],
        loadChildren: () =>
          import('./features/vets/vets.routes').then((m) => m.vetsRoutes),
      },
      {
        path: 'pet-owners',
        canActivate: [roleGuard(RoleNames.Admin, RoleNames.Vet)],
        loadChildren: () =>
          import('./features/pet-owners/pet-owners.routes').then(
            (m) => m.petOwnersRoutes
          ),
      },
      {
        path: 'stores',
        canActivate: [roleGuard(RoleNames.Admin, RoleNames.PetOwner, RoleNames.StoreManager)],
        loadChildren: () =>
          import('./features/stores/stores.routes').then(
            (m) => m.storesRoutes
          ),
      },
      {
        path: 'labs',
        canActivate: [roleGuard(RoleNames.Admin, RoleNames.PetOwner, RoleNames.LabTechnician)],
        loadChildren: () =>
          import('./features/labs/labs.routes').then((m) => m.labsRoutes),
      },
      {
        path: 'reviews',
        canActivate: [roleGuard(RoleNames.Admin, RoleNames.Vet, RoleNames.PetOwner)],
        loadChildren: () =>
          import('./features/reviews/reviews.routes').then(
            (m) => m.reviewsRoutes
          ),
      },
      {
        path: 'medical-records',
        canActivate: [roleGuard(RoleNames.Admin, RoleNames.Vet, RoleNames.PetOwner)],
        loadChildren: () =>
          import('./features/medical-records/medical-records.routes').then(
            (m) => m.medicalRecordsRoutes
          ),
      },
    ],
  },

  // ── Fallback ───────────────────────────────────────
  { path: '**', redirectTo: 'dashboard' },
];
