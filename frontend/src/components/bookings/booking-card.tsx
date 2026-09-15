import Link from "next/link";
import { CalendarDays, Clock, Car } from "lucide-react";
import { Button } from "@/components/ui/button";
import { Booking } from "@/types/booking";
import { BookingStatusBadge } from "./booking-status-badge";
import { formatBookingDate, formatBookingTime } from "@/utils/formatters";

interface BookingCardProps {
  booking: Booking;
}

export function BookingCard({ booking }: BookingCardProps) {
  return (
    <div className="rounded-xl border bg-card p-5">
      <div className="flex items-start justify-between gap-4">
        <div>
          <h2 className="font-semibold">{booking.serviceName}</h2>

          <div className="mt-2 flex items-center gap-2 text-sm text-muted-foreground">
            <Car className="size-4" />

            <span>
              {booking.vehicleMake} {booking.vehicleModel}
            </span>

            <span>·</span>

            <span>{booking.vehiclePlateNumber}</span>
          </div>
        </div>

        <BookingStatusBadge status={booking.status} />
      </div>

      <div className="mt-5 flex flex-wrap gap-4 text-sm text-muted-foreground">
        <div className="flex items-center gap-2">
          <CalendarDays className="size-4" />

          <span>{formatBookingDate(booking.bookingDate)}</span>
        </div>

        <div className="flex items-center gap-2">
          <Clock className="size-4" />

          <span>{formatBookingTime(booking.bookingTime)}</span>
        </div>
      </div>
      <div className="mt-5 flex items-center justify-end border-t pt-4">
        <Button variant="outline" size="sm">
          <Link href={`/dashboard/bookings/${booking.id}`}>View Details</Link>
        </Button>
      </div>
    </div>
  );
}
