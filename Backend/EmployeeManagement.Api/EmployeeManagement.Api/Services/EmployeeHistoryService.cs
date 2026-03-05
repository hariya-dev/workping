// Services/EmployeeHistoryService.cs
// Triển khai dịch vụ lịch sử chỉnh sửa nhân viên

using EmployeeManagement.Api.Data;
using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Services;

/// <summary>
/// Service quản lý lịch sử chỉnh sửa nhân viên
/// Ghi lại tất cả các thay đổi field-level
/// </summary>
public class EmployeeHistoryService : IEmployeeHistoryService
{
    private readonly AppDbContext _context;
    private readonly ILogger<EmployeeHistoryService> _logger;

    /// <summary>
    /// Mapping tên field kỹ thuật sang tên hiển thị tiếng Việt
    /// </summary>
    private static readonly Dictionary<string, string> FieldDisplayNames = new()
    {
        { "FullName", "Họ và tên" },
        { "Email", "Email" },
        { "PhoneNumber", "Số điện thoại" },
        { "DateOfBirth", "Ngày sinh" },
        { "Status", "Trạng thái" },
        { "Department", "Phòng ban" },
        { "Position", "Chức vụ" },
        { "Notes", "Ghi chú" },
        { "ProbationStartDate", "Ngày bắt đầu thử việc" },
        { "ProbationEndDate", "Ngày kết thúc thử việc" },
        { "Contract", "Hợp đồng" }
    };

    public EmployeeHistoryService(AppDbContext context, ILogger<EmployeeHistoryService> logger)
    {
        _context = context;
        _logger = logger;
    }

    /// <summary>
    /// Lấy danh sách lịch sử chỉnh sửa của nhân viên
    /// </summary>
    public async Task<List<EmployeeEditHistoryDto>> GetEditHistoryAsync(Guid employeeId)
    {
        return await _context.EmployeeEditHistories
            .Where(h => h.EmployeeId == employeeId)
            .OrderByDescending(h => h.ChangedAt)
            .Select(h => new EmployeeEditHistoryDto
            {
                Id = h.Id,
                EmployeeId = h.EmployeeId,
                FieldName = h.FieldName,
                FieldDisplayName = h.FieldDisplayName,
                OldValue = h.OldValue,
                NewValue = h.NewValue,
                ChangedBy = h.ChangedBy,
                ChangedByName = h.ChangedByName,
                ChangedAt = h.ChangedAt,
                ChangeType = h.ChangeType
            })
            .ToListAsync();
    }

    /// <summary>
    /// Ghi log khi tạo nhân viên mới
    /// </summary>
    public async Task TrackCreateAsync(Guid employeeId, Employee employee, Guid? userId, string userName)
    {
        var histories = new List<EmployeeEditHistory>();
        var now = DateTime.UtcNow;

        // Log tất cả các field có giá trị khi tạo mới
        AddHistoryIfNotNull(histories, employeeId, "FullName", null, employee.FullName, userId, userName, now, "Create");
        AddHistoryIfNotNull(histories, employeeId, "Email", null, employee.Email, userId, userName, now, "Create");
        AddHistoryIfNotNull(histories, employeeId, "PhoneNumber", null, employee.PhoneNumber, userId, userName, now, "Create");
        AddHistoryIfNotNull(histories, employeeId, "DateOfBirth", null, employee.DateOfBirth.ToString("dd/MM/yyyy"), userId, userName, now, "Create");
        AddHistoryIfNotNull(histories, employeeId, "Status", null, GetStatusDisplayName(employee.Status), userId, userName, now, "Create");
        AddHistoryIfNotNull(histories, employeeId, "Department", null, employee.Department, userId, userName, now, "Create");
        AddHistoryIfNotNull(histories, employeeId, "Position", null, employee.Position, userId, userName, now, "Create");
        AddHistoryIfNotNull(histories, employeeId, "Notes", null, employee.Notes, userId, userName, now, "Create");
        
        if (employee.ProbationStartDate.HasValue)
            AddHistoryIfNotNull(histories, employeeId, "ProbationStartDate", null, employee.ProbationStartDate.Value.ToString("dd/MM/yyyy"), userId, userName, now, "Create");
        
        if (employee.ProbationEndDate.HasValue)
            AddHistoryIfNotNull(histories, employeeId, "ProbationEndDate", null, employee.ProbationEndDate.Value.ToString("dd/MM/yyyy"), userId, userName, now, "Create");

        if (histories.Any())
        {
            _context.EmployeeEditHistories.AddRange(histories);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Đã ghi {Count} bản ghi lịch sử tạo nhân viên {EmployeeId}", histories.Count, employeeId);
        }
    }

