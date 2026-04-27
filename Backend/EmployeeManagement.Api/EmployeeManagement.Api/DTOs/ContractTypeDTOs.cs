// DTOs/ContractTypeDTOs.cs
// Data Transfer Objects cho ContractType API

using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.DTOs;

/// <summary>
/// DTO tạo loại hợp đồng mới
/// </summary>
public class CreateContractTypeDto
{
    [Required(ErrorMessage = "Ten loai hop dong la bat buoc")]
    [MaxLength(200, ErrorMessage = "Ten toi da 200 ky tu")]
    public string Name { get; set; } = string.Empty;

    public int? DurationDays { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }
}

/// <summary>
/// DTO cập nhật loại hợp đồng
/// </summary>
public class UpdateContractTypeDto
{
    [Required(ErrorMessage = "Ten loai hop dong la bat buoc")]
    [MaxLength(200, ErrorMessage = "Ten toi da 200 ky tu")]
    public string Name { get; set; } = string.Empty;

    public int? DurationDays { get; set; }

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsActive { get; set; } = true;
}

/// <summary>
/// DTO trả về thông tin loại hợp đồng
/// </summary>
public class ContractTypeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int? DurationDays { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public int EmployeeCount { get; set; }
}
