// Endpoints/DashboardEndpoints.cs
// Minimal API endpoints cho Dashboard

using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Services;

namespace EmployeeManagement.Api.Endpoints;

/// <summary>
/// Định nghĩa các API endpoints cho Dashboard
/// </summary>
public static class DashboardEndpoints
{
    /// <summary>
    /// Map các routes cho Dashboard
    /// </summary>
    public static void MapDashboardEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/dashboard")
            .WithTags("Dashboard")
            .RequireAuthorization();

        // GET /api/dashboard/stats - Lấy thống kê tổng quan
        group.MapGet("/stats", async (IDashboardService service) =>
        {
            var stats = await service.GetDashboardStatsAsync();
            return Results.Ok(ApiResult<DashboardStatsDto>.Ok(stats));
        })
        .WithName("GetDashboardStats")
        .WithSummary("Lấy thống kê tổng quan cho dashboard");

        // GET /api/dashboard/probation-expiring - Lấy danh sách sắp hết thử việc
        group.MapGet("/probation-expiring", async (int? daysAhead, IDashboardService service) =>
        {
            var result = await service.GetProbationExpiringAsync(daysAhead ?? 30);
            return Results.Ok(ApiResult<List<UpcomingReminderDto>>.Ok(result));
        })
        .WithName("GetProbationExpiring")
        .WithSummary("Lấy danh sách nhân viên sắp hết thử việc");

        // GET /api/dashboard/contract-expiring - Lấy danh sách sắp hết hợp đồng
        group.MapGet("/contract-expiring", async (int? daysAhead, IDashboardService service) =>
        {
            var result = await service.GetContractExpiringAsync(daysAhead ?? 30);
            return Results.Ok(ApiResult<List<UpcomingReminderDto>>.Ok(result));
        })
        .WithName("GetContractExpiring")
        .WithSummary("Lấy danh sách nhân viên sắp hết hợp đồng");

        // GET /api/dashboard/birthdays - Lấy danh sách sinh nhật trong tháng
        group.MapGet("/birthdays", async (int? month, int? year, IDashboardService service) =>
        {
            var result = await service.GetBirthdaysInMonthAsync(month, year);
            return Results.Ok(ApiResult<List<BirthdayDto>>.Ok(result));
        })
        .WithName("GetBirthdays")
        .WithSummary("Lấy danh sách sinh nhật trong tháng");
    }
}
