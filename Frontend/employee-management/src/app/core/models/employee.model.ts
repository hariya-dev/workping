// core/models/employee.model.ts
// Models cho Employee

// Enum trạng thái nhân viên (chỉ 2 trạng thái)
export enum EmployeeStatus {
  Active = 0,    // Đang làm việc
  Resigned = 1   // Đã nghỉ việc
}

// Map tên hiển thị của trạng thái
export const EmployeeStatusLabels: { [key: number]: string } = {
  [EmployeeStatus.Active]: 'Đang làm việc',
  [EmployeeStatus.Resigned]: 'Đã nghỉ việc'
};

// Model hợp đồng nhân viên
export interface EmployeeContract {
  id: string;
  employeeId: string;
  contractTypeId: string;
  contractTypeName: string;
  durationMonths?: number;
  startDate: string;
  endDate?: string;
  status: string; // "Đang thực hiện" hoặc "Đã kết thúc"
  notes?: string;
  createdAt: string;
  createdByName?: string;
}

// Model nhân viên
export interface Employee {
  id: string;
  fullName: string;
  email?: string;
  phoneNumber?: string;
  dateOfBirth: string;
  probationStartDate?: string;
  probationEndDate?: string;
  status: EmployeeStatus;
  statusDisplayName: string;
  department?: string;
  position?: string;
  notes?: string;
  createdAt: string;
  updatedAt?: string;
  activeContract?: EmployeeContract; // Hợp đồng đang thực hiện
  files: EmployeeFile[];
}

// Model file đính kèm
export interface EmployeeFile {
  id: string;
  originalFileName: string;
  filePath: string;
  fileExtension?: string;
  fileSize: number;
  description?: string;
  uploadedAt: string;
}

// DTO tạo nhân viên (không bao gồm hợp đồng - thêm riêng sau)
export interface CreateEmployeeDto {
  fullName: string;
  email?: string;
  phoneNumber?: string;
  dateOfBirth: string;
  probationStartDate?: string;
  probationEndDate?: string;
  status: EmployeeStatus;
  department?: string;
  position?: string;
  notes?: string;
}

// DTO cập nhật nhân viên
export interface UpdateEmployeeDto extends CreateEmployeeDto {}

// DTO tạo hợp đồng mới
export interface CreateEmployeeContractDto {
  contractTypeId: string;
  startDate: string;
  endDate?: string;
  notes?: string;
}

// DTO kết thúc hợp đồng
export interface TerminateContractDto {
  notes?: string;
}

// DTO bộ lọc nhân viên
export interface EmployeeFilter {
  searchTerm?: string;
  status?: EmployeeStatus;
  contractTypeId?: string;
  probationEndDateFrom?: string;
  probationEndDateTo?: string;
  contractEndDateFrom?: string;
  contractEndDateTo?: string;
  pageNumber: number;
  pageSize: number;
}

// Model lịch sử chỉnh sửa
export interface EmployeeEditHistory {
  id: string;
  employeeId: string;
  fieldName: string;
  fieldDisplayName: string;
  oldValue?: string;
  newValue?: string;
  changedBy?: string;
  changedByName: string;
  changedAt: string;
  changeType: 'Create' | 'Update' | 'Delete';
}
