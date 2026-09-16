import { apiClient } from "./client";
import {
  SignUpRequest,
  SignUpResponse,
  SignInRequest,
  SignInResponse,
  ExternalAuthResponse,
  ReactivateAccountRequest,
} from "@/types/auth";

const API_URL = process.env.NEXT_PUBLIC_API_URL;

export async function SignUp(request: SignUpRequest): Promise<SignUpResponse> {
  const response = await apiClient.post<SignUpResponse>(
    "/api/auth/sign-up",
    request,
  );

  return response.data;
}

export async function SignIn(request: SignInRequest): Promise<SignInResponse> {
  const response = await apiClient.post<SignInResponse>(
    "/api/auth/sign-in",
    request,
  );

  return response.data;
}

export function SignInWithGoogle() {
  window.location.href = `${API_URL}/api/external-auth/google`;
}

export async function exchangeExternalAuthCode(
  code: string,
): Promise<ExternalAuthResponse> {
  const response = await apiClient.post<ExternalAuthResponse>(
    "/api/external-auth/exchange",
    code,
    {
      headers: {
        "Content-Type": "application/json",
      },
    },
  );

  return response.data;
}

export async function verifyAndLinkGoogle(
  code: string,
  password: string,
): Promise<ExternalAuthResponse> {
  const response = await apiClient.post<ExternalAuthResponse>(
    "/api/external-auth/link/verify",
    {
      code,
      password,
    },
  );

  return response.data;
}

export async function reactivateAccount(
  request: ReactivateAccountRequest,
): Promise<SignInResponse> {
  const response = await apiClient.post<SignInResponse>(
    "/api/auth/reactivate",
    request,
  );

  return response.data;
}

export async function reactivateExternalAccount(
  code: string,
): Promise<SignInResponse> {
  const response = await apiClient.post<SignInResponse>(
    "/api/external-auth/reactivate",
    { code },
  );

  return response.data;
}