// Services/ISystemSettingService.cs
// Interface dịch vụ cài đặt hệ thống

using EmployeeManagement.Api.DTOs;

namespace EmployeeManagement.Api.Services;

/// <summary>
/// Interface dịch vụ quản lý cài đặt hệ thống
/// </summary>
public interface ISystemSettingService
{
    /// <summary>
    /// Lấy tất cả cài đặt
    /// </summary>
    Task<List<SystemSettingDto>> GetAllAsync();

    /// <summary>
    /// Lấy giá trị cài đặt theo key
    /// </summary>
    Task<string?> GetValueAsync(string key);

    /// <summary>
    /// Lấy giá trị cài đặt dạng int
    /// </summary>
    Task<int> GetIntValueAsync(string key, int defaultValue = 0);

    /// <summary>
    /// Lấy giá trị cài đặt dạng array int (phân cách bởi dấu phẩy)
    /// </summary>
    Task<int[]> GetIntArrayValueAsync(string key);

    /// <summary>
    /// Cập nhật cài đặt
    /// </summary>
    Task<ApiResult> UpdateSettingAsync(string key, string value);

    /// <summary>
    /// Lấy cài đặt reminder
    /// </summary>
    Task<ReminderSettingsDto> GetReminderSettingsAsync();

    /// <summary>
    /// Cập nhật cài đặt reminder
    /// </summary>
    Task<ApiResult> UpdateReminderSettingsAsync(ReminderSettingsDto dto);
}
