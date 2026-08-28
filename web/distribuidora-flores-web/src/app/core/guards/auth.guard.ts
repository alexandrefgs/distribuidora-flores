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

  // Se a rota define quais roles podem acessar, verifica
  const rolesPermitidas = route.data['roles'] as string[] | undefined;
  if (rolesPermitidas && !rolesPermitidas.includes(authService.currentUser()!.role)) {
    router.navigate(['/']); // ou uma página de "acesso negado"
    return false;
  }

  return true;
};