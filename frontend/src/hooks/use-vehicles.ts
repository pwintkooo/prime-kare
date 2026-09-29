import { useQuery } from "@tanstack/react-query";
import { getVehicles, getVehicle } from "@/api/vehicles";

export function useVehicles(enabled = true) {
  return useQuery({
    queryKey: ["vehicles"],
    queryFn: getVehicles,
    enabled,
  });
}

export function useVehicle(id: number) {
  return useQuery({
    queryKey: ["vehicles", id],
    queryFn: () => getVehicle(id),
    enabled: Number.isInteger(id) && id > 0,
  });
}
