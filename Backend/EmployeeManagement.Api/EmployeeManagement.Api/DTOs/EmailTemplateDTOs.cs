// DTOs/EmailTemplateDTOs.cs
// Data Transfer Objects cho Email Template API

using System.ComponentModel.DataAnnotations;
using EmployeeManagement.Api.Entities;

namespace EmployeeManagement.Api.DTOs;

/// <summary>
/// DTO hiển thị template email
/// </summary>
public class EmailTemplateDto
{
    public Guid Id { get; set; }
    public EmailTemplateType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string BodyHtml { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO tạo/cập nhật template email
/// </summary>
public class UpsertEmailTemplateDto
{
    /// <summary>
    /// Loại template (bắt buộc khi tạo mới)
    /// </summary>
    public EmailTemplateType? Type { get; set; }

    [Required(ErrorMessage = "Tên template là bắt buộc")]
    [MaxLength(100, ErrorMessage = "Tên không được quá 100 ký tự")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Tiêu đề email là bắt buộc")]
    [MaxLength(500, ErrorMessage = "Tiêu đề không được quá 500 ký tự")]
    public string Subject { get; set; } = string.Empty;

    [Required(ErrorMessage = "Nội dung email là bắt buộc")]
    public string BodyHtml { get; set; } = string.Empty;

    public bool IsActive { get; set; } = true;

    [MaxLength(1000, ErrorMessage = "Mô tả không được quá 1000 ký tự")]
    public string? Description { get; set; }
}

/// <summary>
/// DTO danh sách template (tóm tắt)
/// </summary>
public class EmailTemplateSummaryDto
{
    public Guid Id { get; set; }
    public EmailTemplateType Type { get; set; }
    public string TypeName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

/// <summary>
/// DTO preview template với dữ liệu mẫu
/// </summary>
public class PreviewTemplateDto
{
    [Required]
    public EmailTemplateType Type { get; set; }
    
    /// <summary>
    /// Dữ liệu mẫu để preview (optional)
    /// </summary>
    public Dictionary<string, string>? SampleData { get; set; }
}

/// <summary>
/// Kết quả preview template
/// </summary>
public class TemplatePreviewResultDto
{
    public string Subject { get; set; } = string.Empty;
    public string BodyHtml { get; set; } = string.Empty;
}

/// <summary>
/// Danh sách các placeholder có sẵn cho từng loại template
/// </summary>
public class TemplatePlaceholderInfoDto
{
    public EmailTemplateType TemplateType { get; set; }
    public string TemplateName { get; set; } = string.Empty;
    public List<PlaceholderDto> AvailablePlaceholders { get; set; } = new();
}

/// <summary>
/// Thông tin một placeholder
/// </summary>
public class PlaceholderDto
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Example { get; set; } = string.Empty;
}
