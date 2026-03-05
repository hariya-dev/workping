// Services/IAuthService.cs
// Interface dịch vụ xác thực

using EmployeeManagement.Api.DTOs;

namespace EmployeeManagement.Api.Services;

/// <summary>
/// Interface dịch vụ xác thực người dùng
/// </summary>
public interface IAuthService
{
    /// <summary>
    /// Đăng nhập và trả về JWT token
    /// </summary>
    Task<LoginResultDto> LoginAsync(LoginDto loginDto);

    /// <summary>
    /// Đổi mật khẩu người dùng
    /// </summary>
    Task<ApiResult> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);

    /// <summary>
    /// Lấy thông tin user theo ID
    /// </summary>
    Task<UserDto?> GetUserByIdAsync(Guid userId);

    /// <summary>
    /// Kiểm tra token còn hiệu lực không
    /// </summary>
    Task<bool> ValidateTokenAsync(string token);
}
