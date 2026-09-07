"use client";

import React, { useState } from "react";
import Image from "next/image";
import Link from "next/link";
import { useRouter } from "next/navigation";
import { SignUp } from "@/api/auth";
import PasswordRequirements from "@/components/auth/PasswordRequirements";
import ConfirmPassword from "@/components/auth/ConfirmPassword";
import { useForm, useWatch } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { signUpSchema, SignUpFormData } from "@/lib/validations/auth";
import { SignInWithGoogle } from "@/api/auth";
import GuestOnly from "@/components/auth/GuestOnly";

export default function SignUpPage() {
  const router = useRouter();
  const [serverError, setServerError] = useState("");
  const {
    register,
    handleSubmit,
    control,
    formState: { errors, isSubmitting },
  } = useForm<SignUpFormData>({
    resolver: zodResolver(signUpSchema),
    defaultValues: {
      name: "",
      email: "",
      phone: "",
      password: "",
      confirmPassword: "",
      terms: false,
    },
  });

  const password =
    useWatch({
      control,
      name: "password",
    }) ?? "";

  const confirmPassword =
    useWatch({
      control,
      name: "confirmPassword",
    }) ?? "";

  const onSubmit = async (data: SignUpFormData) => {
    setServerError("");

    try {
      const response = await SignUp({
        name: data.name,
        email: data.email,
        phone: data.phone,
        password: data.password,
      });

      console.log("Account created:", response);

      router.push("/sign-in");
    } catch (error) {
      setServerError(
        error instanceof Error ? error.message : "Something went wrong.",
      );
    }
  };

  return (
    <GuestOnly>
      <main className="min-h-screen bg-slate-950">
        <div className="grid min-h-screen lg:grid-cols-2">
          {/* Left - Branding */}
          <div className="relative hidden lg:block">
            <Image
              src="/images/auth/register.jpg"
              alt="PrimeKare automotive workshop"
              fill
              priority
              className="object-cover"
            />

            {/* Overlay */}
            <div className="absolute inset-0 bg-slate-950/70" />

            {/* Content */}
            <div className="absolute inset-0 flex items-end p-12 xl:p-16">
              <div className="max-w-lg">
                <p className="text-sm font-semibold uppercase tracking-[0.2em] text-blue-400">
                  PrimeKare
                </p>

                <h1 className="mt-4 text-4xl font-bold tracking-tight text-white xl:text-5xl">
                  Take better care
                  <br />
                  <span className="text-slate-400">of your vehicle.</span>
                </h1>

                <p className="mt-6 max-w-md text-base leading-7 text-slate-200">
                  Create your PrimeKare account to manage your vehicles,
                  appointments, and service history in one place.
                </p>
              </div>
            </div>
          </div>

          {/* Right - Register */}
          <div className="flex items-center justify-center px-6 py-12 sm:px-8">
            <div className="w-full max-w-md">
              {/* Logo */}
              <div className="mb-8">
                <Link
                  href="/"
                  className="text-xl font-bold tracking-tight text-white"
                >
                  Prime<span className="text-blue-400">Kare</span>
                </Link>
              </div>

              {/* Header */}
              <div>
                <h2 className="text-3xl font-bold tracking-tight text-white">
                  Create your account
                </h2>

                <p className="mt-2 text-sm text-slate-400">
                  Get started with PrimeKare today.
                </p>
              </div>

              {/* Form */}
              <form
                onSubmit={handleSubmit(onSubmit)}
                className="mt-8 space-y-5"
              >
                {/* Full Name */}
                <div>
                  <label
                    htmlFor="name"
                    className="mb-2 block text-sm font-medium text-slate-200"
                  >
                    Full name
                  </label>

                  <input
                    id="name"
                    type="text"
                    autoComplete="name"
                    placeholder="John Tan"
                    {...register("name")}
                    className="w-full rounded-xl border border-slate-700 bg-slate-900 px-4 py-3 text-sm text-white outline-none placeholder:text-slate-500 transition focus:border-blue-400 focus:ring-2 focus:ring-blue-400/20"
                    required
                  />
                </div>

                {errors.name && (
                  <p className="mt-1 text-sm text-red-400">
                    {errors.name.message}
                  </p>
                )}

                {/* Email */}
                <div>
                  <label
                    htmlFor="email"
                    className="mb-2 block text-sm font-medium text-slate-200"
                  >
                    Email address
                  </label>

                  <input
                    id="email"
                    type="email"
                    autoComplete="email"
                    placeholder="you@example.com"
                    {...register("email")}
                    className="w-full rounded-xl border border-slate-700 bg-slate-900 px-4 py-3 text-sm text-white outline-none placeholder:text-slate-500 transition focus:border-blue-400 focus:ring-2 focus:ring-blue-400/20"
                    required
                  />
                </div>

                {errors.email && (
                  <p className="mt-1 text-sm text-red-400">
                    {errors.email.message}
                  </p>
                )}

                {/* Phone */}
                <div>
                  <label
                    htmlFor="phone"
                    className="mb-2 block text-sm font-medium text-slate-200"
                  >
                    Phone number
                  </label>

                  <input
                    id="phone"
                    type="tel"
                    autoComplete="tel"
                    placeholder="+65 9123 4567"
                    {...register("phone")}
                    className="w-full rounded-xl border border-slate-700 bg-slate-900 px-4 py-3 text-sm text-white outline-none placeholder:text-slate-500 transition focus:border-blue-400 focus:ring-2 focus:ring-blue-400/20"
                    required
                  />
                </div>

                {errors.phone && (
                  <p className="mt-1 text-sm text-red-400">
                    {errors.phone.message}
                  </p>
                )}

                {/* Password */}
                <div>
                  <label
                    htmlFor="password"
                    className="mb-2 block text-sm font-medium text-slate-200"
                  >
                    Password
                  </label>

                  <input
                    id="password"
                    type="password"
                    autoComplete="new-password"
                    placeholder="Create a password"
                    {...register("password")}
                    className="w-full rounded-xl border border-slate-700 bg-slate-900 px-4 py-3 text-sm text-white outline-none placeholder:text-slate-500 transition focus:border-blue-400 focus:ring-2 focus:ring-blue-400/20"
                    required
                  />
                </div>

                <PasswordRequirements password={password} />

                {errors.password && (
                  <p className="text-sm text-red-400">
                    {errors.password.message}
                  </p>
                )}

                {/* Confirm Password */}
                <div>
                  <label
                    htmlFor="confirmPassword"
                    className="mb-2 block text-sm font-medium text-slate-200"
                  >
                    Confirm password
                  </label>

                  <input
                    id="confirmPassword"
                    type="password"
                    autoComplete="new-password"
                    placeholder="Confirm your password"
                    {...register("confirmPassword")}
                    className="w-full rounded-xl border border-slate-700 bg-slate-900 px-4 py-3 text-sm text-white outline-none placeholder:text-slate-500 transition focus:border-blue-400 focus:ring-2 focus:ring-blue-400/20"
                    required
                  />
                </div>

                <ConfirmPassword
                  password={password}
                  confirmPassword={confirmPassword}
                />

                {/* Terms */}
                <div className="flex items-start gap-3">
                  <input
                    id="terms"
                    type="checkbox"
                    {...register("terms")}
                    className="mt-0.5 h-4 w-4 rounded border-slate-700 bg-slate-900 text-blue-600 focus:ring-blue-400"
                    required
                  />

                  <label
                    htmlFor="terms"
                    className="text-sm leading-5 text-slate-400"
                  >
                    I agree to the{" "}
                    <Link
                      href="/terms"
                      className="text-blue-400 transition hover:text-blue-300"
                    >
                      Terms of Service
                    </Link>{" "}
                    and{" "}
                    <Link
                      href="/privacy"
                      className="text-blue-400 transition hover:text-blue-300"
                    >
                      Privacy Policy
                    </Link>
                    .
                  </label>
                </div>

                {errors.terms && (
                  <p className="text-sm text-red-400">{errors.terms.message}</p>
                )}

                {serverError && (
                  <div className="rounded-xl border border-red-500/30 bg-red-500/10 px-4 py-3 text-sm text-red-400">
                    {serverError}
                  </div>
                )}

                {/* Submit */}
                <button
                  type="submit"
                  disabled={isSubmitting}
                  className="w-full rounded-xl bg-blue-600 px-4 py-3 text-sm font-semibold text-white transition hover:bg-blue-500 disabled:cursor-not-allowed disabled:opacity-50 focus:outline-none focus:ring-2 focus:ring-blue-400 focus:ring-offset-2 focus:ring-offset-slate-950"
                >
                  {isSubmitting ? "Creating account..." : "Create account"}
                </button>
              </form>

              <div className="my-6 flex items-center gap-4">
                <div className="h-px flex-1 bg-slate-800" />
                <span className="text-xs font-medium uppercase tracking-wider text-slate-500">
                  Or continue with
                </span>
                <div className="h-px flex-1 bg-slate-800" />
              </div>

              <button
                type="button"
                onClick={SignInWithGoogle}
                className="flex w-full items-center justify-center gap-3 rounded-xl border border-slate-700 bg-slate-900 px-4 py-3 text-sm font-semibold text-white transition hover:border-slate-600 hover:bg-slate-800 focus:outline-none focus:ring-2 focus:ring-blue-400/20"
              >
                <svg className="h-5 w-5" viewBox="0 0 24 24" aria-hidden="true">
                  <path
                    fill="#4285F4"
                    d="M21.35 12.27c0-.79-.07-1.55-.2-2.27H12v4.3h5.24a4.48 4.48 0 0 1-1.94 2.94v2.45h3.14c1.84-1.69 2.91-4.18 2.91-7.42Z"
                  />
                  <path
                    fill="#34A853"
                    d="M12 21.5c2.63 0 4.84-.87 6.45-2.36l-3.14-2.45c-.87.58-1.98.93-3.31.93-2.54 0-4.69-1.72-5.46-4.03H3.3v2.53A9.74 9.74 0 0 0 12 21.5Z"
                  />
                  <path
                    fill="#FBBC05"
                    d="M6.54 13.59a5.86 5.86 0 0 1 0-3.18V7.88H3.3a9.5 9.5 0 0 0 0 8.24l3.24-2.53Z"
                  />
                  <path
                    fill="#EA4335"
                    d="M12 6.38c1.43 0 2.71.49 3.72 1.45l2.79-2.79C16.84 3.48 14.63 2.5 12 2.5a9.74 9.74 0 0 0-8.7 5.38l3.24 2.53C7.31 8.1 9.46 6.38 12 6.38Z"
                  />
                </svg>

                <span>Continue with Google</span>
              </button>

              {/* Sign In */}
              <p className="mt-8 text-center text-sm text-slate-400">
                Already have an account?{" "}
                <Link
                  href="/sign-in"
                  className="font-semibold text-blue-400 transition hover:text-blue-300"
                >
                  Sign in
                </Link>
              </p>
            </div>
          </div>
        </div>
      </main>
    </GuestOnly>
  );
}
