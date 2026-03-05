-- Script tạo user admin mặc định
-- Password: admin (đã hash bằng BCrypt)
-- Chạy script này trong MySQL để tạo user admin

INSERT INTO `Users` (`Id`, `Username`, `Email`, `PasswordHash`, `FullName`, `Role`, `IsActive`, `LastLoginAt`, `CreatedAt`, `UpdatedAt`)
VALUES (
    '00000000-0000-0000-0000-000000000001',
    'admin',
    'admin@company.com',
    '$2a$11$qJZhLzXXcJvRqR3ZP8H4R.VLkL3YW5aLnYkKjxkGXZ1Q8VqxKZqZa',
    'Quản trị viên hệ thống',
    'Admin',
    1,
    NULL,
    UTC_TIMESTAMP(),
    NULL
)
ON DUPLICATE KEY UPDATE
    `PasswordHash` = '$2a$11$qJZhLzXXcJvRqR3ZP8H4R.VLkL3YW5aLnYkKjxkGXZ1Q8VqxKZqZa',
    `IsActive` = 1;

-- Hoặc nếu muốn tạo user admin với password là "admin" (hash mới)
-- Sử dụng hash BCrypt của "admin": $2a$11$3QJ/8Q2Z7Q2Z7Q2Z7Q2Z7O3QJ/8Q2Z7Q2Z7Q2Z7Q2Z7O
-- Lưu ý: Hash trên là ví dụ, bạn nên tạo hash đúng bằng BCrypt

SELECT 'User admin đã được tạo/cập nhật thành công!' AS Message;
SELECT * FROM `Users` WHERE `Username` = 'admin';
