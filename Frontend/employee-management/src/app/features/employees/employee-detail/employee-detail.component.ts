// features/employees/employee-detail/employee-detail.component.ts
// Component xem chi tiết nhân viên với quản lý hợp đồng và lịch sử chỉnh sửa

import { Component, OnInit, signal, ElementRef, ViewChild } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router, ActivatedRoute, RouterLink } from '@angular/router';
import { EmployeeService } from '../../../core/services/employee.service';
import { ContractTypeService } from '../../../core/services/contract-type.service';
import { 
  Employee, 
  EmployeeFile, 
  EmployeeEditHistory, 
  EmployeeStatus, 
  EmployeeContract,
  CreateEmployeeContractDto,
  UpdateEmployeeContractDto,
  ContractType
} from '../../../core/models';

@Component({
  selector: 'app-employee-detail',
  standalone: true,
  imports: [CommonModule, FormsModule, RouterLink],
  template: `
    <div class="space-y-6">
      <!-- Header -->
      <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-4">
        <div>
          <nav class="flex items-center space-x-2 text-sm text-gray-500 mb-2">
            <a routerLink="/employees" class="hover:text-primary-600">Nhân viên</a>
            <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 5l7 7-7 7"/>
            </svg>
            <span class="text-gray-900">Chi tiết</span>
          </nav>
          <h1 class="text-2xl font-bold text-gray-900">{{ employee()?.fullName || 'Chi tiết nhân viên' }}</h1>
        </div>
        <div class="flex items-center space-x-3">
          <a routerLink="/employees" class="btn-secondary">
            <svg class="w-5 h-5 mr-2 inline" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M10 19l-7-7m0 0l7-7m-7 7h18"/>
            </svg>
            Quay lại
          </a>
          <a [routerLink]="['/employees', employeeId, 'edit']" class="btn-primary">
            <svg class="w-5 h-5 mr-2 inline" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
            </svg>
            Chỉnh sửa
          </a>
        </div>
      </div>

      @if (loading()) {
        <div class="flex justify-center py-12">
          <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
        </div>
      } @else if (employee()) {
        <!-- Tabs - Mobile scrollable -->
        <div class="border-b border-gray-200 -mx-4 px-4 sm:mx-0 sm:px-0">
          <nav class="flex space-x-4 sm:space-x-8 overflow-x-auto scrollbar-hide pb-px">
            <button 
              (click)="activeTab.set('info')"
              [class.border-primary-500]="activeTab() === 'info'"
              [class.text-primary-600]="activeTab() === 'info'"
              [class.border-transparent]="activeTab() !== 'info'"
              [class.text-gray-500]="activeTab() !== 'info'"
              class="py-3 sm:py-4 px-1 border-b-2 font-medium text-sm transition-colors hover:text-gray-700 hover:border-gray-300 whitespace-nowrap flex-shrink-0">
              Thông tin
            </button>
            <button 
              (click)="activeTab.set('contracts'); loadContracts()"
              [class.border-primary-500]="activeTab() === 'contracts'"
              [class.text-primary-600]="activeTab() === 'contracts'"
              [class.border-transparent]="activeTab() !== 'contracts'"
              [class.text-gray-500]="activeTab() !== 'contracts'"
              class="py-3 sm:py-4 px-1 border-b-2 font-medium text-sm transition-colors hover:text-gray-700 hover:border-gray-300 whitespace-nowrap flex-shrink-0">
              Hợp đồng ({{ contracts().length }})
            </button>
            <button 
              (click)="activeTab.set('documents')"
              [class.border-primary-500]="activeTab() === 'documents'"
              [class.text-primary-600]="activeTab() === 'documents'"
              [class.border-transparent]="activeTab() !== 'documents'"
              [class.text-gray-500]="activeTab() !== 'documents'"
              class="py-3 sm:py-4 px-1 border-b-2 font-medium text-sm transition-colors hover:text-gray-700 hover:border-gray-300 whitespace-nowrap flex-shrink-0">
              Tài liệu ({{ files().length }})
            </button>
            <button 
              (click)="activeTab.set('history'); loadHistory()"
              [class.border-primary-500]="activeTab() === 'history'"
              [class.text-primary-600]="activeTab() === 'history'"
              [class.border-transparent]="activeTab() !== 'history'"
              [class.text-gray-500]="activeTab() !== 'history'"
              class="py-3 sm:py-4 px-1 border-b-2 font-medium text-sm transition-colors hover:text-gray-700 hover:border-gray-300 whitespace-nowrap flex-shrink-0">
              Lịch sử
            </button>
          </nav>
        </div>

        <!-- Tab Content: Info -->
        @if (activeTab() === 'info') {
          <div class="grid grid-cols-1 lg:grid-cols-3 gap-6">
            <!-- Main Info -->
            <div class="lg:col-span-2 space-y-6">
              <!-- Thông tin cơ bản -->
              <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                <h3 class="text-lg font-semibold text-gray-900 mb-4 flex items-center">
                  <svg class="w-5 h-5 mr-2 text-primary-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"/>
                  </svg>
                  Thông tin cơ bản
                </h3>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label class="text-sm text-gray-500">Họ và tên</label>
                    <p class="font-medium text-gray-900">{{ employee()!.fullName }}</p>
                  </div>
                  <div>
                    <label class="text-sm text-gray-500">Email</label>
                    <p class="font-medium text-gray-900">{{ employee()!.email || '-' }}</p>
                  </div>
                  <div>
                    <label class="text-sm text-gray-500">Số điện thoại</label>
                    <p class="font-medium text-gray-900">{{ employee()!.phoneNumber || '-' }}</p>
                  </div>
                  <div>
                    <label class="text-sm text-gray-500">Ngày sinh</label>
                    <p class="font-medium text-gray-900">{{ formatDate(employee()!.dateOfBirth) }}</p>
                  </div>
                  <div>
                    <label class="text-sm text-gray-500">Phòng ban</label>
                    <p class="font-medium text-gray-900">{{ employee()!.department || '-' }}</p>
                  </div>
                  <div>
                    <label class="text-sm text-gray-500">Chức vụ</label>
                    <p class="font-medium text-gray-900">{{ employee()!.position || '-' }}</p>
                  </div>
                </div>
              </div>

              <!-- Thông tin thử việc -->
              <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                <h3 class="text-lg font-semibold text-gray-900 mb-4 flex items-center">
                  <svg class="w-5 h-5 mr-2 text-amber-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"/>
                  </svg>
                  Thông tin thử việc
                </h3>
                <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <label class="text-sm text-gray-500">Ngày bắt đầu</label>
                    <p class="font-medium text-gray-900">{{ employee()!.probationStartDate ? formatDate(employee()!.probationStartDate!) : '-' }}</p>
                  </div>
                  <div>
                    <label class="text-sm text-gray-500">Ngày kết thúc</label>
                    <p class="font-medium text-gray-900">{{ employee()!.probationEndDate ? formatDate(employee()!.probationEndDate!) : '-' }}</p>
                  </div>
                </div>
              </div>

              <!-- Thông tin hợp đồng hiện tại -->
              <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                <h3 class="text-lg font-semibold text-gray-900 mb-4 flex items-center">
                  <svg class="w-5 h-5 mr-2 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
                  </svg>
                  Hợp đồng hiện tại
                </h3>
                @if (employee()!.activeContract) {
                  <div class="grid grid-cols-1 md:grid-cols-2 gap-4">
                    <div>
                      <label class="text-sm text-gray-500">Loại hợp đồng</label>
                      <p class="font-medium text-gray-900">{{ employee()!.activeContract!.contractTypeName }}</p>
                    </div>
                    <div class="flex flex-col sm:flex-row sm:items-center gap-1 sm:gap-3">
                      <label class="text-sm text-gray-500 whitespace-nowrap">Trạng thái</label>
                      <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium mt-1 sm:mt-0 border"
                            [class.bg-green-100]="employee()!.activeContract!.status === 'Đang thực hiện'"
                            [class.text-green-700]="employee()!.activeContract!.status === 'Đang thực hiện'"
                            [class.border-green-200]="employee()!.activeContract!.status === 'Đang thực hiện'"
                            [class.bg-gray-100]="employee()!.activeContract!.status !== 'Đang thực hiện'"
                            [class.text-gray-700]="employee()!.activeContract!.status !== 'Đang thực hiện'"
                            [class.border-gray-200]="employee()!.activeContract!.status !== 'Đang thực hiện'">
                        {{ employee()!.activeContract!.status }}
                      </span>
                    </div>
                    <div>
                      <label class="text-sm text-gray-500">Ngày bắt đầu</label>
                      <p class="font-medium text-gray-900">{{ formatDate(employee()!.activeContract!.startDate) }}</p>
                    </div>
                    <div>
                      <label class="text-sm text-gray-500">Ngày kết thúc</label>
                      <p class="font-medium text-gray-900">{{ employee()!.activeContract!.endDate ? formatDate(employee()!.activeContract!.endDate!) : 'Không thời hạn' }}</p>
                    </div>
                  </div>
                  <div class="mt-4 pt-4 border-t border-gray-100">
                    <a (click)="activeTab.set('contracts'); loadContracts()" 
                       class="text-primary-600 hover:text-primary-700 text-sm font-medium cursor-pointer">
                      Xem tất cả hợp đồng →
                    </a>
                  </div>
                } @else {
                  <div class="text-center py-4">
                    <p class="text-gray-500 mb-3">Chưa có hợp đồng nào</p>
                    <button (click)="activeTab.set('contracts'); loadContracts(); showContractModal.set(true)" 
                            class="btn-primary btn-sm">
                      Thêm hợp đồng
                    </button>
                  </div>
                }
              </div>

              <!-- Ghi chú -->
              @if (employee()!.notes) {
                <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                  <h3 class="text-lg font-semibold text-gray-900 mb-4 flex items-center">
                    <svg class="w-5 h-5 mr-2 text-gray-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
                    </svg>
                    Ghi chú
                  </h3>
                  <p class="text-gray-700 whitespace-pre-wrap">{{ employee()!.notes }}</p>
                </div>
              }
            </div>

            <!-- Sidebar -->
            <div class="space-y-6">
              <!-- Status Card -->
              <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                <div class="text-center">
                  <div class="w-20 h-20 bg-gradient-to-br from-primary-500 to-primary-600 rounded-full flex items-center justify-center text-white text-2xl font-bold mx-auto mb-4">
                    {{ employee()!.fullName.charAt(0) }}
                  </div>
                  <h3 class="font-semibold text-gray-900">{{ employee()!.fullName }}</h3>
                  <p class="text-sm text-gray-500">{{ employee()!.position || 'Chưa có chức vụ' }}</p>
                  <span 
                    class="inline-flex items-center px-3 py-1 rounded-full text-sm font-medium mt-3"
                    [class]="getStatusClass(employee()!.status)">
                    {{ employee()!.statusDisplayName }}
                  </span>
                </div>
              </div>

              <!-- Quick Stats -->
              <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-6">
                <h4 class="font-medium text-gray-900 mb-4">Thông tin nhanh</h4>
                <div class="space-y-3 text-sm">
                  <div class="flex justify-between">
                    <span class="text-gray-500">Ngày tạo</span>
                    <span class="font-medium">{{ formatDate(employee()!.createdAt) }}</span>
                  </div>
                  @if (employee()!.updatedAt) {
                    <div class="flex justify-between">
                      <span class="text-gray-500">Cập nhật lần cuối</span>
                      <span class="font-medium">{{ formatDate(employee()!.updatedAt!) }}</span>
                    </div>
                  }
                  <div class="flex justify-between">
                    <span class="text-gray-500">Số tài liệu</span>
                    <span class="font-medium">{{ files().length }}</span>
                  </div>
                </div>
              </div>
            </div>
          </div>
        }

        <!-- Tab Content: Contracts -->
        @if (activeTab() === 'contracts') {
          <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-4 sm:p-6">
            <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3 mb-4 sm:mb-6">
              <h3 class="text-lg font-semibold text-gray-900">Lịch sử hợp đồng</h3>
              <button (click)="showContractModal.set(true)" class="btn-primary w-full sm:w-auto"
                      [disabled]="hasActiveContract()">
                <svg class="w-5 h-5 mr-2 inline" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"/>
                </svg>
                Thêm hợp đồng
              </button>
            </div>

            @if (hasActiveContract()) {
              <div class="mb-4 p-3 bg-amber-50 border border-amber-200 rounded-lg text-sm text-amber-700">
                <svg class="w-4 h-4 inline mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"/>
                </svg>
                Nhân viên đang có hợp đồng. Vui lòng kết thúc hợp đồng hiện tại trước khi thêm mới.
              </div>
            }

            @if (contractsLoading()) {
              <div class="flex justify-center py-8">
                <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
              </div>
            } @else if (contracts().length === 0) {
              <div class="text-center py-12">
                <svg class="w-16 h-16 text-gray-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M9 12h6m-6 4h6m2 5H7a2 2 0 01-2-2V5a2 2 0 012-2h5.586a1 1 0 01.707.293l5.414 5.414a1 1 0 01.293.707V19a2 2 0 01-2 2z"/>
                </svg>
                <p class="text-gray-500 font-medium">Chưa có hợp đồng nào</p>
                <p class="text-gray-400 text-sm mt-1">Thêm hợp đồng đầu tiên cho nhân viên này</p>
              </div>
            } @else {
              <div class="space-y-4">
                @for (contract of contracts(); track contract.id) {
                  <div class="border rounded-lg p-3 sm:p-4 hover:bg-gray-50 transition-colors"
                       [class.border-green-300]="contract.status === 'Đang thực hiện'"
                       [class.bg-green-50]="contract.status === 'Đang thực hiện'"
                       [class.border-gray-200]="contract.status !== 'Đang thực hiện'">
                    <div class="flex flex-col sm:flex-row sm:items-start sm:justify-between gap-2 sm:gap-0">
                      <div class="flex-1">
                        <div class="flex flex-wrap items-center gap-2 sm:gap-3 mb-2">
                          <h4 class="font-medium text-gray-900">{{ contract.contractTypeName }}</h4>
                          <span class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium border"
                                [class.bg-green-100]="contract.status === 'Đang thực hiện'"
                                [class.text-green-700]="contract.status === 'Đang thực hiện'"
                                [class.border-green-200]="contract.status === 'Đang thực hiện'"
                                [class.bg-gray-100]="contract.status !== 'Đang thực hiện'"
                                [class.text-gray-700]="contract.status !== 'Đang thực hiện'"
                                [class.border-gray-200]="contract.status !== 'Đang thực hiện'">
                            {{ contract.status }}
                          </span>
                        </div>
                        <div class="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-2 sm:gap-4 text-sm">
                          <div class="flex justify-between sm:block">
                            <span class="text-gray-500">Bắt đầu:</span>
                            <span class="sm:ml-1 font-medium">{{ formatDate(contract.startDate) }}</span>
                          </div>
                          <div class="flex justify-between sm:block">
                            <span class="text-gray-500">Kết thúc:</span>
                            <span class="sm:ml-1 font-medium">{{ contract.endDate ? formatDate(contract.endDate) : 'Không thời hạn' }}</span>
                          </div>
                          <div class="flex justify-between sm:block">
                            <span class="text-gray-500">Thời hạn:</span>
                            <span class="sm:ml-1 font-medium">{{ contract.durationDays ? contract.durationDays + ' ngày' : 'Không thời hạn' }}</span>
                          </div>
                          <div class="flex justify-between sm:block">
                            <span class="text-gray-500">Tạo lúc:</span>
                            <span class="sm:ml-1 font-medium">{{ formatDate(contract.createdAt) }}</span>
                          </div>
                        </div>
                        @if (contract.notes) {
                          <p class="mt-2 text-sm text-gray-600">{{ contract.notes }}</p>
                        }
                      </div>
                      <div class="flex gap-2 sm:ml-4">
                        <button (click)="openEditContractModal(contract)"
                                class="w-full sm:w-auto px-3 py-1.5 text-sm text-blue-600 hover:bg-blue-50 rounded-lg transition-colors border border-blue-200 sm:border-0">
                          Sửa HĐ
                        </button>
                        @if (contract.status === 'Đang thực hiện') {
                          <button (click)="confirmTerminateContract(contract)"
                                  class="w-full sm:w-auto px-3 py-1.5 text-sm text-red-600 hover:bg-red-50 rounded-lg transition-colors border border-red-200 sm:border-0">
                            Kết thúc HĐ
                          </button>
                        }
                      </div>
                    </div>
                  </div>
                }
              </div>
            }
          </div>
        }

        <!-- Tab Content: Documents -->
        @if (activeTab() === 'documents') {
          <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-4 sm:p-6">
            <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-3 mb-4 sm:mb-6">
              <h3 class="text-lg font-semibold text-gray-900">Tài liệu đính kèm</h3>
              <button (click)="triggerFileInput()" class="btn-primary w-full sm:w-auto">
                <svg class="w-5 h-5 mr-2 inline" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-8l-4-4m0 0L8 8m4-4v12"/>
                </svg>
                Tải lên
              </button>
              <input 
                type="file" 
                #fileInput 
                (change)="onFileSelected($event)" 
                class="hidden"
                multiple>
            </div>

            <!-- Upload Progress -->
            @if (uploading()) {
              <div class="mb-6 p-4 bg-blue-50 rounded-lg">
                <div class="flex items-center">
                  <svg class="animate-spin h-5 w-5 text-blue-600 mr-3" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"></path>
                  </svg>
                  <span class="text-blue-700">Đang tải lên...</span>
                </div>
              </div>
            }

            <!-- File List -->
            @if (files().length === 0) {
              <div class="text-center py-12">
                <svg class="w-16 h-16 text-gray-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 21h10a2 2 0 002-2V9.414a1 1 0 00-.293-.707l-5.414-5.414A1 1 0 0012.586 3H7a2 2 0 00-2 2v14a2 2 0 002 2z"/>
                </svg>
                <p class="text-gray-500 font-medium">Chưa có tài liệu nào</p>
                <p class="text-gray-400 text-sm mt-1">Tải lên tài liệu đầu tiên cho nhân viên này</p>
              </div>
            } @else {
              <div class="space-y-3">
                @for (file of files(); track file.id) {
                  <div class="flex items-center justify-between p-4 bg-gray-50 rounded-lg hover:bg-gray-100 transition-colors">
                    <div class="flex items-center flex-1 min-w-0">
                      <div class="w-10 h-10 bg-blue-100 rounded-lg flex items-center justify-center mr-3 flex-shrink-0">
                        <svg class="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M7 21h10a2 2 0 002-2V9.414a1 1 0 00-.293-.707l-5.414-5.414A1 1 0 0012.586 3H7a2 2 0 00-2 2v14a2 2 0 002 2z"/>
                        </svg>
                      </div>
                      <div class="min-w-0 flex-1">
                        <p class="font-medium text-gray-900 truncate">{{ file.originalFileName }}</p>
                        <p class="text-sm text-gray-500">
                          {{ formatFileSize(file.fileSize) }} - {{ formatDate(file.uploadedAt) }}
                          @if (file.description) {
                            <span class="ml-2">• {{ file.description }}</span>
                          }
                        </p>
                      </div>
                    </div>
                    <div class="flex items-center space-x-2 ml-4">
                      <button 
                        (click)="downloadFile(file)"
                        class="p-2 text-gray-500 hover:text-primary-600 hover:bg-primary-50 rounded-lg transition-colors"
                        title="Tải xuống">
                        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 16v1a3 3 0 003 3h10a3 3 0 003-3v-1m-4-4l-4 4m0 0l-4-4m4 4V4"/>
                        </svg>
                      </button>
                      <button 
                        (click)="confirmDeleteFile(file)"
                        class="p-2 text-gray-500 hover:text-red-600 hover:bg-red-50 rounded-lg transition-colors"
                        title="Xóa">
                        <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                          <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                        </svg>
                      </button>
                    </div>
                  </div>
                }
              </div>
            }
          </div>
        }

        <!-- Tab Content: History -->
        @if (activeTab() === 'history') {
          <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-4 sm:p-6">
            <h3 class="text-lg font-semibold text-gray-900 mb-4 sm:mb-6">Lịch sử chỉnh sửa</h3>
            
            @if (historyLoading()) {
              <div class="flex justify-center py-8">
                <div class="animate-spin rounded-full h-8 w-8 border-b-2 border-primary-600"></div>
              </div>
            } @else if (history().length === 0) {
              <div class="text-center py-8 sm:py-12">
                <svg class="w-12 sm:w-16 h-12 sm:h-16 text-gray-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
                <p class="text-gray-500 font-medium">Chưa có lịch sử chỉnh sửa</p>
              </div>
            } @else {
              <div class="relative">
                <div class="absolute left-3 sm:left-4 top-0 bottom-0 w-0.5 bg-gray-200"></div>
                <div class="space-y-4 sm:space-y-6">
                  @for (item of history(); track item.id) {
                    <div class="relative pl-8 sm:pl-10">
                      <div class="absolute left-1 sm:left-2 w-4 sm:w-5 h-4 sm:h-5 rounded-full flex items-center justify-center"
                           [class.bg-green-500]="item.changeType === 'Create'"
                           [class.bg-blue-500]="item.changeType === 'Update'"
                           [class.bg-red-500]="item.changeType === 'Delete'">
                        @if (item.changeType === 'Create') {
                          <svg class="w-2.5 sm:w-3 h-2.5 sm:h-3 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 6v6m0 0v6m0-6h6m-6 0H6"/>
                          </svg>
                        } @else if (item.changeType === 'Update') {
                          <svg class="w-2.5 sm:w-3 h-2.5 sm:h-3 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
                          </svg>
                        } @else {
                          <svg class="w-2.5 sm:w-3 h-2.5 sm:h-3 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M19 7l-.867 12.142A2 2 0 0116.138 21H7.862a2 2 0 01-1.995-1.858L5 7m5 4v6m4-6v6m1-10V4a1 1 0 00-1-1h-4a1 1 0 00-1 1v3M4 7h16"/>
                          </svg>
                        }
                      </div>
                      <div class="bg-gray-50 rounded-lg p-3 sm:p-4">
                        <div class="flex flex-col sm:flex-row sm:items-center sm:justify-between gap-1 mb-2">
                          <span class="font-medium text-gray-900 text-sm sm:text-base">{{ item.fieldDisplayName }}</span>
                          <span class="text-xs text-gray-500">{{ formatDateTime(item.changedAt) }}</span>
                        </div>
                        <div class="text-sm">
                          @if (item.changeType === 'Create') {
                            <p class="text-gray-600">
                              <span class="text-green-600">Tạo mới:</span> {{ item.newValue || '-' }}
                            </p>
                          } @else if (item.changeType === 'Update') {
                            <p class="text-gray-600 break-words">
                              <span class="text-gray-500 line-through">{{ item.oldValue || '-' }}</span>
                              <span class="mx-1 sm:mx-2">→</span>
                              <span class="text-blue-600 font-medium">{{ item.newValue || '-' }}</span>
                            </p>
                          } @else {
                            <p class="text-gray-600">
                              <span class="text-red-600">Đã xóa:</span> {{ item.oldValue || '-' }}
                            </p>
                          }
                        </div>
                        <p class="text-xs text-gray-500 mt-2">
                          Bởi: {{ item.changedByName }}
                        </p>
                      </div>
                    </div>
                  }
                </div>
              </div>
            }
          </div>
        }
      } @else {
        <div class="bg-white rounded-xl shadow-sm border border-gray-200 p-12 text-center">
          <svg class="w-16 h-16 text-gray-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 8v4m0 4h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
          </svg>
          <p class="text-gray-500 font-medium">Không tìm thấy nhân viên</p>
          <a routerLink="/employees" class="btn-primary mt-4 inline-flex items-center">
            Quay lại danh sách
          </a>
        </div>
      }

      <!-- Contract Modal - Full screen on mobile -->
      @if (showContractModal()) {
        <div class="fixed inset-0 bg-black/50 flex items-end sm:items-center justify-center z-50" (click)="showContractModal.set(false)">
          <div class="bg-white rounded-t-xl sm:rounded-xl shadow-xl w-full sm:max-w-lg sm:mx-4 max-h-[90vh] overflow-y-auto" (click)="$event.stopPropagation()">
            <div class="sticky top-0 bg-white p-4 sm:p-6 border-b border-gray-200 z-10">
              <div class="flex items-center justify-between">
                <div>
                  <h3 class="text-lg font-semibold text-gray-900">Thêm hợp đồng mới</h3>
                  <p class="text-sm text-gray-500 mt-0.5 hidden sm:block">Tạo hợp đồng mới cho nhân viên {{ employee()?.fullName }}</p>
                </div>
                <button (click)="showContractModal.set(false)" class="sm:hidden p-2 -mr-2 text-gray-400 hover:text-gray-600">
                  <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
                  </svg>
                </button>
              </div>
            </div>
            <div class="p-4 sm:p-6 space-y-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">
                  Loại hợp đồng <span class="text-red-500">*</span>
                </label>
                <select [(ngModel)]="newContract.contractTypeId" class="form-input"
                        (change)="onContractTypeChange()">
                  <option value="">-- Chọn loại hợp đồng --</option>
                  @for (type of contractTypes(); track type.id) {
                    <option [value]="type.id">
                      {{ type.name }} {{ type.durationDays ? '(' + type.durationDays + ' ngày)' : '(Không thời hạn)' }}
                    </option>
                  }
                </select>
              </div>
              <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1.5">
                    Ngày bắt đầu <span class="text-red-500">*</span>
                  </label>
                  <input type="date" [(ngModel)]="newContract.startDate" 
                         (change)="calculateContractEndDate()" class="form-input">
                </div>
                <div>
                  <label class="block text-sm font-medium text-gray-700 mb-1.5">Ngày kết thúc</label>
                  <input type="date" [(ngModel)]="newContract.endDate" class="form-input"
                         [class.bg-gray-50]="selectedContractType()?.durationDays">
                  @if (selectedContractType()?.durationDays) {
                    <p class="text-xs text-primary-600 mt-1">Tự động tính theo loại HĐ ({{ selectedContractType()?.durationDays }} ngày)</p>
                  }
                </div>
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">Ghi chú</label>
                <textarea [(ngModel)]="newContract.notes" rows="3" class="form-input"
                          placeholder="Nhập ghi chú (nếu có)..."></textarea>
              </div>
              @if (contractError()) {
                <div class="p-3 bg-red-50 border border-red-200 rounded-lg text-sm text-red-600">
                  {{ contractError() }}
                </div>
              }
            </div>
            <div class="sticky bottom-0 bg-white p-4 sm:p-6 border-t border-gray-200 flex flex-col-reverse sm:flex-row sm:justify-end gap-2 sm:gap-3">
              <button (click)="showContractModal.set(false)" class="btn-secondary w-full sm:w-auto">Hủy</button>
              <button (click)="createContract()" class="btn-primary w-full sm:w-auto" 
                      [disabled]="contractSaving() || !newContract.contractTypeId || !newContract.startDate">
                @if (contractSaving()) {
                  <svg class="animate-spin -ml-1 mr-2 h-4 w-4 text-white inline" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"></path>
                  </svg>
                  Đang lưu...
                } @else {
                  Tạo hợp đồng
                }
              </button>
            </div>
          </div>
        </div>
      }
    </div>

    <!-- Terminate Contract Modal -->
    @if (showTerminateModal()) {
      <div class="fixed inset-0 bg-black/50 flex items-center justify-center z-50 p-4" (click)="closeTerminateModal()">
        <div class="bg-white rounded-xl shadow-xl w-full max-w-md" (click)="$event.stopPropagation()">
          <div class="p-6">
            <div class="flex items-center justify-between mb-4">
              <h3 class="text-lg font-semibold text-gray-900">Kết thúc hợp đồng</h3>
              <button (click)="closeTerminateModal()" class="text-gray-400 hover:text-gray-600">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
                </svg>
              </button>
            </div>
            
            @if (contractToTerminate()) {
              <div class="space-y-4">
                <div class="bg-red-50 border border-red-200 rounded-lg p-4">
                  <div class="flex items-start">
                    <svg class="w-5 h-5 text-red-500 mt-0.5 mr-3 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"/>
                    </svg>
                    <div>
                      <p class="text-red-700 font-medium">Bạn có chắc chắn muốn kết thúc hợp đồng này?</p>
                      <p class="text-red-600 text-sm mt-1">
                        Hợp đồng: <strong>{{ contractToTerminate()!.contractTypeName }}</strong><br>
                        Ngày bắt đầu: {{ formatDate(contractToTerminate()!.startDate) }}<br>
                        @if (contractToTerminate()!.endDate) {
                          Ngày kết thúc: {{ formatDate(contractToTerminate()!.endDate!) }}
                        } @else {
                          Không thời hạn
                        }
                      </p>
                    </div>
                  </div>
                </div>
                
                <div class="flex justify-end gap-3 pt-4">
                  <button (click)="closeTerminateModal()" class="btn-secondary" [disabled]="terminatingContract()">Hủy</button>
                  <button (click)="terminateContract()" class="bg-red-600 hover:bg-red-700 text-white px-4 py-2 rounded-lg transition-colors" [disabled]="terminatingContract()">
                    @if (terminatingContract()) {
                      <svg class="animate-spin -ml-1 mr-2 h-4 w-4 text-white inline" fill="none" viewBox="0 0 24 24">
                        <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                        <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"></path>
                      </svg>
                      Đang kết thúc...
                    } @else {
                      Kết thúc hợp đồng
                    }
                  </button>
                </div>
              </div>
            }
          </div>
        </div>
      </div>
    }

    <!-- Edit Contract Modal -->
    @if (showEditContractModal()) {
      <div class="fixed inset-0 bg-black/50 flex items-end sm:items-center justify-center z-50" (click)="showEditContractModal.set(false)">
        <div class="bg-white rounded-t-xl sm:rounded-xl shadow-xl w-full sm:max-w-lg sm:mx-4 max-h-[90vh] overflow-y-auto" (click)="$event.stopPropagation()">
          <div class="sticky top-0 bg-white p-4 sm:p-6 border-b border-gray-200 z-10">
            <div class="flex items-center justify-between">
              <div>
                <h3 class="text-lg font-semibold text-gray-900">Chỉnh sửa hợp đồng</h3>
                <p class="text-sm text-gray-500 mt-0.5 hidden sm:block">Sửa thông tin hợp đồng khi nhập sai</p>
              </div>
              <button (click)="showEditContractModal.set(false)" class="sm:hidden p-2 -mr-2 text-gray-400 hover:text-gray-600">
                <svg class="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
                </svg>
              </button>
            </div>
          </div>
          <div class="p-4 sm:p-6 space-y-4">
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1.5">
                Loại hợp đồng <span class="text-red-500">*</span>
              </label>
              <select [(ngModel)]="editContract.contractTypeId" class="form-input"
                      (change)="onEditContractTypeChange()">
                <option value="">-- Chọn loại hợp đồng --</option>
                @for (type of contractTypes(); track type.id) {
                  <option [value]="type.id">
                    {{ type.name }} {{ type.durationDays ? '(' + type.durationDays + ' ngày)' : '(Không thời hạn)' }}
                  </option>
                }
              </select>
            </div>
            <div class="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">
                  Ngày bắt đầu <span class="text-red-500">*</span>
                </label>
                <input type="date" [(ngModel)]="editContract.startDate" 
                       (change)="calculateEditContractEndDate()" class="form-input">
              </div>
              <div>
                <label class="block text-sm font-medium text-gray-700 mb-1.5">Ngày kết thúc</label>
                <input type="date" [(ngModel)]="editContract.endDate" class="form-input">
              </div>
            </div>
            <div>
              <label class="block text-sm font-medium text-gray-700 mb-1.5">Ghi chú</label>
              <textarea [(ngModel)]="editContract.notes" rows="3" class="form-input"
                        placeholder="Nhập ghi chú (nếu có)..."></textarea>
            </div>
            @if (editContractError()) {
              <div class="p-3 bg-red-50 border border-red-200 rounded-lg text-sm text-red-600">
                {{ editContractError() }}
              </div>
            }
          </div>
          <div class="sticky bottom-0 bg-white p-4 sm:p-6 border-t border-gray-200 flex flex-col-reverse sm:flex-row sm:justify-end gap-2 sm:gap-3">
            <button (click)="showEditContractModal.set(false)" class="btn-secondary w-full sm:w-auto">Hủy</button>
            <button (click)="updateContract()" class="btn-primary w-full sm:w-auto" 
                    [disabled]="editContractSaving() || !editContract.contractTypeId || !editContract.startDate">
              @if (editContractSaving()) {
                <svg class="animate-spin -ml-1 mr-2 h-4 w-4 text-white inline" fill="none" viewBox="0 0 24 24">
                  <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                  <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"></path>
                </svg>
                Đang lưu...
              } @else {
                Cập nhật hợp đồng
              }
            </button>
          </div>
        </div>
      </div>
    }
  `
})
export class EmployeeDetailComponent implements OnInit {
  @ViewChild('fileInput') fileInput!: ElementRef<HTMLInputElement>;

  employeeId: string = '';
  employee = signal<Employee | null>(null);
  files = signal<EmployeeFile[]>([]);
  history = signal<EmployeeEditHistory[]>([]);
  contracts = signal<EmployeeContract[]>([]);
  contractTypes = signal<ContractType[]>([]);
  loading = signal(true);
  uploading = signal(false);
  historyLoading = signal(false);
  contractsLoading = signal(false);
  contractSaving = signal(false);
  showContractModal = signal(false);
  contractError = signal('');
  showTerminateModal = signal(false);
  contractToTerminate = signal<EmployeeContract | null>(null);
  terminatingContract = signal(false);
  activeTab = signal<'info' | 'contracts' | 'documents' | 'history'>('info');

  // Edit contract state
  showEditContractModal = signal(false);
  editContractSaving = signal(false);
  editContractError = signal('');
  editingContractId = '';

  newContract: CreateEmployeeContractDto = {
    contractTypeId: '',
    startDate: '',
    endDate: '',
    notes: ''
  };

  editContract: UpdateEmployeeContractDto = {
    contractTypeId: '',
    startDate: '',
    endDate: '',
    notes: ''
  };

  constructor(
    private employeeService: EmployeeService,
    private contractTypeService: ContractTypeService,
    private route: ActivatedRoute,
    private router: Router
  ) {}

  ngOnInit(): void {
    this.employeeId = this.route.snapshot.paramMap.get('id') || '';
    if (this.employeeId) {
      this.loadEmployee();
      this.loadFiles();
      this.loadContracts(); // Load contracts ngay để hiển thị số lượng trên tab
    }
  }

  loadEmployee(): void {
    this.loading.set(true);
    this.employeeService.getEmployee(this.employeeId).subscribe({
      next: (result) => {
        if (result.success && result.data) {
          this.employee.set(result.data);
        }
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
      }
    });
  }

  loadFiles(): void {
    this.employeeService.getFiles(this.employeeId).subscribe({
      next: (result) => {
        if (result.success && result.data) {
          this.files.set(result.data);
        }
      }
    });
  }

  loadHistory(): void {
    if (this.history().length > 0) return; // Already loaded
    
    this.historyLoading.set(true);
    this.employeeService.getEditHistory(this.employeeId).subscribe({
      next: (result) => {
        if (result.success && result.data) {
          this.history.set(result.data);
        }
        this.historyLoading.set(false);
      },
      error: () => {
        this.historyLoading.set(false);
      }
    });
  }

  triggerFileInput(): void {
    this.fileInput.nativeElement.click();
  }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    if (!input.files || input.files.length === 0) return;

    const files = Array.from(input.files);
    this.uploadFiles(files);
    input.value = ''; // Reset input
  }

  uploadFiles(files: File[]): void {
    this.uploading.set(true);
    let completed = 0;

    files.forEach(file => {
      this.employeeService.uploadFile(this.employeeId, file).subscribe({
        next: (result) => {
          completed++;
          if (result.success && result.data) {
            this.files.update(current => [...current, result.data!]);
          }
          if (completed === files.length) {
            this.uploading.set(false);
          }
        },
        error: () => {
          completed++;
          if (completed === files.length) {
            this.uploading.set(false);
          }
        }
      });
    });
  }

  downloadFile(file: EmployeeFile): void {
    this.employeeService.downloadFile(this.employeeId, file.id).subscribe({
      next: (blob) => {
        const url = window.URL.createObjectURL(blob);
        const a = document.createElement('a');
        a.href = url;
        a.download = file.originalFileName;
        document.body.appendChild(a);
        a.click();
        document.body.removeChild(a);
        window.URL.revokeObjectURL(url);
      }
    });
  }

  confirmDeleteFile(file: EmployeeFile): void {
    if (confirm(`Bạn có chắc muốn xóa tài liệu "${file.originalFileName}"?`)) {
      this.employeeService.deleteFile(this.employeeId, file.id).subscribe({
        next: (result) => {
          if (result.success) {
            this.files.update(current => current.filter(f => f.id !== file.id));
          }
        }
      });
    }
  }

  formatDate(dateStr: string): string {
    const date = new Date(dateStr);
    return date.toLocaleDateString('vi-VN');
  }

  formatDateTime(dateStr: string): string {
    const date = new Date(dateStr);
    return date.toLocaleString('vi-VN');
  }

  formatFileSize(bytes: number): string {
    if (bytes === 0) return '0 Bytes';
    const k = 1024;
    const sizes = ['Bytes', 'KB', 'MB', 'GB'];
    const i = Math.floor(Math.log(bytes) / Math.log(k));
    return parseFloat((bytes / Math.pow(k, i)).toFixed(2)) + ' ' + sizes[i];
  }

  getStatusClass(status: EmployeeStatus): string {
    const classes: { [key: number]: string } = {
      [EmployeeStatus.Active]: 'bg-green-100 text-green-700',
      [EmployeeStatus.Resigned]: 'bg-gray-100 text-gray-700'
    };
    return classes[status] || 'bg-gray-100 text-gray-700';
  }

  // ========== CONTRACT METHODS ==========

  loadContracts(): void {
    if (this.contracts().length > 0) return; // Already loaded
    
    this.contractsLoading.set(true);
    this.loadContractTypes();
    
    this.employeeService.getContracts(this.employeeId).subscribe({
      next: (result) => {
        if (result.success && result.data) {
          this.contracts.set(result.data);
        }
        this.contractsLoading.set(false);
      },
      error: () => {
        this.contractsLoading.set(false);
      }
    });
  }

  loadContractTypes(): void {
    if (this.contractTypes().length > 0) return;
    
    this.contractTypeService.getAll().subscribe({
      next: (result) => {
        if (result.success && result.data) {
          this.contractTypes.set(result.data);
        }
      }
    });
  }

  hasActiveContract(): boolean {
    return this.contracts().some(c => c.status === 'Đang thực hiện');
  }

  selectedContractType(): ContractType | null {
    if (!this.newContract.contractTypeId) return null;
    return this.contractTypes().find(t => t.id === this.newContract.contractTypeId) || null;
  }

  onContractTypeChange(): void {
    this.calculateContractEndDate();
  }

  calculateContractEndDate(): void {
    if (!this.newContract.startDate || !this.newContract.contractTypeId) return;
    
    const contractType = this.selectedContractType();
    if (!contractType || !contractType.durationDays) {
      this.newContract.endDate = '';
      return;
    }
    
    const startDate = new Date(this.newContract.startDate);
    const endDate = new Date(startDate);
    endDate.setDate(endDate.getDate() + contractType.durationDays - 1);
    
    this.newContract.endDate = endDate.toISOString().split('T')[0];
  }

  createContract(): void {
    if (!this.newContract.contractTypeId || !this.newContract.startDate) return;
    
    this.contractSaving.set(true);
    this.contractError.set('');
    
    const dto: CreateEmployeeContractDto = {
      contractTypeId: this.newContract.contractTypeId,
      startDate: this.newContract.startDate,
      endDate: this.newContract.endDate || undefined,
      notes: this.newContract.notes || undefined
    };
    
    this.employeeService.createContract(this.employeeId, dto).subscribe({
      next: (result) => {
        this.contractSaving.set(false);
        if (result.success && result.data) {
          this.contracts.update(current => [result.data!, ...current]);
          this.showContractModal.set(false);
          this.resetContractForm();
          // Reload employee để cập nhật activeContract
          this.loadEmployee();
        } else {
          this.contractError.set(result.message || 'Có lỗi xảy ra');
        }
      },
      error: () => {
        this.contractSaving.set(false);
        this.contractError.set('Không thể tạo hợp đồng. Vui lòng thử lại.');
      }
    });
  }

  confirmTerminateContract(contract: EmployeeContract): void {
    this.contractToTerminate.set(contract);
    this.showTerminateModal.set(true);
  }

  closeTerminateModal(): void {
    this.showTerminateModal.set(false);
    this.contractToTerminate.set(null);
  }

  terminateContract(): void {
    const contract = this.contractToTerminate();
    if (!contract) return;

    this.terminatingContract.set(true);
    
    this.employeeService.terminateContract(this.employeeId, contract.id).subscribe({
      next: (result) => {
        this.terminatingContract.set(false);
        if (result.success && result.data) {
          // Update contract in list
          this.contracts.update(current => 
            current.map(c => c.id === contract.id ? result.data! : c)
          );
          // Reload employee để cập nhật activeContract
          this.loadEmployee();
          this.closeTerminateModal();
        }
      },
      error: () => {
        this.terminatingContract.set(false);
      }
    });
  }

  resetContractForm(): void {
    this.newContract = {
      contractTypeId: '',
      startDate: '',
      endDate: '',
      notes: ''
    };
    this.contractError.set('');
  }

  // ========== EDIT CONTRACT ==========

  openEditContractModal(contract: EmployeeContract): void {
    this.editingContractId = contract.id;
    this.editContract = {
      contractTypeId: contract.contractTypeId,
      startDate: contract.startDate,
      endDate: contract.endDate || '',
      notes: contract.notes || ''
    };
    this.editContractError.set('');
    this.loadContractTypes();
    this.showEditContractModal.set(true);
  }

  selectedEditContractType(): ContractType | null {
    if (!this.editContract.contractTypeId) return null;
    return this.contractTypes().find(t => t.id === this.editContract.contractTypeId) || null;
  }

  onEditContractTypeChange(): void {
    this.calculateEditContractEndDate();
  }

  calculateEditContractEndDate(): void {
    if (!this.editContract.startDate || !this.editContract.contractTypeId) return;
    
    const contractType = this.selectedEditContractType();
    if (!contractType || !contractType.durationDays) {
      this.editContract.endDate = '';
      return;
    }
    
    const startDate = new Date(this.editContract.startDate);
    const endDate = new Date(startDate);
    endDate.setDate(endDate.getDate() + contractType.durationDays - 1);
    
    this.editContract.endDate = endDate.toISOString().split('T')[0];
  }

  updateContract(): void {
    if (!this.editContract.contractTypeId || !this.editContract.startDate) return;
    
    this.editContractSaving.set(true);
    this.editContractError.set('');
    
    const dto: UpdateEmployeeContractDto = {
      contractTypeId: this.editContract.contractTypeId,
      startDate: this.editContract.startDate,
      endDate: this.editContract.endDate || undefined,
      notes: this.editContract.notes || undefined
    };
    
    this.employeeService.updateContract(this.employeeId, this.editingContractId, dto).subscribe({
      next: (result) => {
        this.editContractSaving.set(false);
        if (result.success && result.data) {
          this.contracts.update(current =>
            current.map(c => c.id === this.editingContractId ? result.data! : c)
          );
          this.showEditContractModal.set(false);
          this.loadEmployee();
        } else {
          this.editContractError.set(result.message || 'Có lỗi xảy ra');
        }
      },
      error: () => {
        this.editContractSaving.set(false);
        this.editContractError.set('Không thể cập nhật hợp đồng. Vui lòng thử lại.');
      }
    });
  }
}
