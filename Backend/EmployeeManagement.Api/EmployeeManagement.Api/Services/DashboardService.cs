// Services/DashboardService.cs
// Triển khai dịch vụ dashboard

using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Services;

/// <summary>
/// Service dashboard - cung cấp thống kê và báo cáo
/// </summary>
public class DashboardService : IDashboardService
{
    private readonly AppDbContext _context;
    private readonly ILogger<DashboardService> _logger;

    public DashboardService(AppDbContext context, ILogger<DashboardService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Lấy thống kê tổng quan cho dashboard
    /// </summary>
    public async Task<DashboardStatsDto> GetDashboardStatsAsync()
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var thirtyDaysLater = today.AddDays(30);

        // Tổng số nhân viên (không tính đã nghỉ việc)
        var totalEmployees = await _context.Employees
            .CountAsync(e => e.Status != EmployeeStatus.Resigned);

        // Thống kê theo trạng thái
        var statusCounts = await _context.Employees
            .GroupBy(e => e.Status)
            .Select(g => new StatusCountDto
            {
                Status = g.Key,
                StatusName = GetStatusDisplayName(g.Key),
                Count = g.Count()
            })
            .ToListAsync();

        // Số nhân viên sắp hết hợp đồng (trong 30 ngày tới)
        // Lấy từ EmployeeContract thay vì Employee.ContractEndDate
        var contractExpiring = await _context.EmployeeContracts
            .CountAsync(c => c.Employee!.Status == EmployeeStatus.Active 
                && c.EndDate.HasValue 
                && c.EndDate.Value >= today 
                && c.EndDate.Value <= thirtyDaysLater);

        // Danh sách nhắc nhở sắp tới (tối đa 10)
        var upcomingReminders = await GetUpcomingRemindersAsync(10);

        // Sinh nhật trong tháng hiện tại
        var birthdays = await GetBirthdaysInMonthAsync(today.Month, today.Year);

        return new DashboardStatsDto
        {
            TotalEmployees = totalEmployees,
            StatusCounts = statusCounts,
            ProbationExpiringSoon = 0,
            ContractExpiringSoon = contractExpiring,
            UpcomingReminders = upcomingReminders,
            CurrentMonthBirthdays = birthdays
        };
    }

    /// <summary>
    /// Lấy danh sách nhân viên sắp hết thử việc
    /// </summary>
    public async Task<List<UpcomingReminderDto>> GetProbationExpiringAsync(int daysAhead = 30)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var endDate = today.AddDays(daysAhead);

        return await _context.Employees
            .Where(e => e.Status == EmployeeStatus.Active 
                && e.ProbationEndDate.HasValue 
                && e.ProbationEndDate.Value >= today 
                && e.ProbationEndDate.Value <= endDate)
            .OrderBy(e => e.ProbationEndDate)
            .Select(e => new UpcomingReminderDto
            {
                EmployeeId = e.Id,
                EmployeeName = e.FullName,
                ReminderType = "Thử việc",
                EndDate = e.ProbationEndDate!.Value,
                DaysRemaining = e.ProbationEndDate!.Value.DayNumber - today.DayNumber
            })
            .ToListAsync();
    }

    /// <summary>
    /// Lấy danh sách nhân viên sắp hết hợp đồng
    /// </summary>
    public async Task<List<UpcomingReminderDto>> GetContractExpiringAsync(int daysAhead = 30)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var endDate = today.AddDays(daysAhead);

        // Lấy từ EmployeeContract thay vì Employee.ContractEndDate
        return await _context.EmployeeContracts
            .Include(c => c.Employee)
            .Where(c => c.Employee!.Status == EmployeeStatus.Active 
                && c.EndDate.HasValue 
                && c.EndDate.Value >= today 
                && c.EndDate.Value <= endDate)
            .OrderBy(c => c.EndDate)
            .Select(c => new UpcomingReminderDto
            {
                EmployeeId = c.EmployeeId,
                EmployeeName = c.Employee!.FullName,
                ReminderType = "Hợp đồng",
                EndDate = c.EndDate!.Value,
                DaysRemaining = c.EndDate!.Value.DayNumber - today.DayNumber
            })
            .ToListAsync();
    }

    /// <summary>
    /// Lấy danh sách sinh nhật trong tháng
    /// </summary>
    public async Task<List<BirthdayDto>> GetBirthdaysInMonthAsync(int? month = null, int? year = null)
    {
        var targetMonth = month ?? DateTime.Today.Month;
        var targetYear = year ?? DateTime.Today.Year;

        return await _context.Employees
            .Where(e => e.Status != EmployeeStatus.Resigned
                && e.DateOfBirth.Month == targetMonth)
            .OrderBy(e => e.DateOfBirth.Day)
            .Select(e => new BirthdayDto
            {
                EmployeeId = e.Id,
                EmployeeName = e.FullName,
                DateOfBirth = e.DateOfBirth,
                Day = e.DateOfBirth.Day,
                Department = e.Department
            })
            .ToListAsync();
    }

    /// <summary>
    /// Lấy danh sách nhắc nhở sắp tới (gộp thử việc + hợp đồng)
    /// </summary>
    private async Task<List<UpcomingReminderDto>> GetUpcomingRemindersAsync(int limit)
    {
        var probation = await GetProbationExpiringAsync(30);
        var contract = await GetContractExpiringAsync(30);

        return probation
            .Concat(contract)
            .OrderBy(r => r.DaysRemaining)
            .Take(limit)
            .ToList();
    }

    /// <summary>
    /// Lấy tên hiển thị của trạng thái
    /// </summary>
    private static string GetStatusDisplayName(EmployeeStatus status)
    {
        return status switch
        {
            EmployeeStatus.Active => "Đang làm việc",
            EmployeeStatus.Resigned => "Đã nghỉ việc",
            _ => "Không xác định"
        };
    }
}
