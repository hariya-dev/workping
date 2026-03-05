// layouts/main-layout/main-layout.component.ts
// Layout chính với sidebar và header

import { Component, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterModule, RouterLink, RouterLinkActive } from '@angular/router';
import { AuthService } from '../../core/services/auth.service';

@Component({
  selector: 'app-main-layout',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterModule, RouterLink, RouterLinkActive],
  template: `
    <div class="min-h-screen bg-gray-50">
      <!-- Mobile sidebar overlay -->
      @if (sidebarOpen()) {
        <div 
          class="fixed inset-0 bg-gray-600 bg-opacity-75 z-40 lg:hidden"
          (click)="closeSidebar()">
        </div>
      }

      <!-- Sidebar -->
      <aside 
        class="fixed inset-y-0 left-0 z-50 w-64 bg-white shadow-lg transform transition-transform duration-300 ease-in-out lg:translate-x-0"
        [class.translate-x-0]="sidebarOpen()"
        [class.-translate-x-full]="!sidebarOpen()">
        
        <!-- Logo -->
        <div class="flex items-center justify-center h-20 bg-gradient-to-r from-primary-600 to-primary-700 px-4">
          <img 
            src="https://atpro.com.vn/wp-content/uploads/2025/12/logo-tet-2026.png" 
            alt="An Tường Technology" 
            class="h-12 w-auto object-contain"
            onerror="this.style.display='none'; this.nextElementSibling.style.display='block';">
          <span class="hidden text-xl font-bold text-white">HR System</span>
        </div>

        <!-- Navigation -->
        <nav class="mt-6 px-3">
          <p class="px-4 text-xs font-semibold text-gray-400 uppercase tracking-wider mb-2">Menu chính</p>
          <div class="space-y-1">
            @for (item of menuItems; track item.path) {
              <a 
                [routerLink]="item.path"
                routerLinkActive="bg-primary-50 text-primary-700 border-primary-500"
                [routerLinkActiveOptions]="{ exact: item.exact }"
                class="flex items-center px-4 py-3 text-gray-600 rounded-lg hover:bg-gray-100 border-l-4 border-transparent transition-all duration-200"
                (click)="closeSidebar()">
                <span [innerHTML]="item.icon" class="w-5 h-5 mr-3"></span>
                <span class="font-medium">{{ item.label }}</span>
              </a>
            }
          </div>
        </nav>

        <!-- Logout button at bottom -->
        <div class="absolute bottom-0 left-0 right-0 p-4 border-t border-gray-200">
          <button 
            (click)="logout()"
            class="flex items-center w-full px-4 py-3 text-red-600 rounded-lg hover:bg-red-50 transition-colors">
            <svg class="w-5 h-5 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"/>
            </svg>
            <span class="font-medium">Đăng xuất</span>
          </button>
        </div>
      </aside>

      <!-- Main content -->
      <div class="lg:pl-64">
        <!-- Header -->
        <header class="sticky top-0 z-30 bg-white shadow-sm border-b border-gray-200">
          <div class="flex items-center justify-between h-16 px-4 lg:px-6">
            <!-- Mobile menu button -->
            <button 
              class="lg:hidden p-2 rounded-lg hover:bg-gray-100 text-gray-600"
              (click)="toggleSidebar()">
              <svg class="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 6h16M4 12h16M4 18h16"/>
              </svg>
            </button>

            <!-- Page title / Breadcrumb -->
            <div class="flex-1 ml-4 lg:ml-0">
              <h2 class="text-lg font-semibold text-gray-800">{{ pageTitle }}</h2>
            </div>

            <!-- User menu -->
            <div class="relative">
              <button 
                class="flex items-center space-x-3 p-2 rounded-lg hover:bg-gray-100 transition-colors"
                (click)="toggleUserMenu()">
                <div class="w-9 h-9 bg-gradient-to-br from-primary-500 to-primary-600 rounded-full flex items-center justify-center shadow-sm">
                  <span class="text-white text-sm font-semibold">
                    {{ authService.currentUser()?.fullName?.charAt(0) || 'U' }}
                  </span>
                </div>
                <div class="hidden md:block text-left">
                  <p class="text-sm font-medium text-gray-700">
                    {{ authService.currentUser()?.fullName || 'User' }}
                  </p>
                  <p class="text-xs text-gray-500">Quản trị viên</p>
                </div>
                <svg class="hidden md:block w-4 h-4 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 9l-7 7-7-7"/>
                </svg>
              </button>

              <!-- Dropdown menu -->
              @if (userMenuOpen()) {
                <div class="absolute right-0 mt-2 w-56 bg-white rounded-xl shadow-lg border border-gray-200 py-2 z-50">
                  <div class="px-4 py-3 border-b border-gray-100">
                    <p class="text-sm font-semibold text-gray-900">{{ authService.currentUser()?.fullName }}</p>
                    <p class="text-xs text-gray-500 mt-0.5">{{ authService.currentUser()?.email }}</p>
                  </div>
                  <a routerLink="/settings" 
                     class="flex items-center px-4 py-2.5 text-sm text-gray-700 hover:bg-gray-50 transition-colors"
                     (click)="userMenuOpen.set(false)">
                    <svg class="w-4 h-4 mr-3 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"/>
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/>
                    </svg>
                    Cài đặt
                  </a>
                  <button 
                    class="flex items-center w-full px-4 py-2.5 text-sm text-gray-700 hover:bg-gray-50 transition-colors"
                    (click)="openChangePasswordModal(); userMenuOpen.set(false)">
                    <svg class="w-4 h-4 mr-3 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 15v2m-6 4h12a2 2 0 002-2v-6a2 2 0 00-2-2H6a2 2 0 00-2 2v6a2 2 0 002 2zm10-10V7a4 4 0 00-8 0v4h8z"/>
                    </svg>
                    Đổi mật khẩu
                  </button>
                  <div class="border-t border-gray-100 my-1"></div>
                  <button 
                    class="flex items-center w-full px-4 py-2.5 text-sm text-red-600 hover:bg-red-50 transition-colors"
                    (click)="logout()">
                    <svg class="w-4 h-4 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 16l4-4m0 0l-4-4m4 4H7m6 4v1a3 3 0 01-3 3H6a3 3 0 01-3-3V7a3 3 0 013-3h4a3 3 0 013 3v1"/>
                    </svg>
                    Đăng xuất
                  </button>
                </div>
              }
            </div>
          </div>
        </header>

        <!-- Page content -->
        <main class="p-4 lg:p-6">
          <router-outlet></router-outlet>
        </main>
      </div>
    </div>

    <!-- Change Password Modal -->
    @if (changePasswordModalOpen()) {
      <div class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4" (click)="closeChangePasswordModal()">
        <div class="bg-white rounded-xl shadow-xl w-full max-w-md" (click)="$event.stopPropagation()">
          <div class="p-6">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-lg font-semibold text-gray-900">Đổi mật khẩu</h3>
              <button (click)="closeChangePasswordModal()" class="text-gray-400 hover:text-gray-600">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
                </svg>
              </button>
            </div>
            
            <div class="space-y-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Mật khẩu hiện tại</label>
                <input type="password" [(ngModel)]="passwordForm.currentPassword" class="form-input w-full" placeholder="Nhập mật khẩu hiện tại">
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Mật khẩu mới</label>
                <input type="password" [(ngModel)]="passwordForm.newPassword" class="form-input w-full" placeholder="Tối thiểu 6 ký tự">
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1">Xác nhận mật khẩu</label>
                <input type="password" [(ngModel)]="passwordForm.confirmPassword" class="form-input w-full" placeholder="Nhập lại mật khẩu mới">
              </div>
              
              @if (passwordError()) {
                <div class="p-3 bg-red-50 border border-red-200 rounded-lg text-sm text-red-600">
                  {{ passwordError() }}
                </div>
              }
              
              @if (passwordSuccess()) {
                <div class="p-3 bg-green-50 border border-green-200 rounded-lg text-sm text-green-600">
                  {{ passwordSuccess() }}
                </div>
              }
              
              <div class="flex justify-end gap-3 pt-4">
                <button (click)="closeChangePasswordModal()" class="btn-secondary" [disabled]="changingPassword()">Hủy</button>
                <button (click)="changePassword()" class="btn-primary" [disabled]="changingPassword()">
                  @if (changingPassword()) {
                    <svg class="animate-spin -ml-1 mr-2 h-4 w-4 text-white inline" fill="none" viewBox="0 0 24 24">
                      <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                      <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"></path>
                    </svg>
                    Đang xử lý...
                  } @else {
                    Đổi mật khẩu
                  }
                </button>
              </div>
            </div>
          </div>
        </div>
      </div>
    }
  `
})
export class MainLayoutComponent {
  sidebarOpen = signal(false);
  userMenuOpen = signal(false);
  changePasswordModalOpen = signal(false);
  changingPassword = signal(false);
  passwordError = signal('');
  passwordSuccess = signal('');
  pageTitle = 'Tổng quan';

  passwordForm = {
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
  };

  // Menu items với SVG icons
  menuItems = [
    {
      path: '/dashboard',
      label: 'Tổng quan',
      exact: true,
      icon: '<svg fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6"/></svg>'
    },
    {
      path: '/employees',
      label: 'Nhân viên',
      exact: false,
      icon: '<svg fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"/></svg>'
    },
    {
      path: '/contract-types',
      label: 'Loại hợp đồng',
      exact: false,
      icon: '<svg fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/></svg>'
    },
    {
      path: '/settings',
      label: 'Cài đặt',
      exact: false,
      icon: '<svg fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10.325 4.317c.426-1.756 2.924-1.756 3.35 0a1.724 1.724 0 002.573 1.066c1.543-.94 3.31.826 2.37 2.37a1.724 1.724 0 001.065 2.572c1.756.426 1.756 2.924 0 3.35a1.724 1.724 0 00-1.066 2.573c.94 1.543-.826 3.31-2.37 2.37a1.724 1.724 0 00-2.572 1.065c-.426 1.756-2.924 1.756-3.35 0a1.724 1.724 0 00-2.573-1.066c-1.543.94-3.31-.826-2.37-2.37a1.724 1.724 0 00-1.065-2.572c-1.756-.426-1.756-2.924 0-3.35a1.724 1.724 0 001.066-2.573c-.94-1.543.826-3.31 2.37-2.37.996.608 2.296.07 2.572-1.065z"/><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/></svg>'
    },
    {
      path: '/email-templates',
      label: 'Mẫu Email',
      exact: false,
      icon: '<svg fill="none" stroke="currentColor" viewBox="0 0 24 24"><path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"/></svg>'
    }
  ];

  constructor(public authService: AuthService) {}

  toggleSidebar(): void {
    this.sidebarOpen.update(v => !v);
  }

  closeSidebar(): void {
    this.sidebarOpen.set(false);
  }

  toggleUserMenu(): void {
    this.userMenuOpen.update(v => !v);
  }

  openChangePasswordModal(): void {
    this.changePasswordModalOpen.set(true);
  }

  closeChangePasswordModal(): void {
    this.changePasswordModalOpen.set(false);
    // Reset form
    this.passwordForm = {
      currentPassword: '',
      newPassword: '',
      confirmPassword: ''
    };
    this.passwordError.set('');
    this.passwordSuccess.set('');
  }

  changePassword(): void {
    this.passwordError.set('');
    this.passwordSuccess.set('');

    // Validate
    if (!this.passwordForm.currentPassword || !this.passwordForm.newPassword || !this.passwordForm.confirmPassword) {
      this.passwordError.set('Vui lòng điền đầy đủ các trường');
      return;
    }

    if (this.passwordForm.newPassword.length < 6) {
      this.passwordError.set('Mật khẩu mới phải có ít nhất 6 ký tự');
      return;
    }

    if (this.passwordForm.newPassword !== this.passwordForm.confirmPassword) {
      this.passwordError.set('Xác nhận mật khẩu không khớp');
      return;
    }

    this.changingPassword.set(true);

    this.authService.changePassword(this.passwordForm).subscribe({
      next: (result) => {
        this.changingPassword.set(false);
        if (result.success) {
          this.passwordSuccess.set('Đổi mật khẩu thành công!');
          // Close modal after success
          setTimeout(() => {
            this.closeChangePasswordModal();
          }, 2000);
        } else {
          this.passwordError.set(result.message || 'Có lỗi xảy ra');
        }
      },
      error: () => {
        this.changingPassword.set(false);
        this.passwordError.set('Không thể đổi mật khẩu. Vui lòng thử lại.');
      }
    });
  }

  logout(): void {
    this.authService.logout();
  }
}
