"use client";

import { useParams } from "next/navigation";
import { useBooking } from "@/hooks/use-bookings";

export default function BookingDetailsPage() {
  const params = useParams();

  const id = Number(params.id);

  const { data: booking, isLoading, isError } = useBooking(id);

  if (isLoading) {
    return <p>Loading booking...</p>;
  }

  if (isError || !booking) {
    return <p>Booking not found.</p>;
  }

  return (
    <div>
      <h1 className="text-2xl font-semibold">{booking.serviceName}</h1>

      <p className="text-muted-foreground">
        {booking.vehicleMake} {booking.vehicleModel} ·{" "}
        {booking.vehiclePlateNumber}
      </p>
    </div>
  );
}