// DTOs/EmployeeContractDTOs.cs
// Data Transfer Objects cho EmployeeContract API

using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.DTOs;

/// <summary>
/// DTO tạo hợp đồng mới cho nhân viên
/// </summary>
public class CreateEmployeeContractDto
{
    /// <summary>
    /// ID loại hợp đồng (bắt buộc)
    /// </summary>
    [Required(ErrorMessage = "Loại hợp đồng là bắt buộc")]
    public Guid ContractTypeId { get; set; }

    /// <summary>
    /// Ngày bắt đầu hợp đồng (bắt buộc)
    /// </summary>
    [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
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
}

/// <summary>
/// DTO trả về thông tin hợp đồng
/// </summary>
public class EmployeeContractDto
{
    /// <summary>
    /// ID hợp đồng
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// ID nhân viên
    /// </summary>
    public Guid EmployeeId { get; set; }

    /// <summary>
    /// ID loại hợp đồng
    /// </summary>
    public Guid ContractTypeId { get; set; }

    /// <summary>
    /// Tên loại hợp đồng
    /// </summary>
    public string ContractTypeName { get; set; } = string.Empty;

    /// <summary>
    /// Thời hạn hợp đồng (số ngày)
    /// </summary>
    public int? DurationDays { get; set; }

    /// <summary>
    /// Ngày bắt đầu
    /// </summary>
    public DateOnly StartDate { get; set; }

    /// <summary>
    /// Ngày kết thúc (null = không thời hạn)
    /// </summary>
    public DateOnly? EndDate { get; set; }

    /// <summary>
    /// Trạng thái hợp đồng (auto-computed)
    /// "Đang thực hiện" hoặc "Đã kết thúc"
    /// </summary>
    public string Status { get; set; } = string.Empty;

    /// <summary>
    /// Ghi chú
    /// </summary>
    public string? Notes { get; set; }

    /// <summary>
    /// Thời điểm tạo
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Tên người tạo
    /// </summary>
    public string? CreatedByName { get; set; }
}

/// <summary>
/// DTO cập nhật hợp đồng (khi nhập sai thông tin)
/// </summary>
public class UpdateEmployeeContractDto
{
    /// <summary>
    /// ID loại hợp đồng
    /// </summary>
    [Required(ErrorMessage = "Loại hợp đồng là bắt buộc")]
    public Guid ContractTypeId { get; set; }

    /// <summary>
    /// Ngày bắt đầu hợp đồng
    /// </summary>
    [Required(ErrorMessage = "Ngày bắt đầu là bắt buộc")]
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
}

/// <summary>
/// DTO kết thúc hợp đồng
/// </summary>
public class TerminateContractDto
{
    /// <summary>
    /// Lý do kết thúc hợp đồng (tùy chọn)
    /// </summary>
    [MaxLength(500)]
    public string? Notes { get; set; }
}
