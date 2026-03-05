// Services/EmailService.cs
// Dịch vụ gửi email thông báo

using EmployeeManagement.Api.Configurations;
using EmployeeManagement.Api.Entities;
using MailKit.Net.Smtp;
using MimeKit;
using System.Text;

namespace EmployeeManagement.Api.Services;

/// <summary>
/// Dịch vụ gửi email
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Gửi email cơ bản
    /// </summary>
    Task SendEmailAsync(IEnumerable<string> toEmails, string subject, string body);
    
    /// <summary>
    /// Gửi email chúc mừng sinh nhật cho nhân viên
    /// </summary>
    Task<bool> SendBirthdayEmailAsync(string employeeEmail, string employeeName, DateOnly birthDate);
    
    /// <summary>
    /// Gửi báo cáo sinh nhật tháng cho HR
    /// </summary>
    Task<bool> SendMonthlyBirthdayReportAsync(IEnumerable<string> hrEmails, List<(string Name, DateOnly Birthday, string? Department)> birthdays, int month, int year);
    
    /// <summary>
    /// Gửi email nhắc nhở thử việc sắp kết thúc
    /// </summary>
    Task SendProbationReminderEmailAsync(string employeeName, string employeeEmail, DateOnly endDate, int daysRemaining);
    
    /// <summary>
    /// Gửi email nhắc nhở hợp đồng sắp hết hạn
    /// </summary>
    Task SendContractReminderEmailAsync(string employeeName, string employeeEmail, DateOnly endDate, int daysRemaining);
    
    /// <summary>
    /// Gửi email danh sách sinh nhật trong tháng
    /// </summary>
    Task SendBirthdayListEmailAsync(
        IEnumerable<string> hrEmails, 
        List<(string Name, DateOnly Birthday, string? Department)> birthdays, 
        int month, 
        int year);
}

/// <summary>
/// Triển khai dịch vụ gửi email
/// </summary>
public class EmailService : IEmailService
{
    private readonly EmailConfiguration _emailConfig;
    private readonly ILogger<EmailService> _logger;
    private readonly IServiceProvider _serviceProvider;

    public EmailService(EmailConfiguration emailConfig, ILogger<EmailService> logger, IServiceProvider serviceProvider)
    {
        _emailConfig = emailConfig;
        _logger = logger;
        _serviceProvider = serviceProvider;
    }

    /// <summary>
    /// Gửi email cơ bản
    /// </summary>
    public async Task SendEmailAsync(IEnumerable<string> toEmails, string subject, string body)
    {
        try
        {
            var emailMessage = new MimeMessage();
            emailMessage.From.Add(new MailboxAddress(_emailConfig.FromDisplayName, _emailConfig.From));
            foreach (var email in toEmails.Where(e => !string.IsNullOrWhiteSpace(e)))
            {
                emailMessage.To.Add(new MailboxAddress("", email));
            }
            emailMessage.Subject = subject;

            var bodyBuilder = new BodyBuilder
            {
                HtmlBody = body
            };
            emailMessage.Body = bodyBuilder.ToMessageBody();

            using var client = new SmtpClient();
            await client.ConnectAsync(_emailConfig.SmtpServer, _emailConfig.Port, true);
            await client.AuthenticateAsync(_emailConfig.Username, _emailConfig.Password);
            await client.SendAsync(emailMessage);
            await client.DisconnectAsync(true);

            _logger.LogInformation("Email sent successfully to {Recipients}", string.Join(", ", toEmails));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send email to {Recipients}", string.Join(", ", toEmails));
            throw;
        }
    }

    /// <summary>
    /// Gửi email chúc mừng sinh nhật cho nhân viên
    /// </summary>
    public async Task<bool> SendBirthdayEmailAsync(string employeeEmail, string employeeName, DateOnly birthDate)
    {
        try
        {
            var age = DateTime.Now.Year - birthDate.Year;
            if (birthDate.ToDateTime(TimeOnly.MinValue) > DateTime.Now.AddYears(-age)) age--;

            // Sử dụng template từ database
            using var scope = _serviceProvider.CreateScope();
            var templateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();

            var data = new Dictionary<string, string>
            {
                ["EmployeeName"] = employeeName,
                ["BirthDate"] = birthDate.ToString("dd/MM/yyyy"),
                ["Age"] = age.ToString()
            };

            var template = await templateService.RenderTemplateAsync(EmailTemplateType.BirthdayWish, data);
            
            string subject, body;
            if (template.HasValue)
            {
                subject = template.Value.Subject;
                body = template.Value.BodyHtml;
            }
            else
            {
                // Fallback về template mặc định
                subject = $"🎂 Chúc Mừng Sinh Nhật - {employeeName}";
                body = GenerateBirthdayEmailTemplate(employeeName, birthDate, age);
            }

            await SendEmailAsync(new[] { employeeEmail }, subject, body);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send birthday email to {Email}", employeeEmail);
            return false;
        }
    }

