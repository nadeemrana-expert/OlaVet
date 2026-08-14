import { Routes } from '@angular/router';

export const reviewsRoutes: Routes = [
  { path: '', loadComponent: () => import('./review-list/review-list.component').then((m) => m.ReviewListComponent) },
];
