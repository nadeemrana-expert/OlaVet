import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { VetService } from '../../../core/services/vet.service';
import { VetWithRating } from '../../../core/models/vet.model';
import { PagedResult } from '../../../core/models/common.model';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { SearchBoxComponent } from '../../../shared/components/search-box/search-box.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';

@Component({
  selector: 'app-vet-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    PaginationComponent,
    SearchBoxComponent,
    LoadingSpinnerComponent,
    EmptyStateComponent,
  ],
  templateUrl: './vet-list.component.html',
  styleUrl: './vet-list.component.scss',
})
export class VetListComponent implements OnInit {
  private vetService = inject(VetService);

  vets: VetWithRating[] = [];
  result: PagedResult<VetWithRating> | null = null;
  loading = true;
  currentPage = 1;
  pageSize = 10;
  searchTerm = '';
  viewMode: 'grid' | 'table' = 'grid';

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    if (this.searchTerm) {
      this.vetService.search(this.searchTerm).subscribe({
        next: (vets) => {
          this.vets = vets as VetWithRating[];
          this.result = null;
          this.loading = false;
        },
        error: () => (this.loading = false),
      });
    } else {
      this.vetService.getWithRatings().subscribe({
        next: (vets) => {
          this.vets = vets;
          this.result = {
            items: vets,
            totalCount: vets.length,
            page: 1,
            pageSize: vets.length,
            totalPages: 1,
            hasPrevious: false,
            hasNext: false,
          };
          this.loading = false;
        },
        error: () => (this.loading = false),
      });
    }
  }

  onSearch(term: string): void {
    this.searchTerm = term;
    this.currentPage = 1;
    this.load();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.load();
  }

  renderStars(rating: number): string {
    return '★'.repeat(Math.round(rating)) + '☆'.repeat(5 - Math.round(rating));
  }
}
