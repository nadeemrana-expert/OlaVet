import { Component, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-header',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <header class="app-header">
      <button class="menu-toggle" (click)="toggleSidebar.emit()">☰</button>
      <div class="header-spacer"></div>
      <div class="header-actions">
        <div class="user-menu" (click)="showDropdown = !showDropdown">
          <div class="avatar">{{ userInitials }}</div>
          <div class="user-info">
            <span class="user-name">{{ auth.currentUser?.firstName }} {{ auth.currentUser?.lastName }}</span>
            <span class="user-role">{{ primaryRole }}</span>
          </div>
          <span class="dropdown-arrow">▾</span>
        </div>

        @if (showDropdown) {
          <div class="dropdown-menu" (click)="showDropdown = false">
            <a routerLink="/profile" class="dropdown-item">👤 Profile</a>
            <a routerLink="/settings" class="dropdown-item">⚙️ Settings</a>
            <hr />
            <button class="dropdown-item logout" (click)="auth.logout()">🚪 Logout</button>
          </div>
        }
      </div>
    </header>
  `,
  styles: [`
    .app-header {
      display: flex; align-items: center; gap: 1rem;
      padding: 0 1.5rem; height: 64px; background: #fff;
      box-shadow: 0 1px 3px rgba(0,0,0,0.06); position: sticky; top: 0; z-index: 50;
    }
    .menu-toggle {
      background: none; border: none; font-size: 1.4rem; cursor: pointer; padding: 0.25rem;
      display: none;
      @media (max-width: 768px) { display: block; }
    }
    .header-spacer { flex: 1; }
    .header-actions { position: relative; }
    .user-menu {
      display: flex; align-items: center; gap: 0.6rem; cursor: pointer;
      padding: 0.4rem 0.6rem; border-radius: 8px; transition: background 0.2s;
      &:hover { background: #f7fafc; }
    }
    .avatar {
      width: 36px; height: 36px; border-radius: 50%; background: #667eea; color: #fff;
      display: flex; align-items: center; justify-content: center;
      font-weight: 700; font-size: 0.85rem;
    }
    .user-info { display: flex; flex-direction: column; }
    .user-name { font-weight: 600; font-size: 0.9rem; color: #333; }
    .user-role { font-size: 0.75rem; color: #888; }
    .dropdown-arrow { font-size: 0.8rem; color: #999; }
    .dropdown-menu {
      position: absolute; right: 0; top: 100%; margin-top: 0.25rem;
      background: #fff; border-radius: 10px; box-shadow: 0 8px 30px rgba(0,0,0,0.12);
      min-width: 180px; padding: 0.5rem 0; z-index: 100;
      hr { margin: 0.25rem 0; border: none; border-top: 1px solid #eee; }
    }
    .dropdown-item {
      display: block; padding: 0.6rem 1rem; font-size: 0.9rem; color: #333;
      text-decoration: none; cursor: pointer; border: none; background: none;
      width: 100%; text-align: left;
      &:hover { background: #f7fafc; }
      &.logout { color: #e53e3e; }
    }
  `],
})
export class HeaderComponent {
  @Output() toggleSidebar = new EventEmitter<void>();
  auth = inject(AuthService);
  showDropdown = false;

  get userInitials(): string {
    const u = this.auth.currentUser;
    if (!u) return '?';
    return `${u.firstName?.[0] ?? ''}${u.lastName?.[0] ?? ''}`.toUpperCase();
  }

  get primaryRole(): string {
    return this.auth.roles[0] ?? 'User';
  }
}
