// core/services/contract-type.service.ts
// Service quản lý Loại hợp đồng

import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { ApiService } from './api.service';
import { ContractType, CreateContractTypeDto, UpdateContractTypeDto } from '../models';
import { ApiResult } from '../models/api-result.model';

@Injectable({
  providedIn: 'root'
})
export class ContractTypeService {
  private endpoint = '/api/contract-types';

  constructor(private api: ApiService) {}

  // Lấy tất cả loại hợp đồng
  getAll(activeOnly: boolean = true): Observable<ApiResult<ContractType[]>> {
    return this.api.get<ApiResult<ContractType[]>>(this.endpoint, { activeOnly });
  }

  // Lấy chi tiết loại hợp đồng
  getById(id: string): Observable<ApiResult<ContractType>> {
    return this.api.get<ApiResult<ContractType>>(`${this.endpoint}/${id}`);
  }

  // Tạo loại hợp đồng mới
  create(dto: CreateContractTypeDto): Observable<ApiResult<ContractType>> {
    return this.api.post<ApiResult<ContractType>>(this.endpoint, dto);
  }

  // Cập nhật loại hợp đồng
  update(id: string, dto: UpdateContractTypeDto): Observable<ApiResult<ContractType>> {
    return this.api.put<ApiResult<ContractType>>(`${this.endpoint}/${id}`, dto);
  }

  // Xóa loại hợp đồng
  delete(id: string): Observable<ApiResult<void>> {
    return this.api.delete<ApiResult<void>>(`${this.endpoint}/${id}`);
  }
}
