import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-empty-state',
  standalone: true,
  imports: [CommonModule],
  template: `
    <div class="empty-state">
      <span class="empty-icon">{{ icon }}</span>
      <h3>{{ title }}</h3>
      @if (message) { <p>{{ message }}</p> }
      <ng-content></ng-content>
    </div>
  `,
  styles: [`
    .empty-state {
      text-align: center; padding: 3rem 1.5rem; color: #888;
      .empty-icon { font-size: 3.5rem; display: block; margin-bottom: 1rem; }
      h3 { font-size: 1.15rem; color: #555; margin: 0 0 0.5rem; }
      p { font-size: 0.9rem; margin: 0; }
    }
  `],
})
export class EmptyStateComponent {
  @Input() icon = '📭';
  @Input() title = 'No data found';
  @Input() message = '';
}
