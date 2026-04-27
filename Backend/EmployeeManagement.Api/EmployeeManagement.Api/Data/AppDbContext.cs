// Data/AppDbContext.cs
// DbContext chính của ứng dụng - quản lý kết nối và mapping với MySQL

using EmployeeManagement.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagement.Api.Data;

/// <summary>
/// DbContext chính - cấu hình EF Core với MySQL qua Pomelo
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Bảng nhân viên
    /// </summary>
    public DbSet<Employee> Employees => Set<Employee>();

    /// <summary>
    /// Bảng loại hợp đồng (master data)
    /// </summary>
    public DbSet<ContractType> ContractTypes => Set<ContractType>();

    /// <summary>
    /// Bảng file đính kèm nhân viên
    /// </summary>
    public DbSet<EmployeeFile> EmployeeFiles => Set<EmployeeFile>();

    /// <summary>
    /// Bảng hợp đồng nhân viên (lịch sử hợp đồng)
    /// </summary>
    public DbSet<EmployeeContract> EmployeeContracts => Set<EmployeeContract>();

    /// <summary>
    /// Bảng lịch sử chỉnh sửa nhân viên
    /// </summary>
    public DbSet<EmployeeEditHistory> EmployeeEditHistories => Set<EmployeeEditHistory>();

    /// <summary>
    /// Bảng cài đặt hệ thống
    /// </summary>
    public DbSet<SystemSetting> SystemSettings => Set<SystemSetting>();

    /// <summary>
    /// Bảng người dùng (HR staff)
    /// </summary>
    public DbSet<User> Users => Set<User>();

    /// <summary>
    /// Bảng template email
    /// </summary>
    public DbSet<EmailTemplate> EmailTemplates => Set<EmailTemplate>();

    /// <summary>
    /// Cấu hình model và relationships
    /// </summary>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Cấu hình Employee
        modelBuilder.Entity<Employee>(entity =>
        {
            // Index cho tìm kiếm theo tên
            entity.HasIndex(e => e.FullName);
            
            // Index cho trạng thái
            entity.HasIndex(e => e.Status);
            
            // Index cho ngày kết thúc thử việc (dùng cho reminder)
            entity.HasIndex(e => e.ProbationEndDate);
            
            // Index cho ngày sinh (dùng cho birthday reminder)
            entity.HasIndex(e => e.DateOfBirth);

            // Quan hệ với EmployeeFile (One-to-Many)
            entity.HasMany(e => e.Files)
                  .WithOne(f => f.Employee)
                  .HasForeignKey(f => f.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ với EmployeeContract (One-to-Many)
            entity.HasMany(e => e.Contracts)
                  .WithOne(c => c.Employee)
                  .HasForeignKey(c => c.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);

            // Quan hệ với EmployeeEditHistory (One-to-Many)
            entity.HasMany(e => e.EditHistories)
                  .WithOne(h => h.Employee)
                  .HasForeignKey(h => h.EmployeeId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        // Cấu hình EmployeeContract
        modelBuilder.Entity<EmployeeContract>(entity =>
        {
            // Index cho truy vấn theo nhân viên
            entity.HasIndex(c => c.EmployeeId);
            
            // Index cho truy vấn hợp đồng active
            entity.HasIndex(c => c.EndDate);
            
            // Index tổng hợp cho query phổ biến
            entity.HasIndex(c => new { c.EmployeeId, c.EndDate });

            // Quan hệ với ContractType
            entity.HasOne(c => c.ContractType)
                  .WithMany(ct => ct.Contracts)
                  .HasForeignKey(c => c.ContractTypeId)
                  .OnDelete(DeleteBehavior.Restrict);
        });

        // Cấu hình EmployeeEditHistory
        modelBuilder.Entity<EmployeeEditHistory>(entity =>
        {
            // Index cho truy vấn theo nhân viên
            entity.HasIndex(h => h.EmployeeId);
            
            // Index cho truy vấn theo thời gian
            entity.HasIndex(h => h.ChangedAt);
            
            // Index tổng hợp cho query phổ biến
            entity.HasIndex(h => new { h.EmployeeId, h.ChangedAt });
        });

        // Cấu hình ContractType
        modelBuilder.Entity<ContractType>(entity =>
        {
            entity.HasIndex(c => c.Name).IsUnique();
        });

        // Cấu hình SystemSetting
        modelBuilder.Entity<SystemSetting>(entity =>
        {
            entity.HasIndex(s => s.Key).IsUnique();
        });

        // Cấu hình User
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasIndex(u => u.Username).IsUnique();
            entity.HasIndex(u => u.Email).IsUnique();
        });

        // Cấu hình EmailTemplate
        modelBuilder.Entity<EmailTemplate>(entity =>
        {
            entity.HasIndex(e => e.Type).IsUnique();
        });

        // Seed data mặc định cho ContractTypes
        SeedContractTypes(modelBuilder);
        
        // Seed data mặc định cho SystemSettings
        SeedSystemSettings(modelBuilder);
        
        // Seed admin user mặc định
        SeedDefaultUser(modelBuilder);
        
        // Seed email templates mặc định
        SeedEmailTemplates(modelBuilder);
    }

    /// <summary>
    /// Seed dữ liệu mặc định cho loại hợp đồng
    /// </summary>
    private void SeedContractTypes(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ContractType>().HasData(
            new ContractType
            {
                Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Name = "Thử việc 2 tháng",
                DurationDays = 60,
                Description = "Hợp đồng thử việc 60 ngày theo quy định",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ContractType
            {
                Id = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Name = "Hợp đồng 6 tháng",
                DurationDays = 180,
                Description = "Hợp đồng lao động có thời hạn 180 ngày",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ContractType
            {
                Id = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Name = "Hợp đồng 1 năm",
                DurationDays = 365,
                Description = "Hợp đồng lao động có thời hạn 365 ngày",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ContractType
            {
                Id = Guid.Parse("44444444-4444-4444-4444-444444444444"),
                Name = "Hợp đồng 2 năm",
                DurationDays = 730,
                Description = "Hợp đồng lao động có thời hạn 730 ngày",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            },
            new ContractType
            {
                Id = Guid.Parse("55555555-5555-5555-5555-555555555555"),
                Name = "Không thời hạn",
                DurationDays = null,
                Description = "Hợp đồng lao động không xác định thời hạn",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );
    }

    /// <summary>
    /// Seed dữ liệu mặc định cho cài đặt hệ thống
    /// </summary>
    private void SeedSystemSettings(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SystemSetting>().HasData(
            new SystemSetting
            {
                Id = Guid.Parse("aaaa1111-1111-1111-1111-111111111111"),
                Key = "DefaultProbationDays",
                Value = "60",
                ValueType = "int",
                Description = "Số ngày thử việc mặc định",
                CreatedAt = DateTime.UtcNow
            },
            new SystemSetting
            {
                Id = Guid.Parse("aaaa2222-2222-2222-2222-222222222222"),
                Key = "ProbationReminderDaysBefore",
                Value = "30,15,7,3,1",
                ValueType = "array",
                Description = "Số ngày trước khi hết thử việc cần gửi nhắc nhở (cách nhau bởi dấu phẩy)",
                CreatedAt = DateTime.UtcNow
            },
            new SystemSetting
            {
                Id = Guid.Parse("aaaa3333-3333-3333-3333-333333333333"),
                Key = "ContractReminderDaysBefore",
                Value = "30,15,7,3,1",
                ValueType = "array",
                Description = "Số ngày trước khi hết hợp đồng cần gửi nhắc nhở (cách nhau bởi dấu phẩy)",
                CreatedAt = DateTime.UtcNow
            },
            new SystemSetting
            {
                Id = Guid.Parse("aaaa4444-4444-4444-4444-444444444444"),
                Key = "HrNotificationEmails",
                Value = "hr@company.com",
                ValueType = "string",
                Description = "Danh sách email HR nhận thông báo (cách nhau bởi dấu phẩy)",
                CreatedAt = DateTime.UtcNow
            }
        );
    }

    /// <summary>
    /// Seed user admin mặc định
    /// Password mặc định: Admin@123 (BCrypt hash)
    /// </summary>
    private void SeedDefaultUser(ModelBuilder modelBuilder)
    {
        // BCrypt hash của "Admin@123"
        var passwordHash = "$2a$11$qJZhLzXXcJvRqR3ZP8H4R.VLkL3YW5aLnYkKjxkGXZ1Q8VqxKZqZa";
        
        modelBuilder.Entity<User>().HasData(
            new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Username = "admin",
                Email = "admin@company.com",
                PasswordHash = passwordHash,
                FullName = "Quản trị viên hệ thống",
                Role = "Admin",
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            }
        );
    }

    /// <summary>
    /// Seed dữ liệu mặc định cho email templates
    /// </summary>
    private void SeedEmailTemplates(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmailTemplate>().HasData(
            // Template nhắc nhở thử việc cho nhân viên
            new EmailTemplate
            {
                Id = Guid.Parse("ee111111-1111-1111-1111-111111111111"),
                Type = EmailTemplateType.ProbationReminder,
                Name = "Nhắc nhở thử việc (Nhân viên)",
                Subject = "[Nhắc nhở] Thử việc của {EmployeeName} sắp kết thúc",
                BodyHtml = GetDefaultProbationReminderTemplate(),
                IsActive = true,
                Description = "Email gửi cho nhân viên khi thử việc sắp kết thúc"
            },
            // Template nhắc nhở thử việc cho HR
            new EmailTemplate
            {
                Id = Guid.Parse("ee222222-2222-2222-2222-222222222222"),
                Type = EmailTemplateType.ProbationReminderHr,
                Name = "Nhắc nhở thử việc (HR)",
                Subject = "[Nhắc nhở Thử việc] {EmployeeName} - Còn {DaysRemaining} ngày",
                BodyHtml = GetDefaultProbationReminderHrTemplate(),
                IsActive = true,
                Description = "Email gửi cho HR khi nhân viên sắp hết thử việc"
            },
            // Template nhắc nhở hợp đồng cho nhân viên
            new EmailTemplate
            {
                Id = Guid.Parse("ee333333-3333-3333-3333-333333333333"),
                Type = EmailTemplateType.ContractReminder,
                Name = "Nhắc nhở hợp đồng (Nhân viên)",
                Subject = "[Nhắc nhở] Hợp đồng của {EmployeeName} sắp hết hạn",
                BodyHtml = GetDefaultContractReminderTemplate(),
                IsActive = true,
                Description = "Email gửi cho nhân viên khi hợp đồng sắp hết hạn"
            },
            // Template nhắc nhở hợp đồng cho HR
            new EmailTemplate
            {
                Id = Guid.Parse("ee444444-4444-4444-4444-444444444444"),
                Type = EmailTemplateType.ContractReminderHr,
                Name = "Nhắc nhở hợp đồng (HR)",
                Subject = "[Nhắc nhở Hợp đồng] {EmployeeName} - Còn {DaysRemaining} ngày",
                BodyHtml = GetDefaultContractReminderHrTemplate(),
                IsActive = true,
                Description = "Email gửi cho HR khi hợp đồng nhân viên sắp hết hạn"
            },
            // Template chúc mừng sinh nhật
            new EmailTemplate
            {
                Id = Guid.Parse("ee555555-5555-5555-5555-555555555555"),
                Type = EmailTemplateType.BirthdayWish,
                Name = "Chúc mừng sinh nhật",
                Subject = "🎂 Chúc Mừng Sinh Nhật - {EmployeeName}",
                BodyHtml = GetDefaultBirthdayTemplate(),
                IsActive = true,
                Description = "Email chúc mừng sinh nhật nhân viên"
            },
            // Template danh sách sinh nhật tháng
            new EmailTemplate
            {
                Id = Guid.Parse("ee666666-6666-6666-6666-666666666666"),
                Type = EmailTemplateType.MonthlyBirthdayList,
                Name = "Danh sách sinh nhật tháng",
                Subject = "[Thông báo] Danh sách sinh nhật tháng {CurrentMonth}/{CurrentYear}",
                BodyHtml = GetDefaultBirthdayListTemplate(),
                IsActive = true,
                Description = "Email gửi HR danh sách sinh nhật trong tháng"
            }
        );
    }

    #region Default Email Templates

    private static string GetDefaultProbationReminderTemplate()
    {
        return @"<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'></head>
<body style='font-family: Arial, sans-serif;'>
    <h2 style='color: #2563eb;'>Thông báo thời gian thử việc sắp kết thúc</h2>
    <p>Kính gửi <strong>{EmployeeName}</strong>,</p>
    <p>Thời gian thử việc của bạn sắp kết thúc. Dưới đây là thông tin chi tiết:</p>
    <table style='border-collapse: collapse; margin: 20px 0;'>
        <tr>
            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Ngày kết thúc thử việc:</td>
            <td style='padding: 8px; border: 1px solid #ddd;'><strong>{EndDate}</strong></td>
        </tr>
        <tr>
            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Số ngày còn lại:</td>
            <td style='padding: 8px; border: 1px solid #ddd; color: #dc2626;'><strong>{DaysRemaining} ngày</strong></td>
        </tr>
    </table>
    <p>Vui lòng liên hệ bộ phận Nhân sự để biết thêm thông tin.</p>
    <hr style='margin: 20px 0;'>
    <p style='color: #666; font-size: 12px;'>Email tự động từ Hệ thống Quản lý Nhân sự</p>
</body>
</html>";
    }

    private static string GetDefaultProbationReminderHrTemplate()
    {
        return @"<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'></head>
<body style='font-family: Arial, sans-serif;'>
    <h2 style='color: #2563eb;'>Nhắc nhở Thời gian thử việc sắp kết thúc</h2>
    <table style='border-collapse: collapse; margin: 20px 0;'>
        <tr>
            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Họ và tên:</td>
            <td style='padding: 8px; border: 1px solid #ddd;'><strong>{EmployeeName}</strong></td>
        </tr>
        <tr>
            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Phòng ban:</td>
            <td style='padding: 8px; border: 1px solid #ddd;'>{Department}</td>
        </tr>
        <tr>
            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Ngày kết thúc thử việc:</td>
            <td style='padding: 8px; border: 1px solid #ddd;'><strong>{EndDate}</strong></td>
        </tr>
        <tr>
            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Số ngày còn lại:</td>
            <td style='padding: 8px; border: 1px solid #ddd; color: #dc2626;'><strong>{DaysRemaining} ngày</strong></td>
        </tr>
    </table>
    <p>Vui lòng xem xét đánh giá và quyết định gia hạn hợp đồng cho nhân viên này.</p>
</body>
</html>";
    }

    private static string GetDefaultContractReminderTemplate()
    {
        return @"<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'></head>
<body style='font-family: Arial, sans-serif;'>
    <h2 style='color: #dc2626;'>Thông báo hợp đồng lao động sắp hết hạn</h2>
    <p>Kính gửi <strong>{EmployeeName}</strong>,</p>
    <p>Hợp đồng lao động của bạn sắp hết hạn. Dưới đây là thông tin chi tiết:</p>
    <table style='border-collapse: collapse; margin: 20px 0;'>
        <tr>
            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Ngày hết hạn hợp đồng:</td>
            <td style='padding: 8px; border: 1px solid #ddd;'><strong>{EndDate}</strong></td>
        </tr>
        <tr>
            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Số ngày còn lại:</td>
            <td style='padding: 8px; border: 1px solid #ddd; color: #dc2626;'><strong>{DaysRemaining} ngày</strong></td>
        </tr>
    </table>
    <p>Vui lòng liên hệ bộ phận Nhân sự để thảo luận về việc gia hạn hợp đồng.</p>
    <hr style='margin: 20px 0;'>
    <p style='color: #666; font-size: 12px;'>Email tự động từ Hệ thống Quản lý Nhân sự</p>
</body>
</html>";
    }

    private static string GetDefaultContractReminderHrTemplate()
    {
        return @"<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'></head>
<body style='font-family: Arial, sans-serif;'>
    <h2 style='color: #dc2626;'>Nhắc nhở Hợp đồng lao động sắp hết hạn</h2>
    <table style='border-collapse: collapse; margin: 20px 0;'>
        <tr>
            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Họ và tên:</td>
            <td style='padding: 8px; border: 1px solid #ddd;'><strong>{EmployeeName}</strong></td>
        </tr>
        <tr>
            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Phòng ban:</td>
            <td style='padding: 8px; border: 1px solid #ddd;'>{Department}</td>
        </tr>
        <tr>
            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Ngày hết hạn hợp đồng:</td>
            <td style='padding: 8px; border: 1px solid #ddd;'><strong>{EndDate}</strong></td>
        </tr>
        <tr>
            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Số ngày còn lại:</td>
            <td style='padding: 8px; border: 1px solid #ddd; color: #dc2626;'><strong>{DaysRemaining} ngày</strong></td>
        </tr>
    </table>
    <p>Vui lòng liên hệ nhân viên để thảo luận về việc gia hạn hợp đồng.</p>
</body>
</html>";
    }

    private static string GetDefaultBirthdayTemplate()
    {
        return @"<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'></head>
<body style='font-family: Arial, sans-serif;'>
    <div style='max-width: 600px; margin: 0 auto; background: #ff6b6b; border-radius: 12px; overflow: hidden;'>
        <div style='background: #ffffff; padding: 30px; text-align: center;'>
            <h1 style='color: #ff6b6b;'>🎂 Chúc Mừng Sinh Nhật!</h1>
        </div>
        <div style='padding: 40px 30px; background: #fff;'>
            <h2 style='color: #333;'>Chào {EmployeeName} thân mến!</h2>
            <p style='color: #666; line-height: 1.6;'>
                Nhân dịp sinh nhật lần thứ <strong>{Age}</strong> của bạn, chúng tôi xin gửi đến bạn 
                những lời chúc tốt đẹp nhất!
            </p>
            <div style='background: #fff5f5; border-radius: 8px; padding: 20px; margin: 25px 0; border-left: 4px solid #ff6b6b;'>
                <p><strong>Ngày sinh:</strong> {BirthDate}</p>
                <p><strong>Tuổi:</strong> {Age} tuổi</p>
            </div>
            <p style='color: #666;'>
                Chúc bạn luôn mạnh khỏe, hạnh phúc và thành công trong công việc!
            </p>
        </div>
        <div style='background: #333; color: white; padding: 20px; text-align: center;'>
            <p style='margin: 0;'>Email tự động từ Hệ thống Quản lý Nhân sự</p>
        </div>
    </div>
</body>
</html>";
    }

    private static string GetDefaultBirthdayListTemplate()
    {
        return @"<!DOCTYPE html>
<html>
<head><meta charset='UTF-8'></head>
<body style='font-family: Arial, sans-serif;'>
    <div style='max-width: 700px; margin: 0 auto; background: #fff; border-radius: 12px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>
        <div style='background: #ff6b6b; padding: 30px; text-align: center; color: white;'>
            <h1 style='margin: 0;'>🎉 Sinh nhật tháng {CurrentMonth}/{CurrentYear}</h1>
        </div>
        <div style='padding: 30px;'>
            <h2 style='color: #333;'>Danh sách nhân viên có sinh nhật trong tháng:</h2>
            {BirthdayList}
            <p style='margin-top: 20px;'><strong>Tổng số:</strong> {TotalCount} nhân viên</p>
        </div>
        <div style='background: #333; color: white; padding: 20px; text-align: center;'>
            <p style='margin: 0;'>Email tự động từ Hệ thống Nhân sự</p>
        </div>
    </div>
</body>
</html>";
    }

    #endregion
}
