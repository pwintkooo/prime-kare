import { z } from "zod";

export const createBookingSchema = z.object({
  vehicleId: z
    .number({
      error: "Please select a vehicle.",
    })
    .min(1, "Vehicle is required."),

  serviceId: z
  .number({
    error: "Please select a service."
  })
  .min(1, "Service is required."),

  bookingDate: z.string().min(1, "Date is required."),

  bookingTime: z.string().min(1, "Appointment time is required."),

  notes: z.string().max(500, "Notes must be 500 characters or less."),
});

export type CreateBookingFormData = z.infer<typeof createBookingSchema>;
