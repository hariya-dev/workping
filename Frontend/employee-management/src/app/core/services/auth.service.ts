// core/services/auth.service.ts
// Service quản lý Xác thực

import { Injectable, signal, computed } from '@angular/core';
import { Router } from '@angular/router';
import { Observable, tap, catchError, of } from 'rxjs';
import { ApiService } from './api.service';
import { LoginDto, LoginResult, User, ChangePasswordDto } from '../models';
import { ApiResult } from '../models/api-result.model';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  // Signal lưu trạng thái user
  private currentUserSignal = signal<User | null>(null);
  private tokenSignal = signal<string | null>(null);

  // Computed values
  readonly currentUser = computed(() => this.currentUserSignal());
  readonly isAuthenticated = computed(() => !!this.tokenSignal());
  readonly isAdmin = computed(() => this.currentUserSignal()?.role === 'Admin');

  private readonly TOKEN_KEY = 'auth_token';
  private readonly USER_KEY = 'auth_user';

  constructor(
    private api: ApiService,
    private router: Router
  ) {
    // Load từ localStorage khi khởi tạo
    this.loadFromStorage();
  }

  // Đăng nhập
  login(dto: LoginDto): Observable<LoginResult> {
    return this.api.post<LoginResult>('/api/auth/login', dto).pipe(
      tap(result => {
        if (result.success && result.token && result.user) {
          this.setAuth(result.token, result.user);
        }
      })
    );
  }

  // Đăng xuất
  logout(): void {
    localStorage.removeItem(this.TOKEN_KEY);
    localStorage.removeItem(this.USER_KEY);
    this.tokenSignal.set(null);
    this.currentUserSignal.set(null);
    this.router.navigate(['/login']);
  }

  // Đổi mật khẩu
  changePassword(dto: ChangePasswordDto): Observable<ApiResult<void>> {
    return this.api.post<ApiResult<void>>('/api/auth/change-password', dto);
  }

  // Lấy thông tin user hiện tại từ API
  fetchCurrentUser(): Observable<ApiResult<User>> {
    return this.api.get<ApiResult<User>>('/api/auth/me').pipe(
      tap(result => {
        if (result.success && result.data) {
          this.currentUserSignal.set(result.data);
          localStorage.setItem(this.USER_KEY, JSON.stringify(result.data));
        }
      }),
      catchError(() => {
        this.logout();
        return of({ success: false, errors: [] } as ApiResult<User>);
      })
    );
  }

  // Test email functionality
  testEmail(): Observable<any> {
    return this.api.post<any>('/api/auth/test-email', {});
  }

  // Lấy token
  getToken(): string | null {
    return this.tokenSignal();
  }

  // Private methods
  private setAuth(token: string, user: User): void {
    localStorage.setItem(this.TOKEN_KEY, token);
    localStorage.setItem(this.USER_KEY, JSON.stringify(user));
    this.tokenSignal.set(token);
    this.currentUserSignal.set(user);
  }

  private loadFromStorage(): void {
    const token = localStorage.getItem(this.TOKEN_KEY);
    const userStr = localStorage.getItem(this.USER_KEY);

    if (token) {
      this.tokenSignal.set(token);
    }

    if (userStr) {
      try {
        const user = JSON.parse(userStr) as User;
        this.currentUserSignal.set(user);
      } catch {
        localStorage.removeItem(this.USER_KEY);
      }
    }
  }
}
