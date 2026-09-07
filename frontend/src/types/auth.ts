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

export interface User {
  id: number;
  name: string;
  email: string;
  role: string;
}

export interface SignInResponse {
  token: string;
  user: User;
}

export interface ExternalAuthResponse {
  token: string;
  user: User;
}