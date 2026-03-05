// DTOs/EmployeeEditHistoryDTOs.cs
// Data Transfer Objects cho EmployeeEditHistory API

namespace EmployeeManagement.Api.DTOs;

/// <summary>
/// DTO trả về lịch sử chỉnh sửa nhân viên
/// </summary>
public class EmployeeEditHistoryDto
{
    /// <summary>
    /// ID bản ghi lịch sử
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID nhân viên
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Tên field kỹ thuật (e.g., "FullName")
    /// </summary>
    public string FieldName { get; set; } = string.Empty;

    /// <summary>
    /// Tên field hiển thị tiếng Việt (e.g., "Họ và tên")
    /// </summary>
    public string FieldDisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Giá trị cũ
    /// </summary>
    public string? OldValue { get; set; }

    /// <summary>
    /// Giá trị mới
    /// </summary>
    public string? NewValue { get; set; }

    /// <summary>
    /// ID người thực hiện thay đổi
    /// </summary>
    public Guid? ChangedBy { get; set; }

    /// <summary>
    /// Tên người thực hiện thay đổi
    /// </summary>
    public string ChangedByName { get; set; } = string.Empty;

    /// <summary>
    /// Thời điểm thay đổi
    /// </summary>
    public DateTime ChangedAt { get; set; }

    /// <summary>
    /// Loại thay đổi: "Create", "Update", "Delete"
    /// </summary>
    public string ChangeType { get; set; } = string.Empty;
}
