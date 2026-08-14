import { Routes } from '@angular/router';
import { roleGuard } from '../../core/guards/auth.guard';
import { RoleNames } from '../../core/constants/permissions';

export const petsRoutes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./pet-list/pet-list.component').then((m) => m.PetListComponent),
  },
  {
    path: 'new',
    loadComponent: () =>
      import('./pet-form/pet-form.component').then((m) => m.PetFormComponent),
    canActivate: [roleGuard(RoleNames.Admin, RoleNames.Vet, RoleNames.PetOwner)],
  },
  {
    path: ':id',
    loadComponent: () =>
      import('./pet-detail/pet-detail.component').then(
        (m) => m.PetDetailComponent
      ),
  },
  {
    path: ':id/edit',
    loadComponent: () =>
      import('./pet-form/pet-form.component').then((m) => m.PetFormComponent),
    canActivate: [roleGuard(RoleNames.Admin, RoleNames.Vet)],
  },
];
