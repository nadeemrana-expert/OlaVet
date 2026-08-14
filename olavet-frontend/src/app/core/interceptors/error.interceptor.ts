import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, throwError } from 'rxjs';
import { NotificationService } from '../services/notification.service';

/** Global HTTP error handler — shows toast notifications */
export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const notify = inject(NotificationService);

  return next(req).pipe(
    catchError((error) => {
      const status = error.status;
      const body = error.error;
      const message = body?.error || body?.message || 'An unexpected error occurred';

      switch (status) {
        case 0:
          notify.error('Network Error', 'Unable to connect to the server.');
          break;
        case 400:
          notify.warning('Bad Request', message);
          break;
        case 403:
          notify.error('Forbidden', 'You do not have permission to perform this action.');
          break;
        case 404:
          notify.warning('Not Found', message);
          break;
        case 429:
          notify.warning('Rate Limited', 'Too many requests. Please wait and try again.');
          break;
        case 500:
          notify.error('Server Error', 'Something went wrong on the server.');
          break;
        default:
          if (status !== 401) {
            notify.error('Error', message);
          }
      }

      return throwError(() => error);
    })
  );
};
