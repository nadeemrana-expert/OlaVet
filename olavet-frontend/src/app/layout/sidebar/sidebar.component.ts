import { Component, Input, Output, EventEmitter, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';
import { RoleNames } from '../../core/constants/permissions';

interface NavItem {
  label: string;
  icon: string;
  route: string;
  roles?: string[];
}

@Component({
  selector: 'app-sidebar',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive],
  templateUrl: './sidebar.component.html',
  styleUrl: './sidebar.component.scss',
})
export class SidebarComponent {
  @Input() collapsed = false;
  @Output() toggle = new EventEmitter<void>();

  private auth = inject(AuthService);

  navItems: NavItem[] = [
    { label: 'Dashboard', icon: '📊', route: '/dashboard' },
    { label: 'Appointments', icon: '📅', route: '/appointments',
      roles: [RoleNames.Admin, RoleNames.Vet, RoleNames.PetOwner, RoleNames.LabTechnician] },
    { label: 'Pets', icon: '🐾', route: '/pets',
      roles: [RoleNames.Admin, RoleNames.Vet, RoleNames.PetOwner] },
    { label: 'Veterinarians', icon: '👨‍⚕️', route: '/vets',
      roles: [RoleNames.Admin, RoleNames.PetOwner] },
    { label: 'Pet Owners', icon: '👤', route: '/pet-owners',
      roles: [RoleNames.Admin, RoleNames.Vet] },
    { label: 'Labs', icon: '🔬', route: '/labs',
      roles: [RoleNames.Admin, RoleNames.PetOwner, RoleNames.LabTechnician] },
    { label: 'Pharmacy', icon: '💊', route: '/stores',
      roles: [RoleNames.Admin, RoleNames.PetOwner, RoleNames.StoreManager] },
    { label: 'Reviews', icon: '⭐', route: '/reviews',
      roles: [RoleNames.Admin, RoleNames.Vet, RoleNames.PetOwner] },
    { label: 'Medical Records', icon: '📋', route: '/medical-records',
      roles: [RoleNames.Admin, RoleNames.Vet, RoleNames.PetOwner] },
  ];

  get visibleItems(): NavItem[] {
    return this.navItems.filter((item) => {
      if (!item.roles) return true;
      return item.roles.some((r) => this.auth.hasRole(r));
    });
  }
}
