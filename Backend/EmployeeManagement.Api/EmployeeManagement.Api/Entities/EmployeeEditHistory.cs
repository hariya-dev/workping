// Entities/EmployeeEditHistory.cs
// Entity lưu trữ lịch sử chỉnh sửa thông tin nhân viên

using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.Entities;

/// <summary>
/// Entity lịch sử chỉnh sửa nhân viên
/// Lưu lại tất cả các thay đổi field-level khi tạo/cập nhật nhân viên
/// </summary>
public class EmployeeEditHistory
{
    /// <summary>
    /// ID bản ghi lịch sử
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID nhân viên được chỉnh sửa (FK)
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Tên field kỹ thuật (e.g., "FullName", "Email")
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// Tên field hiển thị tiếng Việt (e.g., "Họ và tên", "Email")
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string FieldDisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Giá trị cũ (null nếu là tạo mới)
    /// </summary>
    [MaxLength(500)]
    public string? OldValue { get; set; }

    /// <summary>
    /// Giá trị mới (null nếu là xóa)
    /// </summary>
    [MaxLength(500)]
    public string? NewValue { get; set; }

    /// <summary>
    /// ID người thực hiện thay đổi
    /// </summary>
    public Guid? ChangedBy { get; set; }

    /// <summary>
    /// Tên người thực hiện thay đổi (cached để hiển thị)
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string ChangedByName { get; set; } = string.Empty;

    /// <summary>
    /// Thời điểm thay đổi
    /// </summary>
    public DateTime ChangedAt { get; set; }

    /// <summary>
    /// Loại thay đổi: Create, Update, Delete
    /// </summary>
    [Required]
    [MaxLength(20)]
    public string ChangeType { get; set; } = string.Empty;

    // Navigation Properties

    /// <summary>
    /// Nhân viên được chỉnh sửa
    /// </summary>
    public virtual Employee? Employee { get; set; }
}
