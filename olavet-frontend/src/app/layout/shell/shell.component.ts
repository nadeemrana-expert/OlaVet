import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterOutlet } from '@angular/router';
import { SidebarComponent } from '../sidebar/sidebar.component';
import { HeaderComponent } from '../header/header.component';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [CommonModule, RouterOutlet, SidebarComponent, HeaderComponent],
  template: `
    <div class="app-shell" [class.sidebar-collapsed]="sidebarCollapsed">
      <app-sidebar [collapsed]="sidebarCollapsed" (toggle)="sidebarCollapsed = !sidebarCollapsed" />
      <div class="main-area">
        <app-header (toggleSidebar)="sidebarCollapsed = !sidebarCollapsed" />
        <main class="content">
          <router-outlet />
        </main>
      </div>
    </div>
  `,
  styles: [`
    .app-shell {
      display: flex; min-height: 100vh; background: #f5f7fb;
    }
    .main-area {
      flex: 1; display: flex; flex-direction: column;
      margin-left: 260px; transition: margin-left 0.3s ease;
    }
    .sidebar-collapsed .main-area { margin-left: 72px; }
    .content { flex: 1; padding: 1.5rem 2rem; overflow-y: auto; }

    @media (max-width: 768px) {
      .main-area { margin-left: 0 !important; }
    }
  `],
})
export class ShellComponent {
  sidebarCollapsed = false;
}
