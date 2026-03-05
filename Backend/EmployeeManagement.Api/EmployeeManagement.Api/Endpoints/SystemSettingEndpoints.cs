// Endpoints/SystemSettingEndpoints.cs
// Minimal API endpoints cho cài đặt hệ thống

using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Entities;
using EmployeeManagement.Api.Services;

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

        // POST /api/settings/test-email - Gửi email test
        group.MapPost("/test-email", async (TestEmailDto dto, IEmailService emailService, ILogger<IEmailService> logger) =>
        {
            try
            {
                var subject = "[Test] Kiểm tra hệ thống thông báo";
                var body = @"
                <html>
                <body style='font-family: Arial, sans-serif;'>
                    <h2 style='color: #2563eb;'>Test gửi email thành công!</h2>
                    <p>Đây là email test từ Hệ thống Quản lý Nhân sự.</p>
                    <p>Nếu bạn nhận được email này, hệ thống thông báo đang hoạt động bình thường.</p>
                    <hr style='margin: 20px 0;'>
                    <p style='color: #666; font-size: 12px;'>
                        Email test từ Hệ thống Quản lý Nhân sự<br>
                        An Tưởng Technology
                    </p>
                </body>
                </html>";

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
        .WithSummary("Gửi email test để kiểm tra hệ thống thông báo");

        // POST /api/settings/test-birthday-email - Test email sinh nhật (gửi cho nhân viên)
        group.MapPost("/test-birthday-email", async (TestEmailDto dto, IEmailService emailService, ILogger<IEmailService> logger) =>
        {
            try
            {
                // Gửi email sinh nhật cho nhân viên (sử dụng template BirthdayWish - type 3)
                var success = await emailService.SendBirthdayEmailAsync(
                    dto.Email, 
                    "Nguyễn Văn A (Nhân viên test)", 
                    new DateOnly(1994, 2, 6)
                );
                
                if (success)
                {
                    logger.LogInformation("Đã gửi email sinh nhật test (nhân viên) thành công đến: {Email}", dto.Email);
                    return Results.Ok(ApiResult.Ok("Đã gửi email sinh nhật test (nhân viên) thành công. Email này sử dụng template 'Chúc mừng sinh nhật' từ database."));
                }
                else
                {
                    return Results.BadRequest(ApiResult.Fail("Gửi email sinh nhật thất bại"));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi gửi email sinh nhật test đến: {Email}", dto.Email);
                return Results.BadRequest(ApiResult.Fail($"Gửi email sinh nhật thất bại: {ex.Message}"));
            }
        })
        .WithName("SendTestBirthdayEmail")
        .WithSummary("Gửi email test sinh nhật cho nhân viên (template từ database)");

        // POST /api/settings/test-probation-email - Test email thử việc (gửi cho nhân viên)
        group.MapPost("/test-probation-email", async (TestEmailDto dto, IEmailService emailService, ILogger<IEmailService> logger) =>
        {
            try
            {
                // Gửi email nhắc nhở thử việc cho nhân viên (sử dụng template ProbationReminder - type 1)
                await emailService.SendProbationReminderEmailAsync(
                    "Nguyễn Văn A (Nhân viên test)",
                    dto.Email,
                    DateOnly.FromDateTime(DateTime.Now.AddDays(15)),
                    15
                );
                
                logger.LogInformation("Đã gửi email thử việc test (nhân viên) thành công đến: {Email}", dto.Email);
                return Results.Ok(ApiResult.Ok("Đã gửi email thử việc test (nhân viên) thành công. Email này sử dụng template 'Nhắc nhở thử việc (Nhân viên)' từ database."));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi gửi email thử việc test đến: {Email}", dto.Email);
                return Results.BadRequest(ApiResult.Fail($"Gửi email thử việc thất bại: {ex.Message}"));
            }
        })
        .WithName("SendTestProbationEmail")
        .WithSummary("Gửi email test nhắc nhở thử việc cho nhân viên (template từ database)");

        // POST /api/settings/test-contract-email - Test email hợp đồng (gửi cho nhân viên)
        group.MapPost("/test-contract-email", async (TestEmailDto dto, IEmailService emailService, ILogger<IEmailService> logger) =>
        {
            try
            {
                // Gửi email nhắc nhở hợp đồng cho nhân viên (sử dụng template ContractReminder - type 2)
                await emailService.SendContractReminderEmailAsync(
                    "Nguyễn Văn A (Nhân viên test)",
                    dto.Email,
                    DateOnly.FromDateTime(DateTime.Now.AddDays(30)),
                    30
                );
                
                logger.LogInformation("Đã gửi email hợp đồng test (nhân viên) thành công đến: {Email}", dto.Email);
                return Results.Ok(ApiResult.Ok("Đã gửi email hợp đồng test (nhân viên) thành công. Email này sử dụng template 'Nhắc nhở hợp đồng (Nhân viên)' từ database."));
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi gửi email hợp đồng test đến: {Email}", dto.Email);
                return Results.BadRequest(ApiResult.Fail($"Gửi email hợp đồng thất bại: {ex.Message}"));
            }
        })
        .WithName("SendTestContractEmail")
        .WithSummary("Gửi email test nhắc nhở hợp đồng cho nhân viên (template từ database)");

        // POST /api/settings/test-probation-email-hr - Test email thử việc (gửi cho HR)
        group.MapPost("/test-probation-email-hr", async (TestEmailDto dto, IEmailTemplateService templateService, IEmailService emailService, ILogger<IEmailService> logger) =>
        {
            try
            {
                // Sử dụng template ProbationReminderHr - type 5
                var data = new Dictionary<string, string>
                {
                    ["EmployeeName"] = "Nguyễn Văn A (Nhân viên test)",
                    ["Department"] = "Phòng Kỹ thuật",
                    ["EndDate"] = DateOnly.FromDateTime(DateTime.Now.AddDays(15)).ToString("dd/MM/yyyy"),
                    ["DaysRemaining"] = "15"
                };

                var template = await templateService.RenderTemplateAsync(EmailTemplateType.ProbationReminderHr, data);
                
                if (template.HasValue)
                {
                    await emailService.SendEmailAsync(new[] { dto.Email }, template.Value.Subject, template.Value.BodyHtml);
                    logger.LogInformation("Đã gửi email thử việc test (HR) thành công đến: {Email}", dto.Email);
                    return Results.Ok(ApiResult.Ok("Đã gửi email thử việc test (HR) thành công. Email này sử dụng template 'Nhắc nhở thử việc (HR)' từ database."));
                }
                else
                {
                    return Results.BadRequest(ApiResult.Fail("Không tìm thấy template email cho HR"));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi gửi email thử việc test (HR) đến: {Email}", dto.Email);
                return Results.BadRequest(ApiResult.Fail($"Gửi email thử việc (HR) thất bại: {ex.Message}"));
            }
        })
        .WithName("SendTestProbationEmailHr")
        .WithSummary("Gửi email test nhắc nhở thử việc cho HR (template từ database)");

        // POST /api/settings/test-contract-email-hr - Test email hợp đồng (gửi cho HR)
        group.MapPost("/test-contract-email-hr", async (TestEmailDto dto, IEmailTemplateService templateService, IEmailService emailService, ILogger<IEmailService> logger) =>
        {
            try
            {
                // Sử dụng template ContractReminderHr - type 6
                var data = new Dictionary<string, string>
                {
                    ["EmployeeName"] = "Nguyễn Văn A (Nhân viên test)",
                    ["Department"] = "Phòng Kỹ thuật",
                    ["EndDate"] = DateOnly.FromDateTime(DateTime.Now.AddDays(30)).ToString("dd/MM/yyyy"),
                    ["DaysRemaining"] = "30"
                };

                var template = await templateService.RenderTemplateAsync(EmailTemplateType.ContractReminderHr, data);
                
                if (template.HasValue)
                {
                    await emailService.SendEmailAsync(new[] { dto.Email }, template.Value.Subject, template.Value.BodyHtml);
                    logger.LogInformation("Đã gửi email hợp đồng test (HR) thành công đến: {Email}", dto.Email);
                    return Results.Ok(ApiResult.Ok("Đã gửi email hợp đồng test (HR) thành công. Email này sử dụng template 'Nhắc nhở hợp đồng (HR)' từ database."));
                }
                else
                {
                    return Results.BadRequest(ApiResult.Fail("Không tìm thấy template email cho HR"));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi gửi email hợp đồng test (HR) đến: {Email}", dto.Email);
                return Results.BadRequest(ApiResult.Fail($"Gửi email hợp đồng (HR) thất bại: {ex.Message}"));
            }
        })
        .WithName("SendTestContractEmailHr")
        .WithSummary("Gửi email test nhắc nhở hợp đồng cho HR (template từ database)");

        // POST /api/settings/test-monthly-birthday-email - Test email danh sách sinh nhật tháng (HR)
        group.MapPost("/test-monthly-birthday-email", async (TestEmailDto dto, IEmailTemplateService templateService, IEmailService emailService, ILogger<IEmailService> logger) =>
        {
            try
            {
                var now = DateTime.Now;
                var month = now.Month;
                var year = now.Year;

                // Dữ liệu test mẫu
                var sampleBirthdays = new List<(string Name, DateOnly Birthday, string? Department)>
                {
                    ("Nguyễn Văn A", new DateOnly(1990, month, 5), "Phòng Kỹ thuật"),
                    ("Trần Thị B", new DateOnly(1995, month, 12), "Phòng Nhân sự"),
                    ("Lê Văn C", new DateOnly(1988, month, 20), "Phòng Kinh doanh"),
                    ("Phạm Thị D", new DateOnly(1992, month, 25), "Phòng Kế toán")
                };

                // Tạo bảng HTML danh sách sinh nhật
                var birthdayRows = string.Join("", sampleBirthdays.OrderBy(b => b.Birthday.Day).Select(b => $@"
                    <tr>
                        <td style='padding: 8px; border: 1px solid #ddd;'>{b.Birthday.Day:D2}/{month:D2}</td>
                        <td style='padding: 8px; border: 1px solid #ddd;'>{b.Name}</td>
                        <td style='padding: 8px; border: 1px solid #ddd;'>{b.Department ?? "-"}</td>
                        <td style='padding: 8px; border: 1px solid #ddd;'>{year - b.Birthday.Year} tuổi</td>
                    </tr>"));

                // Sử dụng template MonthlyBirthdayList - type 4
                var data = new Dictionary<string, string>
                {
                    ["CurrentMonth"] = month.ToString(),
                    ["CurrentYear"] = year.ToString(),
                    ["BirthdayList"] = $"<table width='100%' style='width: 100%; border-collapse: collapse; margin: 10px 0;'><thead><tr style='background: #ff6b6b; color: white;'><th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Ngày</th><th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Họ tên</th><th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Phòng ban</th><th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Tuổi</th></tr></thead><tbody>{birthdayRows}</tbody></table>",
                    ["TotalCount"] = sampleBirthdays.Count.ToString()
                };

                var template = await templateService.RenderTemplateAsync(EmailTemplateType.MonthlyBirthdayList, data);
                
                if (template.HasValue)
                {
                    await emailService.SendEmailAsync(new[] { dto.Email }, template.Value.Subject, template.Value.BodyHtml);
                    logger.LogInformation("Đã gửi email danh sách sinh nhật tháng test (HR) thành công đến: {Email}", dto.Email);
                    return Results.Ok(ApiResult.Ok("Đã gửi email danh sách sinh nhật tháng test (HR) thành công. Email này sử dụng template 'Danh sách sinh nhật tháng' từ database."));
                }
                else
                {
                    return Results.BadRequest(ApiResult.Fail("Không tìm thấy template email danh sách sinh nhật tháng"));
                }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Lỗi khi gửi email danh sách sinh nhật tháng test đến: {Email}", dto.Email);
                return Results.BadRequest(ApiResult.Fail($"Gửi email danh sách sinh nhật tháng thất bại: {ex.Message}"));
            }
        })
        .WithName("SendTestMonthlyBirthdayEmail")
        .WithSummary("Gửi email test danh sách sinh nhật tháng cho HR (template từ database)");
    }
}

/// <summary>
/// DTO gửi email test
/// </summary>
public class TestEmailDto
{
    public string Email { get; set; } = string.Empty;
}
