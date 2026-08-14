import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ActivatedRoute, RouterLink } from '@angular/router';
import { VetService } from '../../../core/services/vet.service';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { TimeAgoPipe } from '../../../shared/pipes/time-ago.pipe';

@Component({
  selector: 'app-vet-detail',
  standalone: true,
  imports: [CommonModule, RouterLink, LoadingSpinnerComponent, TimeAgoPipe],
  templateUrl: './vet-detail.component.html',
  styleUrl: './vet-detail.component.scss',
})
export class VetDetailComponent implements OnInit {
  private route = inject(ActivatedRoute);
  private vetService = inject(VetService);

  vet: any = null;
  reviews: any[] = [];
  ratingDistribution: any = null;
  loading = true;
  activeTab: 'info' | 'reviews' | 'schedule' = 'info';

  ngOnInit(): void {
    const id = Number(this.route.snapshot.paramMap.get('id'));
    this.loadVet(id);
  }

  loadVet(id: number): void {
    this.vetService.getDetails(id).subscribe((vet) => {
      this.vet = vet;
      this.loading = false;
    });
    this.vetService.getReviews(id).subscribe((r) => (this.reviews = r));
    this.vetService.getRatingDistribution(id).subscribe((r) => (this.ratingDistribution = r));
  }

  renderStars(n: number): string {
    return '★'.repeat(Math.round(n)) + '☆'.repeat(5 - Math.round(n));
  }
}
