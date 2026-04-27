// features/auth/login/login.component.ts
// Component đăng nhập

import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { LoginDto } from '../../../core/models';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="min-h-screen flex items-center justify-center bg-gray-100 py-12 px-4 sm:px-6 lg:px-8">
      <div class="max-w-md w-full">
        <!-- Logo -->
        <div class="text-center mb-8">
          <div class="w-32 h-32 bg-white rounded-2xl shadow-lg mx-auto flex items-center justify-center mb-4 p-2">
            <img
              src="https://atpro.com.vn/wp-content/uploads/2025/12/logo-tet-2026.png"
              alt="An Tường Technology"
              class="w-full h-full object-contain"
              onerror="this.style.display='none'; this.nextElementSibling.style.display='flex';">
            <div class="hidden w-full h-full items-center justify-center">
              <svg class="w-16 h-16 text-primary-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0z"/>
              </svg>
            </div>
          </div>
        </div>

        <!-- Login Form -->
        <div class="bg-white rounded-2xl shadow-xl p-8">
          <h2 class="text-2xl font-semibold text-gray-800 text-center mb-2">Chào mừng trở lại!</h2>
          <p class="text-gray-500 text-center mb-6">Đăng nhập để tiếp tục</p>

          @if (errorMessage()) {
            <div class="mb-4 p-3 bg-red-50 border border-red-200 rounded-lg">
              <p class="text-sm text-red-600">{{ errorMessage() }}</p>
            </div>
          }

          <form (ngSubmit)="onSubmit()" #loginForm="ngForm">
            <!-- Username -->
            <div class="mb-4">
              <label for="username" class="form-label">Tên đăng nhập</label>
              <input
                type="text"
                id="username"
                name="username"
                [(ngModel)]="credentials.username"
                required
                class="form-input"
                placeholder="Nhập tên đăng nhập"
                [disabled]="loading()">
            </div>

            <!-- Password -->
            <div class="mb-6">
              <label for="password" class="form-label">Mật khẩu</label>
              <input
                type="password"
                id="password"
                name="password"
                [(ngModel)]="credentials.password"
                required
                class="form-input"
                placeholder="Nhập mật khẩu"
                [disabled]="loading()">
            </div>

            <!-- Submit Button -->
            <button
              type="submit"
              [disabled]="!loginForm.valid || loading()"
              class="w-full btn-primary py-3 text-lg">
              @if (loading()) {
                <span class="flex items-center justify-center">
                  <svg class="animate-spin -ml-1 mr-3 h-5 w-5 text-white" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                  </svg>
                  Đang xử lý...
                </span>
              } @else {
                Đăng nhập
              }
            </button>
          </form>

          <p class="mt-6 text-center text-sm text-gray-500">
            Copyright 2026 ©. Công ty Cổ Phần Giải Pháp Kỹ Thuật Ấn Tượng
          </p>
        </div>
      </div>
    </div>
  `
})
export class LoginComponent {
  credentials: LoginDto = {
    username: '',
    password: ''
  };

  loading = signal(false);
  errorMessage = signal('');

  constructor(
    private authService: AuthService,
    private router: Router
  ) {
    // Nếu đã đăng nhập, chuyển đến dashboard
    if (this.authService.isAuthenticated()) {
      this.router.navigate(['/dashboard']);
    }
  }

  onSubmit(): void {
    if (!this.credentials.username || !this.credentials.password) return;

    this.loading.set(true);
    this.errorMessage.set('');

    this.authService.login(this.credentials).subscribe({
      next: (result) => {
        this.loading.set(false);
        if (result.success) {
          this.router.navigate(['/dashboard']);
        } else {
          this.errorMessage.set(result.errorMessage || 'Đăng nhập thất bại');
        }
      },
      error: (err) => {
        this.loading.set(false);
        this.errorMessage.set('Lỗi kết nối server. Vui lòng thử lại.');
        console.error('Login error:', err);
      }
    });
  }
}
