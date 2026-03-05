// features/employees/employee-list/employee-list.component.ts
// Component danh sách nhân viên

import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { RouterLink } from '@angular/router';
import { EmployeeService } from '../../../core/services/employee.service';
import { ContractTypeService } from '../../../core/services/contract-type.service';
import { Employee, EmployeeFilter, EmployeeStatus, EmployeeStatusLabels, ContractType } from '../../../core/models';
import { PagedResult } from '../../../core/models/api-result.model';
import { EmployeeCardComponent } from '../employee-card/employee-card.component';

@Component({
  selector: 'app-employee-list',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink, EmployeeCardComponent],
  template: `
    <div class="space-y-6">
      <!-- Header -->
      <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 class="text-2xl font-bold text-gray-900">Quản lý Nhân viên</h1>
          <p class="text-gray-500 mt-1">Danh sách tất cả nhân viên trong hệ thống</p>
        </div>
        <div class="flex items-center gap-3">
          <a routerLink="/employees/new" class="btn-primary inline-flex items-center justify-center">
            <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"/>
            </svg>
            Thêm nhân viên
          </a>
        </div>
      </div>

      <!-- Filters -->
      <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-4 sm:p-6">
        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-3 sm:gap-4">
          <!-- Search -->
          <div class="sm:col-span-2 lg:col-span-1">
            <label class="block text-sm font-medium text-gray-700 mb-1.5">Tìm kiếm</label>
            <div class="relative">
              <input 
                type="text" 
                [(ngModel)]="filter.searchTerm"
                (keyup.enter)="search()"
                placeholder="Tên, email, SĐT..."
                class="form-input pl-10">
              <svg class="w-5 h-5 text-gray-400 absolute left-3 top-1/2 -translate-y-1/2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"/>
              </svg>
            </div>
          </div>

          <!-- Status Filter -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1.5">Trạng thái</label>
            <select [(ngModel)]="filter.status" (change)="search()" class="form-input">
              <option [ngValue]="undefined">Tất cả</option>
              @for (status of statusOptions; track status.value) {
                <option [ngValue]="status.value">{{ status.label }}</option>
              }
            </select>
          </div>

          <!-- Contract Type Filter -->
          <div>
            <label class="block text-sm font-medium text-gray-700 mb-1.5">Loại HĐ</label>
            <select [(ngModel)]="filter.contractTypeId" (change)="search()" class="form-input">
              <option [ngValue]="undefined">Tất cả</option>
              @for (type of contractTypes(); track type.id) {
                <option [value]="type.id">{{ type.name }}</option>
              }
            </select>
          </div>

          <!-- Search Button -->
          <div class="flex items-end">
            <button (click)="search()" class="btn-primary w-full">
              <svg class="w-5 h-5 mr-2 inline" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"/>
              </svg>
              Tìm
            </button>
          </div>
        </div>
      </div>

      <!-- Results -->
      @if (loading()) {
        <div class="flex justify-center py-12">
          <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
        </div>
      } @else {
        <!-- Card Layout -->
        <div class="equal-height-grid">
          @for (employee of employees(); track employee.id) {
            <app-employee-card 
              [employee]="employee"
              (viewDetail)="onViewDetail($event)"
              (edit)="onEdit($event)">
            </app-employee-card>
          } @empty {
            <div class="col-span-full">
              <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-12 text-center">
                <svg class="w-16 h-16 text-gray-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0z"/>
                </svg>
                <h3 class="text-lg font-medium text-gray-900 mb-2">Không tìm thấy nhân viên nào</h3>
                <p class="text-gray-500">Thử thay đổi bộ lọc hoặc thêm nhân viên mới</p>
              </div>
            </div>
          }
        </div>

        <!-- Pagination -->
        @if (pagedResult() && pagedResult()!.totalPages > 1) {
          <div class="mt-8 flex flex-col sm:flex-row items-center justify-between gap-4">
            <div class="text-sm text-gray-600">
              Hiển thị <span class="font-medium">{{ (filter.pageNumber - 1) * filter.pageSize + 1 }}</span> - 
              <span class="font-medium">{{ Math.min(filter.pageNumber * filter.pageSize, pagedResult()!.totalCount) }}</span> 
              / <span class="font-medium">{{ pagedResult()!.totalCount }}</span> kết quả
            </div>
            <div class="flex items-center space-x-2">
              <button 
                (click)="goToPage(filter.pageNumber - 1)"
                [disabled]="!pagedResult()!.hasPrevious"
                class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition-colors">
                Trước
              </button>
              <span class="px-4 py-2 text-sm text-gray-600">
                Trang {{ filter.pageNumber }} / {{ pagedResult()!.totalPages }}
              </span>
              <button 
                (click)="goToPage(filter.pageNumber + 1)"
                [disabled]="!pagedResult()!.hasNext"
                class="px-4 py-2 text-sm font-medium text-gray-700 bg-white border border-gray-300 rounded-lg hover:bg-gray-50 disabled:opacity-50 disabled:cursor-not-allowed transition-colors">
                Sau
              </button>
            </div>
          </div>
        }
      }
    </div>

    <!-- Import Modal -->
    <!-- Removed as per user request -->
  `
})
export class EmployeeListComponent implements OnInit {
  employees = signal<Employee[]>([]);
  pagedResult = signal<PagedResult<Employee> | null>(null);
  contractTypes = signal<ContractType[]>([]);
  loading = signal(true);

