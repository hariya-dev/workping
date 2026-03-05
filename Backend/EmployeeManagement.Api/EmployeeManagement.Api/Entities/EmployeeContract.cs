// Entities/EmployeeContract.cs
// Entity lưu trữ lịch sử hợp đồng của nhân viên

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EmployeeManagement.Api.Entities;

/// <summary>
/// Entity hợp đồng nhân viên
/// Mỗi nhân viên có thể có nhiều hợp đồng (One-to-Many)
/// Chỉ cho phép 1 hợp đồng đang thực hiện tại một thời điểm
/// </summary>
public class EmployeeContract
{
    /// <summary>
    /// ID hợp đồng
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID nhân viên (FK)
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// ID loại hợp đồng (FK)
    /// </summary>
    public Guid ContractTypeId { get; set; }

    /// <summary>
    /// Ngày bắt đầu hợp đồng
    /// </summary>
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// Ngày kết thúc hợp đồng (null = không thời hạn)
    /// </summary>
    public DateOnly? EndDate { get; set; }

    /// <summary>
    /// Ghi chú về hợp đồng
    /// </summary>
    [MaxLength(1000)]
    public string? Notes { get; set; }

    /// <summary>
    /// Thời điểm tạo hợp đồng
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// ID người tạo hợp đồng
    /// </summary>
    public Guid? CreatedBy { get; set; }

    /// <summary>
    /// Trạng thái hợp đồng (computed, không lưu DB)
    /// "Đang thực hiện" nếu EndDate null hoặc >= today
    /// "Đã kết thúc" nếu EndDate < today
    /// </summary>
    [NotMapped]
    public string Status => EndDate == null || EndDate >= DateOnly.FromDateTime(DateTime.Today)
        ? "Đang thực hiện"
        : "Đã kết thúc";

    /// <summary>
    /// Kiểm tra hợp đồng có đang active không
    /// </summary>
    [NotMapped]
    public bool IsActive => EndDate == null || EndDate >= DateOnly.FromDateTime(DateTime.Today);

    // Navigation Properties

    /// <summary>
    /// Nhân viên sở hữu hợp đồng
    /// </summary>
    public virtual Employee? Employee { get; set; }

    /// <summary>
    /// Loại hợp đồng
    /// </summary>
    public virtual ContractType? ContractType { get; set; }
}
