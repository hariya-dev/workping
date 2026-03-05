// features/email-templates/email-templates.component.ts
// Component quản lý email templates

import { Component, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EmailTemplateService } from '../../core/services/email-template.service';
import { 
  EmailTemplateSummaryDto, 
  EmailTemplateDto,
  UpsertEmailTemplateDto,
  TemplatePlaceholderInfoDto,
  TemplatePreviewResultDto,
  EmailTemplateType,
  TemplateTypeLabels,
  TemplateTypeColors
} from '../../core/models/email-template.model';

@Component({
  selector: 'app-email-templates',
  standalone: true,
  imports: [CommonModule, FormsModule],
  template: `
    <div class="space-y-6">
      <!-- Header -->
      <div class="flex items-center justify-between">
        <div>
          <h1 class="text-2xl font-bold text-gray-900">Mẫu Email</h1>
          <p class="text-gray-500 mt-1">Quản lý các mẫu email tự động của hệ thống</p>
        </div>
        <button (click)="openCreateDialog()" class="btn-primary">
          <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M12 4v16m8-8H4"/>
          </svg>
          Thêm mẫu
        </button>
      </div>

      <!-- Loading -->
      @if (loading()) {
        <div class="flex justify-center py-12">
          <div class="animate-spin rounded-full h-12 w-12 border-b-2 border-primary-600"></div>
        </div>
      } @else {
        <!-- Templates Grid -->
        <div class="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          @for (template of templates(); track template.id) {
            <div class="bg-white rounded-xl shadow-sm border border-gray-200 overflow-hidden hover:shadow-md transition-shadow">
              <div class="p-5">
                <!-- Header -->
                <div class="flex items-start justify-between mb-3">
                  <div class="flex-1 min-w-0">
                    <h3 class="text-base font-semibold text-gray-900 truncate">{{ template.name }}</h3>
                    <span class="inline-flex items-center px-2 py-0.5 rounded text-xs font-medium mt-1 {{ getTypeColor(template.type) }}">
                      {{ getTypeLabel(template.type) }}
                    </span>
                  </div>
                  @if (!template.isActive) {
                    <span class="px-2 py-1 bg-gray-100 text-gray-600 text-xs rounded">Vô hiệu</span>
                  }
                </div>
                
                <!-- Subject -->
                <p class="text-sm text-gray-600 mb-3 line-clamp-2">{{ template.subject }}</p>
                
                <!-- Updated -->
                @if (template.updatedAt) {
                  <p class="text-xs text-gray-400 mb-3">
                    Cập nhật: {{ template.updatedAt | date:'dd/MM/yyyy HH:mm' }}
                  </p>
                }

                <!-- Actions -->
                <div class="flex items-center gap-2 pt-3 border-t border-gray-100">
                  <button (click)="editTemplate(template.id)" class="flex-1 btn-secondary text-sm py-2">
                    <svg class="w-4 h-4 mr-1 inline" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M11 5H6a2 2 0 00-2 2v11a2 2 0 002 2h11a2 2 0 002-2v-5m-1.414-9.414a2 2 0 112.828 2.828L11.828 15H9v-2.828l8.586-8.586z"/>
                    </svg>
                    Sửa
                  </button>
                  <button (click)="resetTemplate(template.id)" class="px-3 py-2 text-sm text-gray-600 hover:bg-gray-100 rounded-lg transition-colors" title="Khôi phục mặc định">
                    <svg class="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"/>
                    </svg>
                  </button>
                </div>
              </div>
            </div>
          } @empty {
            <div class="col-span-full text-center py-12 bg-white rounded-xl border border-gray-200">
              <svg class="w-12 h-12 text-gray-300 mx-auto mb-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"/>
              </svg>
              <p class="text-gray-500">Không có mẫu email nào</p>
            </div>
          }
        </div>
      }

      <!-- Edit Dialog -->
      @if (showDialog()) {
        <div class="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div class="bg-white rounded-2xl shadow-xl w-full max-w-4xl max-h-[90vh] overflow-hidden flex flex-col">
            <!-- Dialog Header -->
            <div class="flex items-center justify-between p-6 border-b border-gray-200">
              <div>
                <h2 class="text-xl font-bold text-gray-900">{{ isEditing() ? 'Sửa mẫu email' : 'Thêm mẫu email' }}</h2>
                <p class="text-sm text-gray-500 mt-1">{{ formData.name || 'Nhập tên mẫu' }}</p>
              </div>
              <button (click)="closeDialog()" class="p-2 hover:bg-gray-100 rounded-lg transition-colors">
                <svg class="w-5 h-5 text-gray-500" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
                </svg>
              </button>
            </div>

            <!-- Dialog Body -->
            <div class="flex-1 overflow-y-auto p-6">
              <div class="grid grid-cols-1 lg:grid-cols-2 gap-6">
                <!-- Left: Form -->
                <div class="space-y-5">
                  <!-- Name -->
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1.5">Tên mẫu</label>
                    <input type="text" [(ngModel)]="formData.name" class="form-input" placeholder="VD: Nhắc nhở thử việc">
                  </div>

                  <!-- Subject -->
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1.5">Tiêu đề email</label>
                    <input type="text" [(ngModel)]="formData.subject" class="form-input" placeholder="VD: [Nhắc nhở] Thử việc của {EmployeeName}">
                  </div>

                  <!-- Description -->
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1.5">Mô tả</label>
                    <textarea [(ngModel)]="formData.description" rows="2" class="form-input" placeholder="Mô tả ngắn về mẫu này"></textarea>
                  </div>

                  <!-- Body HTML -->
                  <div>
                    <label class="block text-sm font-medium text-gray-700 mb-1.5">Nội dung HTML</label>
                    <textarea [(ngModel)]="formData.bodyHtml" rows="12" class="form-input font-mono text-sm" placeholder="<html>...</html>"></textarea>
                  </div>

                  <!-- Is Active -->
                  <div class="flex items-center">
                    <input type="checkbox" [(ngModel)]="formData.isActive" id="isActive" class="h-4 w-4 text-primary-600 rounded border-gray-300">
                    <label for="isActive" class="ml-2 text-sm text-gray-700">Kích hoạt mẫu này</label>
                  </div>

                  <!-- Placeholders -->
                  @if (currentPlaceholders().length > 0) {
                    <div class="bg-gray-50 rounded-lg p-4">
                      <h4 class="text-sm font-semibold text-gray-900 mb-3">Các placeholder có sẵn</h4>
                      <div class="space-y-2">
                        @for (ph of currentPlaceholders(); track ph.name) {
                          <div class="flex items-center text-sm">
                            <code class="bg-white px-2 py-0.5 rounded text-primary-600 font-mono text-xs">{{ '{' + ph.name + '}' }}</code>
                            <span class="ml-2 text-gray-600">{{ ph.description }}</span>
                          </div>
                        }
                      </div>
                    </div>
                  }
                </div>

                <!-- Right: Preview -->
                <div class="space-y-4">
                  <div class="flex items-center justify-between">
                    <h4 class="text-sm font-semibold text-gray-900">Xem trước</h4>
                    <button (click)="previewTemplate()" type="button" class="text-sm text-primary-600 hover:text-primary-700">
                      <svg class="w-4 h-4 mr-1 inline" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/>
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"/>
                      </svg>
                      Cập nhật xem trước
                    </button>
                  </div>
                  
                  @if (previewData()) {
                    <div class="border border-gray-200 rounded-lg overflow-hidden">
                      <!-- Preview Subject -->
                      <div class="bg-gray-100 px-4 py-2 border-b border-gray-200">
                        <span class="text-xs text-gray-500">Tiêu đề:</span>
                        <p class="text-sm font-medium text-gray-900">{{ previewData()!.subject }}</p>
                      </div>
                      <!-- Preview Body -->
                      <div class="p-4 bg-white max-h-[500px] overflow-y-auto">
                        <div [innerHTML]="previewData()!.bodyHtml"></div>
                      </div>
                    </div>
                  } @else {
                    <div class="border border-gray-200 rounded-lg p-8 text-center bg-gray-50">
                      <svg class="w-12 h-12 text-gray-300 mx-auto mb-3" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z"/>
                        <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z"/>
                      </svg>
                      <p class="text-sm text-gray-500">Nhập nội dung và nhấn "Cập nhật xem trước"</p>
                    </div>
                  }
                </div>
              </div>
            </div>

            <!-- Dialog Footer -->
            <div class="flex items-center justify-end gap-3 p-6 border-t border-gray-200 bg-gray-50">
              <button (click)="closeDialog()" type="button" class="btn-secondary">Hủy</button>
              <button (click)="saveTemplate()" [disabled]="saving()" type="button" class="btn-primary">
                @if (saving()) {
                  <svg class="animate-spin -ml-1 mr-2 h-4 w-4" fill="none" viewBox="0 0 24 24">
                    <circle class="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" stroke-width="4"></circle>
                    <path class="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4z"></path>
                  </svg>
                  Đang lưu...
                } @else {
                  <svg class="w-4 h-4 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"/>
                  </svg>
                  Lưu
                }
              </button>
            </div>
          </div>
        </div>
      }

      <!-- Toast Messages -->
      @if (successMessage()) {
        <div class="fixed bottom-4 right-4 bg-green-500 text-white px-6 py-3 rounded-lg shadow-lg flex items-center z-50">
          <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M5 13l4 4L19 7"/>
          </svg>
          {{ successMessage() }}
        </div>
      }
      @if (errorMessage()) {
        <div class="fixed bottom-4 right-4 bg-red-500 text-white px-6 py-3 rounded-lg shadow-lg flex items-center z-50">
          <svg class="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path stroke-linecap="round" stroke-linejoin="round" stroke-width="2" d="M6 18L18 6M6 6l12 12"/>
          </svg>
          {{ errorMessage() }}
        </div>
      }
    </div>
  `
})
export class EmailTemplatesComponent implements OnInit {
  loading = signal(true);
  saving = signal(false);
  templates = signal<EmailTemplateSummaryDto[]>([]);
  showDialog = signal(false);
  isEditing = signal(false);
  editingId = signal<string | null>(null);
  successMessage = signal('');
  errorMessage = signal('');
  previewData = signal<TemplatePreviewResultDto | null>(null);
  placeholders = signal<TemplatePlaceholderInfoDto[]>([]);
  
  formData: UpsertEmailTemplateDto = {
    name: '',
    subject: '',
    bodyHtml: '',
    isActive: true,
    description: ''
  };

  constructor(private templateService: EmailTemplateService) {}

  ngOnInit(): void {
    this.loadTemplates();
    this.loadPlaceholders();
  }

  loadTemplates(): void {
    this.loading.set(true);
    this.templateService.getAll().subscribe({
      next: (result) => {
        if (result.success && result.data) {
          this.templates.set(result.data);
        }
        this.loading.set(false);
      },
      error: () => {
        this.loading.set(false);
        this.showError('Không thể tải danh sách mẫu email');
      }
    });
  }

  loadPlaceholders(): void {
    this.templateService.getPlaceholders().subscribe({
      next: (result) => {
        if (result.success && result.data) {
          this.placeholders.set(result.data);
        }
      }
    });
  }

  currentPlaceholders() {
    // Return all unique placeholders for display
    const allPlaceholders = this.placeholders()
      .flatMap(p => p.availablePlaceholders);
    
    // Remove duplicates by name
    const uniqueMap = new Map();
    allPlaceholders.forEach(p => uniqueMap.set(p.name, p));
    return Array.from(uniqueMap.values());
  }

  getTypeLabel(type: EmailTemplateType): string {
    return TemplateTypeLabels[type] || type.toString();
  }

