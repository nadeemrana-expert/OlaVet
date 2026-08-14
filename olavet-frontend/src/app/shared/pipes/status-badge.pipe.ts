import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'statusBadge', standalone: true })
export class StatusBadgePipe implements PipeTransform {
  transform(status: string): string {
    const map: Record<string, string> = {
      Scheduled: 'badge-info',
      Confirmed: 'badge-primary',
      'In Progress': 'badge-warning',
      Completed: 'badge-success',
      Cancelled: 'badge-danger',
      Pending: 'badge-warning',
      Delivered: 'badge-success',
      Processing: 'badge-info',
    };
    return map[status] ?? 'badge-default';
  }
}
