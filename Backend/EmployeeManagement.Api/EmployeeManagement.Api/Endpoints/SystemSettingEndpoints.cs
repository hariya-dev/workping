// Endpoints/SystemSettingEndpoints.cs
// Minimal API endpoints cho cài đặt hệ thống

using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Entities;
using EmployeeManagement.Api.Jobs;
using EmployeeManagement.Api.Services;
using Hangfire;

namespace EmployeeManagement.Api.Endpoints;

/// <summary>
/// Định nghĩa các API endpoints cho System Settings
/// </summary>
public static class SystemSettingEndpoints
{
    /// <summary>
    /// Map các routes cho System Settings
    /// </summary>
    public static void MapSystemSettingEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/settings")
            .WithTags("System Settings")
            .RequireAuthorization();

        // GET /api/settings - Lấy tất cả cài đặt
        group.MapGet("/", async (ISystemSettingService service) =>
        {
            var settings = await service.GetAllAsync();
            return Results.Ok(ApiResult<List<SystemSettingDto>>.Ok(settings));
        })
        .WithName("GetSettings")
        .WithSummary("Lấy tất cả cài đặt hệ thống");

        // GET /api/settings/reminders - Lấy cài đặt reminder
        group.MapGet("/reminders", async (ISystemSettingService service) =>
        {
            var settings = await service.GetReminderSettingsAsync();
            return Results.Ok(ApiResult<ReminderSettingsDto>.Ok(settings));
        })
        .WithName("GetReminderSettings")
        .WithSummary("Lấy cài đặt nhắc nhở");

        // PUT /api/settings/reminders - Cập nhật cài đặt reminder
        group.MapPut("/reminders", async (ReminderSettingsDto dto, ISystemSettingService service) =>
        {
            var result = await service.UpdateReminderSettingsAsync(dto);
            
            if (!result.Success)
            {
                return Results.BadRequest(result);
            }
            
            return Results.Ok(result);
        })
        .WithName("UpdateReminderSettings")
        .WithSummary("Cập nhật cài đặt nhắc nhở");

        // PUT /api/settings/{key} - Cập nhật một cài đặt
        group.MapPut("/{key}", async (string key, UpdateSettingDto dto, ISystemSettingService service) =>
        {
            var result = await service.UpdateSettingAsync(key, dto.Value);
            
            if (!result.Success)
            {
                return Results.BadRequest(result);
            }
            
            return Results.Ok(result);
        })
        .WithName("UpdateSetting")
        .WithSummary("Cập nhật một cài đặt cụ thể");

