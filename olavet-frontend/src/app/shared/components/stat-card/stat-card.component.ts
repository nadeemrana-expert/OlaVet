import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-stat-card',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="stat-card" [style.border-left-color]="color">
      <div class="stat-icon">{{ icon }}</div>
      <div class="stat-content">
        <span class="stat-value">{{ value | number }}</span>
        <span class="stat-label">{{ label }}</span>
      </div>
    </div>
  `,
  styles: [`
    .stat-card {
      display: flex; align-items: center; gap: 1rem;
      background: #fff; border-radius: 12px; padding: 1.25rem 1.5rem;
      box-shadow: 0 1px 3px rgba(0,0,0,0.08); border-left: 4px solid #667eea;
      transition: transform 0.2s;
      &:hover { transform: translateY(-2px); box-shadow: 0 4px 12px rgba(0,0,0,0.1); }
    }
    .stat-icon { font-size: 2rem; }
    .stat-content { display: flex; flex-direction: column; }
    .stat-value { font-size: 1.75rem; font-weight: 700; color: #333; }
    .stat-label { font-size: 0.85rem; color: #888; margin-top: 0.1rem; }
  `],
})
export class StatCardComponent {
  @Input() icon = '📊';
  @Input() label = '';
  @Input() value = 0;
  @Input() color = '#667eea';
}
