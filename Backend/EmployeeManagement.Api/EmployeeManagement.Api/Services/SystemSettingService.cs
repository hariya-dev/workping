// Services/SystemSettingService.cs
// Triển khai dịch vụ cài đặt hệ thống

using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Services;

/// <summary>
/// Service quản lý cài đặt hệ thống
/// Đọc/ghi settings từ database
/// </summary>
public class SystemSettingService : ISystemSettingService
{
    private readonly AppDbContext _context;
    private readonly ILogger<SystemSettingService> _logger;

    public SystemSettingService(AppDbContext context, ILogger<SystemSettingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Lấy tất cả cài đặt
    /// </summary>
    public async Task<List<SystemSettingDto>> GetAllAsync()
    {
        return await _context.SystemSettings
            .OrderBy(s => s.Key)
            .Select(s => new SystemSettingDto
            {
                Id = s.Id,
                Key = s.Key,
                Value = s.Value,
                ValueType = s.ValueType,
                Description = s.Description,
                UpdatedAt = s.UpdatedAt
            })
            .ToListAsync();
    }

    /// <summary>
    /// Lấy giá trị theo key
    /// </summary>
    public async Task<string?> GetValueAsync(string key)
    {
        var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == key);
        return setting?.Value;
    }

    /// <summary>
    /// Lấy giá trị dạng int
    /// </summary>
    public async Task<int> GetIntValueAsync(string key, int defaultValue = 0)
    {
        var value = await GetValueAsync(key);
        if (string.IsNullOrWhiteSpace(value)) return defaultValue;
        return int.TryParse(value, out var result) ? result : defaultValue;
    }

    /// <summary>
    /// Lấy giá trị dạng array int
    /// </summary>
    public async Task<int[]> GetIntArrayValueAsync(string key)
    {
        var value = await GetValueAsync(key);
        if (string.IsNullOrWhiteSpace(value)) return Array.Empty<int>();

        return value.Split(',')
            .Select(v => int.TryParse(v.Trim(), out var num) ? num : 0)
            .Where(v => v > 0)
            .OrderByDescending(v => v)
            .ToArray();
    }

    /// <summary>
    /// Cập nhật cài đặt
    /// </summary>
    public async Task<ApiResult> UpdateSettingAsync(string key, string value)
    {
        try
        {
            var setting = await _context.SystemSettings.FirstOrDefaultAsync(s => s.Key == key);
            
            if (setting == null)
            {
                // Tạo mới nếu chưa có
                setting = new SystemSetting
                {
                    Id = Guid.NewGuid(),
                    Key = key,
                    Value = value,
                    CreatedAt = DateTime.UtcNow
                };
                _context.SystemSettings.Add(setting);
            }
            else
            {
                setting.Value = value;
                setting.UpdatedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();

            _logger.LogInformation("Cập nhật cài đặt: {Key} = {Value}", key, value);

            return ApiResult.Ok("Cập nhật thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật cài đặt: {Key}", key);
            return ApiResult.Fail("Lỗi khi cập nhật cài đặt");
        }
    }

    /// <summary>
    /// Lấy cài đặt reminder
    /// </summary>
    public async Task<ReminderSettingsDto> GetReminderSettingsAsync()
    {
        return new ReminderSettingsDto
        {
            DefaultProbationDays = await GetIntValueAsync("DefaultProbationDays", 60),
            ProbationReminderDaysBefore = (await GetIntArrayValueAsync("ProbationReminderDaysBefore")).ToList(),
            ContractReminderDaysBefore = (await GetIntArrayValueAsync("ContractReminderDaysBefore")).ToList(),
            HrNotificationEmails = await GetValueAsync("HrNotificationEmails") ?? ""
        };
    }

    /// <summary>
    /// Cập nhật cài đặt reminder
    /// </summary>
    public async Task<ApiResult> UpdateReminderSettingsAsync(ReminderSettingsDto dto)
    {
        try
        {
            await UpdateSettingAsync("DefaultProbationDays", dto.DefaultProbationDays.ToString());
            await UpdateSettingAsync("ProbationReminderDaysBefore", string.Join(",", dto.ProbationReminderDaysBefore));
            await UpdateSettingAsync("ContractReminderDaysBefore", string.Join(",", dto.ContractReminderDaysBefore));
            await UpdateSettingAsync("HrNotificationEmails", dto.HrNotificationEmails);

            return ApiResult.Ok("Cập nhật cài đặt nhắc nhở thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật cài đặt nhắc nhở");
            return ApiResult.Fail("Lỗi khi cập nhật cài đặt nhắc nhở");
        }
    }
}