  filter: EmployeeFilter = {
    pageNumber: 1,
    pageSize: 10
  };

  statusOptions = [
    { value: EmployeeStatus.Active, label: 'Đang làm việc' },
    { value: EmployeeStatus.Resigned, label: 'Đã nghỉ việc' }
  ];

  Math = Math;

  constructor(
    private employeeService: EmployeeService,
    private contractTypeService: ContractTypeService
  ) {}

  ngOnInit(): void {
    this.loadContractTypes();
    this.loadEmployees();
  }

  loadContractTypes(): void {
    this.contractTypeService.getAll().subscribe({
      next: (result) => {
        if (result.success && result.data) {
          this.contractTypes.set(result.data);
        }
      }
    });
  }

  loadEmployees(): void {
    this.loading.set(true);
    this.employeeService.getEmployees(this.filter).subscribe({
      next: (result) => {
        if (result.success && result.data) {
          this.pagedResult.set(result.data);
          this.employees.set(result.data.items);
        }
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      }
    });
  }

  search(): void {
    this.filter.pageNumber = 1;
    this.loadEmployees();
  }

  goToPage(page: number): void {
    this.filter.pageNumber = page;
    this.loadEmployees();
  }

  confirmDelete(employee: Employee): void {
    if (confirm(`Bạn có chắc muốn xóa nhân viên "${employee.fullName}"?`)) {
      this.employeeService.deleteEmployee(employee.id).subscribe({
        next: (result) => {
          if (result.success) {
            this.loadEmployees();
          }
        }
      });
    }
  }

  getStatusBadgeClass(status: EmployeeStatus): string {
    const classes: { [key: number]: string } = {
      [EmployeeStatus.Active]: 'bg-green-100 text-green-700',
      [EmployeeStatus.Resigned]: 'bg-gray-100 text-gray-700'
    };
    return classes[status] || 'bg-gray-100 text-gray-700';
  }

  formatDate(dateStr: string): string {
    const date = new Date(dateStr);
    return date.toLocaleDateString('vi-VN');
  }

  // ========== CARD EVENT HANDLERS ==========
  
  onViewDetail(employeeId: string): void {
    // Navigate to employee detail page
    window.location.href = `/employees/${employeeId}`;
  }

  onEdit(employeeId: string): void {
    // Navigate to employee edit page
    window.location.href = `/employees/${employeeId}/edit`;
  }
}
