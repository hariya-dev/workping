// Services/EmailService.cs
// Dịch vụ gửi email thông báo

using EmployeeManagement.Api.Configurations;
using EmployeeManagement.Api.Entities;
using MailKit.Net.Smtp;
using MimeKit;
using System.Text;

namespace EmployeeManagement.Api.Services;

/// <summary>
/// Dịch vụ gửi email - 4 loại mail theo yêu cầu hệ thống:
/// 1. Chúc mừng sinh nhật → nhân viên (BirthdayWish)
/// 2. Danh sách sinh nhật tháng → HR (MonthlyBirthdayList)
/// 3. Nhắc nhở thử việc sắp kết thúc → HR (ProbationReminderHr)
/// 4. Nhắc nhở hợp đồng sắp hết hạn → HR (ContractReminderHr)
/// </summary>
public interface IEmailService
{
    /// <summary>
    /// Gửi email cơ bản (dùng cho test)
    /// </summary>
    Task SendEmailAsync(IEnumerable<string> toEmails, string subject, string body);

    /// <summary>
    /// [1] Chúc mừng sinh nhật → gửi cho nhân viên
    /// </summary>
    Task SendBirthdayEmailAsync(string employeeEmail, string employeeName, DateOnly birthDate);

    /// <summary>
    /// [2] Danh sách sinh nhật tháng → gửi cho HR vào đầu tháng
    /// </summary>
    Task SendBirthdayListEmailAsync(
        IEnumerable<string> hrEmails,
        List<(string Name, DateOnly Birthday, string? Department)> birthdays,
        int month, int year);

    /// <summary>
    /// [3] Nhắc nhở thử việc sắp kết thúc → gửi cho HR theo thời gian cấu hình
    /// </summary>
    Task SendProbationReminderHrEmailAsync(IEnumerable<string> hrEmails, string employeeName, string? department, DateOnly endDate, int daysRemaining);

    /// <summary>
    /// [4] Nhắc nhở hợp đồng sắp hết hạn → gửi cho HR theo thời gian cấu hình
    /// </summary>
    Task SendContractReminderHrEmailAsync(IEnumerable<string> hrEmails, string employeeName, string? department, DateOnly endDate, int daysRemaining);
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

            var bodyBuilder = new BodyBuilder { HtmlBody = body };
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
    /// [1] Chúc mừng sinh nhật → gửi cho nhân viên (template BirthdayWish)
    /// </summary>
    public async Task SendBirthdayEmailAsync(string employeeEmail, string employeeName, DateOnly birthDate)
    {
        var age = DateTime.Now.Year - birthDate.Year;
        if (birthDate.ToDateTime(TimeOnly.MinValue) > DateTime.Now.AddYears(-age)) age--;

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
            subject = $"Chúc Mừng Sinh Nhật - {employeeName}";
            body = $@"<html><body style='font-family: Arial, sans-serif;'>
<h2>Chúc Mừng Sinh Nhật {employeeName}!</h2>
<p>Toàn thể công ty xin gửi đến bạn những lời chúc tốt đẹp nhất nhân dịp sinh nhật lần thứ {age}.</p>
<p>Chúc bạn luôn mạnh khỏe, hạnh phúc và thành công!</p>
</body></html>";
        }

