// =============================================
// Auth models — maps to OlaVet.Application.DTOs.Auth
// =============================================

export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  email: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
  phoneNumber?: string;
  role: 'PetOwner' | 'Vet';
  gdprConsent: boolean;
}

export interface AuthResponse {
  userId: number;
  email: string;
  firstName: string;
  lastName: string;
  accessToken: string;
  accessTokenExpiry: string;
  refreshToken: string;
  roles: string[];
  permissions: string[];
}

export interface RefreshTokenRequest {
  accessToken: string;
  refreshToken: string;
}

export interface ChangePasswordRequest {
  currentPassword: string;
  newPassword: string;
  confirmNewPassword: string;
}

export interface FileUploadResult {
  fileName: string;
  storedFileName: string;
  url?: string;
  fileSizeBytes: number;
  contentType: string;
  uploadedAt: string;
}