  getTypeColor(type: EmailTemplateType): string {
    return TemplateTypeColors[type] || 'bg-gray-100 text-gray-800';
  }

  openCreateDialog(): void {
    this.isEditing.set(false);
    this.editingId.set(null);
    this.formData = {
      name: '',
      subject: '',
      bodyHtml: '',
      isActive: true,
      description: ''
    };
    this.previewData.set(null);
    this.showDialog.set(true);
  }

  editTemplate(id: string): void {
    this.templateService.getById(id).subscribe({
      next: (result) => {
        if (result.success && result.data) {
          const template = result.data;
          this.isEditing.set(true);
          this.editingId.set(id);
          this.formData = {
            name: template.name,
            subject: template.subject,
            bodyHtml: template.bodyHtml,
            isActive: template.isActive,
            description: template.description || ''
          };
          this.previewData.set(null);
          this.showDialog.set(true);
        }
      },
      error: () => {
        this.showError('Không thể tải thông tin mẫu');
      }
    });
  }

  closeDialog(): void {
    this.showDialog.set(false);
    this.previewData.set(null);
  }

  saveTemplate(): void {
    if (!this.formData.name || !this.formData.subject || !this.formData.bodyHtml) {
      this.showError('Vui lòng điền đầy đủ thông tin');
      return;
    }

    this.saving.set(true);
    
    const request = this.isEditing() 
      ? this.templateService.update(this.editingId()!, this.formData)
      : this.templateService.create(this.formData);

    request.subscribe({
      next: (result) => {
        this.saving.set(false);
        if (result.success) {
          this.showSuccess(this.isEditing() ? 'Đã cập nhật mẫu email' : 'Đã tạo mẫu email mới');
          this.closeDialog();
          this.loadTemplates();
        } else {
          this.showError(result.message || 'Lưu thất bại');
        }
      },
      error: () => {
        this.saving.set(false);
        this.showError('Không thể lưu mẫu email');
      }
    });
  }

