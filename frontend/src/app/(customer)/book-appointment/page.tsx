"use client";

import { useState } from "react";
import { useSearchParams, useRouter } from "next/navigation";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Label } from "@/components/ui/label";
import { useServices } from "@/hooks/use-services";
import { useVehicles } from "@/hooks/use-vehicles";
import { createBooking } from "@/api/booking";
import { useBookingAvailability } from "@/hooks/use-booking-availability";
import { createBookingSchema } from "@/validations/booking";
import { CreateBookingFormErrors } from "@/types/booking";

export default function BookAppointmentPage() {
  const router = useRouter();
  const queryClient = useQueryClient();

  const searchParams = useSearchParams();
  const vehicleParam = searchParams.get("vehicle");
  const serviceSlug = searchParams.get("service");

  const [bookingDate, setBookingDate] = useState<string>("");
  const [bookingTime, setBookingTime] = useState<string>("");
  const [notes, setNotes] = useState<string>("");
  const [formErrors, setFormErrors] = useState<CreateBookingFormErrors>({});

  const {
    data: vehicles,
    isError: isVehiclesError,
    isLoading: isLoadingVehicles,
  } = useVehicles();

  const initialVehicle = vehicles?.find(
    (vehicle) => vehicle.plateNumber === vehicleParam,
  );

  const initialVehicleId = initialVehicle?.id ?? null;

  const [vehicleId, setVehicleId] = useState<number | null>(initialVehicleId);

  const {
    data: services,
    isError: isServicesError,
    isLoading: isLoadingServices,
  } = useServices();

  const initialService = services?.find(
    (service) => service.slug === serviceSlug,
  );

  const initialServiceId = initialService?.id ?? null;

  const [serviceId, setServiceId] = useState<number | null>(initialServiceId);

  const {
    data: availability,
    isLoading: isLoadingAvailability,
    isError: isAvailabilityError,
  } = useBookingAvailability(serviceId, bookingDate);

  const createMutation = useMutation({
    mutationFn: createBooking,

    onSuccess: (booking) => {
      queryClient.invalidateQueries({
        queryKey: ["bookings"],
      });

      router.push(`/dashboard/bookings/${booking.id}`);
    },
  });

  const handleSubmit = () => {
    setFormErrors({});

    const result = createBookingSchema.safeParse({
      vehicleId,
      serviceId,
      bookingDate,
      bookingTime,
      notes,
    });

    if (!result.success) {
      const errors: CreateBookingFormErrors = {};

      result.error.issues.forEach((issue) => {
        const field = issue.path[0] as keyof CreateBookingFormErrors;

        if (!errors[field]) {
          errors[field] = issue.message;
        }
      });

      setFormErrors(errors);
      return;
    }

    createMutation.mutate({
      vehicleId: result.data.vehicleId,
      serviceId: result.data.serviceId,
      bookingDate: result.data.bookingDate,
      bookingTime: result.data.bookingTime,
      notes: result.data.notes.trim() || null,
    });
  };

  const isWeekend = (date: string) => {
    if (!date) return false;

    const [year, month, day] = date.split("-").map(Number);

    const selectedDate = new Date(year, month - 1, day);

    const dayOfWeek = selectedDate.getDay();

    return dayOfWeek === 0 || dayOfWeek === 6;
  };

  return (
    <div className="container mx-auto max-w-3xl px-4 py-10">
      <div className="mb-8">
        <h1 className="text-3xl font-semibold">Book an Appointment</h1>

        <p className="mt-2 text-muted-foreground">
          Select your vehicle, service, date, and preferred appointment time.
        </p>
      </div>

      <Card>
        <CardHeader>
          <CardTitle>Appointment Details</CardTitle>
        </CardHeader>

        <CardContent className="space-y-6">
          {/* Vehicle */}
          <div className="space-y-2">
            <Label htmlFor="vehicle">Vehicle</Label>

            <Select
              value={vehicleId?.toString() ?? ""}
              onValueChange={(value) => {
                setVehicleId(Number(value));
                setBookingTime("");
              }}
              disabled={
                isLoadingVehicles || isVehiclesError || !vehicles?.length
              }
            >
              <SelectTrigger id="vehicle">
                <SelectValue
                  placeholder={
                    isLoadingVehicles
                      ? "Loading vehicles..."
                      : "Select a vehicle"
                  }
                >
                  {vehicleId
                    ? vehicles?.find((vehicle) => vehicle.id === vehicleId)
                        ?.plateNumber
                    : undefined}
                </SelectValue>
              </SelectTrigger>

              <SelectContent>
                {vehicles?.map((vehicle) => (
                  <SelectItem key={vehicle.id} value={vehicle.id.toString()}>
                    {vehicle.plateNumber}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>

            {isVehiclesError && (
              <p className="text-sm text-destructive">
                Failed to load vehicles.
              </p>
            )}

            {!isLoadingVehicles &&
              !isVehiclesError &&
              vehicles?.length === 0 && (
                <p className="text-sm text-muted-foreground">
                  No vehicles are currently available.
                </p>
              )}
          </div>

          {formErrors.vehicleId && (
            <p className="text-sm text-red-500">{formErrors.vehicleId}</p>
          )}

          {/* service */}
          <div className="space-y-2">
            <Label htmlFor="service">Service</Label>

            <Select
              value={serviceId?.toString() ?? ""}
              onValueChange={(value) => {
                setServiceId(Number(value));
                setBookingDate("");
              }}
              disabled={
                isLoadingServices || isServicesError || !services?.length
              }
            >
              <SelectTrigger id="service">
                <SelectValue
                  placeholder={
                    isLoadingServices
                      ? "Loading services..."
                      : "Select a service"
                  }
                >
                  {serviceId
                    ? services?.find((service) => service.id === serviceId)
                        ?.name
                    : undefined}
                </SelectValue>
              </SelectTrigger>

              <SelectContent>
                {services?.map((service) => (
                  <SelectItem key={service.id} value={service.id.toString()}>
                    {service.name}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>

            {isServicesError && (
              <p className="text-sm text-destructive">
                Failed to load services.
              </p>
            )}

            {!isLoadingServices &&
              !isServicesError &&
              services?.length === 0 && (
                <p className="text-sm text-muted-foreground">
                  No services are currently available.
                </p>
              )}
          </div>

          {formErrors.serviceId && (
            <p className="text-sm text-red-500">{formErrors.serviceId}</p>
          )}

          {/* Date */}
          <div className="space-y-2">
            <Label htmlFor="booking-date">Date</Label>

            <input
              id="booking-date"
              type="date"
              value={bookingDate}
              onChange={(event) => {
                const date = event.target.value;

                setBookingTime("");

                if (isWeekend(date)) {
                  setBookingDate("");
                  setFormErrors((prev) => ({
                    ...prev,
                    bookingDate:
                      "The workshop is closed on Saturdays and Sundays.",
                  }));
                  return;
                }

                setFormErrors((prev) => ({
                  ...prev,
                  bookingDate: undefined,
                }));
                setBookingDate(date);
              }}
              min={new Date().toISOString().split("T")[0]}
              className="flex h-9 w-full rounded-md border border-input bg-transparent px-3 py-1 text-sm shadow-xs outline-none focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:ring-[3px]"
            />
          </div>

          {formErrors.bookingDate && (
            <p className="text-sm text-destructive">{formErrors.bookingDate}</p>
          )}

          {/* Time */}
          <div className="space-y-2">
            <Label>Available Time</Label>

            {!serviceId || !bookingDate ? (
              <p className="text-sm text-muted-foreground">
                Select a service and date to see available times.
              </p>
            ) : isLoadingAvailability ? (
              <p className="text-sm text-muted-foreground">
                Checking availability...
              </p>
            ) : isAvailabilityError ? (
              <p className="text-sm text-destructive">
                Failed to load available times.
              </p>
            ) : availability?.availableTimes.length === 0 ? (
              <p className="text-sm text-muted-foreground">
                No appointment times are available for this date.
              </p>
            ) : (
              <div className="grid grid-cols-2 gap-2 sm:grid-cols-3">
                {availability?.availableTimes.map((time) => (
                  <Button
                    key={time}
                    type="button"
                    variant={bookingTime === time ? "default" : "outline"}
                    onClick={() => setBookingTime(time)}
                  >
                    {time}
                  </Button>
                ))}
              </div>
            )}
          </div>

          {formErrors.bookingTime && (
            <p className="text-sm text-destructive">{formErrors.bookingTime}</p>
          )}

          {/* Notes */}
          <div className="space-y-2">
            <Label htmlFor="notes">
              Notes
              <span className="text-xs text-muted-foreground">(Optional)</span>
            </Label>

            <textarea
              id="notes"
              value={notes}
              onChange={(event) => setNotes(event.target.value)}
              placeholder="Add any additional information about your appointment..."
              rows={4}
              className="flex w-full rounded-md border border-input bg-transparent px-3 py-2 text-sm shadow-xs outline-none placeholder:text-muted-foreground focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:ring-[3px]"
            />
          </div>

          {formErrors.notes && (
            <p className="text-sm text-destructive">{formErrors.notes}</p>
          )}

          {createMutation.isError && (
            <div className="rounded-md bg-red-50 p-3 text-sm text-red-600">
              {createMutation.error.message}
            </div>
          )}

          <div className="flex justify-end">
            <Button
              type="button"
              onClick={handleSubmit}
              disabled={createMutation.isPending}
            >
              {createMutation.isPending
                ? "Requesting..."
                : "Request Appointment"}
            </Button>
          </div>
        </CardContent>
      </Card>
    </div>
  );
}
