// features/employees/employee-form/employee-form.component.ts
// Component form thêm/sửa nhân viên

import { Component, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { EmployeeService } from '../../../core/services/employee.service';
import { SettingsService } from '../../../core/services/settings.service';
import { Employee, CreateEmployeeDto, UpdateEmployeeDto, EmployeeStatus } from '../../../core/models';

@Component({
  selector: 'app-employee-form',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="space-y-6">
      <!-- Header -->
      <div class="flex items-center justify-between">
        <div>
          <nav class="flex items-center space-x-2 text-sm text-gray-500 mb-2">
            <a routerLink="/employees" class="hover:text-primary-600">Nhân viên</a>
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"/>
            </svg>
            <span class="text-gray-900">{{ isEditMode() ? 'Chỉnh sửa' : 'Thêm mới' }}</span>
          </nav>
          <h1 class="text-2xl font-bold text-gray-900">
            {{ isEditMode() ? 'Chỉnh sửa thông tin nhân viên' : 'Thêm nhân viên mới' }}
          </h1>
          <p class="text-gray-500 mt-1">
            {{ isEditMode() ? 'Cập nhật thông tin nhân viên trong hệ thống' : 'Nhập đầy đủ thông tin nhân viên mới' }}
          </p>
        </div>
        <a routerLink="/employees" class="btn-secondary inline-flex items-center">
          <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"/>
          </svg>
          Quay lại
        </a>
      </div>

      @if (loading()) {
        <div class="flex justify-center py-12">
          <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
        </div>
      } @else {
        <form (ngSubmit)="onSubmit()" #employeeForm="ngForm" class="space-y-6">
          <!-- Thông tin cơ bản -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
            <div class="flex items-center mb-6">
              <div class="w-10 h-10 bg-primary-100 rounded-lg flex items-center justify-center mr-3">
                <svg class="w-5 h-5 text-primary-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"/>
                </svg>
              </div>
              <div>
                <h3 class="text-lg font-semibold text-gray-900">Thông tin cơ bản</h3>
                <p class="text-sm text-gray-500">Thông tin cá nhân của nhân viên</p>
              </div>
            </div>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-5">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">
                  Họ và tên <span class="text-red-500">*</span>
                </label>
                <input type="text" [(ngModel)]="formData.fullName" name="fullName" required 
                       class="form-input" placeholder="Nhập họ và tên đầy đủ">
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">Email</label>
                <input type="email" [(ngModel)]="formData.email" name="email" 
                       class="form-input" placeholder="example@company.com">
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">Số điện thoại</label>
                <input type="text" [(ngModel)]="formData.phoneNumber" name="phoneNumber" 
                       class="form-input" placeholder="0912 345 678">
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">
                  Ngày sinh <span class="text-red-500">*</span>
                </label>
                <input type="date" [(ngModel)]="formData.dateOfBirth" name="dateOfBirth" required 
                       class="form-input">
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">Phòng ban</label>
                <input type="text" [(ngModel)]="formData.department" name="department" 
                       class="form-input" placeholder="VD: Phòng Kỹ thuật">
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">Chức vụ</label>
                <input type="text" [(ngModel)]="formData.position" name="position" 
                       class="form-input" placeholder="VD: Nhân viên">
              </div>
            </div>
          </div>

          <!-- Thông tin thử việc -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
            <div class="flex items-center mb-6">
              <div class="w-10 h-10 bg-amber-100 rounded-lg flex items-center justify-center mr-3">
                <svg class="w-5 h-5 text-amber-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
              </div>
              <div>
                <h3 class="text-lg font-semibold text-gray-900">Thông tin thử việc</h3>
                <p class="text-sm text-gray-500">Quản lý thời gian thử việc của nhân viên</p>
              </div>
            </div>
            <div class="grid grid-cols-1 md:grid-cols-3 gap-5">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">Ngày bắt đầu thử việc</label>
                <input type="date" [(ngModel)]="formData.probationStartDate" name="probationStartDate" 
                       (change)="calculateProbationEndDate()" class="form-input">
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">Số ngày thử việc</label>
                <input type="number" [(ngModel)]="probationDays" name="probationDays" 
                       (change)="calculateProbationEndDate()" min="1" max="365"
                       class="form-input" placeholder="{{ defaultProbationDays() }}">
                <p class="text-xs text-gray-500 mt-1">Mặc định: {{ defaultProbationDays() }} ngày</p>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">Ngày kết thúc thử việc</label>
                <input type="date" [(ngModel)]="formData.probationEndDate" name="probationEndDate" 
                       class="form-input bg-gray-50" [readonly]="autoCalculateProbation()">
                @if (autoCalculateProbation()) {
                  <p class="text-xs text-primary-600 mt-1">Tự động tính từ ngày bắt đầu + số ngày</p>
                }
              </div>
            </div>
          </div>

          <!-- Trạng thái -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
            <div class="flex items-center mb-6">
              <div class="w-10 h-10 bg-blue-100 rounded-lg flex items-center justify-center mr-3">
                <svg class="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
              </div>
              <div>
                <h3 class="text-lg font-semibold text-gray-900">Trạng thái làm việc</h3>
                <p class="text-sm text-gray-500">Hợp đồng sẽ được thêm riêng sau khi tạo nhân viên</p>
              </div>
            </div>
            <div class="grid grid-cols-1 md:grid-cols-2 gap-5">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">Trạng thái</label>
                <select [(ngModel)]="formData.status" name="status" class="form-input">
                  @for (status of statusOptions; track status.value) {
                    <option [ngValue]="status.value">{{ status.label }}</option>
                  }
                </select>
              </div>
              <div class="flex items-center">
                <div class="p-4 bg-blue-50 border border-blue-200 rounded-lg">
                  <p class="text-sm text-blue-700">
                    <svg class="w-4 h-4 inline mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M13 16h-1v-4h-1m1-4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                    </svg>
                    Để thêm hợp đồng, vui lòng lưu nhân viên trước, sau đó vào trang chi tiết để thêm hợp đồng.
                  </p>
                </div>
              </div>
            </div>
          </div>

          <!-- Ghi chú -->
          <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
            <div class="flex items-center mb-6">
              <div class="w-10 h-10 bg-gray-100 rounded-lg flex items-center justify-center mr-3">
                <svg class="w-5 h-5 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
                </svg>
              </div>
              <div>
                <h3 class="text-lg font-semibold text-gray-900">Ghi chú</h3>
                <p class="text-sm text-gray-500">Thông tin bổ sung về nhân viên</p>
              </div>
            </div>
            <textarea 
              [(ngModel)]="formData.notes" 
              name="notes" 
              rows="4" 
              class="form-input"
              placeholder="Nhập ghi chú (nếu có)..."></textarea>
          </div>

          <!-- Error message -->
          @if (errorMessage()) {
            <div class="p-4 bg-red-50 border border-red-200 rounded-xl flex items-start">
              <svg class="w-5 h-5 text-red-500 mr-3 mt-0.5 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
              </svg>
              <p class="text-red-600">{{ errorMessage() }}</p>
            </div>
          }

          <!-- Buttons -->
          <div class="flex items-center justify-end space-x-4 pt-4">
            <a routerLink="/employees" class="btn-secondary px-6">Hủy</a>
            <button type="submit" [disabled]="!employeeForm.valid || saving()" class="btn-primary px-6">
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
                {{ isEditMode() ? 'Cập nhật' : 'Thêm mới' }}
              }
            </button>
          </div>
        </form>
      }
    </div>
  `
})
export class EmployeeFormComponent implements OnInit {
  isEditMode = signal(false);
  loading = signal(false);
  saving = signal(false);
  errorMessage = signal('');
  defaultProbationDays = signal(60);
  employeeId: string | null = null;
  probationDays: number | null = null;

  formData: CreateEmployeeDto = {
    fullName: '',
    email: '',
    phoneNumber: '',
    dateOfBirth: '',
    probationStartDate: '',
    probationEndDate: '',
    status: EmployeeStatus.Active,
    department: '',
    position: '',
    notes: ''
  };

  statusOptions = [
    { value: EmployeeStatus.Active, label: 'Đang làm việc' },
    { value: EmployeeStatus.Resigned, label: 'Đã nghỉ việc' }
  ];

  // Computed signals
  autoCalculateProbation = computed(() => {
    return !!this.formData.probationStartDate && (this.probationDays || this.defaultProbationDays());
  });

  constructor(
    private employeeService: EmployeeService,
    private settingsService: SettingsService,
    private router: Router,
    private route: ActivatedRoute
  ) {}

  ngOnInit(): void {
    this.loadDefaultSettings();
    
    this.employeeId = this.route.snapshot.paramMap.get('id');
    if (this.employeeId && this.employeeId !== 'new') {
      this.isEditMode.set(true);
      this.loadEmployee();
    }
  }

  loadDefaultSettings(): void {
    this.settingsService.getReminderSettings().subscribe({
      next: (result) => {
        if (result.success && result.data) {
          this.defaultProbationDays.set(result.data.defaultProbationDays || 60);
        }
      }
    });
  }

  loadEmployee(): void {
    if (!this.employeeId) return;
    
    this.loading.set(true);
    this.employeeService.getEmployee(this.employeeId).subscribe({
      next: (result) => {
        if (result.success && result.data) {
          const emp = result.data;
          this.formData = {
            fullName: emp.fullName,
            email: emp.email || '',
            phoneNumber: emp.phoneNumber || '',
            dateOfBirth: emp.dateOfBirth,
            probationStartDate: emp.probationStartDate || '',
            probationEndDate: emp.probationEndDate || '',
            status: emp.status,
            department: emp.department || '',
            position: emp.position || '',
            notes: emp.notes || ''
          };
          
          // Calculate probation days if both dates exist
          if (emp.probationStartDate && emp.probationEndDate) {
            const start = new Date(emp.probationStartDate);
            const end = new Date(emp.probationEndDate);
            this.probationDays = Math.round((end.getTime() - start.getTime()) / (1000 * 60 * 60 * 24));
          }
        }
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.errorMessage.set('Không thể tải thông tin nhân viên');
      }
    });
  }

  calculateProbationEndDate(): void {
    if (!this.formData.probationStartDate) return;
    
    const days = this.probationDays || this.defaultProbationDays();
    const startDate = new Date(this.formData.probationStartDate);
    const endDate = new Date(startDate);
    endDate.setDate(endDate.getDate() + days);
    
    this.formData.probationEndDate = endDate.toISOString().split('T')[0];
  }

  onSubmit(): void {
    this.saving.set(true);
    this.errorMessage.set('');

    const data = { ...this.formData };
    // Clean empty strings
    if (!data.email) data.email = undefined;
    if (!data.phoneNumber) data.phoneNumber = undefined;
    if (!data.probationStartDate) data.probationStartDate = undefined;
    if (!data.probationEndDate) data.probationEndDate = undefined;
    if (!data.department) data.department = undefined;
    if (!data.position) data.position = undefined;
    if (!data.notes) data.notes = undefined;

    const request = this.isEditMode() && this.employeeId
      ? this.employeeService.updateEmployee(this.employeeId, data as UpdateEmployeeDto)
      : this.employeeService.createEmployee(data);

    request.subscribe({
      next: (result) => {
        this.saving.set(false);
        if (result.success) {
          // Sau khi lưu, chuyển đến trang chi tiết
          if (!this.isEditMode() && result.data) {
            this.router.navigate(['/employees', result.data.id]);
          } else {
            this.router.navigate(['/employees', this.employeeId]);
          }
        } else {
          this.errorMessage.set(result.message || 'Có lỗi xảy ra');
        }
      },
      error: () => {
        this.saving.set(false);
        this.errorMessage.set('Không thể lưu thông tin. Vui lòng thử lại.');
      }
    });
  }
}
