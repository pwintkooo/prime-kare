import { z } from "zod";

export const createVehicleSchema = z.object({
  plateNumber: z
    .string()
    .min(1, "Plate number is required.")
    .max(20, "Plate number must not exceed 20 characters.")
    .trim(),

  make: z
    .string()
    .min(1, "Make is required.")
    .max(50, "Make must not exceed 50 characters.")
    .trim(),

  model: z
    .string()
    .min(1, "Model is required.")
    .max(50, "Model must not exceed 50 characters.")
    .trim(),

  year: z
    .number()
    .int("Year must be a whole number.")
    .min(1900, "Please enter a valid year.")
    .max(new Date().getFullYear() + 1, "Please enter a valid year."),
});

export type CreateVehicleFormData = z.infer<typeof createVehicleSchema>;