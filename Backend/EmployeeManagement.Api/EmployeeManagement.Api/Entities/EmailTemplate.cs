// Entities/EmailTemplate.cs
// Entity lưu trữ template email trong database

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.Api.Entities;

/// <summary>
/// Loại template email
/// </summary>
public enum EmailTemplateType
{
    /// <summary>
    /// Email nhắc nhở thử việc sắp kết thúc
    /// </summary>
    ProbationReminder = 1,
    
    /// <summary>
    /// Email nhắc nhở hợp đồng sắp hết hạn
    /// </summary>
    ContractReminder = 2,
    
    /// <summary>
    /// Email chúc mừng sinh nhật
    /// </summary>
    BirthdayWish = 3,
    
    /// <summary>
    /// Email danh sách sinh nhật tháng (gửi HR)
    /// </summary>
    MonthlyBirthdayList = 4,
    
    /// <summary>
    /// Email nhắc nhở thử việc cho HR
    /// </summary>
    ProbationReminderHr = 5,
    
    /// <summary>
    /// Email nhắc nhở hợp đồng cho HR
    /// </summary>
    ContractReminderHr = 6
}

/// <summary>
/// Entity template email - lưu trữ nội dung email có thể tùy chỉnh
/// </summary>
[Table("EmailTemplates")]
public class EmailTemplate
{
    /// <summary>
    /// Khóa chính
    /// </summary>
    [Key]
    public Guid Id { get; set; }
    
    /// <summary>
    /// Loại template
    /// </summary>
    public EmailTemplateType Type { get; set; }
    
    /// <summary>
    /// Tên template (mô tả)
    /// </summary>
    [Required]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;
    
    /// <summary>
    /// Tiêu đề email (hỗ trợ placeholder)
    /// Ví dụ: "[Nhắc nhở] Thử việc của {EmployeeName} sắp kết thúc"
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string Subject { get; set; } = string.Empty;
    
    /// <summary>
    /// Nội dung email HTML (hỗ trợ placeholder)
    /// Placeholder format: {PlaceholderName}
    /// Các placeholder phổ biến:
    /// - {EmployeeName}: Tên nhân viên
    /// - {Department}: Phòng ban
    /// - {EndDate}: Ngày kết thúc
    /// - {DaysRemaining}: Số ngày còn lại
    /// - {BirthDate}: Ngày sinh
    /// - {Age}: Tuổi
    /// - {CurrentMonth}: Tháng hiện tại
    /// - {CurrentYear}: Năm hiện tại
    /// - {BirthdayList}: Danh sách sinh nhật (dành cho email tháng)
    /// </summary>
    [Required]
    public string BodyHtml { get; set; } = string.Empty;
    
    /// <summary>
    /// Có đang active không
    /// </summary>
    public bool IsActive { get; set; } = true;
    
    /// <summary>
    /// Mô tả về template này
    /// </summary>
    [MaxLength(1000)]
    public string? Description { get; set; }
    
    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
