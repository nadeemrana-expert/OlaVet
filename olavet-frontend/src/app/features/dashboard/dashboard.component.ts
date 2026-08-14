import { Component, OnInit, inject } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { BaseChartDirective } from 'ng2-charts';
import { ChartConfiguration, ChartData } from 'chart.js';

import { DashboardService } from '../../core/services/dashboard.service';
import { AuthService } from '../../core/services/auth.service';
import { StatCardComponent } from '../../shared/components/stat-card/stat-card.component';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';
import {
  DashboardStats,
  AppointmentStats,
  RevenueTrend,
  SpeciesDistribution,
  TopPerformers,
  PaymentStats,
} from '../../core/models/dashboard.model';
import { Review } from '../../core/models/review.model';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    CommonModule,
    RouterLink,
    BaseChartDirective,
    StatCardComponent,
    LoadingSpinnerComponent,
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss',
})
export class DashboardComponent implements OnInit {
  private dashboardService = inject(DashboardService);
  auth = inject(AuthService);

  loading = true;
  role = '';
  stats: DashboardStats | null = null;
  appointmentStats: AppointmentStats | null = null;
  paymentStats: PaymentStats | null = null;
  topPerformers: TopPerformers | null = null;
  speciesDistribution: SpeciesDistribution[] = [];
  revenueTrend: RevenueTrend | null = null;
  recentReviews: Review[] = [];

  // Revenue trend chart
  revenueChartData: ChartData<'line'> = { labels: [], datasets: [] };
  revenueChartOptions: ChartConfiguration<'line'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: { legend: { display: true } },
    scales: {
      y: { beginAtZero: true, ticks: { callback: (v) => `$${v}` } },
    },
  };

  // Species distribution chart
  speciesChartData: ChartData<'doughnut'> = { labels: [], datasets: [] };
  speciesChartOptions: ChartConfiguration<'doughnut'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: {
      legend: { position: 'right' },
    },
  };

  // Appointment status chart
  appointmentChartData: ChartData<'bar'> = { labels: [], datasets: [] };
  appointmentChartOptions: ChartConfiguration<'bar'>['options'] = {
    responsive: true,
    maintainAspectRatio: false,
    plugins: { legend: { display: false } },
  };

  ngOnInit(): void {
    this.loadDashboard();
  }

  loadDashboard(): void {
    this.loading = true;
    this.loadedCount = 0;
    this.role = this.auth.roles[0] ?? '';

    // Stats and reviews — all roles
    this.expectedLoads = 2;

    this.dashboardService.getStats().subscribe({
      next: (r) => { this.stats = r; this.checkLoading(); },
      error: () => this.checkLoading(),
    });

    this.dashboardService.getRecentReviews().subscribe({
      next: (r) => { this.recentReviews = r; this.checkLoading(); },
      error: () => this.checkLoading(),
    });

    // Appointment stats — Admin, Vet, PetOwner
    if (['Admin', 'Vet', 'PetOwner'].includes(this.role)) {
      this.expectedLoads++;
      this.dashboardService.getAppointmentStats().subscribe({
        next: (r) => { this.appointmentStats = r; this.buildAppointmentChart(r); this.checkLoading(); },
        error: () => this.checkLoading(),
      });
    }

    // Admin-only endpoints
    if (this.role === 'Admin') {
      this.expectedLoads += 3;
      this.dashboardService.getPaymentStats().subscribe({
        next: (r) => { this.paymentStats = r; this.checkLoading(); },
        error: () => this.checkLoading(),
      });
      this.dashboardService.getTopPerformers().subscribe({
        next: (r) => { this.topPerformers = r; this.checkLoading(); },
        error: () => this.checkLoading(),
      });
      this.dashboardService.getRevenueTrend().subscribe({
        next: (r) => { this.revenueTrend = r; this.buildRevenueChart(r); this.checkLoading(); },
        error: () => this.checkLoading(),
      });
    }

    // Species — Admin and Vet
    if (['Admin', 'Vet'].includes(this.role)) {
      this.expectedLoads++;
      this.dashboardService.getSpeciesDistribution().subscribe({
        next: (r) => { this.speciesDistribution = r; this.buildSpeciesChart(r); this.checkLoading(); },
        error: () => this.checkLoading(),
      });
    }
  }

  private loadedCount = 0;
  private expectedLoads = 0;
  private checkLoading(): void {
    this.loadedCount++;
    if (this.loadedCount >= this.expectedLoads) this.loading = false;
  }

  private buildRevenueChart(data: RevenueTrend): void {
    const vetRev = data.vetRevenue ?? [];
    this.revenueChartData = {
      labels: vetRev.map((d) => new Date(d.date).toLocaleDateString('en', { month: 'short', day: 'numeric' })),
      datasets: [
        {
          label: 'Vet Revenue',
          data: vetRev.map((d) => d.amount),
          borderColor: '#667eea',
          backgroundColor: 'rgba(102, 126, 234, 0.1)',
          fill: true,
          tension: 0.35,
        },
        {
          label: 'Lab Revenue',
          data: (data.labRevenue ?? []).map((d) => d.amount),
          borderColor: '#764ba2',
          backgroundColor: 'rgba(118, 75, 162, 0.1)',
          fill: true,
          tension: 0.35,
        },
        {
          label: 'Store Revenue',
          data: (data.storeRevenue ?? []).map((d) => d.amount),
          borderColor: '#48bb78',
          backgroundColor: 'rgba(72, 187, 120, 0.1)',
          fill: true,
          tension: 0.35,
        },
      ],
    };
  }

  private buildSpeciesChart(data: SpeciesDistribution[]): void {
    this.speciesChartData = {
      labels: data.map((d) => d.species),
      datasets: [
        {
          data: data.map((d) => d.count),
          backgroundColor: [
            '#667eea', '#764ba2', '#f093fb', '#5ee7df',
            '#fbc2eb', '#a8edea', '#fdcb6e', '#e17055',
          ],
        },
      ],
    };
  }

  private buildAppointmentChart(data: AppointmentStats): void {
    const vet = data.vetAppointments;
    const statuses = ['Scheduled', 'Completed', 'Cancelled'];
    const counts = [vet.scheduled, vet.completed, vet.cancelled];
    this.appointmentChartData = {
      labels: statuses,
      datasets: [
        {
          label: 'Vet Appointments',
          data: counts,
          backgroundColor: ['#667eea', '#48bb78', '#e53e3e'],
        },
      ],
    };
  }
}
