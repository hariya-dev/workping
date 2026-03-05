// core/models/auth.model.ts
// Models cho Xác thực

export interface LoginDto {
  username: string;
  password: string;
}

export interface LoginResult {
  success: boolean;
  token?: string;
  expiresAt?: string;
  user?: User;
  errorMessage?: string;
}

export interface User {
  id: string;
  username: string;
  email: string;
  fullName?: string;
  role: string;
}

export interface ChangePasswordDto {
  currentPassword: string;
  newPassword: string;
  confirmPassword: string;
}
