// Jobs/ReminderJobs.cs
// Hangfire recurring jobs cho nhắc nhở tự động
// 4 loại mail:
// 1. Sinh nhật → nhân viên (hàng ngày 8:00)
// 2. Danh sách sinh nhật tháng → HR (ngày 1 hàng tháng 8:00)
// 3. Thử việc sắp kết thúc → HR (hàng ngày 8:30, theo số ngày cấu hình)
// 4. Hợp đồng sắp hết hạn → HR (hàng ngày 9:00, theo số ngày cấu hình)

using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.Entities;
using EmployeeManagement.Api.Services;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Jobs;

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
    /// [1] Gửi email chúc mừng sinh nhật cho nhân viên có sinh nhật hôm nay
    /// Chạy hàng ngày lúc 8:00 sáng
    /// </summary>
    public async Task SendDailyBirthdayEmailsAsync()
    {
        _logger.LogInformation("Bắt đầu kiểm tra sinh nhật hôm nay - {Time}", DateTime.Now);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();

            var today = DateTime.Today;

            var employees = await context.Employees
                .Where(e => e.Status == EmployeeStatus.Active
                    && !string.IsNullOrEmpty(e.Email)
                    && e.DateOfBirth.Month == today.Month
                    && e.DateOfBirth.Day == today.Day)
                .ToListAsync();

            foreach (var employee in employees)
            {
                _logger.LogInformation("Gửi email sinh nhật cho {EmployeeName}", employee.FullName);
                await emailService.SendBirthdayEmailAsync(employee.Email!, employee.FullName, employee.DateOfBirth);
            }

            _logger.LogInformation("Hoàn thành gửi email sinh nhật - Đã gửi {Count} email", employees.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gửi email sinh nhật");
            throw;
        }
    }

    /// <summary>
    /// [2] Gửi danh sách sinh nhật trong tháng cho HR
    /// Chạy vào ngày 1 hàng tháng lúc 8:00 sáng
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

            var birthdays = await context.Employees
                .Where(e => e.Status == EmployeeStatus.Active
                    && e.DateOfBirth.Month == today.Month)
                .OrderBy(e => e.DateOfBirth.Day)
                .Select(e => new { e.FullName, e.DateOfBirth, e.Department })
                .ToListAsync();

            if (!birthdays.Any())
            {
                _logger.LogInformation("Không có sinh nhật trong tháng {Month}/{Year}", today.Month, today.Year);
                return;
            }

            var birthdayList = birthdays
                .Select(b => (b.FullName, b.DateOfBirth, b.Department))
                .ToList();

            await emailService.SendBirthdayListEmailAsync(hrEmailList, birthdayList, today.Month, today.Year);

            _logger.LogInformation("Đã gửi danh sách {Count} sinh nhật tháng {Month}/{Year}",
                birthdays.Count, today.Month, today.Year);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi gửi danh sách sinh nhật tháng");
            throw;
        }
    }

    /// <summary>
    /// [3] Gửi nhắc nhở thử việc sắp kết thúc cho HR
    /// Chạy hàng ngày lúc 8:30 sáng, theo số ngày cấu hình
    /// </summary>
    public async Task CheckProbationRemindersAsync()
    {
        _logger.LogInformation("Bắt đầu kiểm tra nhắc nhở thử việc - {Time}", DateTime.Now);

        try
        {
            using var scope = _serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var emailService = scope.ServiceProvider.GetRequiredService<IEmailService>();
            var settingService = scope.ServiceProvider.GetRequiredService<ISystemSettingService>();

            var reminderDays = await settingService.GetIntArrayValueAsync("ProbationReminderDaysBefore");
            if (!reminderDays.Any())
                reminderDays = new[] { 30, 15, 7, 3, 1 };

            var today = DateOnly.FromDateTime(DateTime.Today);

            var hrEmails = await settingService.GetValueAsync("HrNotificationEmails") ?? "";
            var hrEmailList = hrEmails.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim())
                .Where(e => !string.IsNullOrEmpty(e))
                .ToList();

            if (!hrEmailList.Any())
            {
                _logger.LogWarning("Không có email HR để gửi nhắc nhở thử việc");
                return;
            }

            var employees = await context.Employees
                .Where(e => e.Status == EmployeeStatus.Active && e.ProbationEndDate.HasValue)
                .ToListAsync();

            var sentCount = 0;

            foreach (var employee in employees)
            {
                var endDate = employee.ProbationEndDate!.Value;
                var daysRemaining = endDate.DayNumber - today.DayNumber;

                if (daysRemaining >= 0 && reminderDays.Contains(daysRemaining))
                {
                    _logger.LogInformation("Gửi nhắc nhở thử việc cho HR - {EmployeeName}, còn {Days} ngày",
                        employee.FullName, daysRemaining);

                    await emailService.SendProbationReminderHrEmailAsync(
                        hrEmailList,
                        employee.FullName,
                        employee.Department,
                        endDate,
                        daysRemaining);

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
    /// [4] Gửi nhắc nhở hợp đồng sắp hết hạn cho HR
    /// Chạy hàng ngày lúc 9:00 sáng, theo số ngày cấu hình
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

            var reminderDays = await settingService.GetIntArrayValueAsync("ContractReminderDaysBefore");
            if (!reminderDays.Any())
                reminderDays = new[] { 30, 15, 7, 3, 1 };

            var today = DateOnly.FromDateTime(DateTime.Today);

            var hrEmails = await settingService.GetValueAsync("HrNotificationEmails") ?? "";
            var hrEmailList = hrEmails.Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(e => e.Trim())
                .Where(e => !string.IsNullOrEmpty(e))
                .ToList();

            if (!hrEmailList.Any())
            {
                _logger.LogWarning("Không có email HR để gửi nhắc nhở hợp đồng");
                return;
            }

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
                    _logger.LogInformation("Gửi nhắc nhở hợp đồng cho HR - {EmployeeName}, còn {Days} ngày",
                        employee.FullName, daysRemaining);

                    await emailService.SendContractReminderHrEmailAsync(
                        hrEmailList,
                        employee.FullName,
                        employee.Department,
                        endDate,
                        daysRemaining);

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
}
