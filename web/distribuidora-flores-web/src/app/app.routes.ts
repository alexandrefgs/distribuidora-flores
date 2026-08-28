import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { Registro } from './features/auth/registro/registro';
import { Dashboard } from './features/admin/dashboard/dashboard';
import { Lista } from './features/catalogo/lista/lista';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login },
  { path: 'registro', component: Registro },
  { path: 'admin', component: Dashboard },
  { path: 'catalogo', component: Lista },
];