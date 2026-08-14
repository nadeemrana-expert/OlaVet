import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { LabService } from '../../../core/services/lab.service';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { TimeAgoPipe } from '../../../shared/pipes/time-ago.pipe';

@Component({
  selector: 'app-lab-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, LoadingSpinnerComponent, TimeAgoPipe],
  template: `
    @if (loading) { <app-loading-spinner /> }
    @else if (lab) {
      <a routerLink="/labs" class="back-link">← Back</a>
      <div class="detail-card">
        <h1>🔬 {{ lab.labName }}</h1>
        <div class="info-row">
          <span>{{ lab.specialization }}</span>
          <span>📍 {{ lab.location }}</span>
          <span>📞 {{ lab.contactNumber }}</span>
          <span>📧 {{ lab.email }}</span>
        </div>
      </div>
      <div class="tabs">
        <button [class.active]="tab === 'appointments'" (click)="tab = 'appointments'">Appointments</button>
        <button [class.active]="tab === 'reviews'" (click)="tab = 'reviews'">Reviews</button>
      </div>
      @if (tab === 'appointments') {
        @for (a of lab.appointments || []; track a) {
          <div class="item-card">
            <span>{{ a.appointmentDateTime | date:'medium' }}</span>
            <span>{{ a.petName }} — {{ a.ownerName }}</span>
            <span class="badge">{{ a.status }}</span>
          </div>
        } @empty { <p class="no-data">No appointments.</p> }
      }
      @if (tab === 'reviews') {
        @for (r of reviews; track r.id) {
          <div class="item-card">
            <span class="stars">⭐ {{ r.rating }}</span>
            <p>{{ r.comment }}</p>
            <span class="meta">{{ r.createdAt | timeAgo }}</span>
          </div>
        } @empty { <p class="no-data">No reviews.</p> }
      }
    }
  `,
  styles: [`
    .back-link { display:inline-block; margin-bottom:1rem; color:#667eea; text-decoration:none; }
    .detail-card { background:#fff; border-radius:12px; padding:1.5rem 2rem; margin-bottom:1.5rem; box-shadow:0 2px 8px rgba(0,0,0,0.04); h1 { margin:0 0 0.5rem; } }
    .info-row { display:flex; flex-wrap:wrap; gap:1rem; font-size:0.85rem; color:#888; }
    .tabs { display:flex; border-bottom:2px solid #eee; margin-bottom:1.25rem;
      button { padding:0.75rem 1.5rem; background:none; border:none; border-bottom:2px solid transparent; margin-bottom:-2px; cursor:pointer; color:#999; font-weight:500;
        &.active { color:#667eea; border-bottom-color:#667eea; } } }
    .item-card { background:#fff; border-radius:10px; padding:1rem 1.25rem; margin-bottom:0.75rem; box-shadow:0 2px 6px rgba(0,0,0,0.04); display:flex; gap:1rem; align-items:center; flex-wrap:wrap; }
    .badge { padding:2px 8px; border-radius:10px; background:#edf2f7; font-size:0.75rem; font-weight:600; }
    .stars { color:#ecc94b; }
    .meta { font-size:0.8rem; color:#999; }
    .no-data { color:#aaa; text-align:center; padding:2rem; }
  `],
})
export class LabDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private labService = inject(LabService);
  lab: any = null;
  reviews: any[] = [];
  loading = true;
  tab: 'appointments' | 'reviews' = 'appointments';

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.labService.getWithAppointments(id).subscribe({ next: (l) => { this.lab = l; this.loading = false; }, error: () => (this.loading = false) });
    this.labService.getReviews(id).subscribe((r) => (this.reviews = r));
  }
}
