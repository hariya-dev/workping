// Entities/Employee.cs
// Entity chính - Thông tin nhân viên

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.Api.Entities;

/// <summary>
/// Entity nhân viên - chứa toàn bộ thông tin cá nhân
/// Hợp đồng được quản lý qua EmployeeContract (One-to-Many)
/// </summary>
[Table("Employees")]
public class Employee
{
    /// <summary>
    /// Khóa chính - ID nhân viên (GUID)
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Họ và tên đầy đủ (bắt buộc)
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Email nhân viên (dùng để gửi thông báo)
    /// </summary>
    [MaxLength(200)]
    [EmailAddress]
    public string? Email { get; set; }

    /// <summary>
    /// Số điện thoại
    /// </summary>
    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    /// <summary>
    /// Ngày sinh
    /// </summary>
    public DateOnly DateOfBirth { get; set; }

    /// <summary>
    /// Ngày bắt đầu thử việc
    /// </summary>
    public DateOnly? ProbationStartDate { get; set; }

    /// <summary>
    /// Ngày kết thúc thử việc
    /// </summary>
    public DateOnly? ProbationEndDate { get; set; }

    /// <summary>
    /// Trạng thái nhân viên hiện tại
    /// </summary>
    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

    /// <summary>
    /// Phòng ban / Bộ phận
    /// </summary>
    [MaxLength(200)]
    public string? Department { get; set; }

    /// <summary>
    /// Chức vụ
    /// </summary>
    [MaxLength(200)]
    public string? Position { get; set; }

    /// <summary>
    /// Ghi chú thêm
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Ngày tạo record
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Ngày cập nhật gần nhất
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation properties

    /// <summary>
    /// Danh sách file đính kèm của nhân viên
    /// </summary>
    public virtual ICollection<EmployeeFile> Files { get; set; } = new List<EmployeeFile>();

    /// <summary>
    /// Danh sách hợp đồng của nhân viên (lịch sử hợp đồng)
    /// </summary>
    public virtual ICollection<EmployeeContract> Contracts { get; set; } = new List<EmployeeContract>();

    /// <summary>
    /// Lịch sử chỉnh sửa thông tin nhân viên
    /// </summary>
    public virtual ICollection<EmployeeEditHistory> EditHistories { get; set; } = new List<EmployeeEditHistory>();
}
