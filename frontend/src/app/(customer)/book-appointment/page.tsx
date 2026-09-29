"use client";

import { Suspense, useEffect } from "react";
import { useSearchParams, useRouter } from "next/navigation";
import { useForm, useWatch } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import Link from "next/link";
import { Car, Loader2 } from "lucide-react";

import { Button } from "@/components/ui/button";
import { Card, CardContent, CardHeader, CardTitle } from "@/components/ui/card";
import { Input } from "@/components/ui/input";
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
import { useCreateBooking } from "@/hooks/use-bookings";
import { useBookingAvailability } from "@/hooks/use-booking-availability";
import { useAuthStore } from "@/store/authStore";

import {
  createBookingSchema,
  CreateBookingFormData,
} from "@/validations/booking";

function BookAppointmentContent() {
  const router = useRouter();
  const searchParams = useSearchParams();

  const token = useAuthStore((state) => state.token);
  const isLoggedIn = !!token;

  const vehicleParam = searchParams.get("vehicle");
  const serviceSlug = searchParams.get("service");

  const {
    data: vehicles,
    isError: isVehiclesError,
    isLoading: isLoadingVehicles,
  } = useVehicles(isLoggedIn);

  const {
    data: services,
    isError: isServicesError,
    isLoading: isLoadingServices,
  } = useServices();

  const initialVehicle = vehicles?.find(
    (vehicle) => vehicle.plateNumber === vehicleParam,
  );

  const initialService = services?.find(
    (service) => service.slug === serviceSlug,
  );

  const form = useForm<CreateBookingFormData>({
    resolver: zodResolver(createBookingSchema),

    defaultValues: {
      isLoggedIn,

      vehicleId: undefined,

      customerName: "",
      customerEmail: "",
      customerPhone: "",

      vehiclePlateNumber: "",
      vehicleMake: "",
      vehicleModel: "",

      serviceId: undefined,

      bookingDate: "",
      bookingTime: "",
      notes: "",
    },
  });

  useEffect(() => {
    if (initialService) {
      form.setValue("serviceId", initialService.id);
    }
  }, [initialService, form]);

  useEffect(() => {
    if (isLoggedIn && initialVehicle) {
      form.setValue("vehicleId", initialVehicle.id);
    }
  }, [initialVehicle, isLoggedIn, form]);

  const serviceId = useWatch({
    control: form.control,
    name: "serviceId",
  });

  const bookingDate = useWatch({
    control: form.control,
    name: "bookingDate",
  });

  const bookingTime = useWatch({
    control: form.control,
    name: "bookingTime",
  });

  const vehicleId = useWatch({
    control: form.control,
    name: "vehicleId",
  });

  const {
    data: availability,
    isLoading: isLoadingAvailability,
    isError: isAvailabilityError,
  } = useBookingAvailability(serviceId ?? null, bookingDate);

  const createMutation = useCreateBooking();

  const isSunday = (date: string) => {
    if (!date) return false;

    const [year, month, day] = date.split("-").map(Number);

    const selectedDate = new Date(year, month - 1, day);

    return selectedDate.getDay() === 0;
  };

  const handleSubmit = (data: CreateBookingFormData) => {
    if (data.isLoggedIn) {
      if (!data.vehicleId || !data.serviceId) {
        return;
      }

      createMutation.mutate(
        {
          vehicleId: data.vehicleId,
          serviceId: data.serviceId,
          bookingDate: data.bookingDate,
          bookingTime: data.bookingTime,
          notes: data.notes?.trim() || null,
        },
        {
          onSuccess: (booking) => {
            router.push(`/dashboard/bookings/${booking.id}`);
          },
        },
      );

      return;
    }

    if (
      !data.serviceId ||
      !data.customerName ||
      !data.customerEmail ||
      !data.customerPhone ||
      !data.vehiclePlateNumber ||
      !data.vehicleMake ||
      !data.vehicleModel
    ) {
      return;
    }

    createMutation.mutate(
      {
        serviceId: data.serviceId,
        bookingDate: data.bookingDate,
        bookingTime: data.bookingTime,

        customerName: data.customerName,
        customerEmail: data.customerEmail,
        customerPhone: data.customerPhone,

        vehiclePlateNumber: data.vehiclePlateNumber,
        vehicleMake: data.vehicleMake,
        vehicleModel: data.vehicleModel,

        notes: data.notes?.trim() || null,
      },
      {
        onSuccess: (booking) => {
          router.push(
            `/book-appointment/success?ref=${booking.referenceNumber}`,
          );
        },
      },
    );
  };

  return (
    <div className="container mx-auto max-w-3xl px-4 py-10">
      <div className="mb-8">
        <h1 className="text-3xl font-semibold">Book an Appointment</h1>

        <p className="mt-2 text-muted-foreground">
          {isLoggedIn
            ? "Select your vehicle, service, date, and preferred appointment time."
            : "Enter your contact and vehicle details to request an appointment."}
        </p>
      </div>

      <form onSubmit={form.handleSubmit(handleSubmit)}>
        <Card>
          <CardHeader>
            <CardTitle>Appointment Details</CardTitle>
          </CardHeader>

          <CardContent className="space-y-6">
            {/* Guest Contact Information */}
            {!isLoggedIn && (
              <>
                <div className="space-y-4">
                  <div>
                    <h3 className="font-medium">Contact Information</h3>

                    <p className="text-sm text-muted-foreground">
                      We&apos;ll use these details to contact you about your
                      appointment.
                    </p>
                  </div>

                  <div className="space-y-2">
                    <Label htmlFor="customerName">Name</Label>

                    <Input
                      id="customerName"
                      placeholder="Enter your name"
                      {...form.register("customerName")}
                    />

                    {form.formState.errors.customerName && (
                      <p className="text-sm text-destructive">
                        {form.formState.errors.customerName.message}
                      </p>
                    )}
                  </div>

                  <div className="space-y-2">
                    <Label htmlFor="customerEmail">Email</Label>

                    <Input
                      id="customerEmail"
                      type="email"
                      placeholder="Enter your email"
                      {...form.register("customerEmail")}
                    />

                    {form.formState.errors.customerEmail && (
                      <p className="text-sm text-destructive">
                        {form.formState.errors.customerEmail.message}
                      </p>
                    )}
                  </div>

                  <div className="space-y-2">
                    <Label htmlFor="customerPhone">Phone Number</Label>

                    <Input
                      id="customerPhone"
                      type="tel"
                      placeholder="Enter your phone number"
                      {...form.register("customerPhone")}
                    />

                    {form.formState.errors.customerPhone && (
                      <p className="text-sm text-destructive">
                        {form.formState.errors.customerPhone.message}
                      </p>
                    )}
                  </div>
                </div>

                {/* Guest Vehicle Information */}
                <div className="border-t pt-6">
                  <div className="mb-4">
                    <h3 className="font-medium">Vehicle Information</h3>

                    <p className="text-sm text-muted-foreground">
                      Tell us which vehicle you&apos;re bringing in.
                    </p>
                  </div>

                  <div className="space-y-4">
                    <div className="space-y-2">
                      <Label htmlFor="vehiclePlateNumber">Plate Number</Label>

                      <Input
                        id="vehiclePlateNumber"
                        placeholder="e.g. SBA1234A"
                        {...form.register("vehiclePlateNumber")}
                      />

                      {form.formState.errors.vehiclePlateNumber && (
                        <p className="text-sm text-destructive">
                          {form.formState.errors.vehiclePlateNumber.message}
                        </p>
                      )}
                    </div>

                    <div className="grid gap-4 sm:grid-cols-2">
                      <div className="space-y-2">
                        <Label htmlFor="vehicleMake">Make</Label>

                        <Input
                          id="vehicleMake"
                          placeholder="e.g. Toyota"
                          {...form.register("vehicleMake")}
                        />

                        {form.formState.errors.vehicleMake && (
                          <p className="text-sm text-destructive">
                            {form.formState.errors.vehicleMake.message}
                          </p>
                        )}
                      </div>

                      <div className="space-y-2">
                        <Label htmlFor="vehicleModel">Model</Label>

                        <Input
                          id="vehicleModel"
                          placeholder="e.g. Corolla"
                          {...form.register("vehicleModel")}
                        />

                        {form.formState.errors.vehicleModel && (
                          <p className="text-sm text-destructive">
                            {form.formState.errors.vehicleModel.message}
                          </p>
                        )}
                      </div>
                    </div>
                  </div>
                </div>
              </>
            )}

            {/* Registered Customer Vehicle */}
            {isLoggedIn && (
              <div className="space-y-2">
                <Label htmlFor="vehicle">Vehicle</Label>

                <Select
                  value={vehicleId?.toString() ?? ""}
                  onValueChange={(value) => {
                    form.setValue("vehicleId", Number(value), {
                      shouldValidate: true,
                    });

                    form.setValue("bookingTime", "");
                  }}
                >
                  <SelectTrigger id="vehicle">
                    <SelectValue placeholder="Select a vehicle">
                      {
                        vehicles?.find((vehicle) => vehicle.id === vehicleId)
                          ?.plateNumber
                      }
                    </SelectValue>
                  </SelectTrigger>

                  <SelectContent>
                    {vehicles?.map((vehicle) => (
                      <SelectItem
                        key={vehicle.id}
                        value={vehicle.id.toString()}
                      >
                        {vehicle.plateNumber}
                      </SelectItem>
                    ))}
                  </SelectContent>
                </Select>

                {form.formState.errors.vehicleId && (
                  <p className="text-sm text-destructive">
                    {form.formState.errors.vehicleId.message}
                  </p>
                )}

                {isVehiclesError && (
                  <p className="text-sm text-destructive">
                    Failed to load vehicles.
                  </p>
                )}

                {!isLoadingVehicles &&
                  !isVehiclesError &&
                  vehicles?.length === 0 && (
                    <div className="flex flex-col items-center justify-center rounded-xl border border-dashed px-4 py-10 text-center">
                      <div className="mb-4 flex size-12 items-center justify-center rounded-full bg-muted">
                        <Car className="size-6 text-muted-foreground" />
                      </div>

                      <h3 className="font-semibold">No vehicles added</h3>

                      <p className="mt-1 max-w-sm text-sm text-muted-foreground">
                        Add your vehicle before booking a service appointment.
                      </p>

                      <Button className="mt-5">
                        <Link href="/vehicles/new">Add Vehicle</Link>
                      </Button>
                    </div>
                  )}
              </div>
            )}

            {/* Service */}
            <div className="space-y-2">
              <Label htmlFor="service">Service</Label>

              <Select
                value={serviceId?.toString() ?? ""}
                onValueChange={(value) => {
                  form.setValue("serviceId", Number(value), {
                    shouldValidate: true,
                  });

                  form.setValue("bookingDate", "");
                  form.setValue("bookingTime", "");
                }}
              >
                <SelectTrigger id="service">
                  <SelectValue placeholder="Select a service">
                    {
                      services?.find((service) => service.id === serviceId)
                        ?.name
                    }
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

              {form.formState.errors.serviceId && (
                <p className="text-sm text-destructive">
                  {form.formState.errors.serviceId.message}
                </p>
              )}

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

            {/* Date */}
            <div className="space-y-2">
              <Label htmlFor="bookingDate">Date</Label>

              <Input
                id="bookingDate"
                type="date"
                min={new Date().toISOString().split("T")[0]}
                {...form.register("bookingDate", {
                  onChange: (event) => {
                    const date = event.target.value;

                    form.setValue("bookingTime", "");

                    if (isSunday(date)) {
                      form.setValue("bookingDate", "");

                      form.setError("bookingDate", {
                        message: "The workshop is closed on Sundays.",
                      });

                      return;
                    }

                    form.clearErrors("bookingDate");
                  },
                })}
              />

              {form.formState.errors.bookingDate && (
                <p className="text-sm text-destructive">
                  {form.formState.errors.bookingDate.message}
                </p>
              )}
            </div>

            {/* Available Time */}
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
                      onClick={() => {
                        form.setValue("bookingTime", time, {
                          shouldValidate: true,
                        });
                      }}
                    >
                      {time}
                    </Button>
                  ))}
                </div>
              )}

              {form.formState.errors.bookingTime && (
                <p className="text-sm text-destructive">
                  {form.formState.errors.bookingTime.message}
                </p>
              )}
            </div>

            {/* Notes */}
            <div className="space-y-2">
              <Label htmlFor="notes">
                Notes{" "}
                <span className="text-xs text-muted-foreground">
                  (Optional)
                </span>
              </Label>

              <textarea
                id="notes"
                rows={4}
                placeholder="Add any additional information about your appointment..."
                {...form.register("notes")}
                className="flex w-full rounded-md border border-input bg-transparent px-3 py-2 text-sm shadow-xs outline-none placeholder:text-muted-foreground focus-visible:border-ring focus-visible:ring-ring/50 focus-visible:ring-[3px]"
              />

              {form.formState.errors.notes && (
                <p className="text-sm text-destructive">
                  {form.formState.errors.notes.message}
                </p>
              )}
            </div>

            {/* API Error */}
            {createMutation.isError && (
              <div className="rounded-md bg-red-50 p-3 text-sm text-red-600">
                {createMutation.error.message}
              </div>
            )}

            {/* Submit */}
            <div className="flex justify-end">
              <Button type="submit" disabled={createMutation.isPending}>
                {createMutation.isPending
                  ? "Requesting..."
                  : "Request Appointment"}
              </Button>
            </div>
          </CardContent>
        </Card>
      </form>
    </div>
  );
}

export default function BookAppointmentPage() {
  return (
    <Suspense
      fallback={
        <div className="flex min-h-screen items-center justify-center bg-slate-950">
          <Loader2 className="h-6 w-6 animate-spin text-slate-400" />
        </div>
      }
    >
      <BookAppointmentContent />
    </Suspense>
  );
}
