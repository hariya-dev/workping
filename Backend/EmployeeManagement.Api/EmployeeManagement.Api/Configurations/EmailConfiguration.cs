// Configurations/EmailConfiguration.cs
// Class cấu hình email từ appsettings.json

namespace EmployeeManagement.Api.Configurations;

/// <summary>
/// Class chứa cấu hình SMTP email
/// Binding từ section "EmailConfiguration" trong appsettings.json
/// </summary>
public class EmailConfiguration
{
    /// <summary>
    /// Email người gửi
    /// </summary>
    public string From { get; set; } = string.Empty;

    /// <summary>
    /// Tên hiển thị người gửi
    /// </summary>
    public string FromDisplayName { get; set; } = string.Empty;

    /// <summary>
    /// Địa chỉ SMTP server
    /// </summary>
    public string SmtpServer { get; set; } = string.Empty;

    /// <summary>
    /// Port SMTP (465 cho SSL, 587 cho TLS)
    /// </summary>
    public int Port { get; set; }

    /// <summary>
    /// Username đăng nhập SMTP
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Password đăng nhập SMTP (App Password nếu dùng Gmail)
    /// </summary>
    public string Password { get; set; } = string.Empty;
}
