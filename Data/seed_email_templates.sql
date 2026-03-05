-- =============================================
-- SCRIPT: SEED EMAIL TEMPLATES
-- Mô tả: Tạo và chèn dữ liệu mẫu email mặc định
-- =============================================

-- Tạo bảng EmailTemplates nếu chưa tồn tại
CREATE TABLE IF NOT EXISTS `EmailTemplates` (
    `Id` CHAR(36) NOT NULL,
    `Type` INT NOT NULL,
    `Name` VARCHAR(100) NOT NULL,
    `Subject` VARCHAR(500) NOT NULL,
    `BodyHtml` LONGTEXT NOT NULL,
    `IsActive` TINYINT(1) NOT NULL DEFAULT 1,
    `Description` VARCHAR(1000) NULL,
    `CreatedAt` DATETIME(6) NOT NULL,
    `UpdatedAt` DATETIME(6) NULL,
    PRIMARY KEY (`Id`),
    UNIQUE INDEX `IX_EmailTemplates_Type` (`Type`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_unicode_ci;

-- TEMPLATE 1: NHẮC NHỞ THỬ VIỆC (NHÂN VIÊN) - Type = 1
-- Placeholders: {EmployeeName}, {EndDate}, {DaysRemaining}
INSERT INTO `EmailTemplates` (`Id`, `Type`, `Name`, `Subject`, `BodyHtml`, `IsActive`, `Description`, `CreatedAt`)
VALUES (
    'ee111111-1111-1111-1111-111111111111',
    1,
    'Nhắc nhở thử việc (Nhân viên)',
    '[Nhắc nhở] Thử việc của {EmployeeName} sắp kết thúc',
    '<!DOCTYPE html><html><head><meta charset="UTF-8"></head><body style="font-family: Arial, sans-serif;"><div style="max-width:600px;margin:0 auto;background:#fff;border-radius:12px;overflow:hidden;"><div style="background:linear-gradient(135deg,#2563eb,#1d4ed8);padding:30px;text-align:center;"><h1 style="color:#fff;margin:0;">Thông báo Thử việc</h1></div><div style="padding:30px;"><p>Kính gửi <strong style="color:#2563eb;">{EmployeeName}</strong>,</p><p>Thời gian thử việc của bạn sắp kết thúc:</p><table style="width:100%;border-collapse:collapse;margin:20px 0;"><tr><td style="padding:12px;background:#f8fafc;border:1px solid #e2e8f0;font-weight:600;">📅 Ngày kết thúc</td><td style="padding:12px;border:1px solid #e2e8f0;font-weight:600;">{EndDate}</td></tr><tr><td style="padding:12px;background:#f8fafc;border:1px solid #e2e8f0;font-weight:600;">⏳ Còn lại</td><td style="padding:12px;border:1px solid #e2e8f0;color:#dc2626;font-weight:700;font-size:18px;">{DaysRemaining} ngày</td></tr></table><p>Vui lòng liên hệ bộ phận Nhân sự để biết thêm thông tin.</p></div><div style="background:#1e293b;padding:20px;text-align:center;"><p style="color:#94a3b8;font-size:13px;margin:0;">Email tự động từ Hệ thống Quản lý Nhân sự - An Tưởng Technology</p></div></div></body></html>',
    1,
    'Email gửi cho nhân viên khi thử việc sắp kết thúc',
    UTC_TIMESTAMP()
);

-- TEMPLATE 2: NHẮC NHỞ HỢP ĐỒNG (NHÂN VIÊN) - Type = 2
-- Placeholders: {EmployeeName}, {EndDate}, {DaysRemaining}
INSERT INTO `EmailTemplates` (`Id`, `Type`, `Name`, `Subject`, `BodyHtml`, `IsActive`, `Description`, `CreatedAt`)
VALUES (
    'ee333333-3333-3333-3333-333333333333',
    2,
    'Nhắc nhở hợp đồng (Nhân viên)',
    '[Nhắc nhở] Hợp đồng của {EmployeeName} sắp hết hạn',
    '<!DOCTYPE html><html><head><meta charset="UTF-8"></head><body style="font-family: Arial, sans-serif;"><div style="max-width:600px;margin:0 auto;background:#fff;border-radius:12px;overflow:hidden;"><div style="background:linear-gradient(135deg,#dc2626,#b91c1c);padding:30px;text-align:center;"><h1 style="color:#fff;margin:0;">Thông báo Hợp đồng</h1></div><div style="padding:30px;"><p>Kính gửi <strong style="color:#dc2626;">{EmployeeName}</strong>,</p><p>Hợp đồng lao động của bạn sắp hết hạn:</p><table style="width:100%;border-collapse:collapse;margin:20px 0;"><tr><td style="padding:12px;background:#fef2f2;border:1px solid #fecaca;font-weight:600;">📅 Ngày hết hạn</td><td style="padding:12px;border:1px solid #fecaca;font-weight:600;">{EndDate}</td></tr><tr><td style="padding:12px;background:#fef2f2;border:1px solid #fecaca;font-weight:600;">⏳ Còn lại</td><td style="padding:12px;border:1px solid #fecaca;color:#dc2626;font-weight:700;font-size:18px;">{DaysRemaining} ngày</td></tr></table><p>Vui lòng liên hệ bộ phận Nhân sự để thảo luận về việc gia hạn.</p></div><div style="background:#1e293b;padding:20px;text-align:center;"><p style="color:#94a3b8;font-size:13px;margin:0;">Email tự động từ Hệ thống Quản lý Nhân sự - An Tưởng Technology</p></div></div></body></html>',
    1,
    'Email gửi cho nhân viên khi hợp đồng sắp hết hạn',
    UTC_TIMESTAMP()
);

-- TEMPLATE 3: CHÚC MỪNG SINH NHẬT - Type = 3
-- Placeholders: {EmployeeName}, {BirthDate}, {Age}
INSERT INTO `EmailTemplates` (`Id`, `Type`, `Name`, `Subject`, `BodyHtml`, `IsActive`, `Description`, `CreatedAt`)
VALUES (
    'ee555555-5555-5555-5555-555555555555',
    3,
    'Chúc mừng sinh nhật',
    '🎂 Chúc Mừng Sinh Nhật - {EmployeeName}',
    '<!DOCTYPE html><html><head><meta charset="UTF-8"></head><body style="font-family: Arial, sans-serif;"><div style="max-width:600px;margin:0 auto;background:#fff;border-radius:16px;overflow:hidden;"><div style="background:linear-gradient(135deg,#ec4899,#db2777);padding:40px;text-align:center;"><div style="font-size:60px;">🎂</div><h1 style="color:#fff;margin:0;font-size:32px;">Chúc Mừng Sinh Nhật!</h1></div><div style="padding:40px 30px;"><h2 style="color:#be185d;font-size:24px;text-align:center;">🎉 Chào {EmployeeName} thân mến! 🎉</h2><p style="font-size:16px;color:#6b7280;text-align:center;">Nhân dịp sinh nhật lần thứ <strong style="color:#be185d;font-size:20px;">{Age}</strong> của bạn, chúng tôi xin gửi lời chúc tốt đẹp nhất!</p><div style="background:linear-gradient(135deg,#fdf2f8,#fce7f3);border-radius:12px;padding:25px;text-align:center;margin:30px 0;"><p style="margin:8px 0;color:#831843;"><strong>👤 Họ tên:</strong> <span style="color:#1f2937;font-weight:700;">{EmployeeName}</span></p><p style="margin:8px 0;color:#831843;"><strong>📅 Ngày sinh:</strong> <span style="color:#1f2937;">{BirthDate}</span></p><p style="margin:8px 0;color:#831843;"><strong>🎈 Tuổi:</strong> <span style="color:#be185d;font-weight:700;font-size:18px;">{Age} tuổi</span></p></div><div style="background:#f0fdf4;border-left:4px solid #22c55e;padding:20px;border-radius:0 12px 12px 0;"><p style="margin:0;color:#166534;">🌟 Chúc bạn luôn <strong>khỏe mạnh</strong>, <strong>hạnh phúc</strong> và <strong>thành công</strong>!</p></div></div><div style="background:#1f2937;padding:25px;text-align:center;"><p style="color:#f9a8d4;font-size:14px;font-weight:600;margin:0;">CÔNG TY CỔ PHẦN GIẢI PHÁP KỸ THUẬT ẤN TƯỞNG</p></div></div></body></html>',
    1,
    'Email chúc mừng sinh nhật gửi cho nhân viên',
    UTC_TIMESTAMP()
);

-- TEMPLATE 4: DANH SÁCH SINH NHẬT THÁNG - Type = 4
-- Placeholders: {CurrentMonth}, {CurrentYear}, {BirthdayList}, {TotalCount}
INSERT INTO `EmailTemplates` (`Id`, `Type`, `Name`, `Subject`, `BodyHtml`, `IsActive`, `Description`, `CreatedAt`)
VALUES (
    'ee666666-6666-6666-6666-666666666666',
    4,
    'Danh sách sinh nhật tháng',
    '[Thông báo] Danh sách sinh nhật tháng {CurrentMonth}/{CurrentYear}',
    '<!DOCTYPE html><html><head><meta charset="UTF-8"></head><body style="font-family: Arial, sans-serif;"><div style="max-width:700px;margin:0 auto;background:#fff;border-radius:16px;overflow:hidden;"><div style="background:linear-gradient(135deg,#8b5cf6,#7c3aed);padding:35px;text-align:center;"><div style="font-size:50px;">🎉</div><h1 style="color:#fff;margin:0;font-size:28px;">Sinh nhật tháng {CurrentMonth}/{CurrentYear}</h1></div><div style="padding:30px;"><p style="font-size:15px;color:#6b7280;">Kính gửi Bộ phận Nhân sự,</p><p style="font-size:15px;color:#4b5563;">Dưới đây là danh sách nhân viên có sinh nhật trong tháng <strong>{CurrentMonth}/{CurrentYear}</strong>:</p><div style="background:#faf5ff;border-radius:12px;padding:20px;margin:25px 0;">{BirthdayList}</div><div style="background:linear-gradient(135deg,#f0fdf4,#dcfce7);border-radius:12px;padding:20px;text-align:center;"><p style="margin:0;color:#166534;font-size:16px;">📊 Tổng số: <strong style="font-size:24px;color:#15803d;">{TotalCount}</strong> nhân viên</p></div></div><div style="background:#1f293b;padding:20px;text-align:center;"><p style="color:#c4b5fd;font-size:13px;margin:0;">Email tự động từ Hệ thống Quản lý Nhân sự - An Tưởng Technology</p></div></div></body></html>',
    1,
    'Email gửi HR danh sách sinh nhật trong tháng',
    UTC_TIMESTAMP()
);

-- TEMPLATE 5: NHẮC NHỞ THỬ VIỆC (HR) - Type = 5
-- Placeholders: {EmployeeName}, {Department}, {EndDate}, {DaysRemaining}
INSERT INTO `EmailTemplates` (`Id`, `Type`, `Name`, `Subject`, `BodyHtml`, `IsActive`, `Description`, `CreatedAt`)
VALUES (
    'ee222222-2222-2222-2222-222222222222',
    5,
    'Nhắc nhở thử việc (HR)',
    '[Nhắc nhở Thử việc] {EmployeeName} - Còn {DaysRemaining} ngày',
    '<!DOCTYPE html><html><head><meta charset="UTF-8"></head><body style="font-family: Arial, sans-serif;"><div style="max-width:650px;margin:0 auto;background:#fff;border-radius:12px;overflow:hidden;"><div style="background:linear-gradient(135deg,#f59e0b,#d97706);padding:30px;text-align:center;"><h1 style="color:#fff;margin:0;">Nhắc nhở Thử việc</h1><p style="color:#fef3c7;margin:10px 0 0;font-size:14px;">Thông báo cho Bộ phận Nhân sự</p></div><div style="padding:30px;"><p>Nhân viên <strong style="color:#d97706;">{EmployeeName}</strong> có thời gian thử việc sắp kết thúc:</p><table style="width:100%;border-collapse:collapse;margin:20px 0;"><tr><td style="padding:12px;background:#fffbeb;border:1px solid #fde68a;font-weight:600;color:#92400e;">👤 Họ tên</td><td style="padding:12px;border:1px solid #fde68a;font-weight:600;">{EmployeeName}</td></tr><tr><td style="padding:12px;background:#fffbeb;border:1px solid #fde68a;font-weight:600;color:#92400e;">🏢 Phòng ban</td><td style="padding:12px;border:1px solid #fde68a;">{Department}</td></tr><tr><td style="padding:12px;background:#fffbeb;border:1px solid #fde68a;font-weight:600;color:#92400e;">📅 Ngày kết thúc</td><td style="padding:12px;border:1px solid #fde68a;font-weight:600;">{EndDate}</td></tr><tr><td style="padding:12px;background:#fffbeb;border:1px solid #fde68a;font-weight:600;color:#92400e;">⏳ Còn lại</td><td style="padding:12px;border:1px solid #fde68a;color:#dc2626;font-weight:700;font-size:18px;">{DaysRemaining} ngày</td></tr></table><div style="background:#fef3c7;border-left:4px solid #f59e0b;padding:15px;border-radius:0 8px 8px 0;"><p style="margin:0;color:#92400e;font-size:14px;"><strong>📋 Hành động:</strong> Đánh giá hiệu quả, thu thập phản hồi, quyết định gia hạn hợp đồng.</p></div></div><div style="background:#1e293b;padding:20px;text-align:center;"><p style="color:#fcd34d;font-size:13px;margin:0;">Email tự động từ Hệ thống Quản lý Nhân sự</p></div></div></body></html>',
    1,
    'Email gửi HR khi nhân viên sắp hết thử việc',
    UTC_TIMESTAMP()
);

-- TEMPLATE 6: NHẮC NHỚ HỢP ĐỒNG (HR) - Type = 6
-- Placeholders: {EmployeeName}, {Department}, {EndDate}, {DaysRemaining}
INSERT INTO `EmailTemplates` (`Id`, `Type`, `Name`, `Subject`, `BodyHtml`, `IsActive`, `Description`, `CreatedAt`)
VALUES (
    'ee444444-4444-4444-4444-444444444444',
    6,
    'Nhắc nhở hợp đồng (HR)',
    '[Nhắc nhở Hợp đồng] {EmployeeName} - Còn {DaysRemaining} ngày',
    '<!DOCTYPE html><html><head><meta charset="UTF-8"></head><body style="font-family: Arial, sans-serif;"><div style="max-width:650px;margin:0 auto;background:#fff;border-radius:12px;overflow:hidden;"><div style="background:linear-gradient(135deg,#dc2626,#b91c1c);padding:30px;text-align:center;"><h1 style="color:#fff;margin:0;">Nhắc nhở Hợp đồng</h1><p style="color:#fecaca;margin:10px 0 0;font-size:14px;">Thông báo cho Bộ phận Nhân sự</p></div><div style="padding:30px;"><p>Nhân viên <strong style="color:#dc2626;">{EmployeeName}</strong> có hợp đồng sắp hết hạn:</p><table style="width:100%;border-collapse:collapse;margin:20px 0;"><tr><td style="padding:12px;background:#fef2f2;border:1px solid #fecaca;font-weight:600;color:#991b1b;">👤 Họ tên</td><td style="padding:12px;border:1px solid #fecaca;font-weight:600;">{EmployeeName}</td></tr><tr><td style="padding:12px;background:#fef2f2;border:1px solid #fecaca;font-weight:600;color:#991b1b;">🏢 Phòng ban</td><td style="padding:12px;border:1px solid #fecaca;">{Department}</td></tr><tr><td style="padding:12px;background:#fef2f2;border:1px solid #fecaca;font-weight:600;color:#991b1b;">📅 Ngày hết hạn</td><td style="padding:12px;border:1px solid #fecaca;font-weight:600;">{EndDate}</td></tr><tr><td style="padding:12px;background:#fef2f2;border:1px solid #fecaca;font-weight:600;color:#991b1b;">⏳ Còn lại</td><td style="padding:12px;border:1px solid #fecaca;color:#dc2626;font-weight:700;font-size:18px;">{DaysRemaining} ngày</td></tr></table><div style="background:#fef2f2;border-left:4px solid #dc2626;padding:15px;border-radius:0 8px 8px 0;"><p style="margin:0;color:#991b1b;font-size:14px;"><strong>📋 Hành động:</strong> Liên hệ nhân viên, chuẩn bị hợp đồng mới, cập nhật hệ thống.</p></div></div><div style="background:#1e293b;padding:20px;text-align:center;"><p style="color:#fca5a5;font-size:13px;margin:0;">Email tự động từ Hệ thống Quản lý Nhân sự</p></div></div></body></html>',
    1,
    'Email gửi HR khi nhân viên sắp hết hạn hợp đồng',
    UTC_TIMESTAMP()
);

-- KIỂM TRA DỮ LIỆU
SELECT Type AS 'Loại', Name AS 'Tên Template', Subject AS 'Tiêu đề', IsActive AS 'Kích hoạt' FROM EmailTemplates ORDER BY Type;
