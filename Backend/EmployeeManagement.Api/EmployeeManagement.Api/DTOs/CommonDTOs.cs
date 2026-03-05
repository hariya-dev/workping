// DTOs/CommonDTOs.cs
// Data Transfer Objects dùng chung

namespace EmployeeManagement.Api.DTOs;

/// <summary>
/// DTO kết quả API chung
/// </summary>
/// <typeparam name="T">Kiểu dữ liệu trả về</typeparam>
public class ApiResult<T>
{
    public bool Success { get; set; }
    public T? Data { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Tạo kết quả thành công
    /// </summary>
    public static ApiResult<T> Ok(T data, string? message = null)
    {
        return new ApiResult<T>
        {
            Success = true,
            Data = data,
            Message = message
        };
    }

    /// <summary>
    /// Tạo kết quả thất bại
    /// </summary>
    public static ApiResult<T> Fail(string message, List<string>? errors = null)
    {
        return new ApiResult<T>
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}

/// <summary>
/// DTO kết quả không có data
/// </summary>
public class ApiResult
{
    public bool Success { get; set; }
    public string? Message { get; set; }
    public List<string> Errors { get; set; } = new();

    public static ApiResult Ok(string? message = null)
    {
        return new ApiResult { Success = true, Message = message };
    }

    public static ApiResult Fail(string message, List<string>? errors = null)
    {
        return new ApiResult
        {
            Success = false,
            Message = message,
            Errors = errors ?? new List<string>()
        };
    }
}

/// <summary>
/// DTO phân trang
/// </summary>
/// <typeparam name="T">Kiểu dữ liệu item</typeparam>
public class PagedResult<T>
{
    public List<T> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int TotalPages => (int)Math.Ceiling(TotalCount / (double)PageSize);
    public bool HasPrevious => PageNumber > 1;
    public bool HasNext => PageNumber < TotalPages;
}
