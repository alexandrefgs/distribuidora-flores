import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = (route) => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (!authService.isAuthenticated()) {
    router.navigate(['/login']);
    return false;
  }

  const rolesPermitidas = route.data['roles'] as string[] | undefined;
  if (rolesPermitidas && !rolesPermitidas.includes(authService.currentUser()!.role)) {
    router.navigate(['/erro/403']);
    return false;
  }

  return true;
};