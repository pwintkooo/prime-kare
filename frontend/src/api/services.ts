import { apiClient } from "./client";
import { Service } from "@/types/service";

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