// Endpoints/AuthEndpoints.cs
// Minimal API endpoints cho xác thực

using EmployeeManagement.Api.DTOs;
using EmployeeManagement.Api.Services;
using System.Security.Claims;

namespace EmployeeManagement.Api.Endpoints;

/// <summary>
/// Định nghĩa các API endpoints cho Authentication
/// </summary>
public static class AuthEndpoints
{
    /// <summary>
    /// Map các routes cho Authentication
    /// </summary>
    public static void MapAuthEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/auth")
            .WithTags("Authentication");

        // POST /api/auth/login - Đăng nhập
        group.MapPost("/login", async (LoginDto dto, IAuthService authService) =>
        {
            var result = await authService.LoginAsync(dto);
            
            if (!result.Success)
            {
                return Results.BadRequest(result);
            }
            
            return Results.Ok(result);
        })
        .WithName("Login")
        .WithSummary("Đăng nhập và nhận JWT token")
        .AllowAnonymous();

        // GET /api/auth/me - Lấy thông tin user hiện tại
        group.MapGet("/me", async (ClaimsPrincipal user, IAuthService authService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var userDto = await authService.GetUserByIdAsync(userId);
            if (userDto == null)
            {
                return Results.NotFound();
            }

            return Results.Ok(ApiResult<UserDto>.Ok(userDto));
        })
        .WithName("GetCurrentUser")
        .WithSummary("Lấy thông tin người dùng hiện tại")
        .RequireAuthorization();

        // POST /api/auth/change-password - Đổi mật khẩu
        group.MapPost("/change-password", async (ChangePasswordDto dto, ClaimsPrincipal user, IAuthService authService) =>
        {
            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Results.Unauthorized();
            }

            var result = await authService.ChangePasswordAsync(userId, dto);
            
            if (!result.Success)
            {
                return Results.BadRequest(result);
            }
            
            return Results.Ok(result);
        })
        .WithName("ChangePassword")
        .WithSummary("Đổi mật khẩu")
        .RequireAuthorization();

        // Test email sending
        group.MapPost("/test-email", async (
            IEmailService emailService,
            ILogger<Program> logger) =>
        {
            try
            {
                logger.LogInformation("Testing email sending...");
                
                // Test professional birthday email
                var birthdayHtml = @"
<!DOCTYPE html>
<html>
<head>
    <meta charset='UTF-8'>
    <meta name='viewport' content='width=device-width, initial-scale=1.0'>
    <title>Chúc Mừng Sinh Nhật</title>
    <style>
        body { margin: 0; padding: 0; font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; background-color: #f5f7fa; }
        .container { max-width: 600px; margin: 0 auto; background: white; border-radius: 12px; overflow: hidden; box-shadow: 0 10px 30px rgba(0,0,0,0.1); }
        .header { background: linear-gradient(135deg, #667eea 0%, #764ba2 100%); padding: 30px; text-align: center; }
        .logo { width: 80px; height: 80px; background: white; border-radius: 50%; margin: 0 auto 20px; display: flex; align-items: center; justify-content: center; }
        .logo-text { font-size: 24px; font-weight: bold; color: #667eea; }
        .birthday-icon { font-size: 40px; margin-bottom: 15px; }
        .content { padding: 40px 30px; }
        .greeting { font-size: 24px; color: #333; margin-bottom: 20px; font-weight: bold; }
        .message { font-size: 16px; color: #666; line-height: 1.6; margin-bottom: 30px; }
        .details { background: #f8f9fa; border-radius: 8px; padding: 20px; margin: 25px 0; }
        .detail-item { display: flex; justify-content: space-between; padding: 8px 0; border-bottom: 1px solid #eee; }
        .detail-item:last-child { border-bottom: none; }
        .label { font-weight: 600; color: #444; }
        .value { color: #667eea; font-weight: 500; }
        .footer { background: #333; color: white; padding: 25px; text-align: center; }
        .company-name { font-size: 18px; font-weight: bold; margin-bottom: 10px; }
        .company-address { font-size: 14px; opacity: 0.8; }
        @media (max-width: 600px) {
            .container { border-radius: 0; }
            .content { padding: 25px 20px; }
            .header { padding: 20px; }
        }
    </style>
</head>
<body>
    <div class='container'>
        <div class='header'>
            <div class='logo'>
                <div class='logo-text'>AT</div>
            </div>
            <div class='birthday-icon'>🎂</div>
            <h1 style='color: white; margin: 0; font-size: 28px;'>Chúc Mừng Sinh Nhật!</h1>
        </div>
        
        <div class='content'>
            <h2 class='greeting'>Chào Trần Đức Hải thân mến!</h2>
            
            <p class='message'>
                Nhân dịp sinh nhật lần thứ <strong>30</strong> của bạn, toàn thể công ty 
                <strong>Công ty Cổ Phần Giải Pháp Kỹ Thuật Ấn Tượng</strong> xin gửi đến bạn 
                những lời chúc tốt đẹp nhất!
            </p>
            
            <div class='details'>
                <div class='detail-item'>
                    <span class='label'>Họ và tên:</span>
                    <span class='value'>Trần Đức Hải</span>
                </div>
                <div class='detail-item'>
                    <span class='label'>Ngày sinh:</span>
                    <span class='value'>06/02/1994</span>
                </div>
                <div class='detail-item'>
                    <span class='label'>Tuổi:</span>
                    <span class='value'>30 tuổi</span>
                </div>
            </div>
            
            <p class='message'>
                Chúc bạn luôn mạnh khỏe, hạnh phúc và thành công trong công việc. 
                Mong rằng năm mới sẽ mang đến cho bạn nhiều niềm vui và cơ hội phát triển!
            </p>
        </div>
        
        <div class='footer'>
            <div class='company-name'>CÔNG TY CỔ PHẦN GIẢI PHÁP KỸ THUẬT ẤN TƯỢNG</div>
            <div class='company-address'>Số 123 Đường ABC, Quận XYZ, TP. Hồ Chí Minh</div>
            <div class='company-address'>Hotline: 0123 456 789 | Email: info@atpro.com.vn</div>
        </div>
    </div>
</body>
</html>";
                await emailService.SendEmailAsync(
                    new[] { "hai2000.dev@gmail.com" },
                    "🎂 Chúc Mừng Sinh Nhật - Trần Đức Hải",
                    birthdayHtml
                );
                
                return Results.Ok(new
                {
                    Message = "Test email sent successfully",
                    Success = true
                });
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error testing email sending");
                return Results.Problem($"Failed to send test email: {ex.Message}");
            }
        })
        .WithName("TestEmail")
        .WithSummary("Test sending notification email")
        .WithDescription("Send test email to verify email configuration")
        .RequireAuthorization();
    }
}
