// core/services/dashboard.service.ts
// Service cho Tổng quan

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { DashboardStats, UpcomingReminder, Birthday } from '../models';
import { ApiResult } from '../models/api-result.model';

@Injectable({
  providedIn: 'root'
})
export class DashboardService {
  private endpoint = '/api/dashboard';

  constructor(private api: ApiService) {}

  // Lấy thống kê tổng quan
  getStats(): Observable<ApiResult<DashboardStats>> {
    return this.api.get<ApiResult<DashboardStats>>(`${this.endpoint}/stats`);
  }

  // Lấy danh sách sắp hết thử việc
  getProbationExpiring(daysAhead: number = 30): Observable<ApiResult<UpcomingReminder[]>> {
    return this.api.get<ApiResult<UpcomingReminder[]>>(
      `${this.endpoint}/probation-expiring`, 
      { daysAhead }
    );
  }

  // Lấy danh sách sắp hết hợp đồng
  getContractExpiring(daysAhead: number = 30): Observable<ApiResult<UpcomingReminder[]>> {
    return this.api.get<ApiResult<UpcomingReminder[]>>(
      `${this.endpoint}/contract-expiring`, 
      { daysAhead }
    );
  }

  // Lấy danh sách sinh nhật trong tháng
  getBirthdays(month?: number, year?: number): Observable<ApiResult<Birthday[]>> {
    return this.api.get<ApiResult<Birthday[]>>(`${this.endpoint}/birthdays`, { month, year });
  }
}
