// Endpoints/EmployeeEndpoints.cs
// Minimal API endpoints cho quản lý nhân viên

using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Entities;
using EmployeeManagement.Api.Services;

namespace EmployeeManagement.Api.Endpoints;

/// <summary>
/// Định nghĩa các API endpoints cho Employee
/// </summary>
public static class EmployeeEndpoints
{
    /// <summary>
    /// Map các routes cho Employee CRUD
    /// </summary>
    public static void MapEmployeeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/employees")
            .WithTags("Employees")
            .RequireAuthorization();

        // GET /api/employees - Lấy danh sách nhân viên (có filter, phân trang)
        group.MapGet("/", async (
            string? searchTerm,
            EmployeeStatus? status,
            Guid? contractTypeId,
            DateOnly? probationEndDateFrom,
            DateOnly? probationEndDateTo,
            DateOnly? contractEndDateFrom,
            DateOnly? contractEndDateTo,
            int pageNumber,
            int pageSize,
            IEmployeeService service) =>
        {
            // Đảm bảo giá trị mặc định cho phân trang
            if (pageNumber < 1) pageNumber = 1;
            if (pageSize < 1) pageSize = 10;
            if (pageSize > 100) pageSize = 100; // Giới hạn tối đa

            var filter = new EmployeeFilterDto
            {
                SearchTerm = searchTerm,
                Status = status,
                ContractTypeId = contractTypeId,
                ProbationEndDateFrom = probationEndDateFrom,
                ProbationEndDateTo = probationEndDateTo,
                ContractEndDateFrom = contractEndDateFrom,
                ContractEndDateTo = contractEndDateTo,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await service.GetEmployeesAsync(filter);
            return Results.Ok(ApiResult<PagedResult<EmployeeDto>>.Ok(result));
        })
        .WithName("GetEmployees")
        .WithSummary("Lấy danh sách nhân viên (có filter và phân trang)");

        // GET /api/employees/{id} - Lấy chi tiết một nhân viên
        group.MapGet("/{id:guid}", async (Guid id, IEmployeeService service) =>
        {
            var employee = await service.GetEmployeeByIdAsync(id);
            
            if (employee == null)
            {
                return Results.NotFound(ApiResult<EmployeeDto>.Fail("Không tìm thấy nhân viên"));
            }
            
            return Results.Ok(ApiResult<EmployeeDto>.Ok(employee));
        })
        .WithName("GetEmployeeById")
        .WithSummary("Lấy thông tin chi tiết một nhân viên");

        // POST /api/employees - Tạo nhân viên mới
        group.MapPost("/", async (CreateEmployeeDto dto, IEmployeeService service) =>
        {
            var result = await service.CreateEmployeeAsync(dto);
            
            if (!result.Success)
            {
                return Results.BadRequest(result);
            }
            
            return Results.Created($"/api/employees/{result.Data!.Id}", result);
        })
        .WithName("CreateEmployee")
        .WithSummary("Tạo nhân viên mới");

        // PUT /api/employees/{id} - Cập nhật nhân viên
        group.MapPut("/{id:guid}", async (Guid id, UpdateEmployeeDto dto, IEmployeeService service) =>
        {
            var result = await service.UpdateEmployeeAsync(id, dto);
            
            if (!result.Success)
            {
                return Results.BadRequest(result);
            }
            
            return Results.Ok(result);
        })
        .WithName("UpdateEmployee")
        .WithSummary("Cập nhật thông tin nhân viên");

        // DELETE /api/employees/{id} - Xóa nhân viên
        group.MapDelete("/{id:guid}", async (Guid id, IEmployeeService service) =>
        {
            var result = await service.DeleteEmployeeAsync(id);
            
            if (!result.Success)
            {
                return Results.BadRequest(result);
            }
            
            return Results.Ok(result);
        })
        .WithName("DeleteEmployee")
        .WithSummary("Xóa nhân viên");

        // POST /api/employees/{id}/files - Upload file cho nhân viên
        group.MapPost("/{id:guid}/files", async (Guid id, IFormFile file, string? description, IEmployeeService service) =>
        {
            var result = await service.UploadFileAsync(id, file, description);
            
            if (!result.Success)
            {
                return Results.BadRequest(result);
            }
            
            return Results.Ok(result);
        })
        .WithName("UploadEmployeeFile")
        .WithSummary("Upload file cho nhân viên")
        .DisableAntiforgery(); // Cần thiết cho file upload

        // GET /api/employees/{id}/files - Lấy danh sách file của nhân viên
        group.MapGet("/{id:guid}/files", async (Guid id, IEmployeeService service) =>
        {
            var files = await service.GetEmployeeFilesAsync(id);
            return Results.Ok(ApiResult<List<EmployeeFileDto>>.Ok(files));
        })
        .WithName("GetEmployeeFiles")
        .WithSummary("Lấy danh sách file của nhân viên");

        // DELETE /api/employees/{employeeId}/files/{fileId} - Xóa file
        group.MapDelete("/{employeeId:guid}/files/{fileId:guid}", async (Guid employeeId, Guid fileId, IEmployeeService service) =>
        {
            var result = await service.DeleteFileAsync(employeeId, fileId);
            
            if (!result.Success)
            {
                return Results.BadRequest(result);
            }
            
            return Results.Ok(result);
        })
        .WithName("DeleteEmployeeFile")
        .WithSummary("Xóa file của nhân viên");

        // GET /api/employees/{employeeId}/files/{fileId}/download - Download file
        group.MapGet("/{employeeId:guid}/files/{fileId:guid}/download", async (Guid employeeId, Guid fileId, IEmployeeService service) =>
        {
            var (fileStream, fileName, contentType, error) = await service.DownloadFileAsync(employeeId, fileId);

            if (error != null)
            {
                return Results.NotFound(ApiResult.Fail(error));
            }

            if (fileStream == null)
            {
                return Results.NotFound(ApiResult.Fail("Không tìm thấy file"));
            }

            return Results.File(fileStream, contentType, fileName);
        })
        .WithName("DownloadEmployeeFile")
        .WithSummary("Download file của nhân viên");

        // GET /api/employees/{id}/history - Lấy lịch sử chỉnh sửa nhân viên
        group.MapGet("/{id:guid}/history", async (Guid id, IEmployeeHistoryService historyService) =>
        {
            var history = await historyService.GetEditHistoryAsync(id);
            return Results.Ok(ApiResult<List<EmployeeEditHistoryDto>>.Ok(history));
        })
        .WithName("GetEmployeeEditHistory")
        .WithSummary("Lấy lịch sử chỉnh sửa thông tin nhân viên");

        // GET /api/employees/statuses - Lấy danh sách trạng thái (cho dropdown)
        group.MapGet("/statuses", () =>
        {
            var statuses = Enum.GetValues<EmployeeStatus>()
                .Select(s => new
                {
                    Value = (int)s,
                    Name = s.ToString(),
                    DisplayName = GetStatusDisplayName(s)
                })
                .ToList();

            return Results.Ok(ApiResult<object>.Ok(statuses));
        })
        .WithName("GetEmployeeStatuses")
        .WithSummary("Lấy danh sách trạng thái nhân viên");
    }

    /// <summary>
    /// Lấy tên hiển thị của trạng thái
    /// </summary>
    private static string GetStatusDisplayName(EmployeeStatus status)
    {
        return status switch
        {
            EmployeeStatus.Active => "Đang làm việc",
            EmployeeStatus.Resigned => "Đã nghỉ việc",
            _ => "Không xác định"
        };
    }
}
