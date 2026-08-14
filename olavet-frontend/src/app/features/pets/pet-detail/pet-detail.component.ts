import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { PetService } from '../../../core/services/pet.service';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-pet-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, LoadingSpinnerComponent],
  templateUrl: './pet-detail.component.html',
  styleUrl: './pet-detail.component.scss',
})
export class PetDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private petService = inject(PetService);

  pet: any = null;
  medicalHistory: any[] = [];
  loading = true;
  activeTab: 'info' | 'medical' = 'info';

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadPet(id);
  }

  loadPet(id: number): void {
    this.petService.getWithOwner(id).subscribe((pet) => {
      this.pet = pet;
      this.loading = false;
    });
    this.petService.getMedicalHistory(id).subscribe((details: any) => {
      this.medicalHistory = details.medicalRecords ?? details.medicalHistory ?? [];
    });
  }
}
