import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  PetOwner,
  PetOwnerDetails,
  CreatePetOwnerRequest,
  UpdatePetOwnerRequest,
  AddFundsRequest,
  OwnerPaymentSummary,
  PagedResult,
} from '../models';

@Injectable({ providedIn: 'root' })
export class PetOwnerService {
  private readonly apiUrl = `${environment.apiUrl}/petowners`;

  constructor(private http: HttpClient) {}

  getAll(page = 1, pageSize = 10): Observable<PagedResult<PetOwner>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedResult<PetOwner>>(this.apiUrl, { params });
  }

  getById(id: number): Observable<PetOwner> {
    return this.http.get<PetOwner>(`${this.apiUrl}/${id}`);
  }

  getWithPets(id: number): Observable<PetOwnerDetails> {
    return this.http.get<PetOwnerDetails>(`${this.apiUrl}/${id}/with-pets`);
  }

  search(term: string): Observable<PetOwner[]> {
    const params = new HttpParams().set('term', term);
    return this.http.get<PetOwner[]>(`${this.apiUrl}/search`, { params });
  }

  create(owner: CreatePetOwnerRequest): Observable<PetOwner> {
    return this.http.post<PetOwner>(this.apiUrl, owner);
  }

  update(id: number, owner: UpdatePetOwnerRequest): Observable<PetOwner> {
    return this.http.put<PetOwner>(`${this.apiUrl}/${id}`, owner);
  }

  addFunds(id: number, request: AddFundsRequest): Observable<{ wallet: number; message: string }> {
    return this.http.post<{ wallet: number; message: string }>(`${this.apiUrl}/${id}/add-funds`, request);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  getPaymentSummary(id: number): Observable<OwnerPaymentSummary> {
    return this.http.get<OwnerPaymentSummary>(`${this.apiUrl}/${id}/payment-summary`);
  }
}
