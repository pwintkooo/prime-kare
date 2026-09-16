import { useQuery } from "@tanstack/react-query";
import { getBookingAvailability } from "@/api/booking";

export function useBookingAvailability(
  serviceId: number | null,
  date: string,
  bookingId?: number,
) {
  return useQuery({
    queryKey: ["booking-availability", serviceId, date, bookingId],
    queryFn: () => getBookingAvailability(serviceId!, date, bookingId),
    enabled: serviceId !== null && serviceId > 0 && date !== "",
  });
}