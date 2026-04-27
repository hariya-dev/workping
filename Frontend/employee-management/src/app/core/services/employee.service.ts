// core/services/employee.service.ts
// Service quản lý Employee

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { 
  Employee, 
  CreateEmployeeDto, 
  UpdateEmployeeDto, 
  EmployeeFilter,
  EmployeeFile,
  EmployeeEditHistory,
  EmployeeContract,
  CreateEmployeeContractDto,
  UpdateEmployeeContractDto,
  TerminateContractDto
} from '../models';
import { ApiResult, PagedResult } from '../models/api-result.model';

@Injectable({
  providedIn: 'root'
})
export class EmployeeService {
  private endpoint = '/api/employees';

  constructor(private api: ApiService) {}

  // ========== EMPLOYEE CRUD ==========

  // Lấy danh sách nhân viên (có filter, phân trang)
  getEmployees(filter: EmployeeFilter): Observable<ApiResult<PagedResult<Employee>>> {
    return this.api.get<ApiResult<PagedResult<Employee>>>(this.endpoint, filter);
  }

  // Lấy chi tiết nhân viên
  getEmployee(id: string): Observable<ApiResult<Employee>> {
    return this.api.get<ApiResult<Employee>>(`${this.endpoint}/${id}`);
  }

  // Tạo nhân viên mới
  createEmployee(dto: CreateEmployeeDto): Observable<ApiResult<Employee>> {
    return this.api.post<ApiResult<Employee>>(this.endpoint, dto);
  }

  // Cập nhật nhân viên
  updateEmployee(id: string, dto: UpdateEmployeeDto): Observable<ApiResult<Employee>> {
    return this.api.put<ApiResult<Employee>>(`${this.endpoint}/${id}`, dto);
  }

  // Xóa nhân viên
  deleteEmployee(id: string): Observable<ApiResult<void>> {
    return this.api.delete<ApiResult<void>>(`${this.endpoint}/${id}`);
  }

  // ========== CONTRACTS ==========

  // Lấy danh sách hợp đồng của nhân viên (lịch sử hợp đồng)
  getContracts(employeeId: string): Observable<ApiResult<EmployeeContract[]>> {
    return this.api.get<ApiResult<EmployeeContract[]>>(`${this.endpoint}/${employeeId}/contracts`);
  }

  // Lấy hợp đồng đang thực hiện
  getActiveContract(employeeId: string): Observable<ApiResult<EmployeeContract | null>> {
    return this.api.get<ApiResult<EmployeeContract | null>>(`${this.endpoint}/${employeeId}/contracts/active`);
  }

  // Tạo hợp đồng mới
  createContract(employeeId: string, dto: CreateEmployeeContractDto): Observable<ApiResult<EmployeeContract>> {
    return this.api.post<ApiResult<EmployeeContract>>(`${this.endpoint}/${employeeId}/contracts`, dto);
  }

  // Cập nhật hợp đồng (sửa sai)
  updateContract(employeeId: string, contractId: string, dto: UpdateEmployeeContractDto): Observable<ApiResult<EmployeeContract>> {
    return this.api.put<ApiResult<EmployeeContract>>(
      `${this.endpoint}/${employeeId}/contracts/${contractId}`,
      dto
    );
  }

  // Kết thúc hợp đồng
  terminateContract(employeeId: string, contractId: string, dto?: TerminateContractDto): Observable<ApiResult<EmployeeContract>> {
    return this.api.put<ApiResult<EmployeeContract>>(
      `${this.endpoint}/${employeeId}/contracts/${contractId}/terminate`, 
      dto || {}
    );
  }

  // ========== FILES ==========

  // Upload file cho nhân viên
  uploadFile(employeeId: string, file: File, description?: string): Observable<ApiResult<EmployeeFile>> {
    const formData = new FormData();
    formData.append('file', file);
    if (description) {
      formData.append('description', description);
    }
    return this.api.postFormData<ApiResult<EmployeeFile>>(
      `${this.endpoint}/${employeeId}/files?description=${encodeURIComponent(description || '')}`, 
      formData
    );
  }

  // Lấy danh sách file
  getFiles(employeeId: string): Observable<ApiResult<EmployeeFile[]>> {
    return this.api.get<ApiResult<EmployeeFile[]>>(`${this.endpoint}/${employeeId}/files`);
  }

  // Xóa file
  deleteFile(employeeId: string, fileId: string): Observable<ApiResult<void>> {
    return this.api.delete<ApiResult<void>>(`${this.endpoint}/${employeeId}/files/${fileId}`);
  }

  // Download file
  downloadFile(employeeId: string, fileId: string): Observable<Blob> {
    return this.api.getBlob(`${this.endpoint}/${employeeId}/files/${fileId}/download`);
  }

  // ========== HISTORY ==========

  // Lấy lịch sử chỉnh sửa
  getEditHistory(employeeId: string): Observable<ApiResult<EmployeeEditHistory[]>> {
    return this.api.get<ApiResult<EmployeeEditHistory[]>>(`${this.endpoint}/${employeeId}/history`);
  }

  // ========== OTHER ==========

  // Lấy danh sách trạng thái
  getStatuses(): Observable<ApiResult<{ value: number; name: string; displayName: string }[]>> {
    return this.api.get<ApiResult<{ value: number; name: string; displayName: string }[]>>(
      `${this.endpoint}/statuses`
    );
  }
}
