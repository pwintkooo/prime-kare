"use client";

import React, { useState } from "react";
import Image from "next/image";
import Link from "next/link";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { signInSchema, SignInFormData } from "@/validations/auth";
import { SignInWithGoogle } from "@/api/auth";
import GuestOnly from "@/components/auth/GuestOnly";
import { ReactivateAccountDialog } from "@/components/auth/ReactivateAccountDialog";
import { PasswordInput } from "@/components/ui/password-input";
import { isAccountInactiveError, useSignIn } from "@/hooks/use-auth";

export default function SignInPage() {
  const signInMutation = useSignIn();
  const [reactivateOpen, setReactivateOpen] = useState(false);
  const [reactivateCredentials, setReactivateCredentials] =
    useState<SignInFormData | null>(null);

  const serverError =
    signInMutation.isError && !isAccountInactiveError(signInMutation.error)
      ? signInMutation.error.message
      : null;

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<SignInFormData>({
    resolver: zodResolver(signInSchema),
    defaultValues: {
      email: "",
      password: "",
    },
  });

  const onSubmit = async (data: SignInFormData) => {
    try {
      await signInMutation.mutateAsync(data);
    } catch (error) {
      if (isAccountInactiveError(error)) {
        setReactivateCredentials(data);
        setReactivateOpen(true);
      }
    }
  };

  function handleReactivateOpenChange(open: boolean) {
    setReactivateOpen(open);

    if (!open) {
      setReactivateCredentials(null);
      reset();
    }
  }

  return (
    <GuestOnly>
      <main className="min-h-screen bg-slate-900">
        <div className="lg:grid lg:grid-cols-[minmax(0,1.15fr)_minmax(480px,0.85fr)]">
          {/* Left - Image */}
          <div className="relative hidden h-screen lg:sticky lg:top-0 lg:block">
            <Image
              src="/images/auth/auth-background.jpg"
              alt="PrimeKare automotive workshop"
              fill
              priority
              className="object-cover"
            />

            {/* Overlay */}
            <div className="absolute inset-0 bg-linear-to-r from-slate-950/70 via-slate-950/40 to-slate-950/15" />

            {/* Content */}
            <div className="absolute inset-0 flex items-center p-12 xl:p-16">
              <div className="max-w-lg">
                <h1 className="mt-4 text-4xl font-bold tracking-tight text-white xl:text-5xl">
                  Professional care for
                  <br />
                  <span className="text-slate-400">every journey.</span>
                </h1>

                <p className="mt-6 max-w-md text-base leading-7 text-slate-200">
                  Manage your vehicles, appointments, and service history with
                  PrimeKare.
                </p>
              </div>
            </div>
          </div>

          {/* Right - Login */}
          <div className="flex min-h-screen items-center justify-center px-6 py-12 sm:px-8">
            <div className="w-full max-w-md">
              {/* Logo / Brand */}
              <div className="mb-8">
                <Link href="/" className="inline-block">
                  <Image
                    src="/images/branding/primekare-dark-logo.png"
                    alt="PrimeKare Logo"
                    width={160}
                    height={48}
                    className="h-10 w-auto"
                  />
                </Link>
              </div>
              {/* Header */}
              <div>
                <h2 className="text-3xl font-bold tracking-tight text-white">
                  Welcome back
                </h2>

                <p className="mt-2 text-sm text-slate-400">
                  Sign in to your account to continue.
                </p>
              </div>
              {/* Form */}
              <form
                onSubmit={handleSubmit(onSubmit)}
                className="mt-8 space-y-5"
              >
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
                    required
                    className="w-full rounded-xl border border-slate-700 bg-slate-900 px-4 py-3 text-sm text-white outline-none placeholder:text-slate-500 transition focus:border-blue-400 focus:ring-2 focus:ring-blue-400/20"
                  />
                </div>

                {errors.email && (
                  <p className="mt-1 text-sm text-red-400">
                    {errors.email.message}
                  </p>
                )}

                {/* Password */}
                <div>
                  <div className="mb-2 flex items-center justify-between">
                    <label
                      htmlFor="password"
                      className="block text-sm font-medium text-slate-200"
                    >
                      Password
                    </label>

                    <Link
                      href="/forgot-password"
                      className="text-sm font-medium text-blue-400 transition hover:text-blue-300"
                    >
                      Forgot password?
                    </Link>
                  </div>

                  <PasswordInput
                    id="password"
                    autoComplete="current-password"
                    placeholder="Enter your password"
                    {...register("password")}
                    className="h-auto rounded-xl border-slate-700 bg-slate-900 px-4 py-3 pr-12 text-sm text-white placeholder:text-slate-500 focus-visible:border-blue-400 focus-visible:ring-blue-400/20"
                  />
                </div>

                {errors.password && (
                  <p className="mt-1 text-sm text-red-400">
                    {errors.password.message}
                  </p>
                )}

                {/* Remember me */}
                {/* <div className="flex items-center gap-3">
                  <input
                    id="remember"
                    name="remember"
                    type="checkbox"
                    className="h-4 w-4 rounded border-slate-700 bg-slate-900 text-blue-600 focus:ring-blue-400"
                  />

                  <label htmlFor="remember" className="text-sm text-slate-400">
                    Remember me
                  </label>
                </div> */}

                {serverError && (
                  <div className="rounded-xl border border-red-500/30 bg-red-500/10 px-4 py-3 text-sm text-red-400">
                    {serverError}
                  </div>
                )}

                {/* Submit */}
                <button
                  type="submit"
                  disabled={signInMutation.isPending}
                  className="w-full rounded-xl bg-blue-600 px-4 py-3 text-sm font-semibold text-white transition hover:bg-blue-500 disabled:cursor-not-allowed disabled:opacity-50"
                >
                  {signInMutation.isPending ? "Signing in..." : "Sign in"}
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

              {/* Register */}
              <p className="mt-8 text-center text-sm text-slate-400">
                Don&apos;t have an account?{" "}
                <Link
                  href="/sign-up"
                  className="font-semibold text-blue-400 transition hover:text-blue-300"
                >
                  Create an account
                </Link>
              </p>
            </div>
          </div>
        </div>
      </main>
      {reactivateCredentials && (
        <ReactivateAccountDialog
          open={reactivateOpen}
          onOpenChange={handleReactivateOpenChange}
          email={reactivateCredentials.email}
          password={reactivateCredentials.password}
        />
      )}
    </GuestOnly>
  );
}