        await SendEmailAsync(new[] { employeeEmail }, subject, body);
        _logger.LogInformation("Đã gửi email sinh nhật cho {EmployeeName} ({Email})", employeeName, employeeEmail);
    }

    /// <summary>
    /// [2] Danh sách sinh nhật tháng → gửi cho HR (template MonthlyBirthdayList)
    /// </summary>
    public async Task SendBirthdayListEmailAsync(
        IEnumerable<string> hrEmails,
        List<(string Name, DateOnly Birthday, string? Department)> birthdays,
        int month, int year)
    {
        if (!birthdays.Any())
        {
            _logger.LogInformation("Không có sinh nhật trong tháng {Month}/{Year}", month, year);
            return;
        }

        using var scope = _serviceProvider.CreateScope();
        var templateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();

        var birthdayRows = string.Join("", birthdays.OrderBy(b => b.Birthday.Day).Select(b => $@"
            <tr>
                <td style='padding: 8px; border: 1px solid #ddd;'>{b.Birthday.Day:D2}/{month:D2}</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{b.Name}</td>
                <td style='padding: 8px; border: 1px solid #ddd;'>{b.Department ?? "-"}</td>
            </tr>"));

        var birthdayTable = $@"<table style='border-collapse: collapse; width: 100%; margin: 20px 0;'>
            <thead><tr style='background: #ff6b6b; color: white;'>
                <th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Ngày</th>
                <th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Họ và tên</th>
                <th style='padding: 10px; border: 1px solid #ddd; text-align: left;'>Phòng ban</th>
            </tr></thead>
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
            subject = $"[Thông báo] Danh sách sinh nhật tháng {month}/{year}";
            body = $@"<html><body style='font-family: Arial, sans-serif;'>
<h2>Danh sách sinh nhật tháng {month}/{year}</h2>
{birthdayTable}
<p>Tổng số: <strong>{birthdays.Count}</strong> nhân viên</p>
</body></html>";
        }

        await SendEmailAsync(hrEmails, subject, body);
    }

    /// <summary>
    /// [3] Nhắc nhở thử việc sắp kết thúc → gửi cho HR (template ProbationReminderHr)
    /// </summary>
    public async Task SendProbationReminderHrEmailAsync(IEnumerable<string> hrEmails, string employeeName, string? department, DateOnly endDate, int daysRemaining)
    {
        using var scope = _serviceProvider.CreateScope();
        var templateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();

        var data = new Dictionary<string, string>
        {
            ["EmployeeName"] = employeeName,
            ["Department"] = department ?? "-",
            ["EndDate"] = endDate.ToString("dd/MM/yyyy"),
            ["DaysRemaining"] = daysRemaining.ToString()
        };

        var template = await templateService.RenderTemplateAsync(EmailTemplateType.ProbationReminderHr, data);

        string subject, body;
        if (template.HasValue)
        {
            subject = template.Value.Subject;
            body = template.Value.BodyHtml;
        }
        else
        {
            subject = $"[Nhắc nhở Thử việc] {employeeName} - Còn {daysRemaining} ngày";
            body = $@"<html><body style='font-family: Arial, sans-serif;'>
<h2 style='color: #2563eb;'>Nhắc nhở Thời gian thử việc sắp kết thúc</h2>
<table style='border-collapse: collapse; margin: 20px 0;'>
    <tr><td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Họ và tên:</td><td style='padding: 8px; border: 1px solid #ddd;'><strong>{employeeName}</strong></td></tr>
    <tr><td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Phòng ban:</td><td style='padding: 8px; border: 1px solid #ddd;'>{department ?? "-"}</td></tr>
    <tr><td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Ngày kết thúc thử việc:</td><td style='padding: 8px; border: 1px solid #ddd;'><strong>{endDate:dd/MM/yyyy}</strong></td></tr>
    <tr><td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Số ngày còn lại:</td><td style='padding: 8px; border: 1px solid #ddd; color: #dc2626;'><strong>{daysRemaining} ngày</strong></td></tr>
</table>
<p>Vui lòng xem xét đánh giá và quyết định gia hạn hợp đồng cho nhân viên này.</p>
</body></html>";
        }

        await SendEmailAsync(hrEmails, subject, body);
    }

    /// <summary>
    /// [4] Nhắc nhở hợp đồng sắp hết hạn → gửi cho HR (template ContractReminderHr)
    /// </summary>
    public async Task SendContractReminderHrEmailAsync(IEnumerable<string> hrEmails, string employeeName, string? department, DateOnly endDate, int daysRemaining)
    {
        using var scope = _serviceProvider.CreateScope();
        var templateService = scope.ServiceProvider.GetRequiredService<IEmailTemplateService>();

        var data = new Dictionary<string, string>
        {
            ["EmployeeName"] = employeeName,
            ["Department"] = department ?? "-",
            ["EndDate"] = endDate.ToString("dd/MM/yyyy"),
            ["DaysRemaining"] = daysRemaining.ToString()
        };

        var template = await templateService.RenderTemplateAsync(EmailTemplateType.ContractReminderHr, data);

        string subject, body;
        if (template.HasValue)
        {
            subject = template.Value.Subject;
            body = template.Value.BodyHtml;
        }
        else
        {
            subject = $"[Nhắc nhở Hợp đồng] {employeeName} - Còn {daysRemaining} ngày";
            body = $@"<html><body style='font-family: Arial, sans-serif;'>
<h2 style='color: #dc2626;'>Nhắc nhở Hợp đồng lao động sắp hết hạn</h2>
<table style='border-collapse: collapse; margin: 20px 0;'>
    <tr><td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Họ và tên:</td><td style='padding: 8px; border: 1px solid #ddd;'><strong>{employeeName}</strong></td></tr>
    <tr><td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Phòng ban:</td><td style='padding: 8px; border: 1px solid #ddd;'>{department ?? "-"}</td></tr>
    <tr><td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Ngày hết hạn hợp đồng:</td><td style='padding: 8px; border: 1px solid #ddd;'><strong>{endDate:dd/MM/yyyy}</strong></td></tr>
    <tr><td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Số ngày còn lại:</td><td style='padding: 8px; border: 1px solid #ddd; color: #dc2626;'><strong>{daysRemaining} ngày</strong></td></tr>
</table>
<p>Vui lòng liên hệ nhân viên để thảo luận về việc gia hạn hợp đồng.</p>
</body></html>";
        }

        await SendEmailAsync(hrEmails, subject, body);
    }
}