    /// <summary>
    /// Gửi báo cáo sinh nhật tháng cho HR
    /// </summary>
    public async Task<bool> SendMonthlyBirthdayReportAsync(IEnumerable<string> hrEmails, List<(string Name, DateOnly Birthday, string? Department)> birthdays, int month, int year)
    {
        try
        {
            if (!birthdays.Any())
            {
                _logger.LogInformation("Không có sinh nhật trong tháng {Month}/{Year}", month, year);
                return true;
            }

            var subject = $"[Báo cáo] Sinh nhật tháng {month}/{year}";
            var body = GenerateMonthlyBirthdayReportTemplate(birthdays, month, year);

            await SendEmailAsync(hrEmails, subject, body);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send monthly birthday report");
            return false;
        }
    }

    /// <summary>
    /// Gửi email nhắc nhở thử việc sắp kết thúc
    /// </summary>
    public async Task SendProbationReminderEmailAsync(string employeeName, string employeeEmail, DateOnly endDate, int daysRemaining)
    {
        // Sử dụng template từ database
        using var scope = _serviceProvider.CreateScope();
        var templateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();

        var data = new Dictionary<string, string>
        {
            ["EmployeeName"] = employeeName,
            ["EndDate"] = endDate.ToString("dd/MM/yyyy"),
            ["DaysRemaining"] = daysRemaining.ToString()
        };

        var template = await templateService.RenderTemplateAsync(EmailTemplateType.ProbationReminder, data);
        
        string subject, body;
        if (template.HasValue)
        {
            subject = template.Value.Subject;
            body = template.Value.BodyHtml;
        }
        else
        {
            // Fallback về template mặc định
            subject = $"[Nhắc nhở] Thử việc của {employeeName} sắp kết thúc";
            body = GenerateDefaultProbationReminderTemplate(employeeName, endDate, daysRemaining);
        }

        // Gửi đến nhân viên
        var recipients = new List<string>();
        if (!string.IsNullOrWhiteSpace(employeeEmail))
        {
            recipients.Add(employeeEmail);
        }

        await SendEmailAsync(recipients, subject, body);
    }

    /// <summary>
    /// Gửi email nhắc nhở hợp đồng sắp hết hạn
    /// </summary>
    public async Task SendContractReminderEmailAsync(string employeeName, string employeeEmail, DateOnly endDate, int daysRemaining)
    {
        // Sử dụng template từ database
        using var scope = _serviceProvider.CreateScope();
        var templateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();

        var data = new Dictionary<string, string>
        {
            ["EmployeeName"] = employeeName,
            ["EndDate"] = endDate.ToString("dd/MM/yyyy"),
            ["DaysRemaining"] = daysRemaining.ToString()
        };

        var template = await templateService.RenderTemplateAsync(EmailTemplateType.ContractReminder, data);
        
        string subject, body;
        if (template.HasValue)
        {
            subject = template.Value.Subject;
            body = template.Value.BodyHtml;
        }
        else
        {
            // Fallback về template mặc định
            subject = $"[Nhắc nhở] Hợp đồng của {employeeName} sắp hết hạn";
            body = GenerateDefaultContractReminderTemplate(employeeName, endDate, daysRemaining);
        }

        var recipients = new List<string>();
        if (!string.IsNullOrWhiteSpace(employeeEmail))
        {
            recipients.Add(employeeEmail);
        }

        await SendEmailAsync(recipients, subject, body);
    }

