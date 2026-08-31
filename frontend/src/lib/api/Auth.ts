import { apiClient } from "./client";

export interface SignUpRequest {
  name: string;
  email: string;
  phone: string;
  password: string;
}

export interface SignUpResponse {
  id: number;
  email: string;
  role: string;
  status: string;
  createAt: string;
  customerId: number;
  customerName: string;
  customerPhone: string;
}

export interface SignInRequest {
  email: string;
  password: string;
}

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

export async function SignIn(request: SignInRequest) {
  const response = await apiClient.post("/api/auth/sign-in", request);

  return response.data;
}
