// DTOs/EmployeeDTOs.cs
// Data Transfer Objects cho Employee API

using System.ComponentModel.DataAnnotations;
using EmployeeManagement.Api.Entities;

namespace EmployeeManagement.Api.DTOs;

/// <summary>
/// DTO tạo nhân viên mới
/// Hợp đồng sẽ được thêm riêng sau khi tạo nhân viên
/// </summary>
public class CreateEmployeeDto
{
    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [MaxLength(200, ErrorMessage = "Họ tên tối đa 200 ký tự")]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
    public DateOnly DateOfBirth { get; set; }

    public DateOnly? ProbationStartDate { get; set; }
    public DateOnly? ProbationEndDate { get; set; }

    public EmployeeStatus Status { get; set; } = EmployeeStatus.Active;

    [MaxLength(200)]
    public string? Department { get; set; }

    [MaxLength(200)]
    public string? Position { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}

/// <summary>
/// DTO cập nhật nhân viên
/// Hợp đồng được quản lý riêng qua EmployeeContract
/// </summary>
public class UpdateEmployeeDto
{
    [Required(ErrorMessage = "Họ tên là bắt buộc")]
    [MaxLength(200, ErrorMessage = "Họ tên tối đa 200 ký tự")]
    public string FullName { get; set; } = string.Empty;

    [EmailAddress(ErrorMessage = "Email không hợp lệ")]
    public string? Email { get; set; }

    [MaxLength(20)]
    public string? PhoneNumber { get; set; }

    [Required(ErrorMessage = "Ngày sinh là bắt buộc")]
    public DateOnly DateOfBirth { get; set; }

    public DateOnly? ProbationStartDate { get; set; }
    public DateOnly? ProbationEndDate { get; set; }

    public EmployeeStatus Status { get; set; }

    [MaxLength(200)]
    public string? Department { get; set; }

    [MaxLength(200)]
    public string? Position { get; set; }

    [MaxLength(1000)]
    public string? Notes { get; set; }
}

/// <summary>
/// DTO trả về thông tin nhân viên
/// </summary>
public class EmployeeDto
{
    public Guid Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public DateOnly DateOfBirth { get; set; }
    public DateOnly? ProbationStartDate { get; set; }
    public DateOnly? ProbationEndDate { get; set; }
    public EmployeeStatus Status { get; set; }
    public string StatusDisplayName { get; set; } = string.Empty;
    public string? Department { get; set; }
    public string? Position { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    
    /// <summary>
    /// Hợp đồng đang thực hiện (nếu có)
    /// </summary>
    public EmployeeContractDto? ActiveContract { get; set; }
    
    /// <summary>
    /// Danh sách file đính kèm
    /// </summary>
    public List<EmployeeFileDto> Files { get; set; } = new();
}

/// <summary>
/// DTO thông tin file đính kèm
/// </summary>
public class EmployeeFileDto
{
    public Guid Id { get; set; }
    public string OriginalFileName { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string? FileExtension { get; set; }
    public long FileSize { get; set; }
    public string? Description { get; set; }
    public DateTime UploadedAt { get; set; }
}

/// <summary>
/// DTO danh sách nhân viên (phân trang)
/// </summary>
public class EmployeeListDto
{
    public List<EmployeeDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages { get; set; }
}

/// <summary>
/// DTO bộ lọc tìm kiếm nhân viên
/// </summary>
public class EmployeeFilterDto
{
    public string? SearchTerm { get; set; }
    public EmployeeStatus? Status { get; set; }
    public Guid? ContractTypeId { get; set; }
    public DateOnly? ProbationEndDateFrom { get; set; }
    public DateOnly? ProbationEndDateTo { get; set; }
    public DateOnly? ContractEndDateFrom { get; set; }
    public DateOnly? ContractEndDateTo { get; set; }
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}
