import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { environment } from '../../../environments/environment';
import {
  VetAppointment,
  LabAppointment,
  CreateVetAppointmentRequest,
  CreateLabAppointmentRequest,
  TimeSlot,
  PagedResult,
} from '../models';

@Injectable({ providedIn: 'root' })
export class AppointmentService {
  private readonly apiUrl = `${environment.apiUrl}/appointments`;

  constructor(private http: HttpClient) {}

  // ── Vet Appointments ─────────────────────────

  getVetAppointments(page = 1, pageSize = 10): Observable<PagedResult<VetAppointment>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedResult<VetAppointment>>(`${this.apiUrl}/vet`, { params });
  }

  getVetAppointment(id: number): Observable<VetAppointment> {
    return this.http.get<VetAppointment>(`${this.apiUrl}/vet/${id}`);
  }

  getVetAppointmentsByOwner(ownerId: number): Observable<VetAppointment[]> {
    return this.http.get<VetAppointment[]>(`${this.apiUrl}/vet/owner/${ownerId}`);
  }

  getVetSchedule(vetId: number, date: string): Observable<VetAppointment[]> {
    const params = new HttpParams().set('vetId', vetId).set('date', date);
    return this.http.get<VetAppointment[]>(`${this.apiUrl}/vet/schedule`, { params });
  }

  getUpcomingVetAppointments(days = 7): Observable<VetAppointment[]> {
    const params = new HttpParams().set('days', days);
    return this.http.get<VetAppointment[]>(`${this.apiUrl}/vet/upcoming`, { params });
  }

  getPetHistory(petId: number): Observable<VetAppointment[]> {
    return this.http.get<VetAppointment[]>(`${this.apiUrl}/vet/pet/${petId}/history`);
  }

  getAvailableSlots(vetId: number, date: string): Observable<TimeSlot[]> {
    const params = new HttpParams().set('vetId', vetId).set('date', date);
    return this.http.get<TimeSlot[]>(`${this.apiUrl}/vet/available-slots`, { params });
  }

  createVetAppointment(request: CreateVetAppointmentRequest): Observable<VetAppointment> {
    return this.http.post<VetAppointment>(`${this.apiUrl}/vet`, request);
  }

  // ── Lab Appointments ─────────────────────────

  getLabAppointments(page = 1, pageSize = 10): Observable<PagedResult<LabAppointment>> {
    const params = new HttpParams().set('page', page).set('pageSize', pageSize);
    return this.http.get<PagedResult<LabAppointment>>(`${this.apiUrl}/lab`, { params });
  }

  getLabAppointment(id: number): Observable<LabAppointment> {
    return this.http.get<LabAppointment>(`${this.apiUrl}/lab/${id}`);
  }

  getLabAppointmentsByOwner(ownerId: number): Observable<LabAppointment[]> {
    return this.http.get<LabAppointment[]>(`${this.apiUrl}/lab/owner/${ownerId}`);
  }

  getUpcomingLabAppointments(days = 7): Observable<LabAppointment[]> {
    const params = new HttpParams().set('days', days);
    return this.http.get<LabAppointment[]>(`${this.apiUrl}/lab/upcoming`, { params });
  }

  createLabAppointment(request: CreateLabAppointmentRequest): Observable<LabAppointment> {
    return this.http.post<LabAppointment>(`${this.apiUrl}/lab`, request);
  }
}
