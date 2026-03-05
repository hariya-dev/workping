// Services/IEmployeeContractService.cs
// Interface cho dịch vụ quản lý hợp đồng nhân viên

using EmployeeManagement.Api.DTOs;

namespace EmployeeManagement.Api.Services;

/// <summary>
/// Interface cho service quản lý hợp đồng nhân viên
/// </summary>
public interface IEmployeeContractService
{
    /// <summary>
    /// Lấy danh sách tất cả hợp đồng của nhân viên
    /// </summary>
    Task<List<EmployeeContractDto>> GetContractsByEmployeeIdAsync(Guid employeeId);

    /// <summary>
    /// Lấy hợp đồng đang thực hiện của nhân viên
    /// </summary>
    Task<EmployeeContractDto?> GetActiveContractAsync(Guid employeeId);

    /// <summary>
    /// Tạo hợp đồng mới cho nhân viên
    /// Chỉ cho phép 1 hợp đồng đang thực hiện tại một thời điểm
    /// </summary>
    Task<ApiResult<EmployeeContractDto>> CreateContractAsync(Guid employeeId, CreateEmployeeContractDto dto, Guid? userId, string userName);

    /// <summary>
    /// Kết thúc hợp đồng (set EndDate = ngày hôm qua)
    /// </summary>
    Task<ApiResult<EmployeeContractDto>> TerminateContractAsync(Guid employeeId, Guid contractId, TerminateContractDto? dto, Guid? userId, string userName);
}
