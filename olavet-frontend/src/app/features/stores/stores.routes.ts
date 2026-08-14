import { Routes } from '@angular/router';

export const storesRoutes: Routes = [
  { path: '', loadComponent: () => import('./store-list/store-list.component').then((m) => m.StoreListComponent) },
  { path: ':id', loadComponent: () => import('./store-detail/store-detail.component').then((m) => m.StoreDetailComponent) },
  { path: ':id/order', loadComponent: () => import('./order-form/order-form.component').then((m) => m.OrderFormComponent) },
];
