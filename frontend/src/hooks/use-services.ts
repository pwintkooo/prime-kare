import { useQuery } from "@tanstack/react-query";
import { getServices, getServiceById, getServiceBySlug } from "@/api/services";

export function useServices() {
  return useQuery({
    queryKey: ["services"],
    queryFn: getServices,
  });
}

export function useServiceById(id: number) {
  return useQuery({
    queryKey: ["services", id],
    queryFn: () => getServiceById(id),
    enabled: !!id,
  });
}

export function useServiceBySlug(slug: string) {
  return useQuery({
    queryKey: ["services", slug],
    queryFn: () => getServiceBySlug(slug),
    enabled: !!slug,
  });
}