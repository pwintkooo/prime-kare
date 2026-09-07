import { useQuery } from "@tanstack/react-query";
import { getBookings, getBooking } from "@/api/booking";

export function useBookings() {
  return useQuery({
    queryKey: ["bookings"],
    queryFn: getBookings,
  });
}

export function useBooking(id: number) {
  return useQuery({
    queryKey: ["bookings", id],
    queryFn: () => getBooking(id),
    enabled: !!id,
  });
}