// Entities/ContractType.cs
// Entity loại hợp đồng lao động (Master Data)

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.Api.Entities;

/// <summary>
/// Entity loại hợp đồng lao động
/// Ví dụ: "Thử việc 2 tháng", "HĐ 12 tháng", "Không thời hạn"
/// </summary>
[Table("ContractTypes")]
public class ContractType
{
    /// <summary>
    /// Khóa chính - ID loại hợp đồng
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Tên loại hợp đồng (bắt buộc)
    /// </summary>
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Số tháng của hợp đồng (null = không thời hạn)
    /// </summary>
    public int? DurationMonths { get; set; }

    /// <summary>
    /// Mô tả chi tiết loại hợp đồng
    /// </summary>
    [MaxLength(500)]
    public string? Description { get; set; }

    /// <summary>
    /// Trạng thái hoạt động
    /// </summary>
    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Ngày tạo
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Ngày cập nhật
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // Navigation property
    /// <summary>
    /// Danh sách hợp đồng thuộc loại này
    /// </summary>
    public virtual ICollection<EmployeeContract> Contracts { get; set; } = new List<EmployeeContract>();
}
