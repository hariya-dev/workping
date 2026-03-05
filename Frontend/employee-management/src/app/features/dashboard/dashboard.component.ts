// features/dashboard/dashboard.component.ts
// Component Dashboard chính

import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';
import { DashboardService } from '../../core/services/dashboard.service';
import { DashboardStats, EmployeeStatus, EmployeeStatusLabels } from '../../core/models';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  template: `
    <div class="space-y-6">
      <!-- Page Header -->
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900">Tổng quan</h1>
          <p class="text-gray-500 mt-1">Tổng quan hệ thống quản lý nhân sự</p>
        </div>
      </div>

      @if (loading()) {
        <div class="flex justify-center py-12">
          <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
        </div>
      } @else if (stats()) {
        <!-- Stats Cards -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6">
          <!-- Total Employees -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow">
            <div class="flex items-center">
              <div class="p-3 bg-primary-100 rounded-xl">
                <svg class="w-6 h-6 text-primary-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0z"/>
                </svg>
              </div>
              <div class="ml-4">
                <p class="text-sm font-medium text-gray-500">Tổng nhân viên</p>
                <p class="text-2xl font-bold text-gray-900">{{ stats()!.totalEmployees }}</p>
              </div>
            </div>
          </div>

          <!-- Probation Expiring -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow">
            <div class="flex items-center">
              <div class="p-3 bg-amber-100 rounded-xl">
                <svg class="w-6 h-6 text-amber-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
              </div>
              <div class="ml-4">
                <p class="text-sm font-medium text-gray-500">Sắp hết thử việc</p>
                <p class="text-2xl font-bold text-amber-600">{{ stats()!.probationExpiringSoon }}</p>
              </div>
            </div>
          </div>

          <!-- Contract Expiring -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow">
            <div class="flex items-center">
              <div class="p-3 bg-red-100 rounded-xl">
                <svg class="w-6 h-6 text-red-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
                </svg>
              </div>
              <div class="ml-4">
                <p class="text-sm font-medium text-gray-500">Sắp hết hợp đồng</p>
                <p class="text-2xl font-bold text-red-600">{{ stats()!.contractExpiringSoon }}</p>
              </div>
            </div>
          </div>

          <!-- Birthdays This Month -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6 hover:shadow-md transition-shadow">
            <div class="flex items-center">
              <div class="p-3 bg-pink-100 rounded-xl">
                <svg class="w-6 h-6 text-pink-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 15.546c-.523 0-1.046.151-1.5.454a2.704 2.704 0 01-3 0 2.704 2.704 0 00-3 0 2.704 2.704 0 01-3 0 2.704 2.704 0 00-3 0 2.704 2.704 0 01-3 0 2.701 2.701 0 00-1.5-.454M9 6v2m3-2v2m3-2v2M9 3h.01M12 3h.01M15 3h.01M21 21v-7a2 2 0 00-2-2H5a2 2 0 00-2 2v7h18zm-3-9v-2a2 2 0 00-2-2H8a2 2 0 00-2 2v2h12z"/>
                </svg>
              </div>
              <div class="ml-4">
                <p class="text-sm font-medium text-gray-500">Sinh nhật tháng này</p>
                <p class="text-2xl font-bold text-pink-600">{{ stats()!.currentMonthBirthdays.length }}</p>
              </div>
            </div>
          </div>
        </div>

        <!-- Status Breakdown -->
        <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
          <!-- Status Chart -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
            <h3 class="text-lg font-semibold text-gray-900 mb-4">Trạng thái Nhân viên</h3>
            <div class="space-y-4">
              @for (status of stats()!.statusCounts; track status.status) {
                <div class="flex items-center">
                  <span class="w-32 text-sm text-gray-600">{{ status.statusName }}</span>
                  <div class="flex-1 mx-4">
                    <div class="h-3 bg-gray-100 rounded-full overflow-hidden">
                      <div 
                        class="h-full rounded-full transition-all duration-500"
                        [class]="getStatusBarColor(status.status)"
                        [style.width.%]="getStatusPercentage(status.count)">
                      </div>
                    </div>
                  </div>
                  <span class="w-12 text-sm font-semibold text-gray-900 text-right">{{ status.count }}</span>
                </div>
              }
            </div>
          </div>

          <!-- Upcoming Reminders -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-lg font-semibold text-gray-900">Nhắc nhở sắp tới</h3>
              <a routerLink="/employees" class="text-sm text-primary-600 hover:text-primary-700 font-medium">Xem tất cả</a>
            </div>
            
            @if (stats()!.upcomingReminders.length === 0) {
              <div class="text-center py-8">
                <svg class="w-12 h-12 text-gray-300 mx-auto mb-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5H7a2 2 0 00-2 2v12a2 2 0 002 2h10a2 2 0 002-2V7a2 2 0 00-2-2h-2M9 5a2 2 0 002 2h2a2 2 0 002-2M9 5a2 2 0 012-2h2a2 2 0 012 2m-6 9l2 2 4-4"/>
                </svg>
                <p class="text-gray-500">Không có nhắc nhở nào</p>
              </div>
            } @else {
              <div class="space-y-3">
                @for (reminder of stats()!.upcomingReminders.slice(0, 5); track reminder.employeeId) {
                  <div class="flex items-center justify-between p-3 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors">
                    <div>
                      <p class="font-medium text-gray-900">{{ reminder.employeeName }}</p>
                      <p class="text-sm text-gray-500">
                        {{ reminder.reminderType === 'Probation' ? 'Hết thử việc' : 'Hết hợp đồng' }}
                      </p>
                    </div>
                    <span 
                      class="px-2.5 py-1 rounded-full text-xs font-medium"
                      [class.bg-red-100]="reminder.daysRemaining <= 3"
                      [class.text-red-700]="reminder.daysRemaining <= 3"
                      [class.bg-amber-100]="reminder.daysRemaining > 3 && reminder.daysRemaining <= 7"
                      [class.text-amber-700]="reminder.daysRemaining > 3 && reminder.daysRemaining <= 7"
                      [class.bg-green-100]="reminder.daysRemaining > 7"
                      [class.text-green-700]="reminder.daysRemaining > 7">
                      {{ reminder.daysRemaining }} ngày
                    </span>
                  </div>
                }
              </div>
            }
          </div>
        </div>

        <!-- Birthdays -->
        <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
          <div class="flex items-center justify-between mb-4">
            <h3 class="text-lg font-semibold text-gray-900">Sinh nhật tháng này</h3>
          </div>
          
          @if (stats()!.currentMonthBirthdays.length === 0) {
            <div class="text-center py-8">
              <svg class="w-12 h-12 text-gray-300 mx-auto mb-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 15.546c-.523 0-1.046.151-1.5.454a2.704 2.704 0 01-3 0 2.704 2.704 0 00-3 0 2.704 2.704 0 01-3 0 2.704 2.704 0 00-3 0 2.704 2.704 0 01-3 0 2.701 2.701 0 00-1.5-.454M9 6v2m3-2v2m3-2v2M9 3h.01M12 3h.01M15 3h.01M21 21v-7a2 2 0 00-2-2H5a2 2 0 00-2 2v7h18zm-3-9v-2a2 2 0 00-2-2H8a2 2 0 00-2 2v2h12z"/>
              </svg>
              <p class="text-gray-500">Không có sinh nhật nào trong tháng này</p>
            </div>
          } @else {
            <div class="space-y-3">
              @for (birthday of stats()!.currentMonthBirthdays; track birthday.employeeId) {
                <div class="flex items-center p-4 bg-gradient-to-r from-pink-50 to-purple-50 rounded-xl border border-pink-100 hover:shadow-sm transition-shadow">
                  <div class="w-12 h-12 bg-gradient-to-br from-pink-500 to-purple-500 rounded-full flex items-center justify-center text-white font-bold shadow-sm">
                    {{ birthday.day }}
                  </div>
                  <div class="ml-4 flex-1">
                    <p class="font-semibold text-gray-900">{{ birthday.employeeName }}</p>
                    <p class="text-sm text-gray-600">
                      <span class="text-gray-500">Ngày sinh:</span> 
                      <span class="font-medium">{{ birthday.day }}/{{ getMonthFromBirthday(birthday.dateOfBirth) }}</span>
                      @if (birthday.department && birthday.department !== 'NULL') {
                        <span class="ml-2 text-gray-400">•</span>
                        <span class="text-gray-500 ml-2">{{ birthday.department }}</span>
                      }
                    </p>
                  </div>
                  @if (isBirthdayToday(birthday.dateOfBirth)) {
                    <span class="ml-2 px-2.5 py-1 bg-pink-500 text-white text-xs font-bold rounded-full animate-pulse">
                      Hôm nay
                    </span>
                  }
                </div>
              }
            </div>
          }
        </div>
      }
    </div>
  `
})
export class DashboardComponent implements OnInit {
  stats = signal<DashboardStats | null>(null);
  loading = signal(true);

  constructor(private dashboardService: DashboardService) {}

  ngOnInit(): void {
    this.loadStats();
  }

  loadStats(): void {
    this.loading.set(true);
    this.dashboardService.getStats().subscribe({
      next: (result) => {
        if (result.success && result.data) {
          this.stats.set(result.data);
        }
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      }
    });
  }

  getStatusPercentage(count: number): number {
    const total = this.stats()?.totalEmployees || 1;
    return (count / total) * 100;
  }

  getStatusBarColor(status: EmployeeStatus): string {
    const colors: { [key: number]: string } = {
      [EmployeeStatus.Active]: 'bg-green-500',
      [EmployeeStatus.Resigned]: 'bg-gray-400'
    };
    return colors[status] || 'bg-gray-400';
  }

  getMonthFromBirthday(dateStr: string): number {
    return new Date(dateStr).getMonth() + 1;
  }

  isBirthdayToday(dateStr: string): boolean {
    const today = new Date();
    const birthDate = new Date(dateStr);
    return birthDate.getDate() === today.getDate() && 
           birthDate.getMonth() === today.getMonth();
  }
}
