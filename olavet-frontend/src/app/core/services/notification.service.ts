import { Injectable, NgZone } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

export interface ToastMessage {
  id: number;
  type: 'success' | 'error' | 'warning' | 'info';
  title: string;
  message: string;
  duration?: number;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  private counter = 0;
  private toastsSubject = new BehaviorSubject<ToastMessage[]>([]);
  toasts$: Observable<ToastMessage[]> = this.toastsSubject.asObservable();

  constructor(private zone: NgZone) {}

  success(title: string, message = ''): void {
    this.show({ type: 'success', title, message });
  }

  error(title: string, message = ''): void {
    this.show({ type: 'error', title, message, duration: 8000 });
  }

  warning(title: string, message = ''): void {
    this.show({ type: 'warning', title, message });
  }

  info(title: string, message = ''): void {
    this.show({ type: 'info', title, message });
  }

  dismiss(id: number): void {
    this.toastsSubject.next(this.toastsSubject.value.filter((t) => t.id !== id));
  }

  private show(toast: Omit<ToastMessage, 'id'>): void {
    const id = ++this.counter;
    const duration = toast.duration ?? 5000;
    const newToast: ToastMessage = { ...toast, id };

    this.toastsSubject.next([...this.toastsSubject.value, newToast]);

    this.zone.runOutsideAngular(() => {
      setTimeout(() => {
        this.zone.run(() => this.dismiss(id));
      }, duration);
    });
  }
}