        // POST /api/settings/test-email - Gửi email test kết nối SMTP
        group.MapPost("/test-email", async (TestEmailDto dto, IEmailService emailService, ILogger<IEmailService> logger) =>
        {
            try
            {
                var subject = "[Test] Kiểm tra hệ thống thông báo";
                var body = @"<html><body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #2563eb;'>Test gửi email thành công!</h2>
                    <p>Đây là email test từ Hệ thống Quản lý Nhân sự.</p>
                    <p>Nếu bạn nhận được email này, hệ thống thông báo đang hoạt động bình thường.</p>
                    <hr style='margin: 20px 0;'>
                    <p style='color: #666; font-size: 12px;'>An Tưởng Technology</p>
                </body></html>";

                await emailService.SendEmailAsync(new[] { dto.Email }, subject, body);
                logger.LogInformation("Đã gửi email test thành công đến: {Email}", dto.Email);
                return Results.Ok(ApiResult.Ok("Đã gửi email test thành công"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi gửi email test đến: {Email}", dto.Email);
                return Results.BadRequest(ApiResult.Fail($"Gửi email thất bại: {ex.Message}"));
            }
        })
        .WithName("SendTestEmail")
        .WithSummary("Gửi email test kết nối SMTP");

        // POST /api/settings/test-birthday-email - [1] Test email chúc mừng sinh nhật → nhân viên
        group.MapPost("/test-birthday-email", async (TestEmailDto dto, IEmailService emailService, ILogger<IEmailService> logger) =>
        {
            try
            {
                await emailService.SendBirthdayEmailAsync(
                    dto.Email,
                    "Nguyễn Văn A (Test)",
                    new DateOnly(1994, DateTime.Now.Month, DateTime.Now.Day)
                );
                logger.LogInformation("Đã gửi email sinh nhật test đến: {Email}", dto.Email);
                return Results.Ok(ApiResult.Ok("Đã gửi email sinh nhật test thành công (template BirthdayWish)"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi gửi email sinh nhật test đến: {Email}", dto.Email);
                return Results.BadRequest(ApiResult.Fail($"Gửi email sinh nhật thất bại: {ex.Message}"));
            }
        })
        .WithName("SendTestBirthdayEmail")
        .WithSummary("[1] Test email chúc mừng sinh nhật gửi cho nhân viên");

        // POST /api/settings/test-monthly-birthday-email - [2] Test email danh sách sinh nhật tháng → HR
        group.MapPost("/test-monthly-birthday-email", async (TestEmailDto dto, IEmailService emailService, ILogger<IEmailService> logger) =>
        {
            try
            {
                var now = DateTime.Now;
                var sampleBirthdays = new List<(string Name, DateOnly Birthday, string? Department)>
                {
                    ("Nguyễn Văn A", new DateOnly(1990, now.Month, 5), "Phòng Kỹ thuật"),
                    ("Trần Thị B", new DateOnly(1995, now.Month, 12), "Phòng Nhân sự"),
                    ("Lê Văn C", new DateOnly(1988, now.Month, 20), "Phòng Kinh doanh"),
                };

                await emailService.SendBirthdayListEmailAsync(
                    new[] { dto.Email },
                    sampleBirthdays,
                    now.Month, now.Year);

                logger.LogInformation("Đã gửi email danh sách sinh nhật tháng test đến: {Email}", dto.Email);
                return Results.Ok(ApiResult.Ok("Đã gửi email danh sách sinh nhật tháng test thành công (template MonthlyBirthdayList)"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi gửi email danh sách sinh nhật tháng test đến: {Email}", dto.Email);
                return Results.BadRequest(ApiResult.Fail($"Gửi email thất bại: {ex.Message}"));
            }
        })
        .WithName("SendTestMonthlyBirthdayEmail")
        .WithSummary("[2] Test email danh sách sinh nhật tháng gửi cho HR");

        // POST /api/settings/test-probation-email - [3] Test email nhắc nhở thử việc → HR
        group.MapPost("/test-probation-email", async (TestEmailDto dto, IEmailService emailService, ILogger<IEmailService> logger) =>
        {
            try
            {
                await emailService.SendProbationReminderHrEmailAsync(
                    new[] { dto.Email },
                    "Nguyễn Văn A (Test)",
                    "Phòng Kỹ thuật",
                    DateOnly.FromDateTime(DateTime.Now.AddDays(15)),
                    15
                );
                logger.LogInformation("Đã gửi email nhắc nhở thử việc test đến: {Email}", dto.Email);
                return Results.Ok(ApiResult.Ok("Đã gửi email nhắc nhở thử việc test thành công (template ProbationReminderHr)"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi gửi email nhắc nhở thử việc test đến: {Email}", dto.Email);
                return Results.BadRequest(ApiResult.Fail($"Gửi email thất bại: {ex.Message}"));
            }
        })
        .WithName("SendTestProbationEmail")
        .WithSummary("[3] Test email nhắc nhở thử việc gửi cho HR");

        // POST /api/settings/test-contract-email - [4] Test email nhắc nhở hợp đồng → HR
        group.MapPost("/test-contract-email", async (TestEmailDto dto, IEmailService emailService, ILogger<IEmailService> logger) =>
        {
            try
            {
                await emailService.SendContractReminderHrEmailAsync(
                    new[] { dto.Email },
                    "Nguyễn Văn A (Test)",
                    "Phòng Kỹ thuật",
                    DateOnly.FromDateTime(DateTime.Now.AddDays(30)),
                    30
                );
                logger.LogInformation("Đã gửi email nhắc nhở hợp đồng test đến: {Email}", dto.Email);
                return Results.Ok(ApiResult.Ok("Đã gửi email nhắc nhở hợp đồng test thành công (template ContractReminderHr)"));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi gửi email nhắc nhở hợp đồng test đến: {Email}", dto.Email);
                return Results.BadRequest(ApiResult.Fail($"Gửi email thất bại: {ex.Message}"));
            }
        })
        .WithName("SendTestContractEmail")
        .WithSummary("[4] Test email nhắc nhở hợp đồng gửi cho HR");

        // POST /api/settings/trigger-jobs - Kích hoạt chạy tất cả jobs ngay lập tức
        group.MapPost("/trigger-jobs", async (IBackgroundJobClient backgroundJob, ILogger<IEmailService> logger) =>
        {
            try
            {
                backgroundJob.Enqueue<ReminderJobs>(j => j.SendDailyBirthdayEmailsAsync());
                backgroundJob.Enqueue<ReminderJobs>(j => j.SendMonthlyBirthdayListAsync());
                backgroundJob.Enqueue<ReminderJobs>(j => j.CheckProbationRemindersAsync());
                backgroundJob.Enqueue<ReminderJobs>(j => j.CheckContractRemindersAsync());

                logger.LogInformation("Đã kích hoạt chạy tất cả 4 jobs thủ công");
                return Results.Ok(ApiResult.Ok("Đã kích hoạt chạy tất cả jobs. Hệ thống sẽ xử lý ngay."));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi kích hoạt jobs thủ công");
                return Results.BadRequest(ApiResult.Fail($"Kích hoạt jobs thất bại: {ex.Message}"));
            }
        })
        .WithName("TriggerAllJobs")
        .WithSummary("Kích hoạt chạy tất cả background jobs ngay lập tức");
    }
}

/// <summary>
/// DTO gửi email test
/// </summary>
public class TestEmailDto
{
    public string Email { get; set; } = string.Empty;
}
