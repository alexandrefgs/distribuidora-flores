import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { Registro } from './features/auth/registro/registro';
import { Dashboard } from './features/admin/dashboard/dashboard';
import { Lista } from './features/catalogo/lista/lista';
import { Pagina } from './features/carrinho/pagina/pagina';
import { Erro } from './shared/erro/erro';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login },
  { path: 'registro', component: Registro },
  {
    path: 'admin',
    component: Dashboard,
    canActivate: [authGuard],
    data: { roles: ['Admin'] },
  },
  {
    path: 'catalogo',
    component: Lista,
    canActivate: [authGuard],
    data: { roles: ['Admin', 'Comerciante'] },
  },
  {
    path: 'carrinho',
    component: Pagina,
    canActivate: [authGuard],
    data: { roles: ['Comerciante'] },
  },
  { path: 'erro/:codigo', component: Erro },
  { path: '**', redirectTo: 'erro/404' },
];