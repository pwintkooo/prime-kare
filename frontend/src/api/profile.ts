import { apiClient } from "./client";
import {
  Profile,
  UpdateProfileRequest,
  ChangePasswordRequest,
  ChangeEmailRequest,
} from "@/types/profile";

export async function GetProfile(): Promise<Profile> {
  const response = await apiClient.get<Profile>("/api/profile");

  return response.data;
}

export async function UpdateProfile(
  request: UpdateProfileRequest,
): Promise<void> {
  await apiClient.put("/api/profile", request);
}

export async function ChangePassword(
  request: ChangePasswordRequest,
): Promise<void> {
  await apiClient.patch("/api/profile/change-password", request);
}

export async function ChangeEmail(request: ChangeEmailRequest): Promise<void> {
  await apiClient.patch("/api/profile/change-email", request);
}