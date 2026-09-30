import { apiClient } from "./client";
import {
  Booking,
  CreateBookingRequest,
  UpdateBookingRequest,
  UpdateBookingStatusRequest,
  BookingAvailability,
  CreateBookingResponse,
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
): Promise<CreateBookingResponse> {
  const response = await apiClient.post<CreateBookingResponse>(
    "/api/bookings",
    request,
  );

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

export async function downloadBookingConfirmation(id: number) {
  const response = await apiClient.get(`/api/bookings/${id}/confirmation`, {
    responseType: "blob",
  });

  return response.data;
}

export async function downloadGuestBookingConfirmation(
  token: string,
): Promise<Blob> {
  const response = await apiClient.get("/api/bookings/guest/confirmation", {
    params: {
      token,
    },
    responseType: "blob",
  });

  return response.data;
}