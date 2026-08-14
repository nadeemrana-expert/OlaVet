import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AppointmentService } from '../../../core/services/appointment.service';
import { VetService } from '../../../core/services/vet.service';
import { NotificationService } from '../../../core/services/notification.service';

@Component({
  selector: 'app-appointment-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  template: `
    <div class="form-page">
      <a routerLink="/appointments" class="back-link">← Back</a>
      <h1>New Vet Appointment</h1>
      <form [formGroup]="form" (ngSubmit)="submit()" class="form-card">
        <div class="form-row">
          <div class="form-group">
            <label for="petId">Pet ID *</label>
            <input id="petId" formControlName="petId" type="number" />
          </div>
          <div class="form-group">
            <label for="vetId">Vet ID *</label>
            <input id="vetId" formControlName="vetId" type="number" />
          </div>
        </div>
        <div class="form-row">
          <div class="form-group">
            <label for="petOwnerId">Owner ID *</label>
            <input id="petOwnerId" formControlName="petOwnerId" type="number" />
          </div>
          <div class="form-group">
            <label for="appointmentDateTime">Date & Time *</label>
            <input id="appointmentDateTime" formControlName="appointmentDateTime" type="datetime-local" />
          </div>
        </div>
        <div class="form-group">
          <label for="reason">Reason *</label>
          <textarea id="reason" formControlName="reason" rows="3"></textarea>
        </div>
        <div class="form-group">
          <label for="notes">Notes</label>
          <textarea id="notes" formControlName="notes" rows="2"></textarea>
        </div>
        <div class="form-actions">
          <a routerLink="/appointments" class="btn btn-cancel">Cancel</a>
          <button type="submit" class="btn btn-primary" [disabled]="loading">
            @if (loading) { <span class="spinner"></span> }
            Create Appointment
          </button>
        </div>
      </form>
    </div>
  `,
  styleUrl: './appointment-form.component.scss',
})
export class AppointmentFormComponent implements OnInit {
  private fb = inject(FormBuilder);
  private appointmentService = inject(AppointmentService);
  private router = inject(Router);
  private notify = inject(NotificationService);
  form!: FormGroup;
  loading = false;

  ngOnInit(): void {
    this.form = this.fb.group({
      petId: [null, Validators.required],
      vetId: [null, Validators.required],
      petOwnerId: [null, Validators.required],
      appointmentDateTime: ['', Validators.required],
      reason: ['', Validators.required],
      notes: [''],
      vetAppointmentTypeId: [1],
    });
  }

  submit(): void {
    if (this.form.invalid) { this.form.markAllAsTouched(); return; }
    this.loading = true;
    this.appointmentService.createVetAppointment(this.form.value).subscribe({
      next: () => {
        this.notify.success('Appointment created!');
        this.router.navigate(['/appointments']);
      },
      error: () => (this.loading = false),
    });
  }
}
