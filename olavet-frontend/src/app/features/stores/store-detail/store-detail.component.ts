import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { StoreService } from '../../../core/services/store.service';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-store-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, LoadingSpinnerComponent],
  template: `
    @if (loading) { <app-loading-spinner /> }
    @else if (store) {
      <a routerLink="/stores" class="back-link">← Back</a>
      <div class="store-header">
        <h1>{{ store.storeName }}</h1>
        <p>📍 {{ store.location }} | 📞 {{ store.contactNumber }}</p>
      </div>
      <h3>Inventory</h3>
      <div class="inventory-grid">
        @for (item of store.inventory; track item) {
          <div class="med-card">
            <h4>{{ item.medicineName }}</h4>
            <p class="price">{{ item.price | currency }}</p>
            <span class="stock" [class.low]="item.quantity < 10">Stock: {{ item.quantity }}</span>
          </div>
        } @empty { <p class="no-data">No medicines listed.</p> }
      </div>
    }
  `,
  styles: [`
    .back-link { display:inline-block; margin-bottom:1rem; color:#667eea; text-decoration:none; }
    .store-header { background:#fff; border-radius:12px; padding:1.5rem 2rem; margin-bottom:1.5rem; box-shadow:0 2px 8px rgba(0,0,0,0.04);
      h1 { margin:0; } p { color:#888; margin:0.25rem 0 0; } }
    h3 { margin:0 0 1rem; }
    .inventory-grid { display:grid; grid-template-columns:repeat(auto-fill,minmax(200px,1fr)); gap:1rem; }
    .med-card { background:#fff; border-radius:10px; padding:1.25rem; box-shadow:0 2px 6px rgba(0,0,0,0.04); h4 { margin:0 0 0.5rem; } }
    .price { color:#667eea; font-weight:700; font-size:1.1rem; margin:0 0 0.25rem; }
    .stock { font-size:0.8rem; color:#48bb78; &.low { color:#e53e3e; } }
    .no-data { color:#aaa; text-align:center; padding:2rem; }
  `],
})
export class StoreDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private storeService = inject(StoreService);
  store: any = null;
  loading = true;

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.storeService.getWithInventory(id).subscribe({
      next: (s) => { this.store = s; this.loading = false; },
      error: () => (this.loading = false),
    });
  }
}
