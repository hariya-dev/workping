// Services/IEmailTemplateService.cs
// Interface dịch vụ quản lý template email

using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Entities;

namespace EmployeeManagement.Api.Services;

/// <summary>
/// Interface dịch vụ quản lý template email
/// </summary>
public interface IEmailTemplateService
{
    /// <summary>
    /// Lấy tất cả templates
    /// </summary>
    Task<List<EmailTemplateSummaryDto>> GetAllAsync();

    /// <summary>
    /// Lấy template theo ID
    /// </summary>
    Task<EmailTemplateDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Lấy template theo loại
    /// </summary>
    Task<EmailTemplateDto?> GetByTypeAsync(EmailTemplateType type);

    /// <summary>
    /// Tạo hoặc cập nhật template
    /// </summary>
    Task<ApiResult<UpsertResultDto>> UpsertAsync(Guid? id, UpsertEmailTemplateDto dto);

    /// <summary>
    /// Xóa template (reset về mặc định)
    /// </summary>
    Task<ApiResult> ResetToDefaultAsync(Guid id);

    /// <summary>
    /// Preview template với dữ liệu mẫu
    /// </summary>
    Task<TemplatePreviewResultDto> PreviewAsync(PreviewTemplateDto dto);

    /// <summary>
    /// Lấy danh sách placeholders có sẵn cho từng loại template
    /// </summary>
    Task<List<TemplatePlaceholderInfoDto>> GetAvailablePlaceholdersAsync();

    /// <summary>
    /// Render template với dữ liệu thực tế
    /// </summary>
    /// <param name="type">Loại template</param>
    /// <param name="data">Dữ liệu để điền vào placeholder</param>
    /// <returns>Tuple (Subject, BodyHtml)</returns>
    Task<(string Subject, string BodyHtml)?> RenderTemplateAsync(
        EmailTemplateType type, 
        Dictionary<string, string> data);
}

/// <summary>
/// DTO kết quả upsert
/// </summary>
public class UpsertResultDto
{
    public Guid Id { get; set; }
    public bool IsNew { get; set; }
}
