import { Routes } from '@angular/router';
import { Login } from './features/auth/login/login';
import { Dashboard } from './features/admin/dashboard/dashboard';
import { Lista } from './features/catalogo/lista/lista';

export const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  { path: 'login', component: Login },
  { path: 'admin', component: Dashboard },
  { path: 'catalogo', component: Lista },
];