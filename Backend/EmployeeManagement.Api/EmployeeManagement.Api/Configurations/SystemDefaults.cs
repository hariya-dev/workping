// Configurations/SystemDefaults.cs
// Class cấu hình mặc định hệ thống

namespace EmployeeManagement.Api.Configurations;

/// <summary>
/// Class chứa cấu hình mặc định hệ thống
/// Binding từ section "SystemDefaults" trong appsettings.json
/// </summary>
public class SystemDefaults
{
    /// <summary>
    /// Số ngày thử việc mặc định
    /// </summary>
    public int DefaultProbationDays { get; set; } = 60;

    /// <summary>
    /// Danh sách số ngày trước khi hết thử việc cần gửi nhắc nhở
    /// </summary>
    public int[] ProbationReminderDaysBefore { get; set; } = { 30, 15, 7, 3, 1 };

    /// <summary>
    /// Danh sách số ngày trước khi hết hợp đồng cần gửi nhắc nhở
    /// </summary>
    public int[] ContractReminderDaysBefore { get; set; } = { 30, 15, 7, 3, 1 };

    /// <summary>
    /// Kích thước file tối đa (MB)
    /// </summary>
    public int MaxFileSizeMB { get; set; } = 10;

    /// <summary>
    /// Danh sách đuôi file được phép upload
    /// </summary>
    public string[] AllowedFileExtensions { get; set; } = { ".pdf", ".docx", ".doc", ".jpg", ".jpeg", ".png", ".xlsx", ".xls" };
}
