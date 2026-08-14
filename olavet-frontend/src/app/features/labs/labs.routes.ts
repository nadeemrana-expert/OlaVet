import { Routes } from '@angular/router';

export const labsRoutes: Routes = [
  { path: '', loadComponent: () => import('./lab-list/lab-list.component').then((m) => m.LabListComponent) },
  { path: ':id', loadComponent: () => import('./lab-detail/lab-detail.component').then((m) => m.LabDetailComponent) },
];
