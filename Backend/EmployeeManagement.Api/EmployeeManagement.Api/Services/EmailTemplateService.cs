// Services/EmailTemplateService.cs
// Triển khai dịch vụ quản lý template email

using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Services;

/// <summary>
/// Triển khai dịch vụ quản lý template email
/// </summary>
public class EmailTemplateService : IEmailTemplateService
{
    private readonly AppDbContext _context;
    private readonly ILogger<EmailTemplateService> _logger;

    public EmailTemplateService(AppDbContext context, ILogger<EmailTemplateService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Lấy tất cả templates
    /// </summary>
    public async Task<List<EmailTemplateSummaryDto>> GetAllAsync()
    {
        return await _context.EmailTemplates
            .OrderBy(t => t.Type)
            .Select(t => new EmailTemplateSummaryDto
            {
                Id = t.Id,
                Type = t.Type,
                TypeName = t.Type.ToString(),
                Name = t.Name,
                Subject = t.Subject,
                IsActive = t.IsActive,
                UpdatedAt = t.UpdatedAt
            })
            .ToListAsync();
    }

    /// <summary>
    /// Lấy template theo ID
    /// </summary>
    public async Task<EmailTemplateDto?> GetByIdAsync(Guid id)
    {
        var template = await _context.EmailTemplates.FindAsync(id);
        if (template == null) return null;

        return MapToDto(template);
    }

    /// <summary>
    /// Lấy template theo loại
    /// </summary>
    public async Task<EmailTemplateDto?> GetByTypeAsync(EmailTemplateType type)
    {
        var template = await _context.EmailTemplates
            .FirstOrDefaultAsync(t => t.Type == type && t.IsActive);
        
        if (template == null) return null;

        return MapToDto(template);
    }

    /// <summary>
    /// Tạo hoặc cập nhật template
    /// </summary>
    public async Task<ApiResult<UpsertResultDto>> UpsertAsync(Guid? id, UpsertEmailTemplateDto dto)
    {
        EmailTemplate template;
        var isNew = false;

        if (id.HasValue && id.Value != Guid.Empty)
        {
            // Update existing
            template = await _context.EmailTemplates.FindAsync(id.Value);
            if (template == null)
            {
                return ApiResult<UpsertResultDto>.Fail("Template không tồn tại");
            }
        }
        else
        {
            // Create new
            if (!dto.Type.HasValue)
            {
                return ApiResult<UpsertResultDto>.Fail("Loại template là bắt buộc khi tạo mới");
            }

            // Check if type already exists
            var exists = await _context.EmailTemplates.AnyAsync(t => t.Type == dto.Type.Value);
            if (exists)
            {
                return ApiResult<UpsertResultDto>.Fail($"Template loại '{dto.Type.Value}' đã tồn tại");
            }

            template = new EmailTemplate
            {
                Id = Guid.NewGuid(),
                Type = dto.Type.Value,
                CreatedAt = DateTime.UtcNow
            };
            isNew = true;
        }

        // Update properties
        template.Name = dto.Name;
        template.Subject = dto.Subject;
        template.BodyHtml = dto.BodyHtml;
        template.IsActive = dto.IsActive;
        template.Description = dto.Description;
        template.UpdatedAt = DateTime.UtcNow;

        if (isNew)
        {
            _context.EmailTemplates.Add(template);
        }

        await _context.SaveChangesAsync();

        _logger.LogInformation("Đã {(isNew ? \"tạo\" : \"cập nhật\")} template email: {Type} - {Name}", 
            isNew ? "tạo" : "cập nhật", template.Type, template.Name);

        return ApiResult<UpsertResultDto>.Ok(new UpsertResultDto
        {
            Id = template.Id,
            IsNew = isNew
        });
    }

    /// <summary>
    /// Reset template về mặc định (xóa custom và seed lại)
    /// </summary>
    public async Task<ApiResult> ResetToDefaultAsync(Guid id)
    {
        var template = await _context.EmailTemplates.FindAsync(id);
        if (template == null)
        {
            return ApiResult.Fail("Template không tồn tại");
        }

        // Lấy template mặc định từ seed data
        var defaultTemplates = GetDefaultTemplates();
        var defaultTemplate = defaultTemplates.FirstOrDefault(t => t.Type == template.Type);
        
        if (defaultTemplate == null)
        {
            return ApiResult.Fail("Không tìm thấy template mặc định cho loại này");
        }

        // Reset về mặc định
        template.Name = defaultTemplate.Name;
        template.Subject = defaultTemplate.Subject;
        template.BodyHtml = defaultTemplate.BodyHtml;
        template.Description = defaultTemplate.Description;
        template.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Đã reset template email về mặc định: {Type}", template.Type);

        return ApiResult.Ok("Đã reset template về mặc định");
    }

    /// <summary>
    /// Preview template với dữ liệu mẫu
    /// </summary>
    public async Task<TemplatePreviewResultDto> PreviewAsync(PreviewTemplateDto dto)
    {
        var template = await _context.EmailTemplates
            .FirstOrDefaultAsync(t => t.Type == dto.Type);

        if (template == null)
        {
            // Fallback to default
            var defaults = GetDefaultTemplates();
            template = defaults.FirstOrDefault(t => t.Type == dto.Type);
        }

        if (template == null)
        {
            return new TemplatePreviewResultDto
            {
                Subject = "Template không tồn tại",
                BodyHtml = "<p>Template không tồn tại</p>"
            };
        }

        // Merge sample data with default sample data
        var sampleData = GetSampleDataForType(dto.Type);
        if (dto.SampleData != null)
        {
            foreach (var kvp in dto.SampleData)
            {
                sampleData[kvp.Key] = kvp.Value;
            }
        }

        var subject = ReplacePlaceholders(template.Subject, sampleData);
        var body = ReplacePlaceholders(template.BodyHtml, sampleData);

        return new TemplatePreviewResultDto
        {
            Subject = subject,
            BodyHtml = body
        };
    }

    /// <summary>
    /// Lấy danh sách placeholders có sẵn cho từng loại template
    /// </summary>
    public async Task<List<TemplatePlaceholderInfoDto>> GetAvailablePlaceholdersAsync()
    {
        var result = new List<TemplatePlaceholderInfoDto>
        {
            new()
            {
                TemplateType = EmailTemplateType.ProbationReminder,
                TemplateName = "Nhắc nhở thử việc (Nhân viên)",
                AvailablePlaceholders = GetPlaceholdersForType(EmailTemplateType.ProbationReminder)
            },
            new()
            {
                TemplateType = EmailTemplateType.ProbationReminderHr,
                TemplateName = "Nhắc nhở thử việc (HR)",
                AvailablePlaceholders = GetPlaceholdersForType(EmailTemplateType.ProbationReminderHr)
            },
            new()
            {
                TemplateType = EmailTemplateType.ContractReminder,
                TemplateName = "Nhắc nhở hợp đồng (Nhân viên)",
                AvailablePlaceholders = GetPlaceholdersForType(EmailTemplateType.ContractReminder)
            },
            new()
            {
                TemplateType = EmailTemplateType.ContractReminderHr,
                TemplateName = "Nhắc nhở hợp đồng (HR)",
                AvailablePlaceholders = GetPlaceholdersForType(EmailTemplateType.ContractReminderHr)
            },
            new()
            {
                TemplateType = EmailTemplateType.BirthdayWish,
                TemplateName = "Chúc mừng sinh nhật",
                AvailablePlaceholders = GetPlaceholdersForType(EmailTemplateType.BirthdayWish)
            },
            new()
            {
                TemplateType = EmailTemplateType.MonthlyBirthdayList,
                TemplateName = "Danh sách sinh nhật tháng",
                AvailablePlaceholders = GetPlaceholdersForType(EmailTemplateType.MonthlyBirthdayList)
            }
        };

        return await Task.FromResult(result);
    }

    /// <summary>
    /// Render template với dữ liệu thực tế
    /// </summary>
    public async Task<(string Subject, string BodyHtml)?> RenderTemplateAsync(
        EmailTemplateType type, 
        Dictionary<string, string> data)
    {
        var template = await _context.EmailTemplates
            .FirstOrDefaultAsync(t => t.Type == type && t.IsActive);

        if (template == null)
        {
            _logger.LogWarning("Template {Type} không tồn tại hoặc không active", type);
            return null;
        }

        var subject = ReplacePlaceholders(template.Subject, data);
        var body = ReplacePlaceholders(template.BodyHtml, data);

        return (subject, body);
    }

    #region Private Methods

    private static EmailTemplateDto MapToDto(EmailTemplate template)
    {
        return new EmailTemplateDto
        {
            Id = template.Id,
            Type = template.Type,
            TypeName = template.Type.ToString(),
            Name = template.Name,
            Subject = template.Subject,
            BodyHtml = template.BodyHtml,
            IsActive = template.IsActive,
            Description = template.Description,
            CreatedAt = template.CreatedAt,
            UpdatedAt = template.UpdatedAt
        };
    }

    private static string ReplacePlaceholders(string template, Dictionary<string, string> data)
    {
        if (string.IsNullOrEmpty(template)) return template;

        foreach (var kvp in data)
        {
            template = template.Replace($"{{{kvp.Key}}}", kvp.Value);
        }

        return template;
    }

    private static List<EmailTemplate> GetDefaultTemplates()
    {
        return new List<EmailTemplate>
        {
            new()
            {
                Type = EmailTemplateType.ProbationReminder,
                Name = "Nhắc nhở thử việc (Nhân viên)",
                Subject = "[Nhắc nhở] Thử việc của {EmployeeName} sắp kết thúc",
                BodyHtml = "<html><body><h2>Thông báo thử việc sắp kết thúc</h2><p>Xin chào {EmployeeName},</p></body></html>"
            }
        };
    }

    private static Dictionary<string, string> GetSampleDataForType(EmailTemplateType type)
    {
        return type switch
        {
            EmailTemplateType.ProbationReminder => new Dictionary<string, string>
            {
                ["EmployeeName"] = "Nguyễn Văn A",
                ["EndDate"] = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy"),
                ["DaysRemaining"] = "7"
            },
            EmailTemplateType.ProbationReminderHr => new Dictionary<string, string>
            {
                ["EmployeeName"] = "Nguyễn Văn A",
                ["Department"] = "Phòng Kỹ thuật",
                ["EndDate"] = DateTime.Now.AddDays(7).ToString("dd/MM/yyyy"),
                ["DaysRemaining"] = "7"
            },
            EmailTemplateType.ContractReminder => new Dictionary<string, string>
            {
                ["EmployeeName"] = "Trần Thị B",
                ["EndDate"] = DateTime.Now.AddDays(15).ToString("dd/MM/yyyy"),
                ["DaysRemaining"] = "15"
            },
            EmailTemplateType.ContractReminderHr => new Dictionary<string, string>
            {
                ["EmployeeName"] = "Trần Thị B",
                ["Department"] = "Phòng Marketing",
                ["EndDate"] = DateTime.Now.AddDays(15).ToString("dd/MM/yyyy"),
                ["DaysRemaining"] = "15"
            },
            EmailTemplateType.BirthdayWish => new Dictionary<string, string>
            {
                ["EmployeeName"] = "Lê Văn C",
                ["BirthDate"] = DateTime.Now.ToString("dd/MM/yyyy"),
                ["Age"] = "30"
            },
            EmailTemplateType.MonthlyBirthdayList => new Dictionary<string, string>
            {
                ["CurrentMonth"] = DateTime.Now.Month.ToString(),
                ["CurrentYear"] = DateTime.Now.Year.ToString(),
                ["BirthdayList"] = "<table><tr><td>01/03</td><td>Nguyễn Văn A</td><td>IT</td></tr></table>",
                ["TotalCount"] = "5"
            },
            _ => new Dictionary<string, string>()
        };
    }

    private static List<PlaceholderDto> GetPlaceholdersForType(EmailTemplateType type)
    {
        return type switch
        {
            EmailTemplateType.ProbationReminder => new List<PlaceholderDto>
            {
                new() { Name = "EmployeeName", Description = "Tên nhân viên", Example = "Nguyễn Văn A" },
                new() { Name = "EndDate", Description = "Ngày kết thúc thử việc", Example = "15/03/2026" },
                new() { Name = "DaysRemaining", Description = "Số ngày còn lại", Example = "7" }
            },
            EmailTemplateType.ProbationReminderHr => new List<PlaceholderDto>
            {
                new() { Name = "EmployeeName", Description = "Tên nhân viên", Example = "Nguyễn Văn A" },
                new() { Name = "Department", Description = "Phòng ban", Example = "Phòng Kỹ thuật" },
                new() { Name = "EndDate", Description = "Ngày kết thúc thử việc", Example = "15/03/2026" },
                new() { Name = "DaysRemaining", Description = "Số ngày còn lại", Example = "7" }
            },
            EmailTemplateType.ContractReminder => new List<PlaceholderDto>
            {
                new() { Name = "EmployeeName", Description = "Tên nhân viên", Example = "Trần Thị B" },
                new() { Name = "EndDate", Description = "Ngày hết hạn hợp đồng", Example = "30/04/2026" },
                new() { Name = "DaysRemaining", Description = "Số ngày còn lại", Example = "15" }
            },
            EmailTemplateType.ContractReminderHr => new List<PlaceholderDto>
            {
                new() { Name = "EmployeeName", Description = "Tên nhân viên", Example = "Trần Thị B" },
                new() { Name = "Department", Description = "Phòng ban", Example = "Phòng Marketing" },
                new() { Name = "EndDate", Description = "Ngày hết hạn hợp đồng", Example = "30/04/2026" },
                new() { Name = "DaysRemaining", Description = "Số ngày còn lại", Example = "15" }
            },
            EmailTemplateType.BirthdayWish => new List<PlaceholderDto>
            {
                new() { Name = "EmployeeName", Description = "Tên nhân viên", Example = "Lê Văn C" },
                new() { Name = "BirthDate", Description = "Ngày sinh", Example = "05/03/1996" },
                new() { Name = "Age", Description = "Tuổi", Example = "30" }
            },
            EmailTemplateType.MonthlyBirthdayList => new List<PlaceholderDto>
            {
                new() { Name = "CurrentMonth", Description = "Tháng hiện tại", Example = "3" },
                new() { Name = "CurrentYear", Description = "Năm hiện tại", Example = "2026" },
                new() { Name = "BirthdayList", Description = "Bảng danh sách sinh nhật (HTML)", Example = "<table>...</table>" },
                new() { Name = "TotalCount", Description = "Tổng số nhân viên", Example = "5" }
            },
            _ => new List<PlaceholderDto>()
        };
    }

    #endregion
}
