import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

/** Prevents unauthenticated users from accessing protected routes */
export const authGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (auth.isAuthenticated) {
    return true;
  }

  router.navigate(['/auth/login'], {
    queryParams: { returnUrl: router.routerState.snapshot.url },
  });
  return false;
};

/** Prevents authenticated users from accessing login/register pages */
export const guestGuard: CanActivateFn = () => {
  const auth = inject(AuthService);
  const router = inject(Router);

  if (!auth.isAuthenticated) {
    return true;
  }

  router.navigate(['/dashboard']);
  return false;
};

/** Requires specific role(s) */
export function roleGuard(...roles: string[]): CanActivateFn {
  return () => {
    const auth = inject(AuthService);
    const router = inject(Router);

    if (auth.isAuthenticated && roles.some((r) => auth.hasRole(r))) {
      return true;
    }

    router.navigate(['/dashboard']);
    return false;
  };
}

/** Requires specific permission(s) */
export function permissionGuard(...permissions: string[]): CanActivateFn {
  return () => {
    const auth = inject(AuthService);
    const router = inject(Router);

    if (auth.isAuthenticated && auth.hasAnyPermission(...permissions)) {
      return true;
    }

    router.navigate(['/dashboard']);
    return false;
  };
}
