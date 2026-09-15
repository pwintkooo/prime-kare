"use client";

import Link from "next/link";
import { useParams } from "next/navigation";
import {
  AlertCircle,
  ArrowLeft,
  CalendarDays,
  Car,
  Clock,
  Wrench,
} from "lucide-react";

import { useBooking } from "@/hooks/use-bookings";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Separator } from "@/components/ui/separator";
import { BookingDetailsSkeleton } from "@/components/skeletons/BookingDetailsSkeleton";
import { formatBookingDate, formatBookingTime } from "@/utils/formatters";

export default function BookingDetailsPage() {
  const params = useParams<{ id: string }>();
  const id = Number(params.id);

  const { data: booking, isLoading, isError, refetch } = useBooking(id);

  if (isLoading) {
    return <BookingDetailsSkeleton />;
  }

  if (isError || !booking || Number.isNaN(id)) {
    return (
      <main className="container mx-auto flex min-h-[70vh] max-w-3xl items-center justify-center px-4 py-10">
        <Card className="w-full shadow-sm">
          <CardContent className="space-y-6 p-6 sm:p-8">
            <div className="flex size-14 items-center justify-center rounded-full bg-destructive/10">
              <AlertCircle className="size-7 text-destructive" />
            </div>

            <Alert variant="destructive">
              <AlertCircle />
              <AlertTitle>Unable to load booking</AlertTitle>
              <AlertDescription>
                This booking may not exist, or something went wrong while
                retrieving its details.
              </AlertDescription>
            </Alert>

            <div className="flex flex-col gap-3 sm:flex-row">
              <Button onClick={() => refetch()}>Try again</Button>

              <Button variant="outline">
                <Link href="/bookings">
                  <ArrowLeft />
                  Back to bookings
                </Link>
              </Button>
            </div>
          </CardContent>
        </Card>
      </main>
    );
  }

  return (
    <main className="container mx-auto max-w-4xl px-4 py-8 sm:py-12">
      <Button variant="ghost" className="mb-6 px-0">
        <Link href="/dashboard/bookings">
          <ArrowLeft />
          Back to bookings
        </Link>
      </Button>

      <div className="space-y-6">
        <div className="flex flex-col justify-between gap-4 sm:flex-row sm:items-start">
          <div className="space-y-2">
            <h1 className="text-3xl font-bold tracking-tight">
              {booking.serviceName}
            </h1>

            <p className="text-muted-foreground">
              View the details and current status of your appointment.
            </p>
          </div>

          <Badge variant="secondary" className="w-fit px-3 py-1 text-sm">
            {booking.status}
          </Badge>
        </div>

        <Separator />

        <div className="grid gap-6 md:grid-cols-2">
          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2 text-lg">
                <Car className="size-5 text-primary" />
                Vehicle
              </CardTitle>
            </CardHeader>

            <CardContent className="space-y-4">
              <DetailRow
                label="Vehicle"
                value={`${booking.vehicleMake} ${booking.vehicleModel}`}
              />

              <Separator />

              <DetailRow
                label="Plate number"
                value={booking.vehiclePlateNumber}
              />
            </CardContent>
          </Card>

          <Card>
            <CardHeader>
              <CardTitle className="flex items-center gap-2 text-lg">
                <Wrench className="size-5 text-primary" />
                Appointment
              </CardTitle>
            </CardHeader>

            <CardContent className="space-y-4">
              <DetailRow
                icon={<CalendarDays className="size-4" />}
                label="Date"
                value={formatBookingDate(booking.bookingDate)}
              />

              <Separator />

              <DetailRow
                icon={<Clock className="size-4" />}
                label="Time"
                value={formatBookingTime(booking.bookingTime)}
              />
            </CardContent>
          </Card>
        </div>
      </div>
    </main>
  );
}

type DetailRowProps = {
  label: string;
  value: string;
  icon?: React.ReactNode;
};

function DetailRow({ label, value, icon }: DetailRowProps) {
  return (
    <div className="flex items-center justify-between gap-4">
      <div className="flex items-center gap-2 text-sm text-muted-foreground">
        {icon}
        <span>{label}</span>
      </div>

      <p className="text-right font-medium">{value}</p>
    </div>
  );
}