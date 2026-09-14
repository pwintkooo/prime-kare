export interface Profile {
  id: number;
  name: string;
  email: string;
  role: string;
  phone?: string | null;
  hasPassword: boolean;
  canChangePassword: boolean;
  canChangeEmail: boolean;
  externalProviders: string[];
}

export interface UpdateProfileRequest {
  name: string;
  phone: string;
}

export interface ChangePasswordRequest {
  newPassword: string;
  currentPassword: string;
}

export interface ChangeEmailRequest {
  currentPassword: string;
  newEmail: string;
}
