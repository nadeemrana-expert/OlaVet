import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { PetOwnerService } from '../../../core/services/pet-owner.service';
import { PetOwner } from '../../../core/models/pet-owner.model';
import { PagedResult } from '../../../core/models/common.model';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { SearchBoxComponent } from '../../../shared/components/search-box/search-box.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';

@Component({
  selector: 'app-pet-owner-list',
  standalone: true,
  imports: [CommonModule, RouterLink, PaginationComponent, SearchBoxComponent, LoadingSpinnerComponent, EmptyStateComponent],
  template: `
    <div class="page-header">
      <div><h1>Pet Owners</h1><p class="subtitle">Manage registered pet owners</p></div>
      <a routerLink="new" class="btn btn-primary">+ Add Owner</a>
    </div>
    <div class="toolbar"><app-search-box placeholder="Search owners…" (searchChange)="onSearch($event)" /></div>
    @if (loading) { <app-loading-spinner message="Loading owners…" /> }
    @else if (!result || result.items.length === 0) { <app-empty-state icon="👤" title="No owners found" /> }
    @else {
      <div class="table-card"><table>
        <thead><tr><th>Name</th><th>Email</th><th>Phone</th><th>Balance</th><th></th></tr></thead>
        <tbody>
          @for (o of result.items; track o.petOwnerId) {
            <tr>
              <td><a [routerLink]="[o.petOwnerId]">{{ o.ownerName }}</a></td>
              <td>{{ o.email }}</td>
              <td>{{ o.contactNumber }}</td>
              <td>{{ o.wallet | currency }}</td>
              <td><a [routerLink]="[o.petOwnerId]" class="btn-sm">View</a></td>
            </tr>
          }
        </tbody>
      </table></div>
      <app-pagination [currentPage]="result.page" [totalPages]="result.totalPages" [totalCount]="result.totalCount" (pageChange)="onPageChange($event)" />
    }
  `,
  styles: [`
    .page-header { display:flex; justify-content:space-between; align-items:flex-start; margin-bottom:1.5rem; h1 { margin:0; font-size:1.6rem; } .subtitle { color:#888; font-size:0.9rem; } }
    .toolbar { margin-bottom:1.25rem; }
    .btn { padding:0.55rem 1.25rem; border:none; border-radius:8px; font-weight:600; font-size:0.9rem; cursor:pointer; text-decoration:none; &-primary { background:#667eea; color:#fff; } }
    .table-card { background:#fff; border-radius:12px; overflow:hidden; box-shadow:0 2px 8px rgba(0,0,0,0.04); }
    table { width:100%; border-collapse:collapse; }
    th { text-align:left; padding:0.75rem 1rem; font-size:0.85rem; color:#888; background:#fafbfc; border-bottom:1px solid #eee; }
    td { padding:0.75rem 1rem; font-size:0.9rem; color:#333; border-bottom:1px solid #f7f7f7; a { color:#667eea; text-decoration:none; font-weight:500; } }
    tr:hover td { background:#fafbfe; }
    .btn-sm { padding:0.3rem 0.8rem; background:#f0f4ff; color:#667eea; border-radius:6px; font-size:0.8rem; text-decoration:none; }
  `],
})
export class PetOwnerListComponent implements OnInit {
  private ownerService = inject(PetOwnerService);
  result: PagedResult<PetOwner> | null = null;
  loading = true;
  currentPage = 1;
  pageSize = 10;

  ngOnInit(): void { this.load(); }

  load(): void {
    this.loading = true;
    this.ownerService.getAll(this.currentPage, this.pageSize).subscribe({
      next: (r) => { this.result = r; this.loading = false; },
      error: () => (this.loading = false),
    });
  }

  onSearch(term: string): void {
    this.loading = true;
    this.ownerService.search(term).subscribe({
      next: (owners) => { this.result = { items: owners, totalCount: owners.length, page: 1, pageSize: owners.length, totalPages: 1, hasPrevious: false, hasNext: false }; this.loading = false; },
      error: () => (this.loading = false),
    });
  }

  onPageChange(page: number): void { this.currentPage = page; this.load(); }
}
