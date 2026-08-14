import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Store,
  StoreWithInventory,
  MedicineOrder,
  CreateOrderRequest,
  PagedResult,
} from '../models';
import { StoreReview } from '../models';

@Injectable({ providedIn: 'root' })
export class StoreService {
  private readonly apiUrl = `${environment.apiUrl}/stores`;
  private readonly ordersUrl = `${environment.apiUrl}/orders`;

  constructor(private http: HttpClient) {}

  // ── Stores ───────────────────────────────────

  getAll(page = 1, pageSize = 10): Observable<PagedResult<Store>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedResult<Store>>(this.apiUrl, { params });
  }

  getById(id: number): Observable<Store> {
    return this.http.get<Store>(`${this.apiUrl}/${id}`);
  }

  getWithInventory(id: number): Observable<StoreWithInventory> {
    return this.http.get<StoreWithInventory>(`${this.apiUrl}/${id}/inventory`);
  }

  getWithRatings(): Observable<Store[]> {
    return this.http.get<Store[]>(`${this.apiUrl}/with-ratings`);
  }

  getTopRated(count = 10): Observable<Store[]> {
    const params = new HttpParams().set('count', count);
    return this.http.get<Store[]>(`${this.apiUrl}/top-rated`, { params });
  }

  search(term: string): Observable<Store[]> {
    const params = new HttpParams().set('term', term);
    return this.http.get<Store[]>(`${this.apiUrl}/search`, { params });
  }

  getStoresWithMedicine(medicineId: number): Observable<Store[]> {
    return this.http.get<Store[]>(`${this.apiUrl}/with-medicine/${medicineId}`);
  }

  getOpenStores(): Observable<Store[]> {
    return this.http.get<Store[]>(`${this.apiUrl}/open`);
  }

  getReviews(id: number): Observable<StoreReview[]> {
    return this.http.get<StoreReview[]>(`${this.apiUrl}/${id}/reviews`);
  }

  // ── Orders ───────────────────────────────────

  getOrders(page = 1, pageSize = 10): Observable<PagedResult<MedicineOrder>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedResult<MedicineOrder>>(this.ordersUrl, { params });
  }

  getOrderById(id: number): Observable<MedicineOrder> {
    return this.http.get<MedicineOrder>(`${this.ordersUrl}/${id}`);
  }

  getOrdersByOwner(ownerId: number): Observable<MedicineOrder[]> {
    return this.http.get<MedicineOrder[]>(`${this.ordersUrl}/owner/${ownerId}`);
  }

  getPendingOrders(): Observable<MedicineOrder[]> {
    return this.http.get<MedicineOrder[]>(`${this.ordersUrl}/pending`);
  }

  createOrder(request: CreateOrderRequest): Observable<MedicineOrder> {
    return this.http.post<MedicineOrder>(this.ordersUrl, request);
  }
}
