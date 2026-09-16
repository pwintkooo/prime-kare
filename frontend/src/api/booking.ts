import { apiClient } from "./client";
import {
  Booking,
  CreateBookingRequest,
  UpdateBookingRequest,
  UpdateBookingStatusRequest,
  BookingAvailability,
} from "@/types/booking";

export async function getBookings(): Promise<Booking[]> {
  const response = await apiClient.get<Booking[]>("/api/bookings");

  return response.data;
}

export async function getBooking(id: number): Promise<Booking> {
  const response = await apiClient.get<Booking>(`/api/bookings/${id}`);

  return response.data;
}

export async function createBooking(
  request: CreateBookingRequest,
): Promise<Booking> {
  const response = await apiClient.post<Booking>("/api/bookings", request);

  return response.data;
}

export async function updateBooking(
  request: UpdateBookingRequest,
  id: number,
): Promise<void> {
  await apiClient.put(`/api/bookings/${id}`, request);
}

export async function updateBookingStatus(
  request: UpdateBookingStatusRequest,
  id: number,
): Promise<void> {
  await apiClient.patch(`/api/bookings/${id}/status`, request);
}

export async function getBookingAvailability(
  serviceId: number,
  date: string,
  bookingId?: number,
): Promise<BookingAvailability> {
  const response = await apiClient.get<BookingAvailability>(
    "/api/bookings/availability",
    {
      params: {
        serviceId,
        date,
        bookingId,
      },
    },
  );

  return response.data;
}
