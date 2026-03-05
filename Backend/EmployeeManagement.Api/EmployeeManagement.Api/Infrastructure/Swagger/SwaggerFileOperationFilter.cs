// Infrastructure/Swagger/SwaggerFileOperationFilter.cs
// Custom OperationFilter để xử lý file upload trong Swagger

using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Reflection;

namespace EmployeeManagement.Api.Infrastructure.Swagger
{
    /// <summary>
    /// OperationFilter để xử lý file upload parameters trong Swagger
    /// </summary>
    public class SwaggerFileOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var fileUploadParams = context.MethodInfo.GetParameters()
                .Where(p => p.ParameterType == typeof(IFormFile) || 
                           (p.ParameterType.IsGenericType && 
                            p.ParameterType.GetGenericTypeDefinition() == typeof(IFormFileCollection)))
                .ToList();

            if (!fileUploadParams.Any()) return;

            // Xóa parameters cũ
            operation.Parameters?.Clear();

            // Thêm request body cho file upload
            operation.RequestBody = new OpenApiRequestBody
            {
                Content = new Dictionary<string, OpenApiMediaType>
                {
                    ["multipart/form-data"] = new OpenApiMediaType
                    {
                        Schema = new OpenApiSchema
                        {
                            Type = "object",
                            Properties = fileUploadParams.ToDictionary(
                                p => p.Name!,
                                p => new OpenApiSchema
                                {
                                    Type = "string",
                                    Format = "binary"
                                }
                            )
                        }
                    }
                }
            };
        }
    }
}