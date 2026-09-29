import { z } from "zod";

export const createBookingSchema = z
  .object({
    isLoggedIn: z.boolean(),

    vehicleId: z.number().optional(),

    customerName: z
      .string()
      .trim()
      .max(100, "Name must be 100 characters or less.")
      .optional(),

    customerEmail: z
      .string()
      .trim()
      .max(255, "Email must be 255 characters or less.")
      .optional(),

    customerPhone: z
      .string()
      .trim()
      .max(20, "Phone number must be 20 characters or less.")
      .optional(),

    vehiclePlateNumber: z
      .string()
      .trim()
      .max(20, "Plate number must be 20 characters or less.")
      .optional(),

    vehicleMake: z
      .string()
      .trim()
      .max(50, "Vehicle make must be 50 characters or less.")
      .optional(),

    vehicleModel: z
      .string()
      .trim()
      .max(50, "Vehicle model must be 50 characters or less.")
      .optional(),

    serviceId: z.number().optional(),

    bookingDate: z.string().min(1, "Date is required."),

    bookingTime: z.string().min(1, "Appointment time is required."),

    notes: z
      .string()
      .max(1000, "Notes must be 1000 characters or less.")
      .optional(),
  })
  .superRefine((data, ctx) => {
    if (!data.serviceId) {
      ctx.addIssue({
        code: "custom",
        path: ["serviceId"],
        message: "Service is required.",
      });
    }

    if (data.isLoggedIn) {
      if (!data.vehicleId) {
        ctx.addIssue({
          code: "custom",
          path: ["vehicleId"],
          message: "Vehicle is required.",
        });
      }

      return;
    }

    if (!data.customerName) {
      ctx.addIssue({
        code: "custom",
        path: ["customerName"],
        message: "Name is required.",
      });
    }

    if (!data.customerEmail) {
      ctx.addIssue({
        code: "custom",
        path: ["customerEmail"],
        message: "Email is required.",
      });
    } else {
      const result = z.string().email().safeParse(data.customerEmail);

      if (!result.success) {
        ctx.addIssue({
          code: "custom",
          path: ["customerEmail"],
          message: "Please enter a valid email address.",
        });
      }
    }

    if (!data.customerPhone) {
      ctx.addIssue({
        code: "custom",
        path: ["customerPhone"],
        message: "Phone number is required.",
      });
    }

    if (!data.vehiclePlateNumber) {
      ctx.addIssue({
        code: "custom",
        path: ["vehiclePlateNumber"],
        message: "Plate number is required.",
      });
    }

    if (!data.vehicleMake) {
      ctx.addIssue({
        code: "custom",
        path: ["vehicleMake"],
        message: "Vehicle make is required.",
      });
    }

    if (!data.vehicleModel) {
      ctx.addIssue({
        code: "custom",
        path: ["vehicleModel"],
        message: "Vehicle model is required.",
      });
    }
  });

export type CreateBookingFormData = z.infer<typeof createBookingSchema>;

export const updateBookingSchema = z.object({
  vehicleId: z
    .number({
      error: "Please select a vehicle.",
    })
    .min(1, "Vehicle is required."),

  serviceId: z
    .number({
      error: "Please select a service.",
    })
    .min(1, "Service is required."),

  bookingDate: z.string().min(1, "Please select a date."),

  bookingTime: z.string().min(1, "Please select a time."),

  notes: z
    .string()
    .max(1000, "Notes must be 1000 characters or less.")
    .optional(),
});

export type UpdateBookingFormData = z.infer<typeof updateBookingSchema>;