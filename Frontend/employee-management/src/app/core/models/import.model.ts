// core/models/import.model.ts
// Models cho chức năng import nhân viên

export interface ImportResult {
  message: string;
  totalRecords: number;
  successCount: number;
  failedCount: number;
  errors: string[];
  importedEmployees: ImportedEmployeeSummary[];
}

export interface ImportedEmployeeSummary {
  employeeName: string;
  status: 'Success' | 'Failed';
  message: string;
}

export interface ImportResponse {
  message: string;
  totalRecords: number;
  successCount: number;
  failedCount: number;
  errors: string[];
  importedEmployees: ImportedEmployeeSummary[];
}