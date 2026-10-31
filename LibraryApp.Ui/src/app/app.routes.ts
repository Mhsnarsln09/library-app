import { Routes } from '@angular/router';
import { AuthPageComponent } from './pages/auth-page/auth-page.component';
import { PortalPageComponent } from './pages/portal-page/portal-page.component';

export const routes: Routes = [
  { path: 'auth', component: AuthPageComponent },
  { path: 'test', loadChildren: () => import('./modules/test/test.module').then((m) => m.TestModule) },
  { path: '', component: PortalPageComponent },
  { path: '**', redirectTo: '' }
];
