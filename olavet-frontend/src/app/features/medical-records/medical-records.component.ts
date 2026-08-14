import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { PetService } from '../../core/services/pet.service';
import { SearchBoxComponent } from '../../shared/components/search-box/search-box.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-medical-records',
  standalone: true,
  imports: [CommonModule, SearchBoxComponent, LoadingSpinnerComponent],
  template: `
    <div class="page-header"><h1>Medical Records</h1><p class="subtitle">Search pets and view their medical history</p></div>
    <div class="toolbar"><app-search-box placeholder="Enter Pet ID to view records…" (searchChange)="loadRecords($event)" /></div>
    @if (loading) { <app-loading-spinner /> }
    @else if (pet) {
      <div class="pet-banner">
        <span class="pet-icon">🐾</span>
        <div>
          <h2>{{ pet.name }}</h2>
          <span>{{ pet.species }} — {{ pet.breed }}</span>
        </div>
      </div>
      <div class="records-timeline">
        @for (r of records; track r.recordId) {
          <div class="record-item">
            <div class="timeline-dot"></div>
            <div class="record-content">
              <div class="record-header">
                <span class="record-type">{{ r.recordType }}</span>
                <span class="record-date">{{ r.recordDate | date:'mediumDate' }}</span>
              </div>
              <p><strong>Diagnosis:</strong> {{ r.diagnosis }}</p>
              <p><strong>Treatment:</strong> {{ r.treatmentDescription }}</p>
              @if (r.vetName) { <span class="vet">👨‍⚕️ {{ r.vetName }}</span> }
            </div>
          </div>
        } @empty { <p class="no-data">No medical records found.</p> }
      </div>
    }
  `,
  styles: [`
    .page-header { margin-bottom:1.5rem; h1 { margin:0; font-size:1.6rem; } .subtitle { color:#888; } }
    .toolbar { margin-bottom:1.5rem; }
    .pet-banner { display:flex; align-items:center; gap:1rem; background:#fff; border-radius:12px; padding:1.25rem 1.5rem; margin-bottom:1.5rem; box-shadow:0 2px 8px rgba(0,0,0,0.04);
      .pet-icon { font-size:2rem; } h2 { margin:0; } span { color:#888; font-size:0.85rem; } }
    .records-timeline { position:relative; padding-left:2rem; }
    .record-item { position:relative; margin-bottom:1.25rem; }
    .timeline-dot { position:absolute; left:-2rem; top:0.5rem; width:12px; height:12px; border-radius:50%; background:#667eea; border:2px solid #fff; box-shadow:0 0 0 2px #667eea; }
    .record-item::before { content:''; position:absolute; left:calc(-2rem + 5px); top:1rem; bottom:-1.25rem; width:2px; background:#e2e8f0; }
    .record-item:last-child::before { display:none; }
    .record-content { background:#fff; border-radius:10px; padding:1rem 1.25rem; box-shadow:0 2px 6px rgba(0,0,0,0.04); }
    .record-header { display:flex; justify-content:space-between; margin-bottom:0.5rem; }
    .record-type { padding:2px 8px; border-radius:10px; background:#e2e8f0; font-size:0.75rem; font-weight:600; }
    .record-date { font-size:0.8rem; color:#999; }
    p { margin:0.25rem 0; font-size:0.9rem; color:#555; }
    .vet { font-size:0.8rem; color:#888; }
    .no-data { text-align:center; color:#aaa; padding:2rem; }
  `],
})
export class MedicalRecordsComponent {
  private petService = inject(PetService);
  pet: any = null;
  records: any[] = [];
  loading = false;

  loadRecords(petIdStr: string): void {
    const petId = Number(petIdStr);
    if (!petId) return;
    this.loading = true;
    this.petService.getMedicalHistory(petId).subscribe({
      next: (data: any) => {
        this.pet = { name: data.name, species: data.species, breed: data.breed };
        this.records = data.medicalRecords || [];
        this.loading = false;
      },
      error: () => (this.loading = false),
    });
  }
}
