using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EmployeeManagement.Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddContractHistoryAndEditAuditLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Xóa FK và Index nếu tồn tại (sử dụng try-catch trong SQL)
            migrationBuilder.Sql(@"
                SET @fk_exists = (SELECT COUNT(*) FROM information_schema.TABLE_CONSTRAINTS 
                    WHERE CONSTRAINT_NAME = 'FK_Employees_ContractTypes_ContractTypeId' 
                    AND TABLE_NAME = 'Employees');
                SET @sql = IF(@fk_exists > 0, 
                    'ALTER TABLE `Employees` DROP FOREIGN KEY `FK_Employees_ContractTypes_ContractTypeId`', 
                    'SELECT 1');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            migrationBuilder.Sql(@"
                SET @idx_exists = (SELECT COUNT(*) FROM information_schema.STATISTICS 
                    WHERE INDEX_NAME = 'IX_Employees_ContractEndDate' 
                    AND TABLE_NAME = 'Employees');
                SET @sql = IF(@idx_exists > 0, 
                    'DROP INDEX `IX_Employees_ContractEndDate` ON `Employees`', 
                    'SELECT 1');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            migrationBuilder.Sql(@"
                SET @idx_exists = (SELECT COUNT(*) FROM information_schema.STATISTICS 
                    WHERE INDEX_NAME = 'IX_Employees_ContractTypeId' 
                    AND TABLE_NAME = 'Employees');
                SET @sql = IF(@idx_exists > 0, 
                    'DROP INDEX `IX_Employees_ContractTypeId` ON `Employees`', 
                    'SELECT 1');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            // Xóa các cột nếu tồn tại
            migrationBuilder.Sql(@"
                SET @col_exists = (SELECT COUNT(*) FROM information_schema.COLUMNS 
                    WHERE TABLE_NAME = 'Employees' AND COLUMN_NAME = 'ContractEndDate');
                SET @sql = IF(@col_exists > 0, 
                    'ALTER TABLE `Employees` DROP COLUMN `ContractEndDate`', 
                    'SELECT 1');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            migrationBuilder.Sql(@"
                SET @col_exists = (SELECT COUNT(*) FROM information_schema.COLUMNS 
                    WHERE TABLE_NAME = 'Employees' AND COLUMN_NAME = 'ContractStartDate');
                SET @sql = IF(@col_exists > 0, 
                    'ALTER TABLE `Employees` DROP COLUMN `ContractStartDate`', 
                    'SELECT 1');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            migrationBuilder.Sql(@"
                SET @col_exists = (SELECT COUNT(*) FROM information_schema.COLUMNS 
                    WHERE TABLE_NAME = 'Employees' AND COLUMN_NAME = 'ContractTypeId');
                SET @sql = IF(@col_exists > 0, 
                    'ALTER TABLE `Employees` DROP COLUMN `ContractTypeId`', 
                    'SELECT 1');
                PREPARE stmt FROM @sql;
                EXECUTE stmt;
                DEALLOCATE PREPARE stmt;
            ");

            // Tạo bảng EmployeeContracts nếu chưa tồn tại
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS `EmployeeContracts` (
                    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
                    `EmployeeId` char(36) COLLATE ascii_general_ci NOT NULL,
                    `ContractTypeId` char(36) COLLATE ascii_general_ci NOT NULL,
                    `StartDate` date NOT NULL,
                    `EndDate` date NULL,
                    `Notes` varchar(1000) CHARACTER SET utf8mb4 NULL,
                    `CreatedAt` datetime(6) NOT NULL,
                    `CreatedBy` char(36) COLLATE ascii_general_ci NULL,
                    CONSTRAINT `PK_EmployeeContracts` PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_EmployeeContracts_ContractTypes_ContractTypeId` FOREIGN KEY (`ContractTypeId`) REFERENCES `ContractTypes` (`Id`) ON DELETE RESTRICT,
                    CONSTRAINT `FK_EmployeeContracts_Employees_EmployeeId` FOREIGN KEY (`EmployeeId`) REFERENCES `Employees` (`Id`) ON DELETE CASCADE
                ) CHARACTER SET=utf8mb4;
            ");

            // Tạo bảng EmployeeEditHistories nếu chưa tồn tại
            migrationBuilder.Sql(@"
                CREATE TABLE IF NOT EXISTS `EmployeeEditHistories` (
                    `Id` char(36) COLLATE ascii_general_ci NOT NULL,
                    `EmployeeId` char(36) COLLATE ascii_general_ci NOT NULL,
                    `FieldName` varchar(100) CHARACTER SET utf8mb4 NOT NULL,
                    `FieldDisplayName` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
                    `OldValue` varchar(500) CHARACTER SET utf8mb4 NULL,
                    `NewValue` varchar(500) CHARACTER SET utf8mb4 NULL,
                    `ChangedBy` char(36) COLLATE ascii_general_ci NULL,
                    `ChangedByName` varchar(200) CHARACTER SET utf8mb4 NOT NULL,
                    `ChangedAt` datetime(6) NOT NULL,
                    `ChangeType` varchar(20) CHARACTER SET utf8mb4 NOT NULL,
                    CONSTRAINT `PK_EmployeeEditHistories` PRIMARY KEY (`Id`),
                    CONSTRAINT `FK_EmployeeEditHistories_Employees_EmployeeId` FOREIGN KEY (`EmployeeId`) REFERENCES `Employees` (`Id`) ON DELETE CASCADE
                ) CHARACTER SET=utf8mb4;
            ");

            migrationBuilder.UpdateData(
                table: "ContractTypes",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2026, 2, 6, 2, 47, 0, 729, DateTimeKind.Utc).AddTicks(7883), "Hợp đồng thử việc 2 tháng theo quy định", "Thử việc 2 tháng" });

            migrationBuilder.UpdateData(
                table: "ContractTypes",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2026, 2, 6, 2, 47, 0, 729, DateTimeKind.Utc).AddTicks(7887), "Hợp đồng lao động có thời hạn 6 tháng", "Hợp đồng 6 tháng" });

            migrationBuilder.UpdateData(
                table: "ContractTypes",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2026, 2, 6, 2, 47, 0, 729, DateTimeKind.Utc).AddTicks(7889), "Hợp đồng lao động có thời hạn 1 năm", "Hợp đồng 12 tháng" });

            migrationBuilder.UpdateData(
                table: "ContractTypes",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2026, 2, 6, 2, 47, 0, 729, DateTimeKind.Utc).AddTicks(7892), "Hợp đồng lao động có thời hạn 2 năm", "Hợp đồng 24 tháng" });

            migrationBuilder.UpdateData(
                table: "ContractTypes",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2026, 2, 6, 2, 47, 0, 729, DateTimeKind.Utc).AddTicks(7936), "Hợp đồng lao động không xác định thời hạn", "Không thời hạn" });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("aaaa1111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 2, 6, 2, 47, 0, 729, DateTimeKind.Utc).AddTicks(8044), "Số ngày thử việc mặc định" });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("aaaa2222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 2, 6, 2, 47, 0, 729, DateTimeKind.Utc).AddTicks(8048), "Số ngày trước khi hết thử việc cần gửi nhắc nhở (cách nhau bởi dấu phẩy)" });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("aaaa3333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 2, 6, 2, 47, 0, 729, DateTimeKind.Utc).AddTicks(8051), "Số ngày trước khi hết hợp đồng cần gửi nhắc nhở (cách nhau bởi dấu phẩy)" });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("aaaa4444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 2, 6, 2, 47, 0, 729, DateTimeKind.Utc).AddTicks(8054), "Danh sách email HR nhận thông báo (cách nhau bởi dấu phẩy)" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeContracts_ContractTypeId",
                table: "EmployeeContracts",
                column: "ContractTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeContracts_EmployeeId",
                table: "EmployeeContracts",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeContracts_EmployeeId_EndDate",
                table: "EmployeeContracts",
                columns: new[] { "EmployeeId", "EndDate" });

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeContracts_EndDate",
                table: "EmployeeContracts",
                column: "EndDate");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeEditHistories_ChangedAt",
                table: "EmployeeEditHistories",
                column: "ChangedAt");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeEditHistories_EmployeeId",
                table: "EmployeeEditHistories",
                column: "EmployeeId");

            migrationBuilder.CreateIndex(
                name: "IX_EmployeeEditHistories_EmployeeId_ChangedAt",
                table: "EmployeeEditHistories",
                columns: new[] { "EmployeeId", "ChangedAt" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EmployeeContracts");

            migrationBuilder.DropTable(
                name: "EmployeeEditHistories");

            migrationBuilder.AddColumn<DateOnly>(
                name: "ContractEndDate",
                table: "Employees",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<DateOnly>(
                name: "ContractStartDate",
                table: "Employees",
                type: "date",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ContractTypeId",
                table: "Employees",
                type: "char(36)",
                nullable: true,
                collation: "ascii_general_ci");

            migrationBuilder.UpdateData(
                table: "ContractTypes",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2026, 2, 5, 16, 3, 23, 665, DateTimeKind.Utc).AddTicks(8937), "Hop dong thu viec 2 thang theo quy dinh", "Thu viec 2 thang" });

            migrationBuilder.UpdateData(
                table: "ContractTypes",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2026, 2, 5, 16, 3, 23, 665, DateTimeKind.Utc).AddTicks(8940), "Hop dong lao dong co thoi han 6 thang", "Hop dong 6 thang" });

            migrationBuilder.UpdateData(
                table: "ContractTypes",
                keyColumn: "Id",
                keyValue: new Guid("33333333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2026, 2, 5, 16, 3, 23, 665, DateTimeKind.Utc).AddTicks(8943), "Hop dong lao dong co thoi han 1 nam", "Hop dong 12 thang" });

            migrationBuilder.UpdateData(
                table: "ContractTypes",
                keyColumn: "Id",
                keyValue: new Guid("44444444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2026, 2, 5, 16, 3, 23, 665, DateTimeKind.Utc).AddTicks(8945), "Hop dong lao dong co thoi han 2 nam", "Hop dong 24 thang" });

            migrationBuilder.UpdateData(
                table: "ContractTypes",
                keyColumn: "Id",
                keyValue: new Guid("55555555-5555-5555-5555-555555555555"),
                columns: new[] { "CreatedAt", "Description", "Name" },
                values: new object[] { new DateTime(2026, 2, 5, 16, 3, 23, 665, DateTimeKind.Utc).AddTicks(8947), "Hop dong lao dong khong xac dinh thoi han", "Khong thoi han" });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("aaaa1111-1111-1111-1111-111111111111"),
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 2, 5, 16, 3, 23, 665, DateTimeKind.Utc).AddTicks(9057), "So ngay thu viec mac dinh" });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("aaaa2222-2222-2222-2222-222222222222"),
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 2, 5, 16, 3, 23, 665, DateTimeKind.Utc).AddTicks(9060), "So ngay truoc khi het thu viec can gui nhac nho (cach nhau boi dau phay)" });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("aaaa3333-3333-3333-3333-333333333333"),
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 2, 5, 16, 3, 23, 665, DateTimeKind.Utc).AddTicks(9067), "So ngay truoc khi het hop dong can gui nhac nho (cach nhau boi dau phay)" });

            migrationBuilder.UpdateData(
                table: "SystemSettings",
                keyColumn: "Id",
                keyValue: new Guid("aaaa4444-4444-4444-4444-444444444444"),
                columns: new[] { "CreatedAt", "Description" },
                values: new object[] { new DateTime(2026, 2, 5, 16, 3, 23, 665, DateTimeKind.Utc).AddTicks(9070), "Danh sach email HR nhan thong bao (cach nhau boi dau phay)" });

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ContractEndDate",
                table: "Employees",
                column: "ContractEndDate");

            migrationBuilder.CreateIndex(
                name: "IX_Employees_ContractTypeId",
                table: "Employees",
                column: "ContractTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Employees_ContractTypes_ContractTypeId",
                table: "Employees",
                column: "ContractTypeId",
                principalTable: "ContractTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
