// Endpoints/EmailTemplateEndpoints.cs
// API endpoints cho quản lý template email

using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Entities;
using EmployeeManagement.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace EmployeeManagement.Api.Endpoints;

/// <summary>
/// Extension methods cho Email Template endpoints
/// </summary>
public static class EmailTemplateEndpoints
{
    public static void MapEmailTemplateEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/email-templates")
            .WithTags("Email Templates")
            .RequireAuthorization();

        // GET: /api/email-templates - Lấy tất cả templates
        group.MapGet("/", async (IEmailTemplateService service) =>
        {
            var templates = await service.GetAllAsync();
            return Results.Ok(ApiResult<List<EmailTemplateSummaryDto>>.Ok(templates));
        })
        .WithName("GetAllEmailTemplates")
        .WithDescription("Lấy danh sách tất cả email templates")
        .Produces<ApiResult<List<EmailTemplateSummaryDto>>>(200);

        // GET: /api/email-templates/placeholders - Lấy danh sách placeholders có sẵn
        group.MapGet("/placeholders", async (IEmailTemplateService service) =>
        {
            var placeholders = await service.GetAvailablePlaceholdersAsync();
            return Results.Ok(ApiResult<List<TemplatePlaceholderInfoDto>>.Ok(placeholders));
        })
        .WithName("GetTemplatePlaceholders")
        .WithDescription("Lấy danh sách các placeholder có sẵn cho từng loại template")
        .Produces<ApiResult<List<TemplatePlaceholderInfoDto>>>(200);

        // GET: /api/email-templates/{id} - Lấy template theo ID
        group.MapGet("/{id:guid}", async (Guid id, IEmailTemplateService service) =>
        {
            var template = await service.GetByIdAsync(id);
            return template == null 
                ? Results.NotFound(ApiResult<EmailTemplateDto>.Fail("Template không tồn tại")) 
                : Results.Ok(ApiResult<EmailTemplateDto>.Ok(template));
        })
        .WithName("GetEmailTemplateById")
        .WithDescription("Lấy chi tiết email template theo ID")
        .Produces<ApiResult<EmailTemplateDto>>(200)
        .Produces(404);

        // GET: /api/email-templates/type/{type} - Lấy template theo loại
        group.MapGet("/type/{type:int}", async (EmailTemplateType type, IEmailTemplateService service) =>
        {
            var template = await service.GetByTypeAsync(type);
            return template == null 
                ? Results.NotFound(ApiResult<EmailTemplateDto>.Fail("Template loại này không tồn tại")) 
                : Results.Ok(ApiResult<EmailTemplateDto>.Ok(template));
        })
        .WithName("GetEmailTemplateByType")
        .WithDescription("Lấy email template theo loại")
        .Produces<ApiResult<EmailTemplateDto>>(200)
        .Produces(404);

        // POST: /api/email-templates - Tạo template mới
        group.MapPost("/", async (
            UpsertEmailTemplateDto dto, 
            IEmailTemplateService service,
            ClaimsPrincipal user) =>
        {
            var result = await service.UpsertAsync(null, dto);
            if (!result.Success)
            {
                return Results.BadRequest(ApiResult<UpsertResultDto>.Fail(result.Message));
            }
            return Results.Created($"/api/email-templates/{result.Data!.Id}", ApiResult<UpsertResultDto>.Ok(result.Data));
        })
        .WithName("CreateEmailTemplate")
        .WithDescription("Tạo email template mới")
        .Produces<ApiResult<UpsertResultDto>>(201)
        .Produces(400);

        // PUT: /api/email-templates/{id} - Cập nhật template
        group.MapPut("/{id:guid}", async (
            Guid id, 
            UpsertEmailTemplateDto dto, 
            IEmailTemplateService service,
            ClaimsPrincipal user) =>
        {
            var result = await service.UpsertAsync(id, dto);
            if (!result.Success)
            {
                return Results.BadRequest(ApiResult<UpsertResultDto>.Fail(result.Message));
            }
            return Results.Ok(ApiResult<UpsertResultDto>.Ok(result.Data));
        })
        .WithName("UpdateEmailTemplate")
        .WithDescription("Cập nhật email template")
        .Produces<ApiResult<UpsertResultDto>>(200)
        .Produces(400)
        .Produces(404);

        // POST: /api/email-templates/{id}/reset - Reset template về mặc định
        group.MapPost("/{id:guid}/reset", async (Guid id, IEmailTemplateService service) =>
        {
            var result = await service.ResetToDefaultAsync(id);
            if (!result.Success)
            {
                return Results.BadRequest(ApiResult.Fail(result.Message));
            }
            return Results.Ok(ApiResult.Ok(result.Message));
        })
        .WithName("ResetEmailTemplate")
        .WithDescription("Reset email template về nội dung mặc định")
        .Produces<ApiResult>(200)
        .Produces(400);

        // POST: /api/email-templates/preview - Preview template
        group.MapPost("/preview", async (PreviewTemplateDto dto, IEmailTemplateService service) =>
        {
            var result = await service.PreviewAsync(dto);
            return Results.Ok(ApiResult<TemplatePreviewResultDto>.Ok(result));
        })
        .WithName("PreviewEmailTemplate")
        .WithDescription("Xem trước email template với dữ liệu mẫu")
        .Produces<ApiResult<TemplatePreviewResultDto>>(200);
    }
}
