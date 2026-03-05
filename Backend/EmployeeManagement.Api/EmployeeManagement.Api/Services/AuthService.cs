// Services/AuthService.cs
// Triển khai dịch vụ xác thực

using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using EmployeeManagement.Api.Configurations;
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace EmployeeManagement.Api.Services;

/// <summary>
/// Service xác thực người dùng
/// Sử dụng JWT token và BCrypt để hash password
/// </summary>
public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly JwtSettings _jwtSettings;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        AppDbContext context,
        IOptions<JwtSettings> jwtSettings,
        ILogger<AuthService> logger)
    {
        _context = context;
        _jwtSettings = jwtSettings.Value;
        _logger = logger;
    }

    /// <summary>
    /// Đăng nhập và tạo JWT token
    /// </summary>
    public async Task<LoginResultDto> LoginAsync(LoginDto loginDto)
    {
        try
        {
            // Tìm user theo username
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == loginDto.Username && u.IsActive);

            if (user == null)
            {
                _logger.LogWarning("Đăng nhập thất bại: Tên đăng nhập {Username} không tồn tại hoặc bị khóa", loginDto.Username);
                return new LoginResultDto
                {
                    Success = false,
                    ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng"
                };
            }

            // Kiểm tra password với BCrypt
            if (!BCrypt.Net.BCrypt.Verify(loginDto.Password, user.PasswordHash))
            {
                _logger.LogWarning("Đăng nhập thất bại: Mật khẩu sai cho người dùng {Username}", loginDto.Username);
                return new LoginResultDto
                {
                    Success = false,
                    ErrorMessage = "Tên đăng nhập hoặc mật khẩu không đúng"
                };
            }

            // Cập nhật thời gian đăng nhập
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            // Tạo JWT token
            var token = GenerateJwtToken(user.Id, user.Username, user.Role);
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes);

            _logger.LogInformation("Đăng nhập thành công: {Username}", loginDto.Username);

            return new LoginResultDto
            {
                Success = true,
                Token = token,
                ExpiresAt = expiresAt,
                User = new UserDto
                {
                    Id = user.Id,
                    Username = user.Username,
                    Email = user.Email,
                    FullName = user.FullName,
                    Role = user.Role
                }
            };
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đăng nhập cho người dùng {Username}", loginDto.Username);
            return new LoginResultDto
            {
                Success = false,
                ErrorMessage = "Lỗi hệ thống, vui lòng thử lại sau"
            };
        }
    }

    /// <summary>
    /// Đổi mật khẩu
    /// </summary>
    public async Task<ApiResult> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
    {
        try
        {
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return ApiResult.Fail("Người dùng không tồn tại");
            }

            // Kiểm tra mật khẩu cũ
            if (!BCrypt.Net.BCrypt.Verify(dto.CurrentPassword, user.PasswordHash))
            {
                return ApiResult.Fail("Mật khẩu cũ không đúng");
            }

            // Hash mật khẩu mới
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Đổi mật khẩu thành công cho người dùng {UserId}", userId);

            return ApiResult.Ok("Đổi mật khẩu thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi đổi mật khẩu cho người dùng {UserId}", userId);
            return ApiResult.Fail("Lỗi hệ thống, vui lòng thử lại sau");
        }
    }

    /// <summary>
    /// Lấy thông tin user
    /// </summary>
    public async Task<UserDto?> GetUserByIdAsync(Guid userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) return null;

        return new UserDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            FullName = user.FullName,
            Role = user.Role
        };
    }

    /// <summary>
    /// Kiểm tra token có hiệu lực không
    /// </summary>
    public Task<bool> ValidateTokenAsync(string token)
    {
        try
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_jwtSettings.SecretKey);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidIssuer = _jwtSettings.Issuer,
                ValidateAudience = true,
                ValidAudience = _jwtSettings.Audience,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    /// <summary>
    /// Tạo JWT token
    /// </summary>
    private string GenerateJwtToken(Guid userId, string username, string role)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.SecretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.UniqueName, username),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationMinutes),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
