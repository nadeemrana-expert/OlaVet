import { Routes } from '@angular/router';

export const petOwnersRoutes: Routes = [
  {
    path: '',
    loadComponent: () => import('./pet-owner-list/pet-owner-list.component').then((m) => m.PetOwnerListComponent),
  },
  {
    path: 'new',
    loadComponent: () => import('./pet-owner-form/pet-owner-form.component').then((m) => m.PetOwnerFormComponent),
  },
  {
    path: ':id',
    loadComponent: () => import('./pet-owner-detail/pet-owner-detail.component').then((m) => m.PetOwnerDetailComponent),
  },
  {
    path: ':id/edit',
    loadComponent: () => import('./pet-owner-form/pet-owner-form.component').then((m) => m.PetOwnerFormComponent),
  },
];
