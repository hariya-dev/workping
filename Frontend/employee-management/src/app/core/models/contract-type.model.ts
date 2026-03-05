// core/models/contract-type.model.ts
// Models cho Loại hợp đồng

export interface ContractType {
  id: string;
  name: string;
  durationMonths?: number;
  description?: string;
  isActive: boolean;
  createdAt: string;
  employeeCount: number;
}

export interface CreateContractTypeDto {
  name: string;
  durationMonths?: number;
  description?: string;
}

export interface UpdateContractTypeDto extends CreateContractTypeDto {
  isActive: boolean;
}
