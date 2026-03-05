// features/settings/settings.component.ts
// Component cài đặt hệ thống

import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SettingsService } from '../../core/services/settings.service';
import { AuthService } from '../../core/services/auth.service';
import { ReminderSettings, ChangePasswordDto } from '../../core/models';

@Component({
  selector: 'app-settings',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="space-y-6">
      <!-- Header -->
      <div>
        <h1 class="text-2xl font-bold text-gray-900">Cài đặt hệ thống</h1>
        <p class="text-gray-500 mt-1">Cấu hình thông số và nhắc nhở tự động cho hệ thống</p>
      </div>

      @if (loading()) {
        <div class="flex justify-center py-12">
          <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
        </div>
      } @else {
        <form (ngSubmit)="saveSettings()" class="space-y-6">
          <!-- Cài đặt thử việc -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
            <div class="flex items-center mb-6">
              <div class="w-10 h-10 bg-amber-100 rounded-lg flex items-center justify-center mr-3">
                <svg class="w-5 h-5 text-amber-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
              </div>
              <div>
                <h3 class="text-lg font-semibold text-gray-900">Cài đặt thử việc</h3>
                <p class="text-sm text-gray-500">Cấu hình thời gian thử việc mặc định</p>
              </div>
            </div>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-5">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">Số ngày thử việc mặc định</label>
                <input type="number" [(ngModel)]="settings.defaultProbationDays" name="defaultProbationDays" min="1" 
                       class="form-input" placeholder="60">
                <p class="text-xs text-gray-500 mt-1">Thời gian thử việc mặc định khi tạo nhân viên mới</p>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">Nhắc nhở trước (ngày)</label>
                <input type="text" [(ngModel)]="probationReminderDaysStr" name="probationReminderDays" 
                       class="form-input" placeholder="VD: 30,15,7,3,1">
                <p class="text-xs text-gray-500 mt-1">Các ngày nhắc nhở trước khi hết thử việc, cách nhau bởi dấu phẩy</p>
              </div>
            </div>
          </div>

          <!-- Cài đặt hợp đồng -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
            <div class="flex items-center mb-6">
              <div class="w-10 h-10 bg-blue-100 rounded-lg flex items-center justify-center mr-3">
                <svg class="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
                </svg>
              </div>
              <div>
                <h3 class="text-lg font-semibold text-gray-900">Cài đặt hợp đồng</h3>
                <p class="text-sm text-gray-500">Cấu hình nhắc nhở hợp đồng sắp hết hạn</p>
              </div>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1.5">Nhắc nhở trước khi hết hợp đồng (ngày)</label>
              <input type="text" [(ngModel)]="contractReminderDaysStr" name="contractReminderDays" 
                     class="form-input" placeholder="VD: 30,15,7,3,1">
              <p class="text-xs text-gray-500 mt-1">Các ngày nhắc nhở trước khi hợp đồng hết hạn, cách nhau bởi dấu phẩy</p>
            </div>
          </div>

          <!-- Email HR -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
            <div class="flex items-center mb-6">
              <div class="w-10 h-10 bg-primary-100 rounded-lg flex items-center justify-center mr-3">
                <svg class="w-5 h-5 text-primary-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"/>
                </svg>
              </div>
              <div>
                <h3 class="text-lg font-semibold text-gray-900">Email nhận thông báo</h3>
                <p class="text-sm text-gray-500">Danh sách email HR nhận thông báo nhắc nhở</p>
              </div>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1.5">Danh sách email HR</label>
              <textarea 
                [(ngModel)]="settings.hrNotificationEmails" 
                name="hrNotificationEmails" 
                rows="3" 
                class="form-input"
                placeholder="VD: hr@company.com, admin@company.com"></textarea>
              <p class="text-xs text-gray-500 mt-1">Nhiều email cách nhau bởi dấu phẩy</p>
            </div>
          </div>

          <!-- Test gửi email -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
            <div class="flex items-center mb-6">
              <div class="w-10 h-10 bg-green-100 rounded-lg flex items-center justify-center mr-3">
                <svg class="w-5 h-5 text-green-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
              </div>
              <div>
                <h3 class="text-lg font-semibold text-gray-900">Kiểm tra hệ thống thông báo</h3>
                <p class="text-sm text-gray-500">Gửi email test để xác nhận hệ thống đang hoạt động</p>
              </div>
            </div>
            
            <!-- Email input -->
            <div class="mb-6">
              <label class="block text-sm font-medium text-gray-700 mb-1.5">Email nhận test</label>
              <input type="email" [(ngModel)]="testEmail" name="testEmail" 
                     class="form-input" placeholder="Nhập email nhận test">
            </div>

            <!-- Test buttons grid -->
            <div class="grid grid-cols-1 md:grid-cols-2 gap-4 mb-6">
              <!-- Basic test email -->
              <button type="button" (click)="sendTestEmail()" [disabled]="sendingTestEmail() || !testEmail" 
                      class="btn-secondary flex items-center justify-center">
                @if (sendingTestEmail()) {
                  <svg class="animate-spin -ml-1 mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"></path>
                  </svg>
                  Đang gửi...
                } @else {
                  <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"/>
                  </svg>
                  Test Email Cơ Bản
                }
              </button>

              <!-- Birthday test email -->
              <button type="button" (click)="sendTestBirthdayEmail()" [disabled]="sendingBirthdayTest() || !testEmail" 
                      class="btn-primary flex items-center justify-center">
                @if (sendingBirthdayTest()) {
                  <svg class="animate-spin -ml-1 mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"></path>
                  </svg>
                  Đang gửi...
                } @else {
                  <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"/>
                  </svg>
                  Test Email Sinh Nhật
                }
              </button>

              <!-- Probation test email (HR only) -->
              <button type="button" (click)="sendTestProbationEmailHr()" [disabled]="sendingProbationTestHr() || !testEmail" 
                      class="btn-warning flex items-center justify-center">
                @if (sendingProbationTestHr()) {
                  <svg class="animate-spin -ml-1 mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"></path>
                  </svg>
                  Đang gửi...
                } @else {
                  <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                  </svg>
                  Test Email Thử Việc (HR)
                }
              </button>

              <!-- Contract test email (HR only) -->
              <button type="button" (click)="sendTestContractEmailHr()" [disabled]="sendingContractTestHr() || !testEmail" 
                      class="btn-danger flex items-center justify-center">
                @if (sendingContractTestHr()) {
                  <svg class="animate-spin -ml-1 mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"></path>
                  </svg>
                  Đang gửi...
                } @else {
                  <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
                  </svg>
                  Test Email Hợp Đồng (HR)
                }
              </button>

              <!-- Monthly Birthday List test email (HR only) -->
              <button type="button" (click)="sendTestMonthlyBirthdayEmail()" [disabled]="sendingMonthlyBirthdayTest() || !testEmail" 
                      class="btn-secondary flex items-center justify-center">
                @if (sendingMonthlyBirthdayTest()) {
                  <svg class="animate-spin -ml-1 mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"></path>
                  </svg>
                  Đang gửi...
                } @else {
                  <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0zm6 3a2 2 0 11-4 0 2 2 0 014 0zM7 10a2 2 0 11-4 0 2 2 0 014 0z"/>
                  </svg>
                  Test DS Sinh Nhật Tháng (HR)
                }
              </button>
            </div>

            <!-- Success messages -->
            @if (testEmailSuccess()) {
              <div class="mb-3 p-3 bg-green-50 border border-green-200 rounded-lg text-sm text-green-600 flex items-center">
                <svg class="w-4 h-4 mr-2 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"/>
                </svg>
                {{ testEmailSuccess() }}
              </div>
            }
            @if (birthdayTestSuccess()) {
              <div class="mb-3 p-3 bg-green-50 border border-green-200 rounded-lg text-sm text-green-600 flex items-center">
                <svg class="w-4 h-4 mr-2 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"/>
                </svg>
                {{ birthdayTestSuccess() }}
              </div>
            }
            @if (probationTestHrSuccess()) {
              <div class="mb-3 p-3 bg-green-50 border border-green-200 rounded-lg text-sm text-green-600 flex items-center">
                <svg class="w-4 h-4 mr-2 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"/>
                </svg>
                {{ probationTestHrSuccess() }}
              </div>
            }
            @if (contractTestHrSuccess()) {
              <div class="mb-3 p-3 bg-green-50 border border-green-200 rounded-lg text-sm text-green-600 flex items-center">
                <svg class="w-4 h-4 mr-2 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"/>
                </svg>
                {{ contractTestHrSuccess() }}
              </div>
            }
            @if (monthlyBirthdayTestSuccess()) {
              <div class="mb-3 p-3 bg-green-50 border border-green-200 rounded-lg text-sm text-green-600 flex items-center">
                <svg class="w-4 h-4 mr-2 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"/>
                </svg>
                {{ monthlyBirthdayTestSuccess() }}
              </div>
            }

            <!-- Error messages -->
            @if (testEmailError()) {
              <div class="mb-3 p-3 bg-red-50 border border-red-200 rounded-lg text-sm text-red-600 flex items-center">
                <svg class="w-4 h-4 mr-2 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
                {{ testEmailError() }}
              </div>
            }
            @if (birthdayTestError()) {
              <div class="mb-3 p-3 bg-red-50 border border-red-200 rounded-lg text-sm text-red-600 flex items-center">
                <svg class="w-4 h-4 mr-2 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
                {{ birthdayTestError() }}
              </div>
            }
            @if (probationTestHrError()) {
              <div class="mb-3 p-3 bg-red-50 border border-red-200 rounded-lg text-sm text-red-600 flex items-center">
                <svg class="w-4 h-4 mr-2 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
                {{ probationTestHrError() }}
              </div>
            }
            @if (contractTestHrError()) {
              <div class="mb-3 p-3 bg-red-50 border border-red-200 rounded-lg text-sm text-red-600 flex items-center">
                <svg class="w-4 h-4 mr-2 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
                {{ contractTestHrError() }}
              </div>
            }
            @if (monthlyBirthdayTestError()) {
              <div class="mb-3 p-3 bg-red-50 border border-red-200 rounded-lg text-sm text-red-600 flex items-center">
                <svg class="w-4 h-4 mr-2 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
                {{ monthlyBirthdayTestError() }}
              </div>
            }
          </div>

          <!-- Lưu ý -->
          <div class="bg-amber-50 border border-amber-200 rounded-xl p-4">
            <div class="flex">
              <svg class="w-5 h-5 text-amber-500 mr-3 flex-shrink-0 mt-0.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
              </svg>
              <div>
                <h4 class="text-sm font-semibold text-amber-800">Lưu ý</h4>
                <p class="text-sm text-amber-700 mt-1">
                  Hệ thống sẽ tự động gửi email nhắc nhở vào các ngày đã cấu hình. 
                  Đảm bảo email được cấu hình chính xác để nhận thông báo.
                </p>
              </div>
            </div>
          </div>

          <!-- Success message -->
          @if (successMessage()) {
            <div class="p-4 bg-green-50 border border-green-200 rounded-xl flex items-center">
              <svg class="w-5 h-5 text-green-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"/>
              </svg>
              <p class="text-green-700 font-medium">{{ successMessage() }}</p>
            </div>
          }

          <!-- Error message -->
          @if (errorMessage()) {
            <div class="p-4 bg-red-50 border border-red-200 rounded-xl flex items-center">
              <svg class="w-5 h-5 text-red-500 mr-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
              </svg>
              <p class="text-red-600">{{ errorMessage() }}</p>
            </div>
          }

          <!-- Buttons -->
          <div class="flex justify-end">
            <button type="submit" [disabled]="saving()" class="btn-primary px-6">
              @if (saving()) {
                <svg class="animate-spin -ml-1 mr-2 h-5 w-5 text-white inline" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"></path>
                </svg>
                Đang lưu...
              } @else {
                <svg class="w-5 h-5 mr-2 inline" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"/>
                </svg>
                Lưu cài đặt
              }
            </button>
          </div>
        </form>
      }
    </div>
  `
})
export class SettingsComponent implements OnInit {
  loading = signal(true);
  saving = signal(false);
  successMessage = signal('');
  errorMessage = signal('');

  // Test email states
  testEmail = 'hai2000.dev@gmail.com';
  sendingTestEmail = signal(false);
  testEmailSuccess = signal('');
  testEmailError = signal('');

  // Test email types (Nhân viên - chỉ sinh nhật)
  sendingBirthdayTest = signal(false);
  birthdayTestSuccess = signal('');
  birthdayTestError = signal('');

  // Test email types (HR)
  sendingProbationTestHr = signal(false);
  sendingContractTestHr = signal(false);
  sendingMonthlyBirthdayTest = signal(false);
  probationTestHrSuccess = signal('');
  contractTestHrSuccess = signal('');
  monthlyBirthdayTestSuccess = signal('');
  probationTestHrError = signal('');
  contractTestHrError = signal('');
  monthlyBirthdayTestError = signal('');

  settings: ReminderSettings = {
    defaultProbationDays: 60,
    probationReminderDaysBefore: [30, 15, 7, 3, 1],
    contractReminderDaysBefore: [30, 15, 7, 3, 1],
    hrNotificationEmails: ''
  };

  probationReminderDaysStr = '';
  contractReminderDaysStr = '';

  constructor(
    private settingsService: SettingsService,
    private authService: AuthService
  ) {}

  ngOnInit(): void {
    this.loadSettings();
  }

  loadSettings(): void {
    this.loading.set(true);
    this.settingsService.getReminderSettings().subscribe({
      next: (result) => {
        if (result.success && result.data) {
          this.settings = result.data;
          this.probationReminderDaysStr = this.settings.probationReminderDaysBefore.join(', ');
          this.contractReminderDaysStr = this.settings.contractReminderDaysBefore.join(', ');
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  saveSettings(): void {
    this.saving.set(true);
    this.successMessage.set('');
    this.errorMessage.set('');

    // Parse string to arrays
    this.settings.probationReminderDaysBefore = this.parseNumberArray(this.probationReminderDaysStr);
    this.settings.contractReminderDaysBefore = this.parseNumberArray(this.contractReminderDaysStr);

    this.settingsService.updateReminderSettings(this.settings).subscribe({
      next: (result) => {
        this.saving.set(false);
        if (result.success) {
          this.successMessage.set('Đã lưu cài đặt thành công!');
          setTimeout(() => this.successMessage.set(''), 3000);
        } else {
          this.errorMessage.set(result.message || 'Có lỗi xảy ra');
        }
      },
      error: () => {
        this.saving.set(false);
        this.errorMessage.set('Không thể lưu cài đặt. Vui lòng thử lại.');
      }
    });
  }

  sendTestEmail(): void {
    if (!this.testEmail) return;

    this.testEmailSuccess.set('');
    this.testEmailError.set('');
    this.sendingTestEmail.set(true);

    // Call auth service to test email
    this.authService.testEmail().subscribe({
      next: (result) => {
        this.sendingTestEmail.set(false);
        if (result.success) {
          this.testEmailSuccess.set(`Đã gửi email test thành công đến ${this.testEmail}`);
          setTimeout(() => this.testEmailSuccess.set(''), 5000);
        } else {
          this.testEmailError.set(result.message || 'Gửi email thất bại');
        }
      },
      error: (err) => {
        this.sendingTestEmail.set(false);
        this.testEmailError.set('Không thể gửi email test. Kiểm tra cấu hình SMTP.');
      }
    });
  }

  // Test birthday email
  sendTestBirthdayEmail(): void {
    if (!this.testEmail) return;

    this.birthdayTestSuccess.set('');
    this.birthdayTestError.set('');
    this.sendingBirthdayTest.set(true);

    this.settingsService.sendTestBirthdayEmail(this.testEmail).subscribe({
      next: (result) => {
        this.sendingBirthdayTest.set(false);
        if (result.success) {
          this.birthdayTestSuccess.set(`Đã gửi email sinh nhật test thành công đến ${this.testEmail}`);
          setTimeout(() => this.birthdayTestSuccess.set(''), 5000);
        } else {
          this.birthdayTestError.set(result.message || 'Gửi email sinh nhật thất bại');
        }
      },
      error: (err) => {
        this.sendingBirthdayTest.set(false);
        this.birthdayTestError.set('Không thể gửi email sinh nhật test.');
      }
    });
  }

  // Test probation email for HR
  sendTestProbationEmailHr(): void {
    if (!this.testEmail) return;

    this.probationTestHrSuccess.set('');
    this.probationTestHrError.set('');
    this.sendingProbationTestHr.set(true);

    this.settingsService.sendTestProbationEmailHr(this.testEmail).subscribe({
      next: (result) => {
        this.sendingProbationTestHr.set(false);
        if (result.success) {
          this.probationTestHrSuccess.set(`Đã gửi email thử việc (HR) test thành công đến ${this.testEmail}`);
          setTimeout(() => this.probationTestHrSuccess.set(''), 5000);
        } else {
          this.probationTestHrError.set(result.message || 'Gửi email thử việc (HR) thất bại');
        }
      },
      error: (err) => {
        this.sendingProbationTestHr.set(false);
        this.probationTestHrError.set('Không thể gửi email thử việc (HR) test.');
      }
    });
  }

  // Test contract email for HR
  sendTestContractEmailHr(): void {
    if (!this.testEmail) return;

    this.contractTestHrSuccess.set('');
    this.contractTestHrError.set('');
    this.sendingContractTestHr.set(true);

    this.settingsService.sendTestContractEmailHr(this.testEmail).subscribe({
      next: (result) => {
        this.sendingContractTestHr.set(false);
        if (result.success) {
          this.contractTestHrSuccess.set(`Đã gửi email hợp đồng (HR) test thành công đến ${this.testEmail}`);
          setTimeout(() => this.contractTestHrSuccess.set(''), 5000);
        } else {
          this.contractTestHrError.set(result.message || 'Gửi email hợp đồng (HR) thất bại');
        }
      },
      error: (err) => {
        this.sendingContractTestHr.set(false);
        this.contractTestHrError.set('Không thể gửi email hợp đồng (HR) test.');
      }
    });
  }

  // Test monthly birthday list email for HR
  sendTestMonthlyBirthdayEmail(): void {
    if (!this.testEmail) return;

    this.monthlyBirthdayTestSuccess.set('');
    this.monthlyBirthdayTestError.set('');
    this.sendingMonthlyBirthdayTest.set(true);

    this.settingsService.sendTestMonthlyBirthdayEmail(this.testEmail).subscribe({
      next: (result) => {
        this.sendingMonthlyBirthdayTest.set(false);
        if (result.success) {
          this.monthlyBirthdayTestSuccess.set(`Đã gửi email danh sách sinh nhật tháng test thành công đến ${this.testEmail}`);
          setTimeout(() => this.monthlyBirthdayTestSuccess.set(''), 5000);
        } else {
          this.monthlyBirthdayTestError.set(result.message || 'Gửi email danh sách sinh nhật tháng thất bại');
        }
      },
      error: (err) => {
        this.sendingMonthlyBirthdayTest.set(false);
        this.monthlyBirthdayTestError.set('Không thể gửi email danh sách sinh nhật tháng test.');
      }
    });
  }

  private parseNumberArray(str: string): number[] {
    return str.split(',')
      .map(s => parseInt(s.trim(), 10))
      .filter(n => !isNaN(n) && n > 0)
      .sort((a, b) => b - a);
  }
}