    /// <summary>
    /// Ghi log thay đổi một field
    /// </summary>
    public async Task TrackChangeAsync(
        Guid employeeId,
        string fieldName,
        string fieldDisplayName,
        string? oldValue,
        string? newValue,
        Guid? userId,
        string userName,
        string changeType)
    {
        var history = new EmployeeEditHistory
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            FieldName = fieldName,
            FieldDisplayName = fieldDisplayName,
            OldValue = oldValue,
            NewValue = newValue,
            ChangedBy = userId,
            ChangedByName = userName,
            ChangedAt = DateTime.UtcNow,
            ChangeType = changeType
        };

        _context.EmployeeEditHistories.Add(history);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Đã ghi lịch sử {ChangeType} field {FieldName} cho nhân viên {EmployeeId}", 
            changeType, fieldName, employeeId);
    }

    /// <summary>
    /// Ghi log khi cập nhật nhân viên (so sánh và log tất cả các field thay đổi)
    /// </summary>
    public async Task TrackUpdateAsync(Guid employeeId, Employee oldEmployee, Employee newEmployee, Guid? userId, string userName)
    {
        var histories = new List<EmployeeEditHistory>();
        var now = DateTime.UtcNow;

        // So sánh từng field và ghi log nếu có thay đổi
        CompareAndAdd(histories, employeeId, "FullName", oldEmployee.FullName, newEmployee.FullName, userId, userName, now);
        CompareAndAdd(histories, employeeId, "Email", oldEmployee.Email, newEmployee.Email, userId, userName, now);
        CompareAndAdd(histories, employeeId, "PhoneNumber", oldEmployee.PhoneNumber, newEmployee.PhoneNumber, userId, userName, now);
        CompareAndAdd(histories, employeeId, "DateOfBirth", 
            oldEmployee.DateOfBirth.ToString("dd/MM/yyyy"), 
            newEmployee.DateOfBirth.ToString("dd/MM/yyyy"), 
            userId, userName, now);
        CompareAndAdd(histories, employeeId, "Status", 
            GetStatusDisplayName(oldEmployee.Status), 
            GetStatusDisplayName(newEmployee.Status), 
            userId, userName, now);
        CompareAndAdd(histories, employeeId, "Department", oldEmployee.Department, newEmployee.Department, userId, userName, now);
        CompareAndAdd(histories, employeeId, "Position", oldEmployee.Position, newEmployee.Position, userId, userName, now);
        CompareAndAdd(histories, employeeId, "Notes", oldEmployee.Notes, newEmployee.Notes, userId, userName, now);
        CompareAndAdd(histories, employeeId, "ProbationStartDate", 
            oldEmployee.ProbationStartDate?.ToString("dd/MM/yyyy"), 
            newEmployee.ProbationStartDate?.ToString("dd/MM/yyyy"), 
            userId, userName, now);
        CompareAndAdd(histories, employeeId, "ProbationEndDate", 
            oldEmployee.ProbationEndDate?.ToString("dd/MM/yyyy"), 
            newEmployee.ProbationEndDate?.ToString("dd/MM/yyyy"), 
            userId, userName, now);

        if (histories.Any())
        {
            _context.EmployeeEditHistories.AddRange(histories);
            await _context.SaveChangesAsync();
            _logger.LogInformation("Đã ghi {Count} bản ghi lịch sử cập nhật nhân viên {EmployeeId}", histories.Count, employeeId);
        }
    }

    /// <summary>
    /// Ghi log khi thêm hợp đồng mới
    /// </summary>
    public async Task TrackContractCreatedAsync(Guid employeeId, EmployeeContract contract, Guid? userId, string userName)
    {
        var contractInfo = $"{contract.ContractType?.Name ?? "Không xác định"} ({contract.StartDate:dd/MM/yyyy} - {(contract.EndDate?.ToString("dd/MM/yyyy") ?? "Không thời hạn")})";
        
        var history = new EmployeeEditHistory
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            FieldName = "Contract",
            FieldDisplayName = "Hợp đồng",
            OldValue = null,
            NewValue = contractInfo,
            ChangedBy = userId,
            ChangedByName = userName,
            ChangedAt = DateTime.UtcNow,
            ChangeType = "Create"
        };

        _context.EmployeeEditHistories.Add(history);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Đã ghi lịch sử tạo hợp đồng cho nhân viên {EmployeeId}", employeeId);
    }

    /// <summary>
    /// Ghi log khi kết thúc hợp đồng
    /// </summary>
    public async Task TrackContractTerminatedAsync(Guid employeeId, EmployeeContract contract, Guid? userId, string userName)
    {
        var contractInfo = $"{contract.ContractType?.Name ?? "Không xác định"} ({contract.StartDate:dd/MM/yyyy} - {contract.EndDate:dd/MM/yyyy})";
        
        var history = new EmployeeEditHistory
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            FieldName = "Contract",
            FieldDisplayName = "Hợp đồng",
            OldValue = "Đang thực hiện",
            NewValue = $"Đã kết thúc: {contractInfo}",
            ChangedBy = userId,
            ChangedByName = userName,
            ChangedAt = DateTime.UtcNow,
            ChangeType = "Update"
        };

        _context.EmployeeEditHistories.Add(history);
        await _context.SaveChangesAsync();

        _logger.LogInformation("Đã ghi lịch sử kết thúc hợp đồng cho nhân viên {EmployeeId}", employeeId);
    }

    #region Private Helper Methods

    private void AddHistoryIfNotNull(
        List<EmployeeEditHistory> histories,
        Guid employeeId,
        string fieldName,
        string? oldValue,
        string? newValue,
        Guid? userId,
        string userName,
        DateTime changedAt,
        string changeType)
    {
        if (string.IsNullOrEmpty(newValue) && string.IsNullOrEmpty(oldValue))
            return;

        histories.Add(new EmployeeEditHistory
        {
            Id = Guid.NewGuid(),
            EmployeeId = employeeId,
            FieldName = fieldName,
            FieldDisplayName = GetFieldDisplayName(fieldName),
            OldValue = oldValue,
            NewValue = newValue,
            ChangedBy = userId,
            ChangedByName = userName,
            ChangedAt = changedAt,
            ChangeType = changeType
        });
    }

    private void CompareAndAdd(
        List<EmployeeEditHistory> histories,
        Guid employeeId,
        string fieldName,
        string? oldValue,
        string? newValue,
        Guid? userId,
        string userName,
        DateTime changedAt)
    {
        // Chỉ log nếu giá trị thực sự thay đổi
        if (oldValue != newValue)
        {
            histories.Add(new EmployeeEditHistory
            {
                Id = Guid.NewGuid(),
                EmployeeId = employeeId,
                FieldName = fieldName,
                FieldDisplayName = GetFieldDisplayName(fieldName),
                OldValue = oldValue,
                NewValue = newValue,
                ChangedBy = userId,
                ChangedByName = userName,
                ChangedAt = changedAt,
                ChangeType = "Update"
            });
        }
    }

    private static string GetFieldDisplayName(string fieldName)
    {
        return FieldDisplayNames.TryGetValue(fieldName, out var displayName) ? displayName : fieldName;
    }

    private static string GetStatusDisplayName(EmployeeStatus status)
    {
        return status switch
        {
            EmployeeStatus.Active => "Đang làm việc",
            EmployeeStatus.Resigned => "Đã nghỉ việc",
            _ => "Không xác định"
        };
    }

    #endregion
}
