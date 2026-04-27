// core/services/settings.service.ts
// Service quản lý Cài đặt hệ thống

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { SystemSetting, ReminderSettings, ApiResult } from '../models';

@Injectable({
  providedIn: 'root'
})
export class SettingsService {
  private endpoint = '/api/settings';

  constructor(private api: ApiService) {}

  // Lấy tất cả cài đặt
  getAll(): Observable<ApiResult<SystemSetting[]>> {
    return this.api.get<ApiResult<SystemSetting[]>>(this.endpoint);
  }

  // Lấy cài đặt nhắc nhở
  getReminderSettings(): Observable<ApiResult<ReminderSettings>> {
    return this.api.get<ApiResult<ReminderSettings>>(`${this.endpoint}/reminders`);
  }

  // Cập nhật cài đặt nhắc nhở
  updateReminderSettings(settings: ReminderSettings): Observable<ApiResult<void>> {
    return this.api.put<ApiResult<void>>(`${this.endpoint}/reminders`, settings);
  }

  // Cập nhật một cài đặt
  updateSetting(key: string, value: string): Observable<ApiResult<void>> {
    return this.api.put<ApiResult<void>>(`${this.endpoint}/${key}`, { key, value });
  }

  // Gửi email test cơ bản
  sendTestEmail(email: string): Observable<ApiResult<void>> {
    return this.api.post<ApiResult<void>>(`${this.endpoint}/test-email`, { email });
  }

  // Gửi email test sinh nhật
  sendTestBirthdayEmail(email: string): Observable<ApiResult<void>> {
    return this.api.post<ApiResult<void>>(`${this.endpoint}/test-birthday-email`, { email });
  }

  // Gửi email test thử việc
  sendTestProbationEmail(email: string): Observable<ApiResult<void>> {
    return this.api.post<ApiResult<void>>(`${this.endpoint}/test-probation-email`, { email });
  }

  // Gửi email test hợp đồng
  sendTestContractEmail(email: string): Observable<ApiResult<void>> {
    return this.api.post<ApiResult<void>>(`${this.endpoint}/test-contract-email`, { email });
  }

  // Gửi email test thử việc cho HR
  sendTestProbationEmailHr(email: string): Observable<ApiResult<void>> {
    return this.api.post<ApiResult<void>>(`${this.endpoint}/test-probation-email-hr`, { email });
  }

  // Gửi email test hợp đồng cho HR
  sendTestContractEmailHr(email: string): Observable<ApiResult<void>> {
    return this.api.post<ApiResult<void>>(`${this.endpoint}/test-contract-email-hr`, { email });
  }

  // Gửi email test danh sách sinh nhật tháng cho HR
  sendTestMonthlyBirthdayEmail(email: string): Observable<ApiResult<void>> {
    return this.api.post<ApiResult<void>>(`${this.endpoint}/test-monthly-birthday-email`, { email });
  }

  // Kích hoạt chạy tất cả jobs ngay lập tức
  triggerAllJobs(): Observable<ApiResult<void>> {
    return this.api.post<ApiResult<void>>(`${this.endpoint}/trigger-jobs`, {});
  }
}
