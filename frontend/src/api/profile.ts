import { apiClient } from "./client";
import {
  Profile,
  UpdateProfileRequest,
  ChangePasswordRequest,
  ChangeEmailRequest,
} from "@/types/profile";

export async function getProfile(): Promise<Profile> {
  const response = await apiClient.get<Profile>("/api/profile");

  return response.data;
}

export async function updateProfile(
  request: UpdateProfileRequest,
): Promise<void> {
  await apiClient.put("/api/profile", request);
}

export async function changePassword(
  request: ChangePasswordRequest,
): Promise<void> {
  await apiClient.patch("/api/profile/change-password", request);
}

export async function changeEmail(request: ChangeEmailRequest): Promise<void> {
  await apiClient.patch("/api/profile/change-email", request);
}