// Entities/EmployeeFile.cs
// Entity lưu thông tin file đính kèm của nhân viên

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.Api.Entities;

/// <summary>
/// Entity file đính kèm nhân viên (CV, CMND, bằng cấp, v.v.)
/// </summary>
[Table("EmployeeFiles")]
public class EmployeeFile
{
    /// <summary>
    /// Khóa chính - ID file
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// ID nhân viên (FK)
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// Tên file gốc khi upload
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string OriginalFileName { get; set; } = string.Empty;

    /// <summary>
    /// Tên file lưu trữ trên server (có thể khác tên gốc để tránh trùng)
    /// </summary>
    [Required]
    [MaxLength(500)]
    public string StoredFileName { get; set; } = string.Empty;

    /// <summary>
    /// Đường dẫn tương đối đến file (uploads/employees/{employeeId}/filename)
    /// </summary>
    [Required]
    [MaxLength(1000)]
    public string FilePath { get; set; } = string.Empty;

    /// <summary>
    /// Phần mở rộng file (.pdf, .docx, .jpg, v.v.)
    /// </summary>
    [MaxLength(20)]
    public string? FileExtension { get; set; }

    /// <summary>
    /// Kích thước file (bytes)
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    /// Loại MIME của file
    /// </summary>
    [MaxLength(100)]
    public string? ContentType { get; set; }

    /// <summary>
    /// Mô tả/ghi chú về file (ví dụ: "Bằng đại học", "CMND mặt trước")
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Ngày upload
    /// </summary>
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Navigation property
    /// <summary>
    /// Nhân viên sở hữu file này
    /// </summary>
    [ForeignKey("EmployeeId")]
    public virtual Employee? Employee { get; set; }
}
