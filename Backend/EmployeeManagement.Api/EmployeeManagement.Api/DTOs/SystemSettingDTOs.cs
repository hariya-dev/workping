// DTOs/SystemSettingDTOs.cs
// Data Transfer Objects cho System Settings API

using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.DTOs;

/// <summary>
/// DTO cài đặt hệ thống
/// </summary>
public class SystemSettingDto
{
    public Guid Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? ValueType { get; set; }
    public string? Description { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO cập nhật cài đặt
/// </summary>
public class UpdateSettingDto
{
    [Required(ErrorMessage = "Key la bat buoc")]
    public string Key { get; set; } = string.Empty;

    [Required(ErrorMessage = "Value la bat buoc")]
    public string Value { get; set; } = string.Empty;
}

/// <summary>
/// DTO cài đặt reminder
/// </summary>
public class ReminderSettingsDto
{
    public int DefaultProbationDays { get; set; }
    public List<int> ProbationReminderDaysBefore { get; set; } = new();
    public List<int> ContractReminderDaysBefore { get; set; } = new();
    public string HrNotificationEmails { get; set; } = string.Empty;
}
