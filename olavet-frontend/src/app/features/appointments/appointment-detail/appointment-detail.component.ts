import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { AppointmentService } from '../../../core/services/appointment.service';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { StatusBadgePipe } from '../../../shared/pipes/status-badge.pipe';

@Component({
  selector: 'app-appointment-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, LoadingSpinnerComponent, StatusBadgePipe],
  template: `
    @if (loading) {
      <app-loading-spinner message="Loading appointment…" />
    } @else if (appointment) {
      <a routerLink="/appointments" class="back-link">← Back</a>
      <div class="detail-card">
        <div class="detail-header">
          <h1>Appointment #{{ appointment.vetAppointmentId || appointment.labAppointmentId }}</h1>
          <span [class]="appointment.status | statusBadge">{{ appointment.status }}</span>
        </div>
        <div class="info-grid">
          <div class="info-item"><span class="label">Date & Time</span><span class="value">{{ appointment.appointmentDateTime | date:'full' }}</span></div>
          <div class="info-item"><span class="label">Pet</span><span class="value">{{ appointment.petName }}</span></div>
          <div class="info-item"><span class="label">Owner</span><span class="value">{{ appointment.ownerName }}</span></div>
          <div class="info-item"><span class="label">Vet/Lab</span><span class="value">{{ appointment.vetName || appointment.labName || '—' }}</span></div>
          <div class="info-item"><span class="label">Reason</span><span class="value">{{ appointment.reason || '—' }}</span></div>
          <div class="info-item"><span class="label">Notes</span><span class="value">{{ appointment.notes || '—' }}</span></div>
          <div class="info-item"><span class="label">Type</span><span class="value">{{ appointment.appointmentType || '—' }}</span></div>
          <div class="info-item"><span class="label">Fee</span><span class="value">{{ appointment.fee | currency }}</span></div>
        </div>
      </div>
    }
  `,
  styles: [`
    .back-link { display: inline-block; margin-bottom: 1rem; color: #667eea; text-decoration: none; &:hover { text-decoration: underline; } }
    .detail-card { background: #fff; border-radius: 12px; padding: 2rem; box-shadow: 0 2px 8px rgba(0,0,0,0.04); }
    .detail-header { display: flex; justify-content: space-between; align-items: center; margin-bottom: 1.5rem; h1 { margin: 0; font-size: 1.4rem; } }
    .info-grid { display: grid; grid-template-columns: repeat(auto-fit, minmax(240px, 1fr)); gap: 1rem; }
    .info-item { display: flex; flex-direction: column; padding: 0.5rem 0; }
    .label { font-size: 0.8rem; color: #888; margin-bottom: 0.2rem; }
    .value { font-size: 0.95rem; color: #333; font-weight: 500; }
    :host ::ng-deep { .badge { padding: 3px 10px; border-radius: 12px; font-size: 0.75rem; font-weight: 600; }
      .badge-success { background: #c6f6d5; color: #276749; } .badge-warning { background: #fefcbf; color: #975a16; }
      .badge-danger { background: #fed7d7; color: #9b2c2c; } .badge-info { background: #bee3f8; color: #2a4365; }
      .badge-default { background: #edf2f7; color: #4a5568; } }
  `],
})
export class AppointmentDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private appointmentService = inject(AppointmentService);
  appointment: any = null;
  loading = true;

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.appointmentService.getVetAppointment(id).subscribe({
      next: (a: any) => { this.appointment = a; this.loading = false; },
      error: () => (this.loading = false),
    });
  }
}
