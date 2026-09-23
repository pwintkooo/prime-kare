import { z } from "zod";

export const createContactSchema = z.object({
  name: z.string().min(1, "Name is required."),

  email: z
    .string()
    .trim()
    .min(1, "Email is required.")
    .email("Email format is invalid.")
    .regex(
      /^[^@\s]+@[^@\s]+\.[A-Za-z]{2,}$/,
      "Email must contain a valid domain.",
    )
    .max(254, "Email must not exceed 254 characters."),

  phone: z
    .string()
    .trim()
    .min(1, "Phone number is required")
    .max(20, "Phone number must not exceed 20 characters.")
    .refine(
      (value) => {
        if (value === "") {
          return true;
        }

        return /^\+?[0-9\s\-()]+$/.test(value);
      },
      {
        message: "Phone number format is invalid.",
      },
    )
    .refine(
      (value) => {
        if (value === "") {
          return true;
        }

        const digitCount = value.replace(/\D/g, "").length;

        return digitCount >= 7;
      },
      {
        message: "Phone number must contain at least 7 digits.",
      },
    ),

  message: z.string().min(1, "Message is required."),
});

export type CreateContactFormData = z.infer<typeof createContactSchema>;
