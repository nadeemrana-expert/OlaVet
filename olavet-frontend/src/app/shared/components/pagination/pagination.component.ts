import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-pagination',
  standalone: true,
  imports: [CommonModule],
  template: `
    <nav class="pagination" *ngIf="totalPages > 1">
      <button (click)="onPageChange(currentPage - 1)" [disabled]="currentPage === 1" class="page-btn">
        ← Prev
      </button>
      @for (p of pages; track p) {
        <button
          (click)="onPageChange(p)"
          [class.active]="p === currentPage"
          class="page-btn page-number"
        >{{ p }}</button>
      }
      <button (click)="onPageChange(currentPage + 1)" [disabled]="currentPage === totalPages" class="page-btn">
        Next →
      </button>
      <span class="page-info">Page {{ currentPage }} of {{ totalPages }} ({{ totalCount }} items)</span>
    </nav>
  `,
  styles: [`
    .pagination { display: flex; align-items: center; gap: 0.25rem; flex-wrap: wrap; margin-top: 1rem; }
    .page-btn {
      padding: 0.4rem 0.75rem; border: 1px solid #e2e8f0; border-radius: 6px;
      background: #fff; cursor: pointer; font-size: 0.85rem; transition: all 0.2s;
      &:hover:not(:disabled):not(.active) { background: #f7fafc; border-color: #667eea; }
      &:disabled { opacity: 0.4; cursor: not-allowed; }
      &.active { background: #667eea; color: #fff; border-color: #667eea; }
    }
    .page-info { margin-left: auto; font-size: 0.8rem; color: #888; }
  `],
})
export class PaginationComponent {
  @Input() currentPage = 1;
  @Input() totalPages = 1;
  @Input() totalCount = 0;
  @Output() pageChange = new EventEmitter<number>();

  get pages(): number[] {
    const delta = 2;
    const range: number[] = [];
    for (let i = Math.max(1, this.currentPage - delta); i <= Math.min(this.totalPages, this.currentPage + delta); i++) {
      range.push(i);
    }
    return range;
  }

  onPageChange(page: number): void {
    if (page >= 1 && page <= this.totalPages && page !== this.currentPage) {
      this.pageChange.emit(page);
    }
  }
}
