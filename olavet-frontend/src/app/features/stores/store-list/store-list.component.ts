import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { StoreService } from '../../../core/services/store.service';
import { PagedResult } from '../../../core/models/common.model';
import { SearchBoxComponent } from '../../../shared/components/search-box/search-box.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';

@Component({
  selector: 'app-store-list',
  standalone: true,
  imports: [CommonModule, RouterLink, SearchBoxComponent, LoadingSpinnerComponent, EmptyStateComponent],
  template: `
    <div class="page-header"><div><h1>Pharmacy & Stores</h1><p class="subtitle">Browse medicine stores and place orders</p></div></div>
    <div class="toolbar"><app-search-box placeholder="Search stores…" (searchChange)="onSearch($event)" /></div>
    @if (loading) { <app-loading-spinner message="Loading stores…" /> }
    @else if (stores.length === 0) { <app-empty-state icon="💊" title="No stores found" /> }
    @else {
      <div class="store-grid">
        @for (s of stores; track s.storeId) {
          <div class="store-card">
            <div class="store-icon">💊</div>
            <h3><a [routerLink]="[s.storeId]">{{ s.storeName }}</a></h3>
            <p class="location">📍 {{ s.location }}</p>
            <p class="contact">📞 {{ s.contactNumber }}</p>
            <a [routerLink]="[s.storeId]" class="view-btn">Browse Inventory →</a>
          </div>
        }
      </div>
    }
  `,
  styles: [`
    .page-header { display:flex; justify-content:space-between; margin-bottom:1.5rem; h1 { margin:0; font-size:1.6rem; } .subtitle { color:#888; } }
    .toolbar { margin-bottom:1.25rem; }
    .store-grid { display:grid; grid-template-columns:repeat(auto-fill,minmax(280px,1fr)); gap:1.25rem; }
    .store-card { background:#fff; border-radius:12px; padding:1.5rem; text-align:center; box-shadow:0 2px 8px rgba(0,0,0,0.04); transition:transform 0.15s; &:hover { transform:translateY(-3px); } }
    .store-icon { font-size:2.5rem; margin-bottom:0.5rem; }
    h3 a { color:#333; text-decoration:none; &:hover { color:#667eea; } }
    .location,.contact { margin:0.25rem 0; font-size:0.85rem; color:#888; }
    .view-btn { display:inline-block; margin-top:0.75rem; color:#667eea; font-size:0.85rem; text-decoration:none; &:hover { text-decoration:underline; } }
  `],
})
export class StoreListComponent implements OnInit {
  private storeService = inject(StoreService);
  stores: any[] = [];
  loading = true;

  ngOnInit(): void { this.load(); }

  load(): void {
    this.storeService.getAll(1, 50).subscribe({
      next: (r) => { this.stores = r.items; this.loading = false; },
      error: () => (this.loading = false),
    });
  }

  onSearch(term: string): void {
    this.loading = true;
    this.storeService.search(term).subscribe({
      next: (s) => { this.stores = s; this.loading = false; },
      error: () => (this.loading = false),
    });
  }
}
