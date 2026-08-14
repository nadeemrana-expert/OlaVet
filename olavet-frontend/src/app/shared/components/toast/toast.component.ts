import { Component, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { NotificationService, ToastMessage } from '../../../core/services/notification.service';

@Component({
  selector: 'app-toast',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="toast-container">
      @for (toast of (notify.toasts$ | async); track toast.id) {
        <div class="toast" [class]="'toast-' + toast.type" (click)="notify.dismiss(toast.id)">
          <span class="toast-icon">{{ getIcon(toast.type) }}</span>
          <div class="toast-body">
            <strong>{{ toast.title }}</strong>
            @if (toast.message) { <p>{{ toast.message }}</p> }
          </div>
          <button class="toast-close">✕</button>
        </div>
      }
    </div>
  `,
  styles: [`
    .toast-container {
      position: fixed; top: 1rem; right: 1rem; z-index: 9999;
      display: flex; flex-direction: column; gap: 0.5rem; max-width: 380px;
    }
    .toast {
      display: flex; align-items: flex-start; gap: 0.75rem;
      padding: 0.85rem 1rem; border-radius: 10px; cursor: pointer;
      box-shadow: 0 4px 16px rgba(0,0,0,0.15); animation: slideIn 0.3s ease;
      color: #fff;
    }
    .toast-success { background: #38a169; }
    .toast-error   { background: #e53e3e; }
    .toast-warning { background: #dd6b20; }
    .toast-info    { background: #3182ce; }
    .toast-icon { font-size: 1.25rem; }
    .toast-body { flex: 1; strong { display: block; } p { margin: 0.2rem 0 0; font-size: 0.85rem; opacity: 0.9; } }
    .toast-close { background: none; border: none; color: #fff; cursor: pointer; opacity: 0.7; font-size: 0.85rem; }
    @keyframes slideIn { from { transform: translateX(100%); opacity: 0; } to { transform: translateX(0); opacity: 1; } }
  `],
})
export class ToastComponent {
  notify = inject(NotificationService);

  getIcon(type: string): string {
    const icons: Record<string, string> = { success: '✅', error: '❌', warning: '⚠️', info: 'ℹ️' };
    return icons[type] ?? 'ℹ️';
  }
}
