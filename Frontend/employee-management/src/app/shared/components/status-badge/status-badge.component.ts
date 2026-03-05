// shared/components/status-badge/status-badge.component.ts
// Component hiển thị badge trạng thái tái sử dụng

import { Component, Input } from '@angular/core';
import { CommonModule } from '@angular/common';

export interface StatusConfig {
  text: string;
  bgColor: string;
  textColor: string;
  borderColor?: string;
}

@Component({
  selector: 'app-status-badge',
  standalone: true,
  imports: [CommonModule],
  template: `
    <span 
      class="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium transition-all duration-200"
      [class]="getStatusClasses()">
      {{ status }}
    </span>
  `
})
export class StatusBadgeComponent {
  @Input() status: string = '';
  @Input() type: 'contract' | 'employee' | 'default' = 'default';
  
  getStatusClasses(): string {
    const baseClasses = 'whitespace-nowrap';
    
    switch (this.type) {
      case 'contract':
        return `${baseClasses} ${
          this.status === 'Đang thực hiện' 
            ? 'bg-green-100 text-green-700 border border-green-200' 
            : 'bg-gray-100 text-gray-700 border border-gray-200'
        }`;
      
      case 'employee':
        return `${baseClasses} ${
          this.status === 'Đang làm việc'
            ? 'bg-green-100 text-green-700 border border-green-200'
            : 'bg-gray-100 text-gray-700 border border-gray-200'
        }`;
      
      default:
        return `${baseClasses} bg-gray-100 text-gray-700 border border-gray-200`;
    }
  }
}