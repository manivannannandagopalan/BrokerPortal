import { Routes } from '@angular/router';

export const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'home' },
  { path: 'home', loadComponent: () => import('./home/home.component').then((module) => module.HomeComponent) },
  { path: '**', redirectTo: 'home' }
];