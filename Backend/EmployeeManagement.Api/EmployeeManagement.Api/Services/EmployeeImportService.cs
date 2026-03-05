using CsvHelper;
using CsvHelper.Configuration;
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Entities;
using EmployeeManagement.Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Globalization;

namespace EmployeeManagement.Api.Services
{
    public class EmployeeImportService : IEmployeeImportService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<EmployeeImportService> _logger;

        // Hardcoded contract type IDs từ yêu cầu
        private const string ONE_YEAR_CONTRACT_ID = "93ca3b77-d150-47de-818b-c4e1beb97ddb";
        private const string NO_LIMIT_CONTRACT_ID = "aed1e27b-ea9b-4de5-96f6-1e34946c4caf";

        public EmployeeImportService(AppDbContext context, ILogger<EmployeeImportService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ImportResult> ImportFromCsvAsync(IFormFile file)
        {
            var result = new ImportResult();
            
            try
            {
                var records = await ParseCsvFileAsync(file);
                result.TotalRecords = records.Count;

                foreach (var record in records)
                {
                    try
                    {
                        var employee = await CreateEmployeeFromRecordAsync(record);
                        await _context.Employees.AddAsync(employee);
                        await _context.SaveChangesAsync();

                        result.SuccessCount++;
                        result.ImportedEmployees.Add(new ImportedEmployeeSummary
                        {
                            EmployeeName = employee.FullName,
                            Status = "Success",
                            Message = $"Import thành công nhân viên {employee.FullName}"
                        });

                        _logger.LogInformation("Imported employee: {Name}", employee.FullName);
                    }
                    catch (Exception ex)
                    {
                        result.FailedCount++;
                        var errorMessage = $"Lỗi khi import nhân viên {record.Ten}: {ex.Message}";
                        result.Errors.Add(errorMessage);
                        result.ImportedEmployees.Add(new ImportedEmployeeSummary
                        {
                            EmployeeName = record.Ten,
                            Status = "Failed",
                            Message = errorMessage
                        });

                        _logger.LogError(ex, "Error importing employee: {Name}", record.Ten);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error parsing CSV file");
                result.Errors.Add($"Lỗi khi đọc file CSV: {ex.Message}");
            }

            return result;
        }

        private async Task<List<CsvEmployeeRecord>> ParseCsvFileAsync(IFormFile file)
        {
            var records = new List<CsvEmployeeRecord>();

            using var stream = file.OpenReadStream();
            using var reader = new StreamReader(stream);
            using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                HasHeaderRecord = true,
                Delimiter = ",",
                MissingFieldFound = null,
                BadDataFound = null
            });

            await csv.ReadAsync();
            csv.ReadHeader();

            while (await csv.ReadAsync())
            {
                var record = new CsvEmployeeRecord
                {
                    Id = csv.GetField<int>("Id"),
                    Ten = csv.GetField("Ten") ?? "",
                    Email = csv.GetField("Email"),
                    SoDienThoai = csv.GetField("SoDienThoai"),
                    DiaChi = csv.GetField("DiaChi"),
                    NgayVaoLam = csv.GetField("NgayVaoLam"),
                    NgaySinh = csv.GetField("NgaySinh") ?? "",
                    NgayLamViecChinhThuc = csv.GetField("NgayLamViecChinhThuc") ?? "",
                    NgayKetThucThuViec = csv.GetField("NgayKetThucThuViec"),
                    NgayKetThucHopDong = csv.GetField("NgayKetThucHopDong"),
                    LoaiHopDong = csv.GetField("LoaiHopDong") ?? "",
                    SoThangHopDong = csv.GetField<int>("SoThangHopDong"),
                    IsDeleted = csv.GetField<int>("IsDeleted"),
                    NgayNghiViec = csv.GetField("NgayNghiViec")
                };

                records.Add(record);
            }

            return records;
        }

        private async Task<Employee> CreateEmployeeFromRecordAsync(CsvEmployeeRecord record)
        {
            // Xác định trạng thái nhân viên
            var status = EmployeeStatus.Active;
            if (record.IsDeleted == 1 || !string.IsNullOrEmpty(record.NgayNghiViec))
            {
                status = EmployeeStatus.Resigned;
            }

            // Helper method để parse date an toàn
            DateOnly? ParseDate(string dateString)
            {
                if (string.IsNullOrWhiteSpace(dateString) || 
                    dateString.Equals("NULL", StringComparison.OrdinalIgnoreCase) ||
                    dateString.Equals("null", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
                
                if (DateTime.TryParse(dateString, out DateTime date))
                {
                    return DateOnly.FromDateTime(date);
                }
                
                return null;
            }

            var employee = new Employee
            {
                FullName = record.Ten.Trim(),
                Email = string.IsNullOrWhiteSpace(record.Email) ? null : record.Email.Trim(),
                PhoneNumber = string.IsNullOrWhiteSpace(record.SoDienThoai) ? null : record.SoDienThoai.Trim(),
                Department = string.IsNullOrWhiteSpace(record.DiaChi) ? null : record.DiaChi.Trim(),
                DateOfBirth = ParseDate(record.NgaySinh) ?? DateOnly.MinValue, // Cần có giá trị mặc định
                ProbationStartDate = ParseDate(record.NgayVaoLam),
                ProbationEndDate = ParseDate(record.NgayKetThucThuViec),
                Status = status,
                CreatedAt = DateTime.Now
            };

            // Tạo hợp đồng nếu có thông tin
            if (!string.IsNullOrEmpty(record.LoaiHopDong) && 
                !record.LoaiHopDong.Equals("NULL", StringComparison.OrdinalIgnoreCase))
            {
                var contract = await CreateContractFromRecordAsync(record, employee.Id);
                employee.Contracts.Add(contract);
            }

            return employee;
        }

        private async Task<EmployeeContract> CreateContractFromRecordAsync(CsvEmployeeRecord record, Guid employeeId)
        {
            string contractTypeId;
            
            // Mapping loại hợp đồng
            if (record.LoaiHopDong.Equals("vothoihan", StringComparison.OrdinalIgnoreCase))
            {
                contractTypeId = NO_LIMIT_CONTRACT_ID;
            }
            else if (record.LoaiHopDong.Equals("1nam", StringComparison.OrdinalIgnoreCase))
            {
                contractTypeId = ONE_YEAR_CONTRACT_ID;
            }
            else
            {
                throw new ArgumentException($"Loại hợp đồng không hợp lệ: {record.LoaiHopDong}");
            }

            // Kiểm tra contract type tồn tại
            var contractTypeExists = await _context.ContractTypes.AnyAsync(ct => ct.Id.ToString() == contractTypeId);
            if (!contractTypeExists)
            {
                throw new InvalidOperationException($"Contract type ID {contractTypeId} không tồn tại trong hệ thống");
            }

            // Helper method để parse date an toàn
            DateOnly? ParseDate(string dateString)
            {
                if (string.IsNullOrWhiteSpace(dateString) || 
                    dateString.Equals("NULL", StringComparison.OrdinalIgnoreCase) ||
                    dateString.Equals("null", StringComparison.OrdinalIgnoreCase))
                {
                    return null;
                }
                
                if (DateTime.TryParse(dateString, out DateTime date))
                {
                    return DateOnly.FromDateTime(date);
                }
                
                return null;
            }

            // Helper method để tính ngày kết thúc hợp đồng
            DateOnly? CalculateContractEndDate(DateOnly startDate, string contractType)
            {
                if (contractType.Equals("1nam", StringComparison.OrdinalIgnoreCase))
                {
                    // Hợp đồng 1 năm: cộng thêm 1 năm
                    return startDate.AddYears(1);
                }
                else if (contractType.Equals("vothoihan", StringComparison.OrdinalIgnoreCase))
                {
                    // Hợp đồng vô thời hạn: null
                    return null;
                }
                return null;
            }

            var contract = new EmployeeContract
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                ContractTypeId = Guid.Parse(contractTypeId),
                StartDate = ParseDate(record.NgayLamViecChinhThuc) ?? DateOnly.FromDateTime(DateTime.Today),
                CreatedAt = DateTime.Now,
                CreatedBy = null
            };

            // Tự động tính ngày kết thúc dựa trên loại hợp đồng
            var contractStartDate = contract.StartDate;
            var calculatedEndDate = CalculateContractEndDate(contractStartDate, record.LoaiHopDong);
            
            if (calculatedEndDate.HasValue)
            {
                contract.EndDate = calculatedEndDate.Value;
            }
            else
            {
                // Nếu không có ngày kết thúc được tính toán, kiểm tra trong record
                var endDate = ParseDate(record.NgayKetThucHopDong);
                if (endDate.HasValue)
                {
                    contract.EndDate = endDate.Value;
                }
            }

            return contract;
        }
    }
}