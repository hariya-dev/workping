// Services/IEmployeeService.cs
// Interface dịch vụ quản lý nhân viên

using EmployeeManagement.Api.DTOs;
using Microsoft.AspNetCore.Http;

namespace EmployeeManagement.Api.Services;

/// <summary>
/// Interface dịch vụ quản lý nhân viên
/// </summary>
public interface IEmployeeService
{
    /// <summary>
    /// Lấy danh sách nhân viên (có lọc, phân trang)
    /// </summary>
    Task<PagedResult<EmployeeDto>> GetEmployeesAsync(EmployeeFilterDto filter);

    /// <summary>
    /// Lấy thông tin chi tiết nhân viên
    /// </summary>
    Task<EmployeeDto?> GetEmployeeByIdAsync(Guid id);

    /// <summary>
    /// Tạo nhân viên mới
    /// </summary>
    Task<ApiResult<EmployeeDto>> CreateEmployeeAsync(CreateEmployeeDto dto);

    /// <summary>
    /// Cập nhật nhân viên
    /// </summary>
    Task<ApiResult<EmployeeDto>> UpdateEmployeeAsync(Guid id, UpdateEmployeeDto dto);

    /// <summary>
    /// Xóa nhân viên
    /// </summary>
    Task<ApiResult> DeleteEmployeeAsync(Guid id);

    /// <summary>
    /// Upload file cho nhân viên
    /// </summary>
    Task<ApiResult<EmployeeFileDto>> UploadFileAsync(Guid employeeId, IFormFile file, string? description);

    /// <summary>
    /// Xóa file của nhân viên
    /// </summary>
    Task<ApiResult> DeleteFileAsync(Guid employeeId, Guid fileId);

    /// <summary>
    /// Lấy danh sách file của nhân viên
    /// </summary>
    Task<List<EmployeeFileDto>> GetEmployeeFilesAsync(Guid employeeId);

    /// <summary>
    /// Download file của nhân viên
    /// </summary>
    Task<(Stream? FileStream, string? FileName, string? ContentType, string? Error)> DownloadFileAsync(Guid employeeId, Guid fileId);
}
