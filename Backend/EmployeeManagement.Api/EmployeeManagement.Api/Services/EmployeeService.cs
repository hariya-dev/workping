// Services/EmployeeService.cs
// Implementation dịch vụ quản lý nhân viên

using EmployeeManagement.Api.Configurations;
using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EmployeeManagement.Api.Services;

/// <summary>
/// Service quản lý nhân viên
/// Xử lý CRUD và upload file
/// </summary>
public class EmployeeService : IEmployeeService
{
    private readonly AppDbContext _context;
    private readonly SystemDefaults _systemDefaults;
    private readonly ILogger<EmployeeService> _logger;
    private readonly IWebHostEnvironment _environment;
    private readonly IEmployeeHistoryService _historyService;

    public EmployeeService(
        AppDbContext context,
        IOptions<SystemDefaults> systemDefaults,
        ILogger<EmployeeService> logger,
        IWebHostEnvironment environment,
        IEmployeeHistoryService historyService)
    {
        _context = context;
        _systemDefaults = systemDefaults.Value;
        _logger = logger;
        _environment = environment;
        _historyService = historyService;
    }

    /// <summary>
    /// Lấy danh sách nhân viên với bộ lọc và phân trang
    /// </summary>
    public async Task<PagedResult<EmployeeDto>> GetEmployeesAsync(EmployeeFilterDto filter)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        
        // Bắt đầu query với Include để lấy thông tin liên quan
        var query = _context.Employees
            .Include(e => e.Files)
            .Include(e => e.Contracts)
                .ThenInclude(c => c.ContractType)
            .AsQueryable();

        // Áp dụng bộ lọc tìm kiếm theo tên
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.ToLower();
            query = query.Where(e => 
                e.FullName.ToLower().Contains(searchTerm) ||
                (e.Email != null && e.Email.ToLower().Contains(searchTerm)) ||
                (e.PhoneNumber != null && e.PhoneNumber.Contains(searchTerm)));
        }

        // Lọc theo trạng thái
        if (filter.Status.HasValue)
        {
            query = query.Where(e => e.Status == filter.Status.Value);
        }

        // Lọc theo loại hợp đồng (dựa trên hợp đồng đang active)
        if (filter.ContractTypeId.HasValue)
        {
            query = query.Where(e => e.Contracts.Any(c => 
                c.ContractTypeId == filter.ContractTypeId.Value && 
                (c.EndDate == null || c.EndDate >= today)));
        }

        // Lọc theo khoảng ngày kết thúc thử việc
        if (filter.ProbationEndDateFrom.HasValue)
        {
            query = query.Where(e => e.ProbationEndDate >= filter.ProbationEndDateFrom.Value);
        }
        if (filter.ProbationEndDateTo.HasValue)
        {
            query = query.Where(e => e.ProbationEndDate <= filter.ProbationEndDateTo.Value);
        }

        // Lọc theo khoảng ngày kết thúc hợp đồng (dựa trên hợp đồng đang active)
        if (filter.ContractEndDateFrom.HasValue)
        {
            query = query.Where(e => e.Contracts.Any(c => 
                (c.EndDate == null || c.EndDate >= today) && 
                c.EndDate >= filter.ContractEndDateFrom.Value));
        }
        if (filter.ContractEndDateTo.HasValue)
        {
            query = query.Where(e => e.Contracts.Any(c => 
                (c.EndDate == null || c.EndDate >= today) && 
                c.EndDate <= filter.ContractEndDateTo.Value));
        }

        // Đếm tổng số record
        var totalCount = await query.CountAsync();

        // Phân trang và sắp xếp
        var employees = await query
            .OrderByDescending(e => e.CreatedAt)
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        var items = employees.Select(e => MapToDto(e, today)).ToList();

        return new PagedResult<EmployeeDto>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    /// <summary>
    /// Lấy thông tin chi tiết một nhân viên
    /// </summary>
    public async Task<EmployeeDto?> GetEmployeeByIdAsync(Guid id)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        
        var employee = await _context.Employees
            .Include(e => e.Files)
            .Include(e => e.Contracts)
                .ThenInclude(c => c.ContractType)
            .FirstOrDefaultAsync(e => e.Id == id);

        return employee == null ? null : MapToDto(employee, today);
    }

    /// <summary>
    /// Tạo nhân viên mới
    /// Hợp đồng sẽ được thêm riêng qua API Contract
    /// </summary>
    public async Task<ApiResult<EmployeeDto>> CreateEmployeeAsync(CreateEmployeeDto dto)
    {
        try
        {
            var employee = new Employee
            {
                Id = Guid.NewGuid(),
                FullName = dto.FullName,
                Email = dto.Email,
                PhoneNumber = dto.PhoneNumber,
                DateOfBirth = dto.DateOfBirth,
                ProbationStartDate = dto.ProbationStartDate,
                ProbationEndDate = dto.ProbationEndDate,
                Status = dto.Status,
                Department = dto.Department,
                Position = dto.Position,
                Notes = dto.Notes,
                CreatedAt = DateTime.UtcNow
            };

            // Tự động tính ngày kết thúc thử việc nếu chưa có
            if (employee.ProbationStartDate.HasValue && !employee.ProbationEndDate.HasValue)
            {
                employee.ProbationEndDate = employee.ProbationStartDate.Value.AddDays(_systemDefaults.DefaultProbationDays);
            }

            _context.Employees.Add(employee);
            await _context.SaveChangesAsync();

            // Ghi log lịch sử tạo mới
            // TODO: Lấy userId và userName từ HttpContext khi có authentication
            await _historyService.TrackCreateAsync(employee.Id, employee, null, "Hệ thống");

            _logger.LogInformation("Tạo nhân viên mới: {EmployeeId} - {FullName}", employee.Id, employee.FullName);

            // Load lại để lấy thông tin đầy đủ
            var result = await GetEmployeeByIdAsync(employee.Id);
            return ApiResult<EmployeeDto>.Ok(result!, "Tạo nhân viên thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tạo nhân viên: {FullName}", dto.FullName);
            return ApiResult<EmployeeDto>.Fail("Lỗi khi tạo nhân viên");
        }
    }

    /// <summary>
    /// Cập nhật thông tin nhân viên
    /// Hợp đồng được quản lý riêng qua API Contract
    /// </summary>
    public async Task<ApiResult<EmployeeDto>> UpdateEmployeeAsync(Guid id, UpdateEmployeeDto dto)
    {
        try
        {
            var employee = await _context.Employees.FindAsync(id);
            if (employee == null)
            {
                return ApiResult<EmployeeDto>.Fail("Không tìm thấy nhân viên");
            }

            // Lưu snapshot trước khi update để so sánh
            var oldEmployee = new Employee
            {
                FullName = employee.FullName,
                Email = employee.Email,
                PhoneNumber = employee.PhoneNumber,
                DateOfBirth = employee.DateOfBirth,
                ProbationStartDate = employee.ProbationStartDate,
                ProbationEndDate = employee.ProbationEndDate,
                Status = employee.Status,
                Department = employee.Department,
                Position = employee.Position,
                Notes = employee.Notes
            };

            // Cập nhật thông tin
            employee.FullName = dto.FullName;
            employee.Email = dto.Email;
            employee.PhoneNumber = dto.PhoneNumber;
            employee.DateOfBirth = dto.DateOfBirth;
            employee.ProbationStartDate = dto.ProbationStartDate;
            employee.ProbationEndDate = dto.ProbationEndDate;
            employee.Status = dto.Status;
            employee.Department = dto.Department;
            employee.Position = dto.Position;
            employee.Notes = dto.Notes;
            employee.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            // Ghi log lịch sử thay đổi
            // TODO: Lấy userId và userName từ HttpContext khi có authentication
            await _historyService.TrackUpdateAsync(id, oldEmployee, employee, null, "Hệ thống");

            _logger.LogInformation("Cập nhật nhân viên: {EmployeeId} - {FullName}", id, dto.FullName);

            var result = await GetEmployeeByIdAsync(id);
            return ApiResult<EmployeeDto>.Ok(result!, "Cập nhật thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật nhân viên: {EmployeeId}", id);
            return ApiResult<EmployeeDto>.Fail("Lỗi khi cập nhật nhân viên");
        }
    }

    /// <summary>
    /// Xóa nhân viên (và các file liên quan)
    /// </summary>
    public async Task<ApiResult> DeleteEmployeeAsync(Guid id)
    {
        try
        {
            var employee = await _context.Employees
                .Include(e => e.Files)
                .FirstOrDefaultAsync(e => e.Id == id);

            if (employee == null)
            {
                return ApiResult.Fail("Không tìm thấy nhân viên");
            }

            // Xóa các file vật lý
            var uploadDir = Path.Combine(_environment.WebRootPath, "uploads", "employees", id.ToString());
            if (Directory.Exists(uploadDir))
            {
                Directory.Delete(uploadDir, true);
            }

            // Xóa record (cascade sẽ xóa EmployeeFiles)
            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Xóa nhân viên: {EmployeeId} - {FullName}", id, employee.FullName);

            return ApiResult.Ok("Xóa nhân viên thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xóa nhân viên: {EmployeeId}", id);
            return ApiResult.Fail("Lỗi khi xóa nhân viên");
        }
    }

    /// <summary>
    /// Upload file cho nhân viên
    /// </summary>
    public async Task<ApiResult<EmployeeFileDto>> UploadFileAsync(Guid employeeId, IFormFile file, string? description)
    {
        try
        {
            // Kiểm tra nhân viên tồn tại
            var employee = await _context.Employees.FindAsync(employeeId);
            if (employee == null)
            {
                return ApiResult<EmployeeFileDto>.Fail("Không tìm thấy nhân viên");
            }

            // Kiểm tra kích thước file
            var maxSizeBytes = _systemDefaults.MaxFileSizeMB * 1024 * 1024;
            if (file.Length > maxSizeBytes)
            {
                return ApiResult<EmployeeFileDto>.Fail($"File vượt quá giới hạn {_systemDefaults.MaxFileSizeMB}MB");
            }

            // Kiểm tra đuôi file
            var extension = Path.GetExtension(file.FileName).ToLower();
            if (!_systemDefaults.AllowedFileExtensions.Contains(extension))
            {
                return ApiResult<EmployeeFileDto>.Fail($"Định dạng file không được phép. Chỉ chấp nhận: {string.Join(", ", _systemDefaults.AllowedFileExtensions)}");
            }

            // Tạo thư mục lưu file
            var uploadDir = Path.Combine(_environment.WebRootPath, "uploads", "employees", employeeId.ToString());
            if (!Directory.Exists(uploadDir))
            {
                Directory.CreateDirectory(uploadDir);
            }

            // Tạo tên file unique để tránh trùng
            var storedFileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadDir, storedFileName);
            var relativePath = $"uploads/employees/{employeeId}/{storedFileName}";

            // Lưu file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Tạo record trong DB
            var employeeFile = new EmployeeFile
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                OriginalFileName = file.FileName,
                StoredFileName = storedFileName,
                FilePath = relativePath,
                FileExtension = extension,
                FileSize = file.Length,
                ContentType = file.ContentType,
                Description = description,
                UploadedAt = DateTime.UtcNow
            };

            _context.EmployeeFiles.Add(employeeFile);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Upload file thành công: {FileId} cho nhân viên {EmployeeId}", employeeFile.Id, employeeId);

            return ApiResult<EmployeeFileDto>.Ok(new EmployeeFileDto
            {
                Id = employeeFile.Id,
                OriginalFileName = employeeFile.OriginalFileName,
                FilePath = employeeFile.FilePath,
                FileExtension = employeeFile.FileExtension,
                FileSize = employeeFile.FileSize,
                Description = employeeFile.Description,
                UploadedAt = employeeFile.UploadedAt
            }, "Upload file thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi upload file cho nhân viên: {EmployeeId}", employeeId);
            return ApiResult<EmployeeFileDto>.Fail("Lỗi khi upload file");
        }
    }

    /// <summary>
    /// Xóa file của nhân viên
    /// </summary>
    public async Task<ApiResult> DeleteFileAsync(Guid employeeId, Guid fileId)
    {
        try
        {
            var file = await _context.EmployeeFiles
                .FirstOrDefaultAsync(f => f.Id == fileId && f.EmployeeId == employeeId);

            if (file == null)
            {
                return ApiResult.Fail("Không tìm thấy file");
            }

            // Xóa file vật lý
            var fullPath = Path.Combine(_environment.WebRootPath, file.FilePath);
            if (File.Exists(fullPath))
            {
                File.Delete(fullPath);
            }

            // Xóa record
            _context.EmployeeFiles.Remove(file);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Xóa file: {FileId} của nhân viên {EmployeeId}", fileId, employeeId);

            return ApiResult.Ok("Xóa file thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xóa file: {FileId}", fileId);
            return ApiResult.Fail("Lỗi khi xóa file");
        }
    }

    /// <summary>
    /// Lấy danh sách file của nhân viên
    /// </summary>
    public async Task<List<EmployeeFileDto>> GetEmployeeFilesAsync(Guid employeeId)
    {
        return await _context.EmployeeFiles
            .Where(f => f.EmployeeId == employeeId)
            .OrderByDescending(f => f.UploadedAt)
            .Select(f => new EmployeeFileDto
            {
                Id = f.Id,
                OriginalFileName = f.OriginalFileName,
                FilePath = f.FilePath,
                FileExtension = f.FileExtension,
                FileSize = f.FileSize,
                Description = f.Description,
                UploadedAt = f.UploadedAt
            })
            .ToListAsync();
    }

    /// <summary>
    /// Chuyển đổi Employee entity sang DTO
    /// </summary>
    private static EmployeeDto MapToDto(Employee e, DateOnly today)
    {
        // Tìm hợp đồng đang active (EndDate null hoặc >= today)
        var activeContract = e.Contracts?
            .Where(c => c.EndDate == null || c.EndDate >= today)
            .OrderByDescending(c => c.StartDate)
            .FirstOrDefault();

        return new EmployeeDto
        {
            Id = e.Id,
            FullName = e.FullName,
            Email = e.Email,
            PhoneNumber = e.PhoneNumber,
            DateOfBirth = e.DateOfBirth,
            ProbationStartDate = e.ProbationStartDate,
            ProbationEndDate = e.ProbationEndDate,
            Status = e.Status,
            StatusDisplayName = GetStatusDisplayName(e.Status),
            Department = e.Department,
            Position = e.Position,
            Notes = e.Notes,
            CreatedAt = e.CreatedAt,
            UpdatedAt = e.UpdatedAt,
            ActiveContract = activeContract != null ? new EmployeeContractDto
            {
                Id = activeContract.Id,
                EmployeeId = activeContract.EmployeeId,
                ContractTypeId = activeContract.ContractTypeId,
                ContractTypeName = activeContract.ContractType?.Name ?? "Không xác định",
                DurationMonths = activeContract.ContractType?.DurationMonths,
                StartDate = activeContract.StartDate,
                EndDate = activeContract.EndDate,
                Status = activeContract.Status,
                Notes = activeContract.Notes,
                CreatedAt = activeContract.CreatedAt
            } : null,
            Files = e.Files?.Select(f => new EmployeeFileDto
            {
                Id = f.Id,
                OriginalFileName = f.OriginalFileName,
                FilePath = f.FilePath,
                FileExtension = f.FileExtension,
                FileSize = f.FileSize,
                Description = f.Description,
                UploadedAt = f.UploadedAt
            }).ToList() ?? new List<EmployeeFileDto>()
        };
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
