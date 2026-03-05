// features/contract-types/contract-type-list/contract-type-list.component.ts
// Component quản lý loại hợp đồng

import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { ContractTypeService } from '../../../core/services/contract-type.service';
import { ContractType, CreateContractTypeDto, UpdateContractTypeDto } from '../../../core/models';

@Component({
  selector: 'app-contract-type-list',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="space-y-6">
      <!-- Header -->
      <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <h1 class="text-2xl font-bold text-gray-900">Quản lý Loại hợp đồng</h1>
          <p class="text-gray-500 mt-1">Cấu hình các loại hợp đồng lao động trong hệ thống</p>
        </div>
        <button (click)="openModal()" class="btn-primary inline-flex items-center justify-center">
          <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"/>
          </svg>
          Thêm loại hợp đồng
        </button>
      </div>

      <!-- List -->
      @if (loading()) {
        <div class="flex justify-center py-12">
          <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
        </div>
      } @else {
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-5">
          @for (type of contractTypes(); track type.id) {
            <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-5 hover:shadow-md transition-shadow">
              <div class="flex items-start justify-between">
                <div class="flex-1">
                  <div class="flex items-center">
                    <div class="w-10 h-10 bg-blue-100 rounded-lg flex items-center justify-center mr-3">
                      <svg class="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
                      </svg>
                    </div>
                    <div>
                      <h3 class="font-semibold text-gray-900">{{ type.name }}</h3>
                      <p class="text-sm text-gray-500">
                        {{ type.durationMonths ? type.durationMonths + ' tháng' : 'Không thời hạn' }}
                      </p>
                    </div>
                  </div>
                  @if (type.description) {
                    <p class="text-sm text-gray-600 mt-3 pl-13">{{ type.description }}</p>
                  }
                </div>
                <span 
                  class="px-2.5 py-1 rounded-full text-xs font-medium"
                  [class.bg-green-100]="type.isActive"
                  [class.text-green-700]="type.isActive"
                  [class.bg-gray-100]="!type.isActive"
                  [class.text-gray-600]="!type.isActive">
                  {{ type.isActive ? 'Hoạt động' : 'Vô hiệu' }}
                </span>
              </div>
              <div class="mt-4 pt-4 border-t border-gray-100 flex items-center justify-between">
                <div class="flex items-center text-sm text-gray-500">
                  <svg class="w-4 h-4 mr-1.5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M17 20h5v-2a3 3 0 00-5.356-1.857M17 20H7m10 0v-2c0-.656-.126-1.283-.356-1.857M7 20H2v-2a3 3 0 015.356-1.857M7 20v-2c0-.656.126-1.283.356-1.857m0 0a5.002 5.002 0 019.288 0M15 7a3 3 0 11-6 0 3 3 0 016 0z"/>
                  </svg>
                  {{ type.employeeCount }} nhân viên
                </div>
                <div class="flex items-center space-x-1">
                  <button 
                    (click)="openModal(type)"
                    class="p-2 text-gray-500 hover:text-primary-600 hover:bg-primary-50 rounded-lg transition-colors"
                    title="Chỉnh sửa">
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
                    </svg>
                  </button>
                  <button 
                    (click)="confirmDelete(type)"
                    class="p-2 text-gray-500 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                    title="Xóa">
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                    </svg>
                  </button>
                </div>
              </div>
            </div>
          } @empty {
            <div class="col-span-full">
              <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-12 text-center">
                <svg class="w-16 h-16 text-gray-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
                </svg>
                <p class="text-gray-500 font-medium">Chưa có loại hợp đồng nào</p>
                <p class="text-gray-400 text-sm mt-1">Bắt đầu bằng cách thêm loại hợp đồng đầu tiên</p>
                <button (click)="openModal()" class="btn-primary mt-4">
                  <svg class="w-5 h-5 mr-2 inline" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"/>
                  </svg>
                  Thêm loại hợp đồng
                </button>
              </div>
            </div>
          }
        </div>
      }

      <!-- Modal -->
      @if (showModal()) {
        <div class="fixed inset-0 bg-gray-900 bg-opacity-50 overflow-y-auto h-full w-full z-50 flex items-center justify-center p-4">
          <div class="bg-white rounded-2xl shadow-xl max-w-md w-full transform transition-all">
            <div class="px-6 py-5 border-b border-gray-200">
              <div class="flex items-center">
                <div class="w-10 h-10 bg-primary-100 rounded-lg flex items-center justify-center mr-3">
                  <svg class="w-5 h-5 text-primary-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
                  </svg>
                </div>
                <div>
                  <h3 class="text-lg font-semibold text-gray-900">
                    {{ editingType() ? 'Chỉnh sửa loại hợp đồng' : 'Thêm loại hợp đồng mới' }}
                  </h3>
                  <p class="text-sm text-gray-500">{{ editingType() ? 'Cập nhật thông tin loại hợp đồng' : 'Tạo loại hợp đồng mới cho hệ thống' }}</p>
                </div>
              </div>
            </div>
            <form (ngSubmit)="saveContractType()" #typeForm="ngForm" class="p-6 space-y-5">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">
                  Tên loại hợp đồng <span class="text-red-500">*</span>
                </label>
                <input type="text" [(ngModel)]="formData.name" name="name" required 
                       class="form-input" placeholder="VD: Hợp đồng thử việc">
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">Thời hạn (tháng)</label>
                <input type="number" [(ngModel)]="formData.durationMonths" name="durationMonths" min="1" 
                       class="form-input" placeholder="Để trống = Không thời hạn">
                <p class="text-xs text-gray-500 mt-1">Để trống nếu là hợp đồng không xác định thời hạn</p>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">Mô tả</label>
                <textarea [(ngModel)]="formData.description" name="description" rows="3" 
                          class="form-input" placeholder="Mô tả chi tiết về loại hợp đồng..."></textarea>
              </div>
              @if (editingType()) {
                <div class="flex items-center p-3 bg-gray-50 rounded-lg">
                  <input type="checkbox" [(ngModel)]="formData.isActive" name="isActive" id="isActive" 
                         class="w-4 h-4 rounded border-gray-300 text-primary-600 focus:ring-primary-500">
                  <label for="isActive" class="ml-3 text-sm text-gray-700">Loại hợp đồng đang hoạt động</label>
                </div>
              }
              <div class="flex justify-end space-x-3 pt-4 border-t border-gray-200">
                <button type="button" (click)="closeModal()" class="btn-secondary px-5">Hủy</button>
                <button type="submit" [disabled]="!typeForm.valid || saving()" class="btn-primary px-5">
                  @if (saving()) {
                    <svg class="animate-spin -ml-1 mr-2 h-4 w-4 text-white inline" fill="none" viewBox="0 0 24 24">
                      <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                      <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"></path>
                    </svg>
                    Đang lưu...
                  } @else {
                    Lưu
                  }
                </button>
              </div>
            </form>
          </div>
        </div>
      }
    </div>
  `
})
export class ContractTypeListComponent implements OnInit {
  contractTypes = signal<ContractType[]>([]);
  loading = signal(true);
  saving = signal(false);
  showModal = signal(false);
  editingType = signal<ContractType | null>(null);

  formData: CreateContractTypeDto & { isActive?: boolean } = {
    name: '',
    durationMonths: undefined,
    description: '',
    isActive: true
  };

  constructor(private contractTypeService: ContractTypeService) {}

  ngOnInit(): void {
    this.loadContractTypes();
  }

  loadContractTypes(): void {
    this.loading.set(true);
    this.contractTypeService.getAll(false).subscribe({
      next: (result) => {
        if (result.success && result.data) {
          this.contractTypes.set(result.data);
        }
        this.loading.set(false);
      },
      error: () => this.loading.set(false)
    });
  }

  openModal(type?: ContractType): void {
    if (type) {
      this.editingType.set(type);
      this.formData = {
        name: type.name,
        durationMonths: type.durationMonths,
        description: type.description || '',
        isActive: type.isActive
      };
    } else {
      this.editingType.set(null);
      this.formData = { name: '', durationMonths: undefined, description: '', isActive: true };
    }
    this.showModal.set(true);
  }

  closeModal(): void {
    this.showModal.set(false);
    this.editingType.set(null);
  }

  saveContractType(): void {
    this.saving.set(true);
    const editing = this.editingType();

    const request = editing
      ? this.contractTypeService.update(editing.id, this.formData as UpdateContractTypeDto)
      : this.contractTypeService.create(this.formData);

    request.subscribe({
      next: (result) => {
        this.saving.set(false);
        if (result.success) {
          this.closeModal();
          this.loadContractTypes();
        }
      },
      error: () => this.saving.set(false)
    });
  }

  confirmDelete(type: ContractType): void {
    if (confirm(`Bạn có chắc muốn xóa loại hợp đồng "${type.name}"?`)) {
      this.contractTypeService.delete(type.id).subscribe({
        next: (result) => {
          if (result.success) {
            this.loadContractTypes();
          }
        }
      });
    }
  }
}
