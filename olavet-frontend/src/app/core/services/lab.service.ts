import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Lab, LabWithRating, LabWithAppointments } from '../models';
import { LabReview, RatingDistribution, PagedResult } from '../models';

@Injectable({ providedIn: 'root' })
export class LabService {
  private readonly apiUrl = `${environment.apiUrl}/labs`;

  constructor(private http: HttpClient) {}

  getAll(page = 1, pageSize = 10): Observable<PagedResult<Lab>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedResult<Lab>>(this.apiUrl, { params });
  }

  getById(id: number): Observable<Lab> {
    return this.http.get<Lab>(`${this.apiUrl}/${id}`);
  }

  getWithAppointments(id: number): Observable<LabWithAppointments> {
    return this.http.get<LabWithAppointments>(`${this.apiUrl}/${id}/appointments`);
  }

  getWithRatings(): Observable<LabWithRating[]> {
    return this.http.get<LabWithRating[]>(`${this.apiUrl}/with-ratings`);
  }

  getTopRated(count = 10): Observable<LabWithRating[]> {
    const params = new HttpParams().set('count', count);
    return this.http.get<LabWithRating[]>(`${this.apiUrl}/top-rated`, { params });
  }

  search(term: string): Observable<Lab[]> {
    const params = new HttpParams().set('term', term);
    return this.http.get<Lab[]>(`${this.apiUrl}/search`, { params });
  }

  getBySpecialization(specialization: string): Observable<Lab[]> {
    return this.http.get<Lab[]>(`${this.apiUrl}/specialization/${specialization}`);
  }

  getReviews(id: number): Observable<LabReview[]> {
    return this.http.get<LabReview[]>(`${this.apiUrl}/${id}/reviews`);
  }

  getRatingDistribution(id: number): Observable<RatingDistribution> {
    return this.http.get<RatingDistribution>(`${this.apiUrl}/${id}/rating-distribution`);
  }
}
