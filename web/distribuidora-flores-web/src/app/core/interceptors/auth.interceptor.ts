import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { catchError, switchMap, throwError } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const authService = inject(AuthService);
  const accessToken = authService.getAccessToken();

  // Anexa o access token em toda requisição, se existir
  const requestComToken = accessToken
    ? req.clone({ setHeaders: { Authorization: `Bearer ${accessToken}` } })
    : req;

  return next(requestComToken).pipe(
    catchError((error: HttpErrorResponse) => {
      // Se não for 401, ou se a própria chamada de refresh falhou, propaga o erro normalmente
      if (error.status !== 401 || req.url.includes('/auth/refresh')) {
        return throwError(() => error);
      }

      // Access token expirou: tenta renovar automaticamente e repetir a requisição original
      return authService.refresh().pipe(
        switchMap(() => {
          const novoToken = authService.getAccessToken();
          const requestRenovada = req.clone({
            setHeaders: { Authorization: `Bearer ${novoToken}` },
          });
          return next(requestRenovada);
        }),
        catchError((refreshError) => {
          // Refresh token também inválido/expirado: desloga de vez
          authService.logout();
          return throwError(() => refreshError);
        })
      );
    })
  );
};