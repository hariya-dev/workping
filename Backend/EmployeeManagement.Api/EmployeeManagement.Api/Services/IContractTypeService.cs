// Services/IContractTypeService.cs
// Interface dịch vụ quản lý loại hợp đồng

using EmployeeManagement.Api.DTOs;

namespace EmployeeManagement.Api.Services;

/// <summary>
/// Interface dịch vụ quản lý loại hợp đồng (Master Data)
/// </summary>
public interface IContractTypeService
{
    /// <summary>
    /// Lấy tất cả loại hợp đồng
    /// </summary>
    Task<List<ContractTypeDto>> GetAllAsync(bool activeOnly = true);

    /// <summary>
    /// Lấy loại hợp đồng theo ID
    /// </summary>
    Task<ContractTypeDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Tạo loại hợp đồng mới
    /// </summary>
    Task<ApiResult<ContractTypeDto>> CreateAsync(CreateContractTypeDto dto);

    /// <summary>
    /// Cập nhật loại hợp đồng
    /// </summary>
    Task<ApiResult<ContractTypeDto>> UpdateAsync(Guid id, UpdateContractTypeDto dto);

    /// <summary>
    /// Xóa loại hợp đồng
    /// </summary>
    Task<ApiResult> DeleteAsync(Guid id);
}
