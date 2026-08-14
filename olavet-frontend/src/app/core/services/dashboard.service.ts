import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  DashboardStats,
  AppointmentStats,
  TopPerformers,
  SpeciesDistribution,
  RevenueTrend,
  PaymentStats,
} from '../models';
import { Review } from '../models';

@Injectable({ providedIn: 'root' })
export class DashboardService {
  private readonly apiUrl = `${environment.apiUrl}/dashboard`;

  constructor(private http: HttpClient) {}

  getStats(): Observable<DashboardStats> {
    return this.http.get<DashboardStats>(`${this.apiUrl}/stats`);
  }

  getPaymentStats(startDate?: string, endDate?: string): Observable<PaymentStats> {
    let params = new HttpParams();
    if (startDate) params = params.set('startDate', startDate);
    if (endDate) params = params.set('endDate', endDate);
    return this.http.get<PaymentStats>(`${this.apiUrl}/payments`, { params });
  }

  getRecentReviews(count = 10): Observable<Review[]> {
    const params = new HttpParams().set('count', count);
    return this.http.get<Review[]>(`${this.apiUrl}/recent-reviews`, { params });
  }

  getAppointmentStats(): Observable<AppointmentStats> {
    return this.http.get<AppointmentStats>(`${this.apiUrl}/appointments`);
  }

  getTopPerformers(count = 5): Observable<TopPerformers> {
    const params = new HttpParams().set('count', count);
    return this.http.get<TopPerformers>(`${this.apiUrl}/top-performers`, { params });
  }

  getSpeciesDistribution(): Observable<SpeciesDistribution[]> {
    return this.http.get<SpeciesDistribution[]>(`${this.apiUrl}/species-distribution`);
  }

  getRevenueTrend(days = 30): Observable<RevenueTrend> {
    const params = new HttpParams().set('days', days);
    return this.http.get<RevenueTrend>(`${this.apiUrl}/revenue-trend`, { params });
  }
}
