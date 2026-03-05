// Entities/SystemSetting.cs
// Entity lưu trữ cài đặt hệ thống trong database

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.Api.Entities;

/// <summary>
/// Entity cài đặt hệ thống - lưu các thông số cấu hình có thể thay đổi
/// </summary>
[Table("SystemSettings")]
public class SystemSetting
{
    /// <summary>
    /// Khóa chính - ID cài đặt
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Tên/Key của cài đặt (unique)
    /// Ví dụ: "DefaultProbationDays", "ProbationReminderDaysBefore"
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Key { get; set; } = string.Empty;

    /// <summary>
    /// Giá trị của cài đặt (lưu dạng string, parse khi sử dụng)
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string Value { get; set; } = string.Empty;

    /// <summary>
    /// Kiểu dữ liệu của giá trị (int, string, array, json)
    /// </summary>
    [MaxLength(50)]
    public string? ValueType { get; set; }

    /// <summary>
    /// Mô tả cài đặt này dùng để làm gì
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
