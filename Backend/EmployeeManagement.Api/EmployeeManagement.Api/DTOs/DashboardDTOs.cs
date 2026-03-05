// DTOs/DashboardDTOs.cs
// Data Transfer Objects cho Dashboard API

using EmployeeManagement.Api.Entities;

namespace EmployeeManagement.Api.DTOs;

/// <summary>
/// DTO thống kê dashboard
/// </summary>
public class DashboardStatsDto
{
    /// <summary>
    /// Tổng số nhân viên
    /// </summary>
    public int TotalEmployees { get; set; }

    /// <summary>
    /// Thống kê theo trạng thái
    /// </summary>
    public List<StatusCountDto> StatusCounts { get; set; } = new();

    /// <summary>
    /// Số nhân viên sắp hết thử việc (trong 30 ngày tới)
    /// </summary>
    public int ProbationExpiringSoon { get; set; }

    /// <summary>
    /// Số nhân viên sắp hết hợp đồng (trong 30 ngày tới)
    /// </summary>
    public int ContractExpiringSoon { get; set; }

    /// <summary>
    /// Danh sách nhắc nhở sắp tới
    /// </summary>
    public List<UpcomingReminderDto> UpcomingReminders { get; set; } = new();

    /// <summary>
    /// Danh sách sinh nhật trong tháng
    /// </summary>
    public List<BirthdayDto> CurrentMonthBirthdays { get; set; } = new();
}

/// <summary>
/// DTO thống kê theo trạng thái
/// </summary>
public class StatusCountDto
{
    public EmployeeStatus Status { get; set; }
    public string StatusName { get; set; } = string.Empty;
    public int Count { get; set; }
}

/// <summary>
/// DTO nhắc nhở sắp tới
/// </summary>
public class UpcomingReminderDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public string ReminderType { get; set; } = string.Empty; // "Probation" hoặc "Contract"
    public DateOnly EndDate { get; set; }
    public int DaysRemaining { get; set; }
}

/// <summary>
/// DTO sinh nhật nhân viên
/// </summary>
public class BirthdayDto
{
    public Guid EmployeeId { get; set; }
    public string EmployeeName { get; set; } = string.Empty;
    public DateOnly DateOfBirth { get; set; }
    public int Day { get; set; }
    public string? Department { get; set; }
}