    /// <summary>
    /// Gửi email danh sách sinh nhật trong tháng
    /// </summary>
    public async Task SendBirthdayListEmailAsync(
        IEnumerable<string> hrEmails, 
        List<(string Name, DateOnly Birthday, string? Department)> birthdays, 
        int month, 
        int year)
    {
        if (!birthdays.Any())
        {
            _logger.LogInformation("Không có sinh nhật trong tháng {Month}/{Year}", month, year);
            return;
        }

        // Sử dụng template từ database
        using var scope = _serviceProvider.CreateScope();
        var templateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();

        // Tạo bảng HTML danh sách sinh nhật
        var birthdayRows = string.Join("", birthdays.OrderBy(b => b.Birthday.Day).Select(b => $@"
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>{b.Birthday.Day:D2}/{month:D2}</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{b.Name}</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{b.Department ?? "-"}</td>
            </tr>"));

        var birthdayTable = $@"<table style='border-collapse: collapse; width: 100%; margin: 20px 0;'>
            <thead>
                <tr style='background: #ff6b6b; color: white;'>
                    <th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Ngày</th>
                    <th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Họ và tên</th>
                    <th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Phòng ban</th>
                </tr>
            </thead>
            <tbody>{birthdayRows}</tbody>
        </table>";

        var data = new Dictionary<string, string>
        {
            ["CurrentMonth"] = month.ToString(),
            ["CurrentYear"] = year.ToString(),
            ["BirthdayList"] = birthdayTable,
            ["TotalCount"] = birthdays.Count.ToString()
        };

        var template = await templateService.RenderTemplateAsync(EmailTemplateType.MonthlyBirthdayList, data);
        
        string subject, body;
        if (template.HasValue)
        {
            subject = template.Value.Subject;
            body = template.Value.BodyHtml;
        }
        else
        {
            // Fallback về template mặc định
            subject = $"[Thông báo] Danh sách sinh nhật tháng {month}/{year}";
            body = GenerateDefaultBirthdayListTemplate(birthdays, month, year);
        }

        await SendEmailAsync(hrEmails, subject, body);
    }

    private string GenerateBirthdayEmailTemplate(string employeeName, DateOnly birthDate, int age)
    {
        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Chúc Mừng Sinh Nhật</title>
    <style>
        body {{ margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f7fa; }}
        .container {{ max-width: 600px; margin: 0 auto; background: #ff6b6b; box-shadow: 0 4px 12px rgba(255, 107, 107, 0.3); border-radius: 12px; overflow: hidden; box-shadow: 0 10px 30px rgba(0,0,0,0.1); }}
        .header {{ background: #ffffff; border-bottom: 2px solid #ff6b6b; padding: 30px; text-align: center; }}
        .logo {{ width: 80px; height: 80px; background: #ff6b6b; box-shadow: 0 4px 12px rgba(255, 107, 107, 0.3); border-radius: 50%; margin: 0 auto 20px; display: flex; align-items: center; justify-content: center; }}
        .logo-text {{ font-size: 24px; font-weight: bold; color: white; }}
        .birthday-icon {{ font-size: 40px; margin-bottom: 15px; }}
        .content {{ padding: 40px 30px; }}
        .greeting {{ font-size: 24px; color: #333; margin-bottom: 20px; font-weight: bold; }}
        .message {{ font-size: 16px; color: #666; line-height: 1.6; margin-bottom: 30px; }}
        .details {{ background: #fff5f5; border-radius: 8px; padding: 20px; margin: 25px 0; border-left: 4px solid #ff6b6b; }}
        .detail-item {{ display: flex; justify-content: space-between; padding: 8px 0; }}
        .label {{ font-weight: 600; color: #444; }}
        .value {{ color: white; font-weight: 500; }}
        .footer {{ background: #333; color: white; padding: 25px; text-align: center; }}
        .company-name {{ font-size: 18px; font-weight: bold; margin-bottom: 10px; }}
        @media (max-width: 600px) {{
            .container {{ border-radius: 0; }}
            .content {{ padding: 25px 20px; }}
            .header {{ padding: 20px; }}
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='logo'>
                <div class='logo-text'>AT</div>
            </div>
            <div class='birthday-icon'>🎂</div>
            <h1 style='color: white; margin: 0; font-size: 28px;'>Chúc Mừng Sinh Nhật!</h1>
        </div>
        
        <div class='content'>
            <h2 class='greeting'>Chào {employeeName} thân mến!</h2>
            
            <p class='message'>
                Nhân dịp sinh nhật lần thứ <strong>{age}</strong> của bạn, toàn thể công ty 
                <strong>Công ty Cổ Phần Giải Pháp Kỹ Thuật Ấn Tượng</strong> xin gửi đến bạn 
                những lời chúc tốt đẹp nhất!
            </p>
            
            <div class='details'>
                <div class='detail-item'>
                    <span class='label'>Họ và tên:</span>
                    <span class='value'>{employeeName}</span>
                </div>
                <div class='detail-item'>
                    <span class='label'>Ngày sinh:</span>
                    <span class='value'>{birthDate:dd/MM/yyyy}</span>
                </div>
                <div class='detail-item'>
                    <span class='label'>Tuổi:</span>
                    <span class='value'>{age} tuổi</span>
                </div>
            </div>
            
            <p class='message'>
                Chúc bạn luôn mạnh khỏe, hạnh phúc và thành công trong công việc. 
                Mong rằng năm mới sẽ mang đến cho bạn nhiều niềm vui và cơ hội phát triển!
            </p>
        </div>
        
        <div class='footer'>
            <div class='company-name'>CÔNG TY CỔ PHẦN GIẢI PHÁP KỸ THUẬT ẤN TƯỢNG</div>
            <div class='company-address'>Số 123 Đường ABC, Quận XYZ, TP. Hồ Chí Minh</div>
            <div class='company-address'>Hotline: 0123 456 789 | Email: info@atpro.com.vn</div>
        </div>
    </div>
</body>
</html>";
    }

    private string GenerateMonthlyBirthdayReportTemplate(List<(string Name, DateOnly Birthday, string? Department)> birthdays, int month, int year)
    {
        var birthdayRows = string.Join("", birthdays.OrderBy(b => b.Birthday.Day).Select(b => $@"
            <tr>
                <td style='padding: 12px; border: 1px solid #e2e8f0; text-align: center;'>{b.Birthday.Day:D2}/{month:D2}</td>
                <td style='padding: 12px; border: 1px solid #e2e8f0; font-weight: 500;'>{b.Name}</td>
                <td style='padding: 12px; border: 1px solid #e2e8f0;'>{b.Department ?? "-"}</td>
                <td style='padding: 12px; border: 1px solid #e2e8f0; text-align: center;'>
                    <span style='background: #ff6b6b; color: white; padding: 4px 12px; border-radius: 20px; font-size: 12px; font-weight: bold;'>
                        {(b.Birthday.Day == DateTime.Now.Day ? "HÔM NAY" : $"{(new DateTime(year, month, b.Birthday.Day) - DateTime.Now).Days} ngày")}
                    </span>
                </td>
            </tr>"));

        return $@"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Sinh nhật tháng {month}/{year}</title>
    <style>
        body {{ margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f8fafc; }}
        .container {{ max-width: 700px; margin: 0 auto; background: #ff6b6b; box-shadow: 0 4px 12px rgba(255, 107, 107, 0.3); border-radius: 12px; overflow: hidden; box-shadow: 0 10px 25px rgba(0,0,0,0.1); }}
        .header {{ background: #ffffff; border-bottom: 2px solid #ff6b6b; padding: 30px; text-align: center; color: white; }}
        .logo {{ width: 60px; height: 60px; background: #ff6b6b; box-shadow: 0 4px 12px rgba(255, 107, 107, 0.3); border-radius: 50%; margin: 0 auto 15px; display: flex; align-items: center; justify-content: center; }}
        .logo-text {{ font-size: 20px; font-weight: bold; color: white; }}
        .content {{ padding: 30px; }}
        .greeting {{ font-size: 22px; color: #333; margin-bottom: 15px; font-weight: bold; }}
        .subtitle {{ font-size: 16px; color: #666; margin-bottom: 30px; }}
        .birthday-table {{ width: 100%; border-collapse: collapse; margin: 25px 0; background: #ff6b6b; box-shadow: 0 4px 12px rgba(255, 107, 107, 0.3); border-radius: 8px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.05); }}
        .table-header {{ background: #ffffff; border-bottom: 2px solid #ff6b6b; color: white; }}
        .table-header th {{ padding: 15px; text-align: left; font-weight: 600; }}
        .table-header th:first-child {{ text-align: center; }}
        .table-header th:last-child {{ text-align: center; }}
        .tip-box {{ background: #fff5f5; border-left: 4px solid #ff6b6b; padding: 20px; margin: 25px 0; border-radius: 0 8px 8px 0; }}
        .tip-title {{ font-weight: bold; color: white; margin-bottom: 8px; }}
        .tip-content {{ color: #666; font-size: 14px; }}
        .footer {{ background: #333; color: white; padding: 20px; text-align: center; font-size: 14px; }}
        @media (max-width: 600px) {{
            .container {{ border-radius: 0; }}
            .content {{ padding: 20px; }}
            .header {{ padding: 20px; }}
        }}
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='logo'>
                <div class='logo-text'>AT</div>
            </div>
            <h1 style='margin: 0; font-size: 26px;'>Sinh nhật tháng {month}/{year}</h1>
            <p style='margin: 10px 0 0; opacity: 0.9;'>Ngày gửi: {DateTime.Now:dd/MM/yyyy}</p>
        </div>
        
        <div class='content'>
            <h2 class='greeting'>🎉 Chúc mừng các 'ngôi sao' sắp lên sân!</h2>
            <p class='subtitle'>Đừng bỏ lỡ cơ hội gửi lời chúc ấm áp nhé!</p>
            
            <table class='birthday-table'>
                <thead class='table-header'>
                    <tr>
                        <th>Ngày sinh</th>
                        <th>Họ tên</th>
                        <th>Phòng ban</th>
                        <th>Còn lại</th>
                    </tr>
                </thead>
                <tbody>
                    {birthdayRows}
                </tbody>
            </table>
            
            <div class='tip-box'>
                <div class='tip-title'>💡 Mẹo hữu ích:</div>
                <div class='tip-content'>Tag đồng nghiệp trong nhóm Zalo + tặng bánh = tinh thần +1000!</div>
            </div>
        </div>
        
        <div class='footer'>
            Email tự động từ Hệ thống Nhân sự ATPRO © 2025
        </div>
    </div>
</body>
</html>";
    }

    #region Fallback Templates

    /// <summary>
    /// Fallback template cho nhắc nhở thử việc
    /// </summary>
    private static string GenerateDefaultProbationReminderTemplate(string employeeName, DateOnly endDate, int daysRemaining)
    {
        return $@"<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'></head>
<body style='font-family: Arial, sans-serif;'>
    <h2 style='color: #2563eb;'>Thông báo thời gian thử việc sắp kết thúc</h2>
    <p>Kính gửi <strong>{employeeName}</strong>,</p>
    <p>Thời gian thử việc của bạn sắp kết thúc:</p>
    <table style='border-collapse: collapse; margin: 20px 0;'>
        <tr>
            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Ngày kết thúc:</td>
            <td style='padding: 8px; border: 1px solid #ddd;'><strong>{endDate:dd/MM/yyyy}</strong></td>
        </tr>
        <tr>
            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Số ngày còn lại:</td>
            <td style='padding: 8px; border: 1px solid #ddd; color: #dc2626;'><strong>{daysRemaining} ngày</strong></td>
        </tr>
    </table>
    <p>Vui lòng liên hệ bộ phận Nhân sự để biết thêm thông tin.</p>
</body>
</html>";
    }

    /// <summary>
    /// Fallback template cho nhắc nhở hợp đồng
    /// </summary>
    private static string GenerateDefaultContractReminderTemplate(string employeeName, DateOnly endDate, int daysRemaining)
    {
        return $@"<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'></head>
<body style='font-family: Arial, sans-serif;'>
    <h2 style='color: #dc2626;'>Thông báo hợp đồng lao động sắp hết hạn</h2>
    <p>Kính gửi <strong>{employeeName}</strong>,</p>
    <p>Hợp đồng lao động của bạn sắp hết hạn:</p>
    <table style='border-collapse: collapse; margin: 20px 0;'>
        <tr>
            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Ngày hết hạn:</td>
            <td style='padding: 8px; border: 1px solid #ddd;'><strong>{endDate:dd/MM/yyyy}</strong></td>
        </tr>
        <tr>
            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Số ngày còn lại:</td>
            <td style='padding: 8px; border: 1px solid #ddd; color: #dc2626;'><strong>{daysRemaining} ngày</strong></td>
        </tr>
    </table>
    <p>Vui lòng liên hệ bộ phận Nhân sự để thảo luận về việc gia hạn hợp đồng.</p>
</body>
</html>";
    }

    /// <summary>
    /// Fallback template cho danh sách sinh nhật
    /// </summary>
    private static string GenerateDefaultBirthdayListTemplate(List<(string Name, DateOnly Birthday, string? Department)> birthdays, int month, int year)
    {
        var birthdayRows = string.Join("", birthdays.OrderBy(b => b.Birthday.Day).Select(b => $@"
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>{b.Birthday.Day:D2}/{month:D2}</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{b.Name}</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{b.Department ?? "-"}</td>
            </tr>"));

        return $@"<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'></head>
<body style='font-family: Arial, sans-serif;'>
    <h2 style='color: #2563eb;'>Danh sách sinh nhật tháng {month}/{year}</h2>
    <p>Dưới đây là danh sách nhân viên có sinh nhật trong tháng:</p>
    <table style='border-collapse: collapse; width: 100%; margin: 20px 0;'>
        <thead>
            <tr style='background: #2563eb; color: white;'>
                <th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Ngày</th>
                <th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Họ và tên</th>
                <th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Phòng ban</th>
            </tr>
        </thead>
        <tbody>{birthdayRows}</tbody>
    </table>
    <p>Tổng số: <strong>{birthdays.Count}</strong> nhân viên</p>
</body>
</html>";
    }

    #endregion
}
