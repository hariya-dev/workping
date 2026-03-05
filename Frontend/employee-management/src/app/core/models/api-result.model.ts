// core/models/api-result.model.ts
// Models chung cho API response

// Kết quả API có data
export interface ApiResult<T> {
  success: boolean;
  data?: T;
  message?: string;
  errors: string[];
}

// Kết quả phân trang
export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
  totalPages: number;
  hasPrevious: boolean;
  hasNext: boolean;
}

// Cài đặt hệ thống
export interface SystemSetting {
  id: string;
  key: string;
  value: string;
  valueType?: string;
  description?: string;
  updatedAt?: string;
}

// Cài đặt nhắc nhở
export interface ReminderSettings {
  defaultProbationDays: number;
  probationReminderDaysBefore: number[];
  contractReminderDaysBefore: number[];
  hrNotificationEmails: string;
}
