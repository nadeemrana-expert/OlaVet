import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { AppointmentService } from '../../../core/services/appointment.service';
import { VetAppointment } from '../../../core/models/appointment.model';
import { PagedResult } from '../../../core/models/common.model';
import { PaginationComponent } from '../../../shared/components/pagination/pagination.component';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';
import { EmptyStateComponent } from '../../../shared/components/empty-state/empty-state.component';
import { StatusBadgePipe } from '../../../shared/pipes/status-badge.pipe';

@Component({
  selector: 'app-appointment-list',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    PaginationComponent,
    LoadingSpinnerComponent,
    EmptyStateComponent,
    StatusBadgePipe,
  ],
  templateUrl: './appointment-list.component.html',
  styleUrl: './appointment-list.component.scss',
})
export class AppointmentListComponent implements OnInit {
  private appointmentService = inject(AppointmentService);

  result: PagedResult<any> | null = null;
  loading = true;
  currentPage = 1;
  pageSize = 10;
  activeTab: 'vet' | 'lab' = 'vet';

  ngOnInit(): void {
    this.load();
  }

  load(): void {
    this.loading = true;
    if (this.activeTab === 'vet') {
      this.appointmentService
        .getVetAppointments(this.currentPage, this.pageSize)
        .subscribe({
          next: (r) => {
            this.result = r;
            this.loading = false;
          },
          error: () => (this.loading = false),
        });
    } else {
      this.appointmentService
        .getLabAppointments(this.currentPage, this.pageSize)
        .subscribe({
          next: (r: any) => {
            this.result = r;
            this.loading = false;
          },
          error: () => (this.loading = false),
        });
    }
  }

  switchTab(tab: 'vet' | 'lab'): void {
    this.activeTab = tab;
    this.currentPage = 1;
    this.load();
  }

  onPageChange(page: number): void {
    this.currentPage = page;
    this.load();
  }
}
