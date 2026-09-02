import { apiClient } from "./client";

export interface Vehicle {
  id: number;
  plateNumber: string;
  make: string;
  model: string;
  year: number;
  status: string;
  createdAt: string;
  updatedAt: string;
  customerId: number;
}

export interface CreateVehicleRequest {
  plateNumber: string;
  make: string;
  model: string;
  year: number;
}

export async function getVehicles(): Promise<Vehicle[]> {
  const response = await apiClient.get<Vehicle[]>("/api/vehicles");

  return response.data;
}

export async function getVehicle(id: number): Promise<Vehicle> {
  const response = await apiClient.get<Vehicle>(`/api/vehicles/${id}`);

  return response.data;
}

export async function createVehicle(
  request: CreateVehicleRequest,
): Promise<Vehicle> {
  const response = await apiClient.post<Vehicle>("/api/vehicles", request);

  return response.data;
}
