// core/services/email-template.service.ts
// Service quản lý email templates

import { Injectable } from '@angular/core';
import { ApiService } from './api.service';
import { Observable } from 'rxjs';
import { ApiResult } from '../models';
import { 
  EmailTemplateDto, 
  EmailTemplateSummaryDto, 
  UpsertEmailTemplateDto,
  TemplatePlaceholderInfoDto,
  TemplatePreviewResultDto,
  PreviewTemplateDto
} from '../models/email-template.model';

@Injectable({
  providedIn: 'root'
})
export class EmailTemplateService {
  constructor(private api: ApiService) {}

  // Lấy tất cả templates
  getAll(): Observable<ApiResult<EmailTemplateSummaryDto[]>> {
    return this.api.get<ApiResult<EmailTemplateSummaryDto[]>>('/email-templates');
  }

  // Lấy template theo ID
  getById(id: string): Observable<ApiResult<EmailTemplateDto>> {
    return this.api.get<ApiResult<EmailTemplateDto>>(`/email-templates/${id}`);
  }

  // Lấy template theo loại
  getByType(type: number): Observable<ApiResult<EmailTemplateDto>> {
    return this.api.get<ApiResult<EmailTemplateDto>>(`/email-templates/type/${type}`);
  }

  // Tạo template mới
  create(dto: UpsertEmailTemplateDto): Observable<ApiResult<{ id: string; isNew: boolean }>> {
    return this.api.post<ApiResult<{ id: string; isNew: boolean }>>('/email-templates', dto);
  }

  // Cập nhật template
  update(id: string, dto: UpsertEmailTemplateDto): Observable<ApiResult<{ id: string; isNew: boolean }>> {
    return this.api.put<ApiResult<{ id: string; isNew: boolean }>>(`/email-templates/${id}`, dto);
  }

  // Reset template về mặc định
  resetToDefault(id: string): Observable<ApiResult<void>> {
    return this.api.post<ApiResult<void>>(`/email-templates/${id}/reset`, {});
  }

  // Preview template
  preview(dto: PreviewTemplateDto): Observable<ApiResult<TemplatePreviewResultDto>> {
    return this.api.post<ApiResult<TemplatePreviewResultDto>>('/email-templates/preview', dto);
  }

  // Lấy danh sách placeholders có sẵn
  getPlaceholders(): Observable<ApiResult<TemplatePlaceholderInfoDto[]>> {
    return this.api.get<ApiResult<TemplatePlaceholderInfoDto[]>>('/email-templates/placeholders');
  }
}
