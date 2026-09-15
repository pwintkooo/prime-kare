import { z } from "zod";

export const updateProfileSchema = z.object({
  name: z
    .string()
    .trim()
    .min(1, "Name is required.")
    .max(100, "Name must not exceed 100 characters."),

  phone: z
    .string()
    .trim()
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
});

export type UpdateProfileFormData = z.infer<typeof updateProfileSchema>;

export const changePasswordSchema = z
  .object({
    currentPassword: z.string().min(1, "Current password is required."),

    newPassword: z
      .string()
      .min(1, "New password is required.")
      .min(8, "Password must be at least 8 characters.")
      .regex(/[A-Z]/, "Password must contain at least one uppercase letter.")
      .regex(/[a-z]/, "Password must contain at least one lowercase letter.")
      .regex(/[0-9]/, "Password must contain at least one number.")
      .regex(
        /[^a-zA-Z0-9]/,
        "Password must contain at least one special character.",
      ),

    confirmPassword: z.string().min(1, "Confirm password is required."),
  })
  .refine((data) => data.newPassword !== data.currentPassword, {
    message: "New password must be different from the current password.",
    path: ["newPassword"],
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    message: "Passwords do not match.",
    path: ["confirmPassword"],
  });

export type ChangePasswordFormData = z.infer<typeof changePasswordSchema>;

export const changeEmailSchema = z.object({
  currentPassword: z.string().min(1, "Current password is required."),

  newEmail: z
    .string()
    .trim()
    .min(1, "Email is required.")
    .email("Email format is invalid.")
    .regex(
      /^[^@\s]+@[^@\s]+\.[A-Za-z]{2,}$/,
      "Email must contain a valid domain.",
    )
    .max(254, "Email must not exceed 254 characters."),
});

export type ChangeEmailFormData = z.infer<typeof changeEmailSchema>;
