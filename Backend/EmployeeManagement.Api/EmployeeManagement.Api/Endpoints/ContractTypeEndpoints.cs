// Endpoints/ContractTypeEndpoints.cs
// Minimal API endpoints cho quản lý loại hợp đồng

using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Services;

namespace EmployeeManagement.Api.Endpoints;

/// <summary>
/// Định nghĩa các API endpoints cho Contract Type (Master Data)
/// </summary>
public static class ContractTypeEndpoints
{
    /// <summary>
    /// Map các routes cho Contract Type
    /// </summary>
    public static void MapContractTypeEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/contract-types")
            .WithTags("Contract Types")
            .RequireAuthorization();

        // GET /api/contract-types - Lấy tất cả loại hợp đồng
        group.MapGet("/", async (bool? activeOnly, IContractTypeService service) =>
        {
            var result = await service.GetAllAsync(activeOnly ?? true);
            return Results.Ok(ApiResult<List<ContractTypeDto>>.Ok(result));
        })
        .WithName("GetContractTypes")
        .WithSummary("Lấy danh sách loại hợp đồng");

        // GET /api/contract-types/{id} - Lấy chi tiết loại hợp đồng
        group.MapGet("/{id:guid}", async (Guid id, IContractTypeService service) =>
        {
            var result = await service.GetByIdAsync(id);
            
            if (result == null)
            {
                return Results.NotFound(ApiResult<ContractTypeDto>.Fail("Không tìm thấy loại hợp đồng"));
            }
            
            return Results.Ok(ApiResult<ContractTypeDto>.Ok(result));
        })
        .WithName("GetContractTypeById")
        .WithSummary("Lấy thông tin chi tiết loại hợp đồng");

        // POST /api/contract-types - Tạo loại hợp đồng mới
        group.MapPost("/", async (CreateContractTypeDto dto, IContractTypeService service) =>
        {
            var result = await service.CreateAsync(dto);
            
            if (!result.Success)
            {
                return Results.BadRequest(result);
            }
            
            return Results.Created($"/api/contract-types/{result.Data!.Id}", result);
        })
        .WithName("CreateContractType")
        .WithSummary("Tạo loại hợp đồng mới");

        // PUT /api/contract-types/{id} - Cập nhật loại hợp đồng
        group.MapPut("/{id:guid}", async (Guid id, UpdateContractTypeDto dto, IContractTypeService service) =>
        {
            var result = await service.UpdateAsync(id, dto);
            
            if (!result.Success)
            {
                return Results.BadRequest(result);
            }
            
            return Results.Ok(result);
        })
        .WithName("UpdateContractType")
        .WithSummary("Cập nhật loại hợp đồng");

        // DELETE /api/contract-types/{id} - Xóa loại hợp đồng
        group.MapDelete("/{id:guid}", async (Guid id, IContractTypeService service) =>
        {
            var result = await service.DeleteAsync(id);
            
            if (!result.Success)
            {
                return Results.BadRequest(result);
            }
            
            return Results.Ok(result);
        })
        .WithName("DeleteContractType")
        .WithSummary("Xóa loại hợp đồng");
    }
}
