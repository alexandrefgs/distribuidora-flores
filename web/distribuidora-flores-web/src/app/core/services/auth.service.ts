import { Injectable, signal, computed } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable, tap } from 'rxjs';
import { API_BASE_URL } from '../config/api.config';
import { AuthResponse, LoginRequest, RegistrarComercianteRequest, UsuarioLogado } from '../models/auth.models';

const ACCESS_TOKEN_KEY = 'accessToken';
const REFRESH_TOKEN_KEY = 'refreshToken';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private accessTokenSignal = signal<string | null>(localStorage.getItem(ACCESS_TOKEN_KEY));

  // Deriva o usuário logado automaticamente sempre que o token mudar
  currentUser = computed<UsuarioLogado | null>(() => {
    const token = this.accessTokenSignal();
    return token ? this.decodeToken(token) : null;
  });

  isAuthenticated = computed(() => this.currentUser() !== null);

  constructor(private http: HttpClient) {}

  login(request: LoginRequest): Observable<AuthResponse> {
    return this.http.post<AuthResponse>(`${API_BASE_URL}/auth/login`, request).pipe(
      tap((response) => this.salvarTokens(response))
    );
  }

  registrarComerciante(request: RegistrarComercianteRequest): Observable<{ id: string }> {
    return this.http.post<{ id: string }>(`${API_BASE_URL}/auth/registrar-comerciante`, request);
  }

  refresh(): Observable<AuthResponse> {
    const refreshToken = localStorage.getItem(REFRESH_TOKEN_KEY);
    return this.http.post<AuthResponse>(`${API_BASE_URL}/auth/refresh`, { refreshToken }).pipe(
      tap((response) => this.salvarTokens(response))
    );
  }

  logout(): void {
    localStorage.removeItem(ACCESS_TOKEN_KEY);
    localStorage.removeItem(REFRESH_TOKEN_KEY);
    this.accessTokenSignal.set(null);
  }

  getAccessToken(): string | null {
    return this.accessTokenSignal();
  }

  getRefreshToken(): string | null {
    return localStorage.getItem(REFRESH_TOKEN_KEY);
  }

  private salvarTokens(response: AuthResponse): void {
    localStorage.setItem(ACCESS_TOKEN_KEY, response.accessToken);
    localStorage.setItem(REFRESH_TOKEN_KEY, response.refreshToken);
    this.accessTokenSignal.set(response.accessToken);
  }

  // Decodifica o JWT sem precisar validar assinatura (isso o backend já garante) —
  // só extraímos as claims pra saber quem é o usuário no frontend
  private decodeToken(token: string): UsuarioLogado | null {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));

      return {
        id: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier'],
        email: payload['http://schemas.xmlsoap.org/ws/2005/05/identity/claims/emailaddress'],
        role: payload['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'],
        clienteId: payload['clienteId'],
      };
    } catch {
      return null;
    }
  }
}