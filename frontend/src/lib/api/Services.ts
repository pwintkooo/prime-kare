import { apiClient } from "./client";

export interface Service {
  id: number;
  name: string;
  slug: string;
  description: string;
  price: number;
  estimatedMinutes: number;
  isActive: boolean;
  isDeleted: boolean;
  imageUrl: string | null;
  createdAt: string;
  updatedAt: string;
}

export async function getServices(): Promise<Service[]> {
  const response = await apiClient.get<Service[]>("/api/services");

  return response.data;
}

export async function getServiceById(id: number): Promise<Service> {
  const response = await apiClient.get<Service>(`/api/services/${id}`);

  return response.data;
}

export async function getServiceBySlug(slug: string): Promise<Service> {
  const response = await apiClient.get<Service>(`/api/services/${slug}`);

  return response.data;
}
