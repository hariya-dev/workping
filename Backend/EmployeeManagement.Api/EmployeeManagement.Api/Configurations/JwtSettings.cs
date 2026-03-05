// Configurations/JwtSettings.cs
// Class cấu hình JWT Authentication

namespace EmployeeManagement.Api.Configurations;

/// <summary>
/// Class chứa cấu hình JWT
/// Binding từ section "JwtSettings" trong appsettings.json
/// </summary>
public class JwtSettings
{
    /// <summary>
    /// Secret key để ký token
    /// </summary>
    public string SecretKey { get; set; } = string.Empty;

    /// <summary>
    /// Issuer của token
    /// </summary>
    public string Issuer { get; set; } = string.Empty;

    /// <summary>
    /// Audience của token
    /// </summary>
    public string Audience { get; set; } = string.Empty;

    /// <summary>
    /// Thời gian hết hạn token (phút)
    /// </summary>
    public int ExpirationMinutes { get; set; } = 480;
}
