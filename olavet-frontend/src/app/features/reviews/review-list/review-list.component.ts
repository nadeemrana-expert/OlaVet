import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { DashboardService } from '../../../core/services/dashboard.service';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { TimeAgoPipe } from '../../../shared/pipes/time-ago.pipe';

@Component({
  selector: 'app-review-list',
  standalone: true,
  imports: [CommonModule, LoadingSpinnerComponent, EmptyStateComponent, TimeAgoPipe],
  template: `
    <div class="page-header"><h1>Reviews</h1><p class="subtitle">All reviews across the platform</p></div>
    @if (loading) { <app-loading-spinner /> }
    @else if (reviews.length === 0) { <app-empty-state icon="⭐" title="No reviews yet" /> }
    @else {
      <div class="reviews-list">
        @for (r of reviews; track r.reviewId) {
          <div class="review-card">
            <div class="review-header">
              <div class="stars">
                @for (s of [1,2,3,4,5]; track s) { <span [class.filled]="s <= r.rating">★</span> }
              </div>
              <span class="date">{{ r.reviewDateTime | timeAgo }}</span>
            </div>
            <p class="comment">{{ r.comments }}</p>
            <div class="review-meta">
              <span>By: {{ r.ownerName || 'Anonymous' }}</span>
              <span>For: {{ r.reviewType }} — {{ r.entityName || 'N/A' }}</span>
            </div>
          </div>
        }
      </div>
    }
  `,
  styles: [`
    .page-header { margin-bottom:1.5rem; h1 { margin:0; font-size:1.6rem; } .subtitle { color:#888; } }
    .reviews-list { display:flex; flex-direction:column; gap:0.75rem; }
    .review-card { background:#fff; border-radius:12px; padding:1.25rem 1.5rem; box-shadow:0 2px 8px rgba(0,0,0,0.04); }
    .review-header { display:flex; justify-content:space-between; margin-bottom:0.5rem; }
    .stars { color:#ddd; font-size:1.1rem; .filled { color:#ecc94b; } }
    .date { font-size:0.8rem; color:#999; }
    .comment { margin:0 0 0.5rem; font-size:0.95rem; color:#555; }
    .review-meta { display:flex; gap:1.5rem; font-size:0.8rem; color:#999; }
  `],
})
export class ReviewListComponent implements OnInit {
  private dashboardService = inject(DashboardService);
  reviews: any[] = [];
  loading = true;

  ngOnInit(): void {
    this.dashboardService.getRecentReviews().subscribe({
      next: (r) => { this.reviews = r; this.loading = false; },
      error: () => (this.loading = false),
    });
  }
}
