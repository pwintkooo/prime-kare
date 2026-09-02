import axios from "axios";
import { apiClient } from "./client";
import { ApiError } from "./apiError";

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
  try {
    const response = await apiClient.post<Vehicle>("/api/vehicles", request);

    return response.data;
  } catch (error) {
    if (axios.isAxiosError(error)) {
      if (error.response?.status === 409) {
        throw new ApiError(
          "A vehicle with this plate number already exists.",
          409,
        );
      }

      if (error.response?.status === 400) {
        throw new ApiError("Please check the vehicle information.", 400);
      }
    }

    throw error;
  }
}
