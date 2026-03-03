import { Routes } from '@angular/router';
import { AuthPageComponent } from './pages/auth-page.component';
import { PortalPageComponent } from './pages/portal-page.component';

export const routes: Routes = [
  { path: 'auth', component: AuthPageComponent },
  { path: '', component: PortalPageComponent },
  { path: '**', redirectTo: '' }
];
