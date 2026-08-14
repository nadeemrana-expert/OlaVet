import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  Vet,
  VetWithRating,
  VetDetails,
  CreateVetRequest,
  UpdateVetRequest,
  PagedResult,
} from '../models';
import { VetReview, RatingDistribution } from '../models';

@Injectable({ providedIn: 'root' })
export class VetService {
  private readonly apiUrl = `${environment.apiUrl}/vets`;

  constructor(private http: HttpClient) {}

  getAll(page = 1, pageSize = 10): Observable<PagedResult<Vet>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedResult<Vet>>(this.apiUrl, { params });
  }

  getById(id: number): Observable<Vet> {
    return this.http.get<Vet>(`${this.apiUrl}/${id}`);
  }

  getDetails(id: number): Observable<VetDetails> {
    return this.http.get<VetDetails>(`${this.apiUrl}/${id}/details`);
  }

  getWithRatings(): Observable<VetWithRating[]> {
    return this.http.get<VetWithRating[]>(`${this.apiUrl}/with-ratings`);
  }

  getTopRated(count = 10): Observable<VetWithRating[]> {
    const params = new HttpParams().set('count', count);
    return this.http.get<VetWithRating[]>(`${this.apiUrl}/top-rated`, { params });
  }

  search(term: string): Observable<Vet[]> {
    const params = new HttpParams().set('term', term);
    return this.http.get<Vet[]>(`${this.apiUrl}/search`, { params });
  }

  getBySpecialization(specialization: string): Observable<Vet[]> {
    return this.http.get<Vet[]>(`${this.apiUrl}/specialization/${specialization}`);
  }

  getAvailable(dateTime: string): Observable<Vet[]> {
    const params = new HttpParams().set('dateTime', dateTime);
    return this.http.get<Vet[]>(`${this.apiUrl}/available`, { params });
  }

  getReviews(id: number): Observable<VetReview[]> {
    return this.http.get<VetReview[]>(`${this.apiUrl}/${id}/reviews`);
  }

  getRatingDistribution(id: number): Observable<RatingDistribution> {
    return this.http.get<RatingDistribution>(`${this.apiUrl}/${id}/rating-distribution`);
  }

  create(vet: CreateVetRequest): Observable<Vet> {
    return this.http.post<Vet>(this.apiUrl, vet);
  }

  update(id: number, vet: UpdateVetRequest): Observable<Vet> {
    return this.http.put<Vet>(`${this.apiUrl}/${id}`, vet);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
