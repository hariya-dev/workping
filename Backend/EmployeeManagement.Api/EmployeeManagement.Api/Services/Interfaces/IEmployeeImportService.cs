// Services/Interfaces/IEmployeeImportService.cs
// Interface cho service import nhân viên

using EmployeeManagement.Api.DTOs;

namespace EmployeeManagement.Api.Services.Interfaces
{
    public interface IEmployeeImportService
    {
        Task<ImportResult> ImportFromCsvAsync(IFormFile file);
    }
}