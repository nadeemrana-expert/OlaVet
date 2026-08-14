import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { PetOwnerService } from '../../../core/services/pet-owner.service';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-pet-owner-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, LoadingSpinnerComponent],
  template: `
    @if (loading) { <app-loading-spinner message="Loading owner…" /> }
    @else if (owner) {
      <a routerLink="/pet-owners" class="back-link">← Back</a>
      <div class="detail-card">
        <div class="header"><h1>{{ owner.ownerName }}</h1><a [routerLink]="['edit']" class="btn-outline">Edit</a></div>
        <div class="info-grid">
          <div class="info"><span>Email</span><strong>{{ owner.email }}</strong></div>
          <div class="info"><span>Phone</span><strong>{{ owner.contactNumber }}</strong></div>
          <div class="info"><span>Address</span><strong>{{ owner.address || '—' }}</strong></div>
          <div class="info"><span>Wallet Balance</span><strong>{{ owner.wallet | currency }}</strong></div>
        </div>
      </div>
      @if (owner.pets?.length) {
        <div class="section-card">
          <h3>Pets ({{ owner.pets.length }})</h3>
          <div class="pets-grid">
            @for (pet of owner.pets; track pet.petId) {
              <a [routerLink]="['/pets', pet.petId]" class="pet-chip">🐾 {{ pet.name }} <span class="sp">{{ pet.species }}</span></a>
            }
          </div>
        </div>
      }
    }
  `,
  styles: [`
    .back-link { display:inline-block; margin-bottom:1rem; color:#667eea; text-decoration:none; }
    .detail-card { background:#fff; border-radius:12px; padding:2rem; box-shadow:0 2px 8px rgba(0,0,0,0.04); margin-bottom:1.5rem; }
    .header { display:flex; justify-content:space-between; align-items:center; margin-bottom:1.5rem; h1 { margin:0; } }
    .btn-outline { padding:0.45rem 1rem; border:1px solid #667eea; color:#667eea; border-radius:8px; text-decoration:none; font-weight:600; &:hover { background:#667eea; color:#fff; } }
    .info-grid { display:grid; grid-template-columns:repeat(auto-fit,minmax(200px,1fr)); gap:1rem; }
    .info { display:flex; flex-direction:column; span { font-size:0.8rem; color:#888; } strong { font-size:0.95rem; color:#333; } }
    .section-card { background:#fff; border-radius:12px; padding:1.5rem; box-shadow:0 2px 8px rgba(0,0,0,0.04); h3 { margin:0 0 1rem; } }
    .pets-grid { display:flex; flex-wrap:wrap; gap:0.5rem; }
    .pet-chip { padding:0.4rem 0.9rem; background:#f0f4ff; border-radius:20px; color:#333; text-decoration:none; font-size:0.85rem; .sp { color:#888; font-size:0.75rem; margin-left:0.3rem; } &:hover { background:#667eea; color:#fff; .sp { color:rgba(255,255,255,0.7); } } }
  `],
})
export class PetOwnerDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private ownerService = inject(PetOwnerService);
  owner: any = null;
  loading = true;

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.ownerService.getWithPets(id).subscribe({
      next: (o) => { this.owner = o; this.loading = false; },
      error: () => (this.loading = false),
    });
  }
}
