import { Component, Input, Output, EventEmitter, computed } from '@angular/core';
import { DatePipe } from '@angular/common';
import { Employee, EmployeeContract, EmployeeStatus } from '../../../core/models/employee.model';

@Component({
  selector: 'app-employee-card',
  standalone: true,
  imports: [DatePipe],
  templateUrl: './employee-card.component.html',
})
export class EmployeeCardComponent {
  @Input({ required: true }) employee!: Employee;
  @Output() viewDetail = new EventEmitter<string>();
  @Output() edit = new EventEmitter<string>();

  // Tính hợp đồng đang active
  activeContract = computed(() => {
    return this.employee.activeContract || null;
  });

  // Kiểm tra có đang thử việc không
  hasActiveProbation = computed(() => {
    return !!this.employee.probationStartDate && 
           !!this.employee.probationEndDate &&
           new Date() >= new Date(this.employee.probationStartDate) &&
           new Date() <= new Date(this.employee.probationEndDate);
  });

  // Kiểm tra hợp đồng sắp hết hạn (trong 30 ngày)
  isExpiringSoon(endDate: string): boolean {
    const today = new Date();
    const expiringDate = new Date(endDate);
    const diffTime = expiringDate.getTime() - today.getTime();
    const diffDays = Math.ceil(diffTime / (1000 * 60 * 60 * 24));
    return diffDays <= 30 && diffDays >= 0;
  }

  // Get employment status label
  getEmploymentStatusLabel(status: EmployeeStatus): string {
    switch (status) {
      case EmployeeStatus.Active:
        return 'Đang làm việc';
      case EmployeeStatus.Resigned:
        return 'Đã nghỉ việc';
      default:
        return 'Không xác định';
    }
  }

  // Get employment status CSS class
  getEmploymentStatusClass(status: EmployeeStatus): string {
    switch (status) {
      case EmployeeStatus.Active:
        return 'bg-green-100 text-green-800 border border-green-200';
      case EmployeeStatus.Resigned:
        return 'bg-red-100 text-red-800 border border-red-200';
      default:
        return 'bg-gray-100 text-gray-800 border border-gray-200';
    }
  }

  // Get contract status CSS class
  getContractStatusClass(status: string): string {
    if (status === 'Đang thực hiện') {
      return 'bg-blue-100 text-blue-800 border-blue-200';
    } else if (status === 'Đã kết thúc') {
      return 'bg-gray-100 text-gray-800 border-gray-200';
    }
    return 'bg-gray-100 text-gray-800 border border-gray-200';
  }

  getAvatarColor(name: string): string {
    const colors = [
      '#3b82f6', '#8b5cf6', '#06b6d4', '#10b981',
      '#f59e0b', '#ef4444', '#ec4899', '#6366f1'
    ];
    let hash = 0;
    for (let i = 0; i < name.length; i++) hash = name.charCodeAt(i) + ((hash << 5) - hash);
    return colors[Math.abs(hash) % colors.length];
  }

  onViewDetail(): void {
    this.viewDetail.emit(this.employee.id);
  }

  onEdit(): void {
    this.edit.emit(this.employee.id);
  }
}