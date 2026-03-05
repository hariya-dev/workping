// Services/EmployeeContractService.cs
// Triển khai dịch vụ quản lý hợp đồng nhân viên

using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Services;

/// <summary>
/// Service quản lý hợp đồng nhân viên
/// Hỗ trợ One-to-Many relationship (1 nhân viên có nhiều hợp đồng)
/// </summary>
public class EmployeeContractService : IEmployeeContractService
{
    private readonly AppDbContext _context;
    private readonly IEmployeeHistoryService _historyService;
    private readonly ILogger<EmployeeContractService> _logger;

    public EmployeeContractService(
        AppDbContext context, 
        IEmployeeHistoryService historyService,
        ILogger<EmployeeContractService> logger)
    {
        _context = context;
        _historyService = historyService;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách tất cả hợp đồng của nhân viên
    /// Sắp xếp theo ngày bắt đầu giảm dần (mới nhất trước)
    /// </summary>
    public async Task<List<EmployeeContractDto>> GetContractsByEmployeeIdAsync(Guid employeeId)
    {
        var contracts = await _context.EmployeeContracts
            .Where(c => c.EmployeeId == employeeId)
            .Include(c => c.ContractType)
            .OrderByDescending(c => c.StartDate)
            .ToListAsync();

        return contracts.Select(MapToDto).ToList();
    }

    /// <summary>
    /// Lấy hợp đồng đang thực hiện của nhân viên
    /// </summary>
    public async Task<EmployeeContractDto?> GetActiveContractAsync(Guid employeeId)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        
        var contract = await _context.EmployeeContracts
            .Where(c => c.EmployeeId == employeeId)
            .Where(c => c.EndDate == null || c.EndDate >= today)
            .Include(c => c.ContractType)
            .OrderByDescending(c => c.StartDate)
            .FirstOrDefaultAsync();

        return contract != null ? MapToDto(contract) : null;
    }

    /// <summary>
    /// Tạo hợp đồng mới cho nhân viên
    /// Validation: Chỉ cho phép 1 hợp đồng đang thực hiện
    /// </summary>
    public async Task<ApiResult<EmployeeContractDto>> CreateContractAsync(
        Guid employeeId, 
        CreateEmployeeContractDto dto, 
        Guid? userId, 
        string userName)
    {
        // Kiểm tra nhân viên tồn tại
        var employee = await _context.Employees.FindAsync(employeeId);
        if (employee == null)
        {
            return ApiResult<EmployeeContractDto>.Fail("Không tìm thấy nhân viên");
        }

        // Kiểm tra loại hợp đồng tồn tại
        var contractType = await _context.ContractTypes.FindAsync(dto.ContractTypeId);
        if (contractType == null)
        {
            return ApiResult<EmployeeContractDto>.Fail("Loại hợp đồng không hợp lệ");
        }

        // Kiểm tra không có hợp đồng đang thực hiện
        var today = DateOnly.FromDateTime(DateTime.Today);
        var existingActiveContract = await _context.EmployeeContracts
            .Where(c => c.EmployeeId == employeeId)
            .Where(c => c.EndDate == null || c.EndDate >= today)
            .FirstOrDefaultAsync();

        if (existingActiveContract != null)
        {
            return ApiResult<EmployeeContractDto>.Fail(
                "Nhân viên đã có hợp đồng đang thực hiện. Vui lòng kết thúc hợp đồng hiện tại trước khi tạo hợp đồng mới.");
        }

        // Validate ngày
        if (dto.EndDate.HasValue && dto.EndDate < dto.StartDate)
        {
            return ApiResult<EmployeeContractDto>.Fail("Ngày kết thúc phải sau ngày bắt đầu");
        }

        // Tạo hợp đồng mới
        var contract = new EmployeeContract
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            ContractTypeId = dto.ContractTypeId,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Notes = dto.Notes,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = userId
        };

        _context.EmployeeContracts.Add(contract);
        await _context.SaveChangesAsync();

        // Load ContractType để trả về DTO đầy đủ
        contract.ContractType = contractType;

        // Ghi log lịch sử
        await _historyService.TrackContractCreatedAsync(employeeId, contract, userId, userName);

        _logger.LogInformation("Đã tạo hợp đồng {ContractId} cho nhân viên {EmployeeId}", contract.Id, employeeId);

        return ApiResult<EmployeeContractDto>.Ok(MapToDto(contract));
    }

    /// <summary>
    /// Kết thúc hợp đồng (set EndDate = ngày hôm qua)
    /// </summary>
    public async Task<ApiResult<EmployeeContractDto>> TerminateContractAsync(
        Guid employeeId, 
        Guid contractId, 
        TerminateContractDto? dto, 
        Guid? userId, 
        string userName)
    {
        // Tìm hợp đồng
        var contract = await _context.EmployeeContracts
            .Include(c => c.ContractType)
            .FirstOrDefaultAsync(c => c.Id == contractId && c.EmployeeId == employeeId);

        if (contract == null)
        {
            return ApiResult<EmployeeContractDto>.Fail("Không tìm thấy hợp đồng");
        }

        // Kiểm tra hợp đồng còn đang active không
        if (!contract.IsActive)
        {
            return ApiResult<EmployeeContractDto>.Fail("Hợp đồng đã kết thúc trước đó");
        }

        // Set EndDate = ngày hôm qua để đảm bảo Status = "Đã kết thúc"
        contract.EndDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-1));
        
        // Thêm ghi chú nếu có
        if (!string.IsNullOrEmpty(dto?.Notes))
        {
            contract.Notes = string.IsNullOrEmpty(contract.Notes) 
                ? $"Lý do kết thúc: {dto.Notes}"
                : $"{contract.Notes}\nLý do kết thúc: {dto.Notes}";
        }

        await _context.SaveChangesAsync();

        // Ghi log lịch sử
        await _historyService.TrackContractTerminatedAsync(employeeId, contract, userId, userName);

        _logger.LogInformation("Đã kết thúc hợp đồng {ContractId} của nhân viên {EmployeeId}", contractId, employeeId);

        return ApiResult<EmployeeContractDto>.Ok(MapToDto(contract));
    }

    #region Private Helper Methods

    /// <summary>
    /// Map EmployeeContract entity sang DTO
    /// </summary>
    private static EmployeeContractDto MapToDto(EmployeeContract contract)
    {
        return new EmployeeContractDto
        {
            Id = contract.Id,
            EmployeeId = contract.EmployeeId,
            ContractTypeId = contract.ContractTypeId,
            ContractTypeName = contract.ContractType?.Name ?? "Không xác định",
            DurationMonths = contract.ContractType?.DurationMonths,
            StartDate = contract.StartDate,
            EndDate = contract.EndDate,
            Status = contract.Status,
            Notes = contract.Notes,
            CreatedAt = contract.CreatedAt,
            CreatedByName = null // Sẽ được populate nếu cần thiết
        };
    }

    #endregion
}
