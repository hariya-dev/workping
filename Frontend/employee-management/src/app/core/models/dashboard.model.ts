// core/models/dashboard.model.ts
// Models cho Tổng quan

import { EmployeeStatus } from './employee.model';

export interface DashboardStats {
  totalEmployees: number;
  statusCounts: StatusCount[];
  probationExpiringSoon: number;
  contractExpiringSoon: number;
  upcomingReminders: UpcomingReminder[];
  currentMonthBirthdays: Birthday[];
}

export interface StatusCount {
  status: EmployeeStatus;
  statusName: string;
  count: number;
}

export interface UpcomingReminder {
  employeeId: string;
  employeeName: string;
  reminderType: 'Probation' | 'Contract';
  endDate: string;
  daysRemaining: number;
}

export interface Birthday {
  employeeId: string;
  employeeName: string;
  dateOfBirth: string;
  day: number;
  department?: string;
}
