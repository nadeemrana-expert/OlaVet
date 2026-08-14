import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { PetService } from '../../../core/services/pet.service';
import { Pet } from '../../../core/models/pet.model';
import { PagedResult } from '../../../core/models/common.model';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { SearchBoxComponent } from '../../../shared/components/search-box/search-box.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';

@Component({
  selector: 'app-pet-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    PaginationComponent,
    SearchBoxComponent,
    LoadingSpinnerComponent,
    EmptyStateComponent,
  ],
  templateUrl: './pet-list.component.html',
  styleUrl: './pet-list.component.scss',
})
export class PetListComponent implements OnInit {
  private petService = inject(PetService);

  result: PagedResult<Pet> | null = null;
  loading = true;
  currentPage = 1;
  pageSize = 10;
  searchTerm = '';
  speciesFilter = '';

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    if (this.speciesFilter) {
      this.petService.getBySpecies(this.speciesFilter).subscribe({
        next: (pets) => {
          this.result = {
            items: pets,
            totalCount: pets.length,
            page: 1,
            pageSize: pets.length,
            totalPages: 1,
            hasPrevious: false,
            hasNext: false,
          };
          this.loading = false;
        },
        error: () => (this.loading = false),
      });
    } else {
      this.petService.getAll(this.currentPage, this.pageSize).subscribe({
        next: (r) => {
          this.result = r;
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

  onSpeciesFilter(species: string): void {
    this.speciesFilter = species;
    this.currentPage = 1;
    this.load();
  }

  deletePet(id: number): void {
    this.petService.delete(id).subscribe(() => this.load());
  }
}
