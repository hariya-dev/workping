using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace EmployeeManagement.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailTemplates : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EmailTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "char(36)", nullable: false, collation: "ascii_general_ci"),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    Subject = table.Column<string>(type: "varchar(500)", maxLength: 500, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    BodyHtml = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false),
                    Description = table.Column<string>(type: "varchar(1000)", maxLength: 1000, nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EmailTemplates", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "ContractTypes",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 5, 3, 56, 7, 596, DateTimeKind.Utc).AddTicks(9217));

            migrationBuilder.UpdateData(
                table: "ContractTypes",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 5, 3, 56, 7, 596, DateTimeKind.Utc).AddTicks(9224));

            migrationBuilder.UpdateData(
                table: "ContractTypes",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 5, 3, 56, 7, 596, DateTimeKind.Utc).AddTicks(9230));

            migrationBuilder.UpdateData(
                table: "ContractTypes",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 5, 3, 56, 7, 596, DateTimeKind.Utc).AddTicks(9234));

            migrationBuilder.UpdateData(
                table: "ContractTypes",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 5, 3, 56, 7, 596, DateTimeKind.Utc).AddTicks(9239));

            migrationBuilder.InsertData(
                table: "EmailTemplates",
                columns: new[] { "Id", "BodyHtml", "CreatedAt", "Description", "IsActive", "Name", "Subject", "Type", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("ee111111-1111-1111-1111-111111111111"), "<!DOCTYPE html>\r\n<html>\r\n<head><meta charset='UTF-8'></head>\r\n<body style='font-family: Arial, sans-serif;'>\r\n    <h2 style='color: #2563eb;'>Thông báo thời gian thử việc sắp kết thúc</h2>\r\n    <p>Kính gửi <strong>{EmployeeName}</strong>,</p>\r\n    <p>Thời gian thử việc của bạn sắp kết thúc. Dưới đây là thông tin chi tiết:</p>\r\n    <table style='border-collapse: collapse; margin: 20px 0;'>\r\n        <tr>\r\n            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Ngày kết thúc thử việc:</td>\r\n            <td style='padding: 8px; border: 1px solid #ddd;'><strong>{EndDate}</strong></td>\r\n        </tr>\r\n        <tr>\r\n            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Số ngày còn lại:</td>\r\n            <td style='padding: 8px; border: 1px solid #ddd; color: #dc2626;'><strong>{DaysRemaining} ngày</strong></td>\r\n        </tr>\r\n    </table>\r\n    <p>Vui lòng liên hệ bộ phận Nhân sự để biết thêm thông tin.</p>\r\n    <hr style='margin: 20px 0;'>\r\n    <p style='color: #666; font-size: 12px;'>Email tự động từ Hệ thống Quản lý Nhân sự</p>\r\n</body>\r\n</html>", new DateTime(2026, 3, 5, 3, 56, 7, 596, DateTimeKind.Utc).AddTicks(9496), "Email gửi cho nhân viên khi thử việc sắp kết thúc", true, "Nhắc nhở thử việc (Nhân viên)", "[Nhắc nhở] Thử việc của {EmployeeName} sắp kết thúc", 1, null },
                    { new Guid("ee222222-2222-2222-2222-222222222222"), "<!DOCTYPE html>\r\n<html>\r\n<head><meta charset='UTF-8'></head>\r\n<body style='font-family: Arial, sans-serif;'>\r\n    <h2 style='color: #2563eb;'>Nhắc nhở Thời gian thử việc sắp kết thúc</h2>\r\n    <table style='border-collapse: collapse; margin: 20px 0;'>\r\n        <tr>\r\n            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Họ và tên:</td>\r\n            <td style='padding: 8px; border: 1px solid #ddd;'><strong>{EmployeeName}</strong></td>\r\n        </tr>\r\n        <tr>\r\n            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Phòng ban:</td>\r\n            <td style='padding: 8px; border: 1px solid #ddd;'>{Department}</td>\r\n        </tr>\r\n        <tr>\r\n            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Ngày kết thúc thử việc:</td>\r\n            <td style='padding: 8px; border: 1px solid #ddd;'><strong>{EndDate}</strong></td>\r\n        </tr>\r\n        <tr>\r\n            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Số ngày còn lại:</td>\r\n            <td style='padding: 8px; border: 1px solid #ddd; color: #dc2626;'><strong>{DaysRemaining} ngày</strong></td>\r\n        </tr>\r\n    </table>\r\n    <p>Vui lòng xem xét đánh giá và quyết định gia hạn hợp đồng cho nhân viên này.</p>\r\n</body>\r\n</html>", new DateTime(2026, 3, 5, 3, 56, 7, 596, DateTimeKind.Utc).AddTicks(9505), "Email gửi cho HR khi nhân viên sắp hết thử việc", true, "Nhắc nhở thử việc (HR)", "[Nhắc nhở Thử việc] {EmployeeName} - Còn {DaysRemaining} ngày", 5, null },
                    { new Guid("ee333333-3333-3333-3333-333333333333"), "<!DOCTYPE html>\r\n<html>\r\n<head><meta charset='UTF-8'></head>\r\n<body style='font-family: Arial, sans-serif;'>\r\n    <h2 style='color: #dc2626;'>Thông báo hợp đồng lao động sắp hết hạn</h2>\r\n    <p>Kính gửi <strong>{EmployeeName}</strong>,</p>\r\n    <p>Hợp đồng lao động của bạn sắp hết hạn. Dưới đây là thông tin chi tiết:</p>\r\n    <table style='border-collapse: collapse; margin: 20px 0;'>\r\n        <tr>\r\n            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Ngày hết hạn hợp đồng:</td>\r\n            <td style='padding: 8px; border: 1px solid #ddd;'><strong>{EndDate}</strong></td>\r\n        </tr>\r\n        <tr>\r\n            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Số ngày còn lại:</td>\r\n            <td style='padding: 8px; border: 1px solid #ddd; color: #dc2626;'><strong>{DaysRemaining} ngày</strong></td>\r\n        </tr>\r\n    </table>\r\n    <p>Vui lòng liên hệ bộ phận Nhân sự để thảo luận về việc gia hạn hợp đồng.</p>\r\n    <hr style='margin: 20px 0;'>\r\n    <p style='color: #666; font-size: 12px;'>Email tự động từ Hệ thống Quản lý Nhân sự</p>\r\n</body>\r\n</html>", new DateTime(2026, 3, 5, 3, 56, 7, 596, DateTimeKind.Utc).AddTicks(9510), "Email gửi cho nhân viên khi hợp đồng sắp hết hạn", true, "Nhắc nhở hợp đồng (Nhân viên)", "[Nhắc nhở] Hợp đồng của {EmployeeName} sắp hết hạn", 2, null },
                    { new Guid("ee444444-4444-4444-4444-444444444444"), "<!DOCTYPE html>\r\n<html>\r\n<head><meta charset='UTF-8'></head>\r\n<body style='font-family: Arial, sans-serif;'>\r\n    <h2 style='color: #dc2626;'>Nhắc nhở Hợp đồng lao động sắp hết hạn</h2>\r\n    <table style='border-collapse: collapse; margin: 20px 0;'>\r\n        <tr>\r\n            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Họ và tên:</td>\r\n            <td style='padding: 8px; border: 1px solid #ddd;'><strong>{EmployeeName}</strong></td>\r\n        </tr>\r\n        <tr>\r\n            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Phòng ban:</td>\r\n            <td style='padding: 8px; border: 1px solid #ddd;'>{Department}</td>\r\n        </tr>\r\n        <tr>\r\n            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Ngày hết hạn hợp đồng:</td>\r\n            <td style='padding: 8px; border: 1px solid #ddd;'><strong>{EndDate}</strong></td>\r\n        </tr>\r\n        <tr>\r\n            <td style='padding: 8px; border: 1px solid #ddd; background: #f8f9fa;'>Số ngày còn lại:</td>\r\n            <td style='padding: 8px; border: 1px solid #ddd; color: #dc2626;'><strong>{DaysRemaining} ngày</strong></td>\r\n        </tr>\r\n    </table>\r\n    <p>Vui lòng liên hệ nhân viên để thảo luận về việc gia hạn hợp đồng.</p>\r\n</body>\r\n</html>", new DateTime(2026, 3, 5, 3, 56, 7, 596, DateTimeKind.Utc).AddTicks(9514), "Email gửi cho HR khi hợp đồng nhân viên sắp hết hạn", true, "Nhắc nhở hợp đồng (HR)", "[Nhắc nhở Hợp đồng] {EmployeeName} - Còn {DaysRemaining} ngày", 6, null },
                    { new Guid("ee555555-5555-5555-5555-555555555555"), "<!DOCTYPE html>\r\n<html>\r\n<head><meta charset='UTF-8'></head>\r\n<body style='font-family: Arial, sans-serif;'>\r\n    <div style='max-width: 600px; margin: 0 auto; background: #ff6b6b; border-radius: 12px; overflow: hidden;'>\r\n        <div style='background: #ffffff; padding: 30px; text-align: center;'>\r\n            <h1 style='color: #ff6b6b;'>🎂 Chúc Mừng Sinh Nhật!</h1>\r\n        </div>\r\n        <div style='padding: 40px 30px; background: #fff;'>\r\n            <h2 style='color: #333;'>Chào {EmployeeName} thân mến!</h2>\r\n            <p style='color: #666; line-height: 1.6;'>\r\n                Nhân dịp sinh nhật lần thứ <strong>{Age}</strong> của bạn, chúng tôi xin gửi đến bạn \r\n                những lời chúc tốt đẹp nhất!\r\n            </p>\r\n            <div style='background: #fff5f5; border-radius: 8px; padding: 20px; margin: 25px 0; border-left: 4px solid #ff6b6b;'>\r\n                <p><strong>Ngày sinh:</strong> {BirthDate}</p>\r\n                <p><strong>Tuổi:</strong> {Age} tuổi</p>\r\n            </div>\r\n            <p style='color: #666;'>\r\n                Chúc bạn luôn mạnh khỏe, hạnh phúc và thành công trong công việc!\r\n            </p>\r\n        </div>\r\n        <div style='background: #333; color: white; padding: 20px; text-align: center;'>\r\n            <p style='margin: 0;'>Email tự động từ Hệ thống Quản lý Nhân sự</p>\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>", new DateTime(2026, 3, 5, 3, 56, 7, 596, DateTimeKind.Utc).AddTicks(9570), "Email chúc mừng sinh nhật nhân viên", true, "Chúc mừng sinh nhật", "🎂 Chúc Mừng Sinh Nhật - {EmployeeName}", 3, null },
                    { new Guid("ee666666-6666-6666-6666-666666666666"), "<!DOCTYPE html>\r\n<html>\r\n<head><meta charset='UTF-8'></head>\r\n<body style='font-family: Arial, sans-serif;'>\r\n    <div style='max-width: 700px; margin: 0 auto; background: #fff; border-radius: 12px; overflow: hidden; box-shadow: 0 2px 10px rgba(0,0,0,0.1);'>\r\n        <div style='background: #ff6b6b; padding: 30px; text-align: center; color: white;'>\r\n            <h1 style='margin: 0;'>🎉 Sinh nhật tháng {CurrentMonth}/{CurrentYear}</h1>\r\n        </div>\r\n        <div style='padding: 30px;'>\r\n            <h2 style='color: #333;'>Danh sách nhân viên có sinh nhật trong tháng:</h2>\r\n            {BirthdayList}\r\n            <p style='margin-top: 20px;'><strong>Tổng số:</strong> {TotalCount} nhân viên</p>\r\n        </div>\r\n        <div style='background: #333; color: white; padding: 20px; text-align: center;'>\r\n            <p style='margin: 0;'>Email tự động từ Hệ thống Nhân sự</p>\r\n        </div>\r\n    </div>\r\n</body>\r\n</html>", new DateTime(2026, 3, 5, 3, 56, 7, 596, DateTimeKind.Utc).AddTicks(9575), "Email gửi HR danh sách sinh nhật trong tháng", true, "Danh sách sinh nhật tháng", "[Thông báo] Danh sách sinh nhật tháng {CurrentMonth}/{CurrentYear}", 4, null }
                });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("aaaa1111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 5, 3, 56, 7, 596, DateTimeKind.Utc).AddTicks(9421));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("aaaa2222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 5, 3, 56, 7, 596, DateTimeKind.Utc).AddTicks(9425));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("aaaa3333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 5, 3, 56, 7, 596, DateTimeKind.Utc).AddTicks(9430));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("aaaa4444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 5, 3, 56, 7, 596, DateTimeKind.Utc).AddTicks(9435));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 5, 3, 56, 7, 596, DateTimeKind.Utc).AddTicks(9472));

            migrationBuilder.CreateIndex(
                name: "IX_EmailTemplates_Type",
                table: "EmailTemplates",
                column: "Type",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmailTemplates");

            migrationBuilder.UpdateData(
                table: "ContractTypes",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 5, 3, 0, 30, 656, DateTimeKind.Utc).AddTicks(2311));

            migrationBuilder.UpdateData(
                table: "ContractTypes",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 5, 3, 0, 30, 656, DateTimeKind.Utc).AddTicks(2320));

            migrationBuilder.UpdateData(
                table: "ContractTypes",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 5, 3, 0, 30, 656, DateTimeKind.Utc).AddTicks(2327));

            migrationBuilder.UpdateData(
                table: "ContractTypes",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 5, 3, 0, 30, 656, DateTimeKind.Utc).AddTicks(2331));

            migrationBuilder.UpdateData(
                table: "ContractTypes",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 5, 3, 0, 30, 656, DateTimeKind.Utc).AddTicks(2335));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("aaaa1111-1111-1111-1111-111111111111"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 5, 3, 0, 30, 656, DateTimeKind.Utc).AddTicks(2565));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("aaaa2222-2222-2222-2222-222222222222"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 5, 3, 0, 30, 656, DateTimeKind.Utc).AddTicks(2571));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("aaaa3333-3333-3333-3333-333333333333"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 5, 3, 0, 30, 656, DateTimeKind.Utc).AddTicks(2576));

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("aaaa4444-4444-4444-4444-444444444444"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 5, 3, 0, 30, 656, DateTimeKind.Utc).AddTicks(2581));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: new Guid("00000000-0000-0000-0000-000000000001"),
                column: "CreatedAt",
                value: new DateTime(2026, 3, 5, 3, 0, 30, 656, DateTimeKind.Utc).AddTicks(2621));
        }
    }
}
