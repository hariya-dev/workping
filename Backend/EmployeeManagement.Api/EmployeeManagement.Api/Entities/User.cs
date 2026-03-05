// Entities/User.cs
// Entity người dùng hệ thống (HR staff)

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.Api.Entities;

/// <summary>
/// Entity người dùng - nhân viên HR sử dụng hệ thống
/// Không có chức năng đăng ký, admin tạo tài khoản thủ công
/// </summary>
[Table("Users")]
public class User
{
    /// <summary>
    /// Khóa chính - ID người dùng
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Tên đăng nhập (unique)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Email người dùng
    /// </summary>
    [Required]
    [MaxLength(200)]
    [EmailAddress]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// Hash của mật khẩu (BCrypt)
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string PasswordHash { get; set; } = string.Empty;

    /// <summary>
    /// Họ tên đầy đủ
    /// </summary>
    [MaxLength(200)]
    public string? FullName { get; set; }

    /// <summary>
    /// Vai trò người dùng (Admin, HR)
    /// </summary>
    [MaxLength(50)]
    public string Role { get; set; } = "HR";

    /// <summary>
    /// Trạng thái tài khoản
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Lần đăng nhập cuối
    /// </summary>
    public DateTime? LastLoginAt { get; set; }

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
