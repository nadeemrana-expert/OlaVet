import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-confirm-dialog',
  standalone: true,
  imports: [CommonModule],
  template: `
    @if (isOpen) {
      <div class="backdrop" (click)="onCancel()">
        <div class="dialog" (click)="$event.stopPropagation()">
          <h3>{{ title }}</h3>
          <p>{{ message }}</p>
          <div class="dialog-actions">
            <button class="btn-cancel" (click)="onCancel()">Cancel</button>
            <button class="btn-confirm" [class]="'btn-' + type" (click)="onConfirm()">{{ confirmText }}</button>
          </div>
        </div>
      </div>
    }
  `,
  styles: [`
    .backdrop {
      position: fixed; inset: 0; background: rgba(0,0,0,0.5); z-index: 10000;
      display: flex; align-items: center; justify-content: center;
    }
    .dialog {
      background: #fff; border-radius: 12px; padding: 1.75rem; max-width: 400px; width: 90%;
      box-shadow: 0 20px 60px rgba(0,0,0,0.2);
      h3 { margin: 0 0 0.5rem; } p { color: #666; margin: 0 0 1.5rem; font-size: 0.9rem; }
    }
    .dialog-actions { display: flex; gap: 0.75rem; justify-content: flex-end; }
    .btn-cancel {
      padding: 0.5rem 1.25rem; border: 1px solid #e2e8f0; border-radius: 8px;
      background: #fff; cursor: pointer;
    }
    .btn-confirm {
      padding: 0.5rem 1.25rem; border: none; border-radius: 8px; color: #fff; cursor: pointer;
    }
    .btn-danger { background: #e53e3e; }
    .btn-warning { background: #dd6b20; }
    .btn-primary { background: #667eea; }
  `],
})
export class ConfirmDialogComponent {
  @Input() isOpen = false;
  @Input() title = 'Confirm';
  @Input() message = 'Are you sure?';
  @Input() confirmText = 'Confirm';
  @Input() type: 'danger' | 'warning' | 'primary' = 'danger';
  @Output() confirmed = new EventEmitter<void>();
  @Output() cancelled = new EventEmitter<void>();

  onConfirm(): void { this.confirmed.emit(); }
  onCancel(): void { this.cancelled.emit(); }
}
