// DTOs/ImportDTOs.cs
// DTOs cho chức năng import nhân viên từ CSV

using System.ComponentModel.DataAnnotations;

namespace EmployeeManagement.Api.DTOs
{
    // Record từ file CSV
    public class CsvEmployeeRecord
    {
        public int Id { get; set; }
        public string Ten { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? SoDienThoai { get; set; }
        public string? DiaChi { get; set; }
        public string? NgayVaoLam { get; set; }
        public string NgaySinh { get; set; } = string.Empty;
        public string NgayLamViecChinhThuc { get; set; } = string.Empty;
        public string? NgayKetThucThuViec { get; set; }
        public string? NgayKetThucHopDong { get; set; }
        public string LoaiHopDong { get; set; } = string.Empty;
        public int SoThangHopDong { get; set; }
        public int IsDeleted { get; set; }
        public string? NgayNghiViec { get; set; }
    }

    // Kết quả import
    public class ImportResult
    {
        public int TotalRecords { get; set; }
        public int SuccessCount { get; set; }
        public int FailedCount { get; set; }
        public List<string> Errors { get; set; } = new();
        public List<ImportedEmployeeSummary> ImportedEmployees { get; set; } = new();
    }

    public class ImportedEmployeeSummary
    {
        public string EmployeeName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
    }

    // Request import
    public class ImportEmployeeRequest
    {
        [Required]
        public IFormFile File { get; set; } = null!;
    }
}