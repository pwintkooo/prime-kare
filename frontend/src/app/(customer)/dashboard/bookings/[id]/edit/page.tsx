"use client";

import { useEffect, useState } from "react";
import { useParams, useRouter } from "next/navigation";
import { useForm, useWatch } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { AlertCircle } from "lucide-react";

import { useBooking, useUpdateBooking } from "@/hooks/use-bookings";
import { useVehicles } from "@/hooks/use-vehicles";
import { useServices } from "@/hooks/use-services";
import { useBookingAvailability } from "@/hooks/use-booking-availability";

import { ApiError } from "@/api/apiError";

import {
  updateBookingSchema,
  UpdateBookingFormData,
} from "@/validations/booking";

import { formatBookingTime, formatTimeValue } from "@/utils/formatters";

import { Button } from "@/components/ui/button";
import { Alert, AlertDescription, AlertTitle } from "@/components/ui/alert";
import { EditBookingSkeleton } from "@/components/skeletons/EditBookingSkeleton";

export default function EditBookingPage() {
  const params = useParams<{ id: string }>();
  const router = useRouter();

  const id = Number(params.id);

  const [serverError, setServerError] = useState("");

  const {
    data: booking,
    isLoading: isLoadingBooking,
    isError: isBookingError,
  } = useBooking(id);

  const { data: vehicles, isLoading: isLoadingVehicles } = useVehicles();

  const { data: services, isLoading: isLoadingServices } = useServices();

  const updateBooking = useUpdateBooking();

  const {
    register,
    handleSubmit,
    reset,
    control,
    setValue,
    getValues,
    formState: { errors },
  } = useForm<UpdateBookingFormData>({
    resolver: zodResolver(updateBookingSchema),
  });

  const serviceId = useWatch({
    control,
    name: "serviceId",
  });

  const bookingDate = useWatch({
    control,
    name: "bookingDate",
  });

  const {
    data: availability,
    isLoading: isLoadingAvailability,
    isError: isAvailabilityError,
  } = useBookingAvailability(serviceId, bookingDate, id);

  useEffect(() => {
    if (!booking || !availability) {
      return;
    }

    const currentFormTime = getValues("bookingTime");

    // Don't overwrite a time the user has already selected.
    if (currentFormTime) {
      return;
    }

    const existingTime = formatTimeValue(booking.bookingTime);

    const isAvailable = availability.availableTimes.includes(existingTime);

    if (isAvailable) {
      setValue("bookingTime", existingTime);
    }
  }, [booking, availability, getValues, setValue]);

  // Fill the form with the existing booking
  useEffect(() => {
    if (!booking) {
      return;
    }

    reset({
      vehicleId: booking.vehicleId,
      serviceId: booking.serviceId,
      bookingDate: booking.bookingDate,
      bookingTime: formatTimeValue(booking.bookingTime),
      notes: booking.notes ?? "",
    });
  }, [booking, reset]);

  const onSubmit = async (data: UpdateBookingFormData) => {
    setServerError("");

    try {
      await updateBooking.mutateAsync({
        id,
        request: {
          vehicleId: data.vehicleId,
          serviceId: data.serviceId,
          bookingDate: data.bookingDate,
          bookingTime: data.bookingTime,
          notes: data.notes || null,
        },
      });

      router.push(`/dashboard/bookings/${id}`);
    } catch (error) {
      if (error instanceof ApiError) {
        setServerError(error.message);
        return;
      }

      setServerError("Unable to update booking.");
    }
  };

  if (isLoadingBooking || isLoadingVehicles || isLoadingServices) {
    return <EditBookingSkeleton />;
  }

  if (isBookingError || !booking || Number.isNaN(id)) {
    return (
      <main className="container mx-auto max-w-3xl px-4 py-10">
        <Alert variant="destructive">
          <AlertCircle />

          <AlertTitle>Unable to load booking</AlertTitle>

          <AlertDescription>
            This booking may not exist, or something went wrong while retrieving
            its details.
          </AlertDescription>
        </Alert>

        <Button
          variant="outline"
          className="mt-4"
          onClick={() => router.push("/dashboard/bookings")}
        >
          Back to Bookings
        </Button>
      </main>
    );
  }

  if (booking.status !== "pending") {
    return (
      <main className="container mx-auto max-w-3xl px-4 py-10">
        <Alert>
          <AlertCircle />

          <AlertTitle>Booking cannot be edited</AlertTitle>

          <AlertDescription>
            Only pending bookings can be edited.
          </AlertDescription>
        </Alert>

        <Button
          className="mt-4"
          onClick={() => router.push(`/dashboard/bookings/${id}`)}
        >
          Back to Booking
        </Button>
      </main>
    );
  }

  return (
    <main className="container mx-auto max-w-3xl px-4 py-8 sm:py-12">
      {/* Header */}
      <div>
        <h1 className="text-3xl font-bold tracking-tight">Edit Booking</h1>

        <p className="mt-2 text-muted-foreground">
          Update your appointment details.
        </p>
      </div>

      <form onSubmit={handleSubmit(onSubmit)} className="mt-8 space-y-6">
        {/* Vehicle + Service */}
        <div className="grid gap-6 sm:grid-cols-2">
          {/* Vehicle */}
          <div className="space-y-2">
            <label htmlFor="vehicleId" className="text-sm font-medium">
              Vehicle
            </label>

            <select
              id="vehicleId"
              {...register("vehicleId", {
                valueAsNumber: true,
              })}
              className="w-full rounded-md border bg-background px-3 py-2 text-sm"
            >
              <option value="">Select a vehicle</option>

              {vehicles?.map((vehicle) => (
                <option key={vehicle.id} value={vehicle.id}>
                  {vehicle.make} {vehicle.model} - {vehicle.plateNumber}
                </option>
              ))}
            </select>

            {errors.vehicleId && (
              <p className="text-sm text-destructive">
                {errors.vehicleId.message}
              </p>
            )}
          </div>

          {/* Service */}
          <div className="space-y-2">
            <label htmlFor="serviceId" className="text-sm font-medium">
              Service
            </label>

            <select
              id="serviceId"
              {...register("serviceId", {
                valueAsNumber: true,
                onChange: () => {
                  setValue("bookingTime", "");
                },
              })}
              className="w-full rounded-md border bg-background px-3 py-2 text-sm"
            >
              <option value="">Select a service</option>

              {services?.map((service) => (
                <option key={service.id} value={service.id}>
                  {service.name}
                </option>
              ))}
            </select>

            {errors.serviceId && (
              <p className="text-sm text-destructive">
                {errors.serviceId.message}
              </p>
            )}
          </div>
        </div>

        {/* Date + Time */}
        <div className="grid gap-6 sm:grid-cols-2">
          {/* Date */}
          <div className="space-y-2">
            <label htmlFor="bookingDate" className="text-sm font-medium">
              Date
            </label>

            <input
              id="bookingDate"
              type="date"
              {...register("bookingDate")}
              className="w-full rounded-md border bg-background px-3 py-2 text-sm"
            />

            {errors.bookingDate && (
              <p className="text-sm text-destructive">
                {errors.bookingDate.message}
              </p>
            )}
          </div>

          {/* Time */}
          <div className="space-y-2">
            <label htmlFor="bookingTime" className="text-sm font-medium">
              Time
            </label>

            {isLoadingAvailability || !availability ? (
              <div className="flex h-10 items-center rounded-md border bg-muted/30 px-3 text-sm text-muted-foreground">
                Loading available times...
              </div>
            ) : (
              <select
                id="bookingTime"
                {...register("bookingTime")}
                disabled={!serviceId || !bookingDate}
                className="w-full rounded-md border bg-background px-3 py-2 text-sm disabled:cursor-not-allowed disabled:opacity-50"
              >
                <option value="">Select a time</option>

                {availability.availableTimes.map((time) => (
                  <option key={time} value={time}>
                    {formatBookingTime(time)}
                  </option>
                ))}
              </select>
            )}

            {errors.bookingTime && (
              <p className="text-sm text-destructive">
                {errors.bookingTime.message}
              </p>
            )}

            {isAvailabilityError && (
              <p className="text-sm text-destructive">
                Unable to load available times.
              </p>
            )}

            {!isLoadingAvailability &&
              !isAvailabilityError &&
              availability?.availableTimes.length === 0 && (
                <p className="text-sm text-muted-foreground">
                  No available times for this date.
                </p>
              )}
          </div>
        </div>

        {/* Notes */}
        <div className="space-y-2">
          <label htmlFor="notes" className="text-sm font-medium">
            Notes
          </label>

          <textarea
            id="notes"
            rows={4}
            placeholder="Add any notes for your appointment..."
            {...register("notes")}
            className="w-full resize-none rounded-md border bg-background px-3 py-2 text-sm"
          />

          {errors.notes && (
            <p className="text-sm text-destructive">{errors.notes.message}</p>
          )}
        </div>

        {/* Server Error */}
        {serverError && (
          <Alert variant="destructive">
            <AlertCircle />

            <AlertTitle>Unable to update booking</AlertTitle>

            <AlertDescription>{serverError}</AlertDescription>
          </Alert>
        )}

        {/* Actions */}
        <div className="flex justify-end gap-3">
          <Button
            type="button"
            variant="outline"
            disabled={updateBooking.isPending}
            onClick={() => router.push(`/dashboard/bookings/${id}`)}
          >
            Cancel
          </Button>

          <Button type="submit" disabled={updateBooking.isPending}>
            {updateBooking.isPending ? "Saving..." : "Save Changes"}
          </Button>
        </div>
      </form>
    </main>
  );
}
