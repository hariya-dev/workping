// Services/ContractTypeService.cs
// Triển khai dịch vụ quản lý loại hợp đồng

using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Services;

/// <summary>
/// Service quản lý loại hợp đồng (Master Data)
/// </summary>
public class ContractTypeService : IContractTypeService
{
    private readonly AppDbContext _context;
    private readonly ILogger<ContractTypeService> _logger;

    public ContractTypeService(AppDbContext context, ILogger<ContractTypeService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Lấy tất cả loại hợp đồng
    /// </summary>
    public async Task<List<ContractTypeDto>> GetAllAsync(bool activeOnly = true)
    {
        var query = _context.ContractTypes.AsQueryable();

        if (activeOnly)
        {
            query = query.Where(c => c.IsActive);
        }

        return await query
            .OrderBy(c => c.Name)
            .Select(c => new ContractTypeDto
            {
                Id = c.Id,
                Name = c.Name,
                DurationMonths = c.DurationMonths,
                Description = c.Description,
                IsActive = c.IsActive,
                CreatedAt = c.CreatedAt,
                EmployeeCount = c.Contracts.Count
            })
            .ToListAsync();
    }

    /// <summary>
    /// Lấy loại hợp đồng theo ID
    /// </summary>
    public async Task<ContractTypeDto?> GetByIdAsync(Guid id)
    {
        var contractType = await _context.ContractTypes
            .Include(c => c.Contracts)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contractType == null) return null;

        return new ContractTypeDto
        {
            Id = contractType.Id,
            Name = contractType.Name,
            DurationMonths = contractType.DurationMonths,
            Description = contractType.Description,
            IsActive = contractType.IsActive,
            CreatedAt = contractType.CreatedAt,
            EmployeeCount = contractType.Contracts.Count
        };
    }

    /// <summary>
    /// Tạo loại hợp đồng mới
    /// </summary>
    public async Task<ApiResult<ContractTypeDto>> CreateAsync(CreateContractTypeDto dto)
    {
        try
        {
            // Kiểm tra tên trùng
            var exists = await _context.ContractTypes.AnyAsync(c => c.Name == dto.Name);
            if (exists)
            {
                return ApiResult<ContractTypeDto>.Fail("Tên loại hợp đồng đã tồn tại");
            }

            var contractType = new ContractType
            {
                Id = Guid.NewGuid(),
                Name = dto.Name,
                DurationMonths = dto.DurationMonths,
                Description = dto.Description,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            _context.ContractTypes.Add(contractType);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Tạo loại hợp đồng mới: {Id} - {Name}", contractType.Id, contractType.Name);

            return ApiResult<ContractTypeDto>.Ok(new ContractTypeDto
            {
                Id = contractType.Id,
                Name = contractType.Name,
                DurationMonths = contractType.DurationMonths,
                Description = contractType.Description,
                IsActive = contractType.IsActive,
                CreatedAt = contractType.CreatedAt,
                EmployeeCount = 0
            }, "Tạo loại hợp đồng thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi tạo loại hợp đồng: {Name}", dto.Name);
            return ApiResult<ContractTypeDto>.Fail("Lỗi khi tạo loại hợp đồng");
        }
    }

    /// <summary>
    /// Cập nhật loại hợp đồng
    /// </summary>
    public async Task<ApiResult<ContractTypeDto>> UpdateAsync(Guid id, UpdateContractTypeDto dto)
    {
        try
        {
            var contractType = await _context.ContractTypes.FindAsync(id);
            if (contractType == null)
            {
                return ApiResult<ContractTypeDto>.Fail("Không tìm thấy loại hợp đồng");
            }

            // Kiểm tra tên trùng (trừ chính nó)
            var exists = await _context.ContractTypes.AnyAsync(c => c.Name == dto.Name && c.Id != id);
            if (exists)
            {
                return ApiResult<ContractTypeDto>.Fail("Tên loại hợp đồng đã tồn tại");
            }

            contractType.Name = dto.Name;
            contractType.DurationMonths = dto.DurationMonths;
            contractType.Description = dto.Description;
            contractType.IsActive = dto.IsActive;
            contractType.UpdatedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();

            _logger.LogInformation("Cập nhật loại hợp đồng: {Id} - {Name}", id, dto.Name);

            var result = await GetByIdAsync(id);
            return ApiResult<ContractTypeDto>.Ok(result!, "Cập nhật thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật loại hợp đồng: {Id}", id);
            return ApiResult<ContractTypeDto>.Fail("Lỗi khi cập nhật loại hợp đồng");
        }
    }

    /// <summary>
    /// Xóa loại hợp đồng (soft delete - đặt IsActive = false)
    /// </summary>
    public async Task<ApiResult> DeleteAsync(Guid id)
    {
        try
        {
            var contractType = await _context.ContractTypes
                .Include(c => c.Contracts)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (contractType == null)
            {
                return ApiResult.Fail("Không tìm thấy loại hợp đồng");
            }

            // Kiểm tra có hợp đồng đang dùng không
            if (contractType.Contracts.Any())
            {
                // Soft delete thay vì hard delete
                contractType.IsActive = false;
                contractType.UpdatedAt = DateTime.UtcNow;
                await _context.SaveChangesAsync();

                _logger.LogInformation("Vô hiệu hóa loại hợp đồng: {Id} (còn {Count} hợp đồng đang dùng)", 
                    id, contractType.Contracts.Count);

                return ApiResult.Ok("Đã vô hiệu hóa loại hợp đồng (vẫn còn hợp đồng đang sử dụng)");
            }

            // Hard delete nếu không có ai dùng
            _context.ContractTypes.Remove(contractType);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Xóa loại hợp đồng: {Id} - {Name}", id, contractType.Name);

            return ApiResult.Ok("Xóa loại hợp đồng thành công");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xóa loại hợp đồng: {Id}", id);
            return ApiResult.Fail("Lỗi khi xóa loại hợp đồng");
        }
    }
}
