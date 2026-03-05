// Endpoints/ImportEndpoints.cs
// API endpoints cho chức năng import nhân viên

using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeManagement.Api.Endpoints
{
    public static class ImportEndpoints
    {
        public static void MapImportEndpoints(this WebApplication app)
        {
            var group = app.MapGroup("/api/import")
                          .WithTags("Import")
                          .RequireAuthorization();

            // Import employees from CSV
            group.MapPost("/employees", async (
                [FromForm] IFormFile file,
                IEmployeeImportService importService,
                ILogger<Program> logger) =>
            {
                if (file == null || file.Length == 0)
                {
                    return Results.BadRequest(new { Message = "Vui lòng chọn file CSV để import" });
                }

                if (!file.FileName.EndsWith(".csv", StringComparison.OrdinalIgnoreCase))
                {
                    return Results.BadRequest(new { Message = "Chỉ chấp nhận file CSV" });
                }

                try
                {
                    logger.LogInformation("Bắt đầu import file: {FileName}", file.FileName);
                    
                    var result = await importService.ImportFromCsvAsync(file);
                    
                    logger.LogInformation("Import hoàn thành. Thành công: {Success}, Thất bại: {Failed}", 
                        result.SuccessCount, result.FailedCount);
                    
                    return Results.Ok(new
                    {
                        Message = "Import hoàn thành",
                        TotalRecords = result.TotalRecords,
                        SuccessCount = result.SuccessCount,
                        FailedCount = result.FailedCount,
                        Errors = result.Errors,
                        ImportedEmployees = result.ImportedEmployees
                    });
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "Lỗi khi import file: {FileName}", file.FileName);
                    return Results.Problem($"Lỗi server: {ex.Message}", statusCode: 500);
                }
            })
            .DisableAntiforgery() // Cho phép upload file
            .WithName("ImportEmployees")
            .WithSummary("Import nhân viên từ file CSV")
            .WithDescription("Import dữ liệu nhân viên từ file CSV của hệ thống cũ. File phải có định dạng CSV với các cột: Id, Ten, Email, SoDienThoai, DiaChi, NgaySinh, NgayLamViecChinhThuc, LoaiHopDong, v.v.")
            .Produces<ImportResult>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status500InternalServerError);
        }
    }
}