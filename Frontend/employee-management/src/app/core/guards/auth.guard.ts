// core/guards/auth.guard.ts
// Guard bảo vệ các routes cần xác thực

import { inject } from '@angular/core';
import { Router, CanActivateFn } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const authGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated()) {
    return true;
  }

  // Chuyển hướng đến trang login nếu chưa đăng nhập
  router.navigate(['/login']);
  return false;
};

// Guard chỉ cho phép Admin
export const adminGuard: CanActivateFn = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.isAuthenticated() && authService.isAdmin()) {
    return true;
  }

  // Chuyển hướng về dashboard nếu không phải admin
  router.navigate(['/dashboard']);
  return false;
};
