// Entities/EmployeeStatus.cs
// Enum định nghĩa các trạng thái của nhân viên trong hệ thống

namespace EmployeeManagement.Api.Entities;

/// <summary>
/// Enum trạng thái nhân viên
/// Chỉ có 2 trạng thái: Đang làm việc và Đã nghỉ việc
/// </summary>
public enum EmployeeStatus
{
    /// <summary>
    /// Đang làm việc
    /// </summary>
    Active = 0,
    
    /// <summary>
    /// Đã nghỉ việc
    /// </summary>
    Resigned = 1
}
