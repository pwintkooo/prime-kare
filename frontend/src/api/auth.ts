import { apiClient } from "./client";
import {
  SignUpRequest,
  SignUpResponse,
  SignInRequest,
  SignInResponse,
  ExternalAuthResponse,
} from "@/types/auth";

const API_URL = process.env.NEXT_PUBLIC_API_URL;

export async function SignUp(request: SignUpRequest): Promise<SignUpResponse> {
  const response = await fetch(`${API_URL}/api/auth/sign-up`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(request),
  });

  if (!response.ok) {
    const errorData = await response.json();

    throw new Error(errorData.message || "Failed to create account.");
  }

  return response.json();
}

export async function SignIn(request: SignInRequest): Promise<SignInResponse> {
  const response = await fetch(`${API_URL}/api/auth/sign-in`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
    },
    body: JSON.stringify(request),
  });

  if (!response.ok) {
    const message = await response.text();

    throw new Error(message || "Failed to sign in.");
  }

  return response.json();
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
