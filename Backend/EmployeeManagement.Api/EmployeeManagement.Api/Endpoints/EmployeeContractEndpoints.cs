// Endpoints/EmployeeContractEndpoints.cs
// Minimal API endpoints cho quản lý hợp đồng nhân viên

using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Services;

namespace EmployeeManagement.Api.Endpoints;

/// <summary>
/// Định nghĩa các API endpoints cho EmployeeContract
/// </summary>
public static class EmployeeContractEndpoints
{
    /// <summary>
    /// Map các routes cho Contract CRUD
    /// </summary>
    public static void MapEmployeeContractEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/employees/{employeeId:guid}/contracts")
            .WithTags("Employee Contracts")
            .RequireAuthorization();

        // GET /api/employees/{employeeId}/contracts - Lấy danh sách hợp đồng của nhân viên
        group.MapGet("/", async (Guid employeeId, IEmployeeContractService service) =>
        {
            var contracts = await service.GetContractsByEmployeeIdAsync(employeeId);
            return Results.Ok(ApiResult<List<EmployeeContractDto>>.Ok(contracts));
        })
        .WithName("GetEmployeeContracts")
        .WithSummary("Lấy danh sách tất cả hợp đồng của nhân viên (lịch sử hợp đồng)");

        // GET /api/employees/{employeeId}/contracts/active - Lấy hợp đồng đang thực hiện
        group.MapGet("/active", async (Guid employeeId, IEmployeeContractService service) =>
        {
            var contract = await service.GetActiveContractAsync(employeeId);
            
            if (contract == null)
            {
                return Results.Ok(ApiResult<EmployeeContractDto?>.Ok(null, "Nhân viên chưa có hợp đồng đang thực hiện"));
            }
            
            return Results.Ok(ApiResult<EmployeeContractDto>.Ok(contract));
        })
        .WithName("GetActiveContract")
        .WithSummary("Lấy hợp đồng đang thực hiện của nhân viên");

        // POST /api/employees/{employeeId}/contracts - Tạo hợp đồng mới
        group.MapPost("/", async (
            Guid employeeId, 
            CreateEmployeeContractDto dto, 
            IEmployeeContractService service,
            HttpContext httpContext) =>
        {
            // TODO: Lấy userId và userName từ JWT claims khi có authentication
            Guid? userId = null;
            string userName = "Hệ thống";
            
            // Thử lấy thông tin user từ claims
            var userIdClaim = httpContext.User.FindFirst("sub") ?? httpContext.User.FindFirst("userId");
            var userNameClaim = httpContext.User.FindFirst("name") ?? httpContext.User.FindFirst("fullName");
            
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var parsedUserId))
            {
                userId = parsedUserId;
            }
            if (userNameClaim != null)
            {
                userName = userNameClaim.Value;
            }

            var result = await service.CreateContractAsync(employeeId, dto, userId, userName);
            
            if (!result.Success)
            {
                return Results.BadRequest(result);
            }
            
            return Results.Created($"/api/employees/{employeeId}/contracts/{result.Data!.Id}", result);
        })
        .WithName("CreateEmployeeContract")
        .WithSummary("Tạo hợp đồng mới cho nhân viên (chỉ được phép 1 hợp đồng đang thực hiện)");

        // PUT /api/employees/{employeeId}/contracts/{contractId}/terminate - Kết thúc hợp đồng
        group.MapPut("/{contractId:guid}/terminate", async (
            Guid employeeId, 
            Guid contractId, 
            TerminateContractDto? dto,
            IEmployeeContractService service,
            HttpContext httpContext) =>
        {
            // TODO: Lấy userId và userName từ JWT claims
            Guid? userId = null;
            string userName = "Hệ thống";
            
            var userIdClaim = httpContext.User.FindFirst("sub") ?? httpContext.User.FindFirst("userId");
            var userNameClaim = httpContext.User.FindFirst("name") ?? httpContext.User.FindFirst("fullName");
            
            if (userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var parsedUserId))
            {
                userId = parsedUserId;
            }
            if (userNameClaim != null)
            {
                userName = userNameClaim.Value;
            }

            var result = await service.TerminateContractAsync(employeeId, contractId, dto, userId, userName);
            
            if (!result.Success)
            {
                return Results.BadRequest(result);
            }
            
            return Results.Ok(result);
        })
        .WithName("TerminateEmployeeContract")
        .WithSummary("Kết thúc hợp đồng đang thực hiện (EndDate = ngày hôm qua)");
    }
}
