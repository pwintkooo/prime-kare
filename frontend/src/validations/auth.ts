import { z } from "zod";

export const signUpSchema = z
  .object({
    name: z
      .string()
      .min(1, "Name is required")
      .max(100, "Name must not exceed 100 characters"),

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

    password: z
      .string()
      .min(8, "Password must be at least 8 characters")
      .regex(/[A-Z]/, "Must contain an uppercase letter")
      .regex(/[a-z]/, "Must contain a lowercase letter")
      .regex(/[0-9]/, "Must contain a number")
      .regex(/[^A-Za-z0-9]/, "Must contain a special character"),

    confirmPassword: z.string().min(1, "Please confirm your password."),

    terms: z.boolean().refine((value) => value === true, {
      message: "You must agree to the Terms of Service and Privacy Policy.",
    }),
  })
  .refine((data) => data.password === data.confirmPassword, {
    message: "Passwords do not match",
    path: ["confirmPassword"],
  });

export type SignUpFormData = z.infer<typeof signUpSchema>;

export const signInSchema = z.object({
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

  password: z.string().min(1, "Password is required."),
});

export type SignInFormData = z.infer<typeof signInSchema>;

export const forgotPasswordSchema = z.object({
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
});

export type ForgotPasswordFormData = z.infer<typeof forgotPasswordSchema>;

export const resetPasswordSchema = z
  .object({
    newPassword: z
      .string()
      .min(8, "Password must be at least 8 characters")
      .regex(/[A-Z]/, "Must contain an uppercase letter")
      .regex(/[a-z]/, "Must contain a lowercase letter")
      .regex(/[0-9]/, "Must contain a number")
      .regex(/[^A-Za-z0-9]/, "Must contain a special character"),

    confirmPassword: z.string().min(1, "Please confirm your password."),
  })
  .refine((data) => data.newPassword === data.confirmPassword, {
    message: "Passwords do not match",
    path: ["confirmPassword"],
  });

export type ResetPasswordFormData = z.infer<typeof resetPasswordSchema>;
