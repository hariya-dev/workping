// DTOs/AuthDTOs.cs
// Data Transfer Objects cho Authentication API

using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.DTOs;

/// <summary>
/// DTO đăng nhập
/// </summary>
public class LoginDto
{
    [Required(ErrorMessage = "Ten dang nhap la bat buoc")]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mat khau la bat buoc")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// DTO kết quả đăng nhập
/// </summary>
public class LoginResultDto
{
    public bool Success { get; set; }
    public string? Token { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public UserDto? User { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// DTO thông tin user
/// </summary>
public class UserDto
{
    public Guid Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string? FullName { get; set; }
    public string Role { get; set; } = string.Empty;
}

/// <summary>
/// DTO đổi mật khẩu
/// </summary>
public class ChangePasswordDto
{
    [Required(ErrorMessage = "Mat khau cu la bat buoc")]
    public string CurrentPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Mat khau moi la bat buoc")]
    [MinLength(6, ErrorMessage = "Mat khau moi toi thieu 6 ky tu")]
    public string NewPassword { get; set; } = string.Empty;

    [Required(ErrorMessage = "Xac nhan mat khau la bat buoc")]
    [Compare("NewPassword", ErrorMessage = "Xac nhan mat khau khong khop")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