  resetTemplate(id: string): void {
    if (!confirm('Bạn có chắc muốn khôi phục mẫu này về mặc định?')) return;

    this.templateService.resetToDefault(id).subscribe({
      next: (result) => {
        if (result.success) {
          this.showSuccess('Đã khôi phục mẫu về mặc định');
          this.loadTemplates();
        } else {
          this.showError(result.message || 'Khôi phục thất bại');
        }
      },
      error: () => {
        this.showError('Không thể khôi phục mẫu');
      }
    });
  }

  previewTemplate(): void {
    if (!this.formData.bodyHtml) return;

    const templateType = this.isEditing() 
      ? this.templates().find(t => t.id === this.editingId())?.type 
      : EmailTemplateType.ProbationReminder;

    this.templateService.preview({
      type: templateType || EmailTemplateType.ProbationReminder,
      sampleData: {
        EmployeeName: 'Nguyễn Văn A',
        Department: 'Phòng Kỹ thuật',
        EndDate: new Date().toLocaleDateString('vi-VN'),
        DaysRemaining: '7',
        BirthDate: '01/01/1990',
        Age: '35',
        CurrentMonth: new Date().getMonth().toString(),
        CurrentYear: new Date().getFullYear().toString(),
        BirthdayList: '<table><tr><td>01/03</td><td>Nguyễn Văn A</td></tr></table>',
        TotalCount: '5'
      }
    }).subscribe({
      next: (result) => {
        if (result.success && result.data) {
          // Replace placeholders manually since preview uses sample data
          let body = this.formData.bodyHtml;
          let subject = this.formData.subject;
          
          const sampleData: { [key: string]: string } = {
            EmployeeName: 'Nguyễn Văn A',
            Department: 'Phòng Kỹ thuật',
            EndDate: new Date().toLocaleDateString('vi-VN'),
            DaysRemaining: '7',
            BirthDate: '01/01/1990',
            Age: '35',
            CurrentMonth: (new Date().getMonth() + 1).toString(),
            CurrentYear: new Date().getFullYear().toString(),
            BirthdayList: '<table style="border-collapse:collapse"><tr><td style="padding:8px;border:1px solid #ddd">01/03</td><td style="padding:8px;border:1px solid #ddd">Nguyễn Văn A</td><td style="padding:8px;border:1px solid #ddd">IT</td></tr></table>',
            TotalCount: '5'
          };

          Object.keys(sampleData).forEach(key => {
            body = body.replace(new RegExp(`\\{${key}\\}`, 'g'), sampleData[key]);
            subject = subject.replace(new RegExp(`\\{${key}\\}`, 'g'), sampleData[key]);
          });

          this.previewData.set({ subject, bodyHtml: body });
        }
      }
    });
  }

  private showSuccess(message: string): void {
    this.successMessage.set(message);
    setTimeout(() => this.successMessage.set(''), 3000);
  }

  private showError(message: string): void {
    this.errorMessage.set(message);
    setTimeout(() => this.errorMessage.set(''), 3000);
  }
}
