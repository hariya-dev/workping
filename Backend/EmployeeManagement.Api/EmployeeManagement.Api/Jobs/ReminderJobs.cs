// Jobs/ReminderJobs.cs
// Hangfire recurring jobs cho nhắc nhở tự động

using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Entities;
using EmployeeManagement.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Jobs;

/// <summary>
/// Class chứa các background jobs cho hệ thống nhắc nhở
/// Sử dụng Hangfire để chạy định kỳ
/// </summary>
public class ReminderJobs
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReminderJobs> _logger;

    public ReminderJobs(IServiceProvider serviceProvider, ILogger<ReminderJobs> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    /// <summary>
    /// Job kiểm tra và gửi nhắc nhở thử việc hàng ngày
    /// Chạy mỗi ngày lúc 8:00 sáng
    /// Kiểm tra xem hôm nay có nằm trong danh sách ngày nhắc nhở trước khi hết thử việc không
    /// </summary>
    public async Task CheckProbationRemindersAsync()
    {
        _logger.LogInformation("Bắt đầu kiểm tra nhắc nhở thử việc - {Time}", DateTime.Now);

        try
        {
            // Tạo scope mới để lấy services
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var settingService = scope.ServiceProvider.GetRequiredService<ISystemSettingService>();

            // Lấy cài đặt số ngày nhắc nhở
            var reminderDays = await settingService.GetIntArrayValueAsync("ProbationReminderDaysBefore");
            if (!reminderDays.Any())
            {
                reminderDays = new[] { 30, 15, 7, 3, 1 }; // Giá trị mặc định
            }

            var today = DateOnly.FromDateTime(DateTime.Today);

            // Lấy email HR để gửi thông báo
            var hrEmails = await settingService.GetValueAsync("HrNotificationEmails") ?? "";
            var hrEmailList = hrEmails.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim())
                .Where(e => !string.IsNullOrEmpty(e))
                .ToList();

            // Tìm nhân viên đang làm việc có ngày kết thúc thử việc nằm trong khoảng nhắc nhở
            var employees = await context.Employees
                .Where(e => e.Status == EmployeeStatus.Active && e.ProbationEndDate.HasValue)
                .ToListAsync();

            var sentCount = 0;

            foreach (var employee in employees)
            {
                var endDate = employee.ProbationEndDate!.Value;
                var daysRemaining = endDate.DayNumber - today.DayNumber;

                // Kiểm tra xem số ngày còn lại có nằm trong danh sách nhắc nhở không
                if (daysRemaining >= 0 && reminderDays.Contains(daysRemaining))
                {
                    _logger.LogInformation(
                        "Gửi nhắc nhở thử việc cho {EmployeeName} - Còn {Days} ngày", 
                        employee.FullName, daysRemaining);

                    // Gửi email cho nhân viên (nếu có email)
                    if (!string.IsNullOrWhiteSpace(employee.Email))
                    {
                        await emailService.SendProbationReminderEmailAsync(
                            employee.FullName,
                            employee.Email,
                            endDate,
                            daysRemaining);
                    }

                    // Gửi email cho HR
                    if (hrEmailList.Any())
                    {
                        await emailService.SendEmailAsync(
                            hrEmailList,
                            $"[Nhắc nhở Thử việc] {employee.FullName} - Còn {daysRemaining} ngày",
                            BuildProbationReminderHtml(employee.FullName, endDate, daysRemaining, employee.Department));
                    }

                    sentCount++;
                }
            }

            _logger.LogInformation("Hoàn thành kiểm tra nhắc nhở thử việc - Đã gửi {Count} email", sentCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi kiểm tra nhắc nhở thử việc");
            throw;
        }
    }

    /// <summary>
    /// Job kiểm tra và gửi nhắc nhở hợp đồng hàng ngày
    /// Chạy mỗi ngày lúc 8:30 sáng
    /// </summary>
    public async Task CheckContractRemindersAsync()
    {
        _logger.LogInformation("Bắt đầu kiểm tra nhắc nhở hợp đồng - {Time}", DateTime.Now);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var settingService = scope.ServiceProvider.GetRequiredService<ISystemSettingService>();

            // Lấy cài đặt số ngày nhắc nhở
            var reminderDays = await settingService.GetIntArrayValueAsync("ContractReminderDaysBefore");
            if (!reminderDays.Any())
            {
                reminderDays = new[] { 30, 15, 7, 3, 1 };
            }

            var today = DateOnly.FromDateTime(DateTime.Today);

            // Lấy email HR
            var hrEmails = await settingService.GetValueAsync("HrNotificationEmails") ?? "";
            var hrEmailList = hrEmails.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim())
                .Where(e => !string.IsNullOrEmpty(e))
                .ToList();

            // Tìm hợp đồng đang active có ngày kết thúc sắp tới
            var activeContracts = await context.EmployeeContracts
                .Include(c => c.Employee)
                .Where(c => c.Employee!.Status == EmployeeStatus.Active 
                    && c.EndDate.HasValue 
                    && c.EndDate >= today)
                .ToListAsync();

            var sentCount = 0;

            foreach (var contract in activeContracts)
            {
                var employee = contract.Employee!;
                var endDate = contract.EndDate!.Value;
                var daysRemaining = endDate.DayNumber - today.DayNumber;

                if (daysRemaining >= 0 && reminderDays.Contains(daysRemaining))
                {
                    _logger.LogInformation(
                        "Gửi nhắc nhở hợp đồng cho {EmployeeName} - Còn {Days} ngày", 
                        employee.FullName, daysRemaining);

                    // Gửi email cho nhân viên
                    if (!string.IsNullOrWhiteSpace(employee.Email))
                    {
                        await emailService.SendContractReminderEmailAsync(
                            employee.FullName,
                            employee.Email,
                            endDate,
                            daysRemaining);
                    }

                    // Gửi email cho HR
                    if (hrEmailList.Any())
                    {
                        await emailService.SendEmailAsync(
                            hrEmailList,
                            $"[Nhắc nhở Hợp đồng] {employee.FullName} - Còn {daysRemaining} ngày",
                            BuildContractReminderHtml(employee.FullName, endDate, daysRemaining, employee.Department));
                    }

                    sentCount++;
                }
            }

            _logger.LogInformation("Hoàn thành kiểm tra nhắc nhở hợp đồng - Đã gửi {Count} email", sentCount);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi kiểm tra nhắc nhở hợp đồng");
            throw;
        }
    }

    /// <summary>
    /// Job gửi danh sách sinh nhật trong tháng
    /// Chạy vào ngày 1 hàng tháng lúc 9:00 sáng
    /// </summary>
    public async Task SendMonthlyBirthdayListAsync()
    {
        _logger.LogInformation("Bắt đầu gửi danh sách sinh nhật tháng - {Time}", DateTime.Now);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var settingService = scope.ServiceProvider.GetRequiredService<ISystemSettingService>();

            var today = DateTime.Today;
            var currentMonth = today.Month;
            var currentYear = today.Year;

            // Lấy email HR
            var hrEmails = await settingService.GetValueAsync("HrNotificationEmails") ?? "";
            var hrEmailList = hrEmails.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim())
                .Where(e => !string.IsNullOrEmpty(e))
                .ToList();

            if (!hrEmailList.Any())
            {
                _logger.LogWarning("Không có email HR để gửi danh sách sinh nhật");
                return;
            }

            // Lấy danh sách nhân viên có sinh nhật trong tháng
            var birthdays = await context.Employees
                .Where(e => e.Status != EmployeeStatus.Resigned
                    && e.DateOfBirth.Month == currentMonth)
                .OrderBy(e => e.DateOfBirth.Day)
                .Select(e => new { e.FullName, e.DateOfBirth, e.Department })
                .ToListAsync();

            if (!birthdays.Any())
            {
                _logger.LogInformation("Không có sinh nhật trong tháng {Month}/{Year}", currentMonth, currentYear);
                return;
            }

            // Chuyển đổi sang tuple format
            var birthdayList = birthdays
                .Select(b => (b.FullName, b.DateOfBirth, b.Department))
                .ToList();

            // Gửi email
            await emailService.SendBirthdayListEmailAsync(hrEmailList, birthdayList, currentMonth, currentYear);

            _logger.LogInformation("Đã gửi danh sách {Count} sinh nhật tháng {Month}/{Year}", 
                birthdays.Count, currentMonth, currentYear);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gửi danh sách sinh nhật tháng");
            throw;
        }
    }

    /// <summary>
    /// Tạo nội dung HTML cho email nhắc nhở thử việc
    /// </summary>
    private static string BuildProbationReminderHtml(string employeeName, DateOnly endDate, int daysRemaining, string? department)
    {
        return $@"
        <html>
        <body style='font-family: Arial, sans-serif;'>
            <h2 style='color: #2563eb;'>Nhắc nhở Thời gian thử việc sắp kết thúc</h2>
            <table style='border-collapse: collapse; margin: 20px 0;'>
                <tr>
                    <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Họ và tên:</td>
                    <td style='padding: 8px; border: 1px solid #ddd;'><strong>{employeeName}</strong></td>
                </tr>
                <tr>
                    <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Phòng ban:</td>
                    <td style='padding: 8px; border: 1px solid #ddd;'>{department ?? "-"}</td>
                </tr>
                <tr>
                    <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Ngày kết thúc thử việc:</td>
                    <td style='padding: 8px; border: 1px solid #ddd;'><strong>{endDate:dd/MM/yyyy}</strong></td>
                </tr>
                <tr>
                    <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Số ngày còn lại:</td>
                    <td style='padding: 8px; border: 1px solid #ddd; color: #dc2626;'><strong>{daysRemaining} ngày</strong></td>
                </tr>
            </table>
            <p>Vui lòng xem xét đánh giá và quyết định gia hạn hợp đồng cho nhân viên này.</p>
        </body>
        </html>";
    }

    /// <summary>
    /// Tạo nội dung HTML cho email nhắc nhở hợp đồng
    /// </summary>
    private static string BuildContractReminderHtml(string employeeName, DateOnly endDate, int daysRemaining, string? department)
    {
        return $@"
        <html>
        <body style='font-family: Arial, sans-serif;'>
            <h2 style='color: #dc2626;'>Nhắc nhở Hợp đồng lao động sắp hết hạn</h2>
            <table style='border-collapse: collapse; margin: 20px 0;'>
                <tr>
                    <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Họ và tên:</td>
                    <td style='padding: 8px; border: 1px solid #ddd;'><strong>{employeeName}</strong></td>
                </tr>
                <tr>
                    <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Phòng ban:</td>
                    <td style='padding: 8px; border: 1px solid #ddd;'>{department ?? "-"}</td>
                </tr>
                <tr>
                    <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Ngày hết hạn hợp đồng:</td>
                    <td style='padding: 8px; border: 1px solid #ddd;'><strong>{endDate:dd/MM/yyyy}</strong></td>
                </tr>
                <tr>
                    <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Số ngày còn lại:</td>
                    <td style='padding: 8px; border: 1px solid #ddd; color: #dc2626;'><strong>{daysRemaining} ngày</strong></td>
                </tr>
            </table>
            <p>Vui lòng liên hệ nhân viên để thảo luận về việc gia hạn hợp đồng.</p>
        </body>
        </html>";
    }
}
