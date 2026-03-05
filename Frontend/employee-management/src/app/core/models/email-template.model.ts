// core/models/email-template.model.ts
// Models cho email template

export enum EmailTemplateType {
  ProbationReminder = 1,
  ContractReminder = 2,
  BirthdayWish = 3,
  MonthlyBirthdayList = 4,
  ProbationReminderHr = 5,
  ContractReminderHr = 6
}

export interface EmailTemplateDto {
  id: string;
  type: EmailTemplateType;
  typeName: string;
  name: string;
  subject: string;
  bodyHtml: string;
  isActive: boolean;
  description?: string;
  createdAt: string;
  updatedAt?: string;
}

export interface EmailTemplateSummaryDto {
  id: string;
  type: EmailTemplateType;
  typeName: string;
  name: string;
  subject: string;
  isActive: boolean;
  updatedAt?: string;
}

export interface UpsertEmailTemplateDto {
  type?: EmailTemplateType;
  name: string;
  subject: string;
  bodyHtml: string;
  isActive: boolean;
  description?: string;
}

export interface PlaceholderDto {
  name: string;
  description: string;
  example: string;
}

export interface TemplatePlaceholderInfoDto {
  templateType: EmailTemplateType;
  templateName: string;
  availablePlaceholders: PlaceholderDto[];
}

export interface TemplatePreviewResultDto {
  subject: string;
  bodyHtml: string;
}

export interface PreviewTemplateDto {
  type: EmailTemplateType;
  sampleData?: { [key: string]: string };
}

// Template type labels cho hiển thị
export const TemplateTypeLabels: { [key: number]: string } = {
  [EmailTemplateType.ProbationReminder]: 'Nhắc nhở thử việc (Nhân viên)',
  [EmailTemplateType.ContractReminder]: 'Nhắc nhở hợp đồng (Nhân viên)',
  [EmailTemplateType.BirthdayWish]: 'Chúc mừng sinh nhật',
  [EmailTemplateType.MonthlyBirthdayList]: 'Danh sách sinh nhật tháng',
  [EmailTemplateType.ProbationReminderHr]: 'Nhắc nhở thử việc (HR)',
  [EmailTemplateType.ContractReminderHr]: 'Nhắc nhở hợp đồng (HR)'
};

// Template type colors cho hiển thị
export const TemplateTypeColors: { [key: number]: string } = {
  [EmailTemplateType.ProbationReminder]: 'bg-blue-100 text-blue-800',
  [EmailTemplateType.ContractReminder]: 'bg-red-100 text-red-800',
  [EmailTemplateType.BirthdayWish]: 'bg-pink-100 text-pink-800',
  [EmailTemplateType.MonthlyBirthdayList]: 'bg-purple-100 text-purple-800',
  [EmailTemplateType.ProbationReminderHr]: 'bg-amber-100 text-amber-800',
  [EmailTemplateType.ContractReminderHr]: 'bg-orange-100 text-orange-800'
};
