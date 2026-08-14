import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import { Pet, PetDetails, CreatePetRequest, UpdatePetRequest, PagedResult } from '../models';

@Injectable({ providedIn: 'root' })
export class PetService {
  private readonly apiUrl = `${environment.apiUrl}/pets`;

  constructor(private http: HttpClient) {}

  getAll(page = 1, pageSize = 10): Observable<PagedResult<Pet>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedResult<Pet>>(this.apiUrl, { params });
  }

  getById(id: number): Observable<Pet> {
    return this.http.get<Pet>(`${this.apiUrl}/${id}`);
  }

  getWithOwner(id: number): Observable<PetDetails> {
    return this.http.get<PetDetails>(`${this.apiUrl}/${id}/with-owner`);
  }

  getMedicalHistory(id: number): Observable<PetDetails> {
    return this.http.get<PetDetails>(`${this.apiUrl}/${id}/medical-history`);
  }

  getByOwner(ownerId: number): Observable<Pet[]> {
    return this.http.get<Pet[]>(`${this.apiUrl}/owner/${ownerId}`);
  }

  getBySpecies(species: string): Observable<Pet[]> {
    return this.http.get<Pet[]>(`${this.apiUrl}/species/${species}`);
  }

  getDueForCheckup(daysThreshold = 180): Observable<Pet[]> {
    const params = new HttpParams().set('daysThreshold', daysThreshold);
    return this.http.get<Pet[]>(`${this.apiUrl}/due-for-checkup`, { params });
  }

  create(pet: CreatePetRequest): Observable<Pet> {
    return this.http.post<Pet>(this.apiUrl, pet);
  }

  update(id: number, pet: UpdatePetRequest): Observable<Pet> {
    return this.http.put<Pet>(`${this.apiUrl}/${id}`, pet);
  }

  delete(id: number): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
