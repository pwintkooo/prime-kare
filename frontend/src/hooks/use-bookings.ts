import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";
import {
  getBookings,
  getBooking,
  updateBooking,
  updateBookingStatus,
} from "@/api/booking";
import {
  UpdateBookingRequest,
  UpdateBookingStatusRequest,
} from "@/types/booking";

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

export function useUpdateBooking() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      id,
      request,
    }: {
      id: number;
      request: UpdateBookingRequest;
    }) => updateBooking(request, id),

    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({
        queryKey: ["bookings"],
      });

      queryClient.invalidateQueries({
        queryKey: ["bookings", variables.id],
      });
    },
  });
}

export function useUpdateBookingStatus() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: ({
      id,
      request,
    }: {
      id: number;
      request: UpdateBookingStatusRequest;
    }) => updateBookingStatus(request, id),

    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({
        queryKey: ["bookings"],
      });

      queryClient.invalidateQueries({
        queryKey: ["bookings", variables.id],
      });
    },
  });
}