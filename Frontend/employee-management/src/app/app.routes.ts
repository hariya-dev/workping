// app.routes.ts
// Cấu hình routing chính của ứng dụng

import { Routes } from '@angular/router';
import { authGuard } from './core/guards/auth.guard';

export const routes: Routes = [
  // Redirect root to dashboard
  {
    path: '',
    redirectTo: 'dashboard',
    pathMatch: 'full'
  },

  // Login - không cần auth
  {
    path: 'login',
    loadComponent: () => import('./features/auth/login/login.component').then(m => m.LoginComponent)
  },

  // Protected routes - cần auth
  {
    path: '',
    loadComponent: () => import('./layouts/main-layout/main-layout.component').then(m => m.MainLayoutComponent),
    canActivate: [authGuard],
    children: [
      // Dashboard
      {
        path: 'dashboard',
        loadComponent: () => import('./features/dashboard/dashboard.component').then(m => m.DashboardComponent)
      },

      // Employees
      {
        path: 'employees',
        children: [
          {
            path: '',
            loadComponent: () => import('./features/employees/employee-list/employee-list.component').then(m => m.EmployeeListComponent)
          },
          {
            path: 'new',
            loadComponent: () => import('./features/employees/employee-form/employee-form.component').then(m => m.EmployeeFormComponent)
          },
          {
            path: ':id',
            loadComponent: () => import('./features/employees/employee-detail/employee-detail.component').then(m => m.EmployeeDetailComponent)
          },
          {
            path: ':id/edit',
            loadComponent: () => import('./features/employees/employee-form/employee-form.component').then(m => m.EmployeeFormComponent)
          }
        ]
      },

      // Contract Types
      {
        path: 'contract-types',
        loadComponent: () => import('./features/contract-types/contract-type-list/contract-type-list.component').then(m => m.ContractTypeListComponent)
      },

      // Settings
      {
        path: 'settings',
        loadComponent: () => import('./features/settings/settings.component').then(m => m.SettingsComponent)
      },

      // Email Templates
      {
        path: 'email-templates',
        loadComponent: () => import('./features/email-templates/email-templates.component').then(m => m.EmailTemplatesComponent)
      }
    ]
  },

  // Wildcard - redirect to dashboard
  {
    path: '**',
    redirectTo: 'dashboard'
  }
];
