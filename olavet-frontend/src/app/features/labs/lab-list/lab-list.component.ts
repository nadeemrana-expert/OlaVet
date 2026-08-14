import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { LabService } from '../../../core/services/lab.service';
import { SearchBoxComponent } from '../../../shared/components/search-box/search-box.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';

@Component({
  selector: 'app-lab-list',
  standalone: true,
  imports: [CommonModule, RouterLink, SearchBoxComponent, LoadingSpinnerComponent, EmptyStateComponent],
  template: `
    <div class="page-header"><div><h1>Laboratories</h1><p class="subtitle">Browse diagnostic labs</p></div></div>
    <div class="toolbar"><app-search-box placeholder="Search labs…" (searchChange)="onSearch($event)" /></div>
    @if (loading) { <app-loading-spinner /> }
    @else if (labs.length === 0) { <app-empty-state icon="🔬" title="No labs found" /> }
    @else {
      <div class="lab-grid">
        @for (lab of labs; track lab.labId) {
          <div class="lab-card">
            <div class="lab-icon">🔬</div>
            <h3><a [routerLink]="[lab.labId]">{{ lab.labName }}</a></h3>
            <p class="spec">{{ lab.specialization }}</p>
            <div class="rating">⭐ {{ lab.averageRating | number:'1.1-1' }} ({{ lab.totalReviews }})</div>
            <p class="contact">📍 {{ lab.location }} | 📞 {{ lab.contactNumber }}</p>
          </div>
        }
      </div>
    }
  `,
  styles: [`
    .page-header { margin-bottom:1.5rem; h1 { margin:0; font-size:1.6rem; } .subtitle { color:#888; } }
    .toolbar { margin-bottom:1.25rem; }
    .lab-grid { display:grid; grid-template-columns:repeat(auto-fill,minmax(280px,1fr)); gap:1.25rem; }
    .lab-card { background:#fff; border-radius:12px; padding:1.5rem; text-align:center; box-shadow:0 2px 8px rgba(0,0,0,0.04); transition:transform 0.15s; &:hover { transform:translateY(-3px); } }
    .lab-icon { font-size:2.5rem; margin-bottom:0.5rem; }
    h3 a { color:#333; text-decoration:none; &:hover { color:#667eea; } }
    .spec { color:#888; font-size:0.85rem; margin:0.25rem 0; }
    .rating { font-size:0.9rem; margin-bottom:0.25rem; }
    .contact { font-size:0.8rem; color:#999; margin:0; }
  `],
})
export class LabListComponent implements OnInit {
  private labService = inject(LabService);
  labs: any[] = [];
  loading = true;

  ngOnInit(): void { this.load(); }

  load(): void {
    this.labService.getAll(1, 50).subscribe({
      next: (r) => { this.labs = r.items; this.loading = false; },
      error: () => (this.loading = false),
    });
  }

  onSearch(term: string): void {
    this.loading = true;
    this.labService.search(term).subscribe({
      next: (labs) => { this.labs = labs; this.loading = false; },
      error: () => (this.loading = false),
    });
  }
}
