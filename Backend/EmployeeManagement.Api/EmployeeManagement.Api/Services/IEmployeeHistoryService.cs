// Services/IEmployeeHistoryService.cs
// Interface cho dịch vụ lịch sử chỉnh sửa nhân viên

using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Entities;

namespace EmployeeManagement.Api.Services;

/// <summary>
/// Interface cho service quản lý lịch sử chỉnh sửa nhân viên
/// </summary>
public interface IEmployeeHistoryService
{
    /// <summary>
    /// Lấy danh sách lịch sử chỉnh sửa của nhân viên
    /// </summary>
    Task<List<EmployeeEditHistoryDto>> GetEditHistoryAsync(Guid employeeId);

    /// <summary>
    /// Ghi log khi tạo nhân viên mới
    /// </summary>
    Task TrackCreateAsync(Guid employeeId, Employee employee, Guid? userId, string userName);

    /// <summary>
    /// Ghi log thay đổi một field
    /// </summary>
    Task TrackChangeAsync(
        Guid employeeId,
        string fieldName,
        string fieldDisplayName,
        string? oldValue,
        string? newValue,
        Guid? userId,
        string userName,
        string changeType);

    /// <summary>
    /// Ghi log khi cập nhật nhân viên (so sánh và log tất cả các field thay đổi)
    /// </summary>
    Task TrackUpdateAsync(Guid employeeId, Employee oldEmployee, Employee newEmployee, Guid? userId, string userName);

    /// <summary>
    /// Ghi log khi thêm hợp đồng mới
    /// </summary>
    Task TrackContractCreatedAsync(Guid employeeId, EmployeeContract contract, Guid? userId, string userName);

    /// <summary>
    /// Ghi log khi kết thúc hợp đồng
    /// </summary>
    Task TrackContractTerminatedAsync(Guid employeeId, EmployeeContract contract, Guid? userId, string userName);
}
