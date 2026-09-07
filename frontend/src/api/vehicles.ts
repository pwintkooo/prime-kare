import { apiClient } from "./client";
import {
  Vehicle,
  CreateVehicleRequest,
  UpdateVehicleRequest,
} from "@/types/vehicle";

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

export async function updateVehicle(
  id: number,
  request: UpdateVehicleRequest,
): Promise<void> {
  await apiClient.put(`/api/vehicles/${id}`, request);
}

export async function deleteVehicle(id: number): Promise<void> {
  await apiClient.delete<Vehicle>(`/api/vehicles/${id}`);
}
