import { useQuery } from "@tanstack/react-query";
import { getBookingAvailability } from "@/api/booking";

export function useBookingAvailability(serviceId: number | null, date: string) {
  return useQuery({
    queryKey: ["booking-availability", serviceId, date],
    queryFn: () => getBookingAvailability(serviceId!, date),
    enabled: serviceId !== null && serviceId > 0 && date !== "",
  });
}