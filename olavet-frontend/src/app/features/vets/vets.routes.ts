import { Routes } from '@angular/router';
import { roleGuard } from '../../core/guards/auth.guard';
import { RoleNames } from '../../core/constants/permissions';

export const vetsRoutes: Routes = [
  {
    path: '',
    loadComponent: () =>
      import('./vet-list/vet-list.component').then((m) => m.VetListComponent),
  },
  {
    path: 'new',
    loadComponent: () =>
      import('./vet-form/vet-form.component').then((m) => m.VetFormComponent),
    canActivate: [roleGuard(RoleNames.Admin)],
  },
  {
    path: ':id',
    loadComponent: () =>
      import('./vet-detail/vet-detail.component').then(
        (m) => m.VetDetailComponent
      ),
  },
  {
    path: ':id/edit',
    loadComponent: () =>
      import('./vet-form/vet-form.component').then((m) => m.VetFormComponent),
    canActivate: [roleGuard(RoleNames.Admin)],
  },
];
