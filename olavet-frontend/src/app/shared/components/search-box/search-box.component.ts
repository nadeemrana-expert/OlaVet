import { Component, Input, Output, EventEmitter, OnInit, OnDestroy } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormControl, ReactiveFormsModule } from '@angular/forms';
import { debounceTime, distinctUntilChanged, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-search-box',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  template: `
    <div class="search-box">
      <span class="search-icon">🔍</span>
      <input
        [formControl]="searchControl"
        [placeholder]="placeholder"
        type="text"
      />
      @if (searchControl.value) {
        <button class="clear-btn" (click)="clear()">✕</button>
      }
    </div>
  `,
  styles: [`
    .search-box {
      position: relative; display: flex; align-items: center;
      background: #fff; border: 2px solid #e2e8f0; border-radius: 10px;
      transition: border-color 0.2s;
      &:focus-within { border-color: #667eea; }
    }
    .search-icon { padding: 0 0.75rem; font-size: 1rem; }
    input {
      flex: 1; padding: 0.65rem 0; border: none; outline: none;
      font-size: 0.95rem; background: transparent;
    }
    .clear-btn {
      background: none; border: none; padding: 0 0.75rem;
      cursor: pointer; color: #999; font-size: 0.9rem;
    }
  `],
})
export class SearchBoxComponent implements OnInit, OnDestroy {
  @Input() placeholder = 'Search...';
  @Input() debounce = 400;
  @Output() searchChange = new EventEmitter<string>();

  searchControl = new FormControl('');
  private destroy$ = new Subject<void>();

  ngOnInit(): void {
    this.searchControl.valueChanges
      .pipe(debounceTime(this.debounce), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe((value) => this.searchChange.emit(value ?? ''));
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  clear(): void {
    this.searchControl.setValue('');
  }
}
