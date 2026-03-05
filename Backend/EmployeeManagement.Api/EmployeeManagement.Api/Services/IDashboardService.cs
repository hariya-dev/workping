// Services/IDashboardService.cs
// Interface dịch vụ dashboard

using EmployeeManagement.Api.DTOs;

namespace EmployeeManagement.Api.Services;

/// <summary>
/// Interface dịch vụ dashboard - cung cấp thống kê và báo cáo
/// </summary>
public interface IDashboardService
{
    /// <summary>
    /// Lấy thống kê tổng quan cho dashboard
    /// </summary>
    Task<DashboardStatsDto> GetDashboardStatsAsync();

    /// <summary>
    /// Lấy danh sách nhân viên sắp hết thử việc
    /// </summary>
    Task<List<UpcomingReminderDto>> GetProbationExpiringAsync(int daysAhead = 30);

    /// <summary>
    /// Lấy danh sách nhân viên sắp hết hợp đồng
    /// </summary>
    Task<List<UpcomingReminderDto>> GetContractExpiringAsync(int daysAhead = 30);

    /// <summary>
    /// Lấy danh sách sinh nhật trong tháng
    /// </summary>
    Task<List<BirthdayDto>> GetBirthdaysInMonthAsync(int? month = null, int? year = null);
}
