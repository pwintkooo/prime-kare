"use client";

import Link from "next/link";
import { useState, Suspense } from "react";
import { useSearchParams } from "next/navigation";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  ArrowLeft,
  CheckCircle2,
  Eye,
  EyeOff,
  KeyRound,
  Loader2,
  TriangleAlert,
} from "lucide-react";

import { useResetPassword } from "@/hooks/use-auth";
import {
  resetPasswordSchema,
  type ResetPasswordFormData,
} from "@/validations/auth";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

function ResetPasswordContent() {
  const searchParams = useSearchParams();

  const email = searchParams.get("email");
  const token = searchParams.get("token");

  const [isSuccess, setIsSuccess] = useState(false);
  const [showNewPassword, setShowNewPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  const resetPasswordMutation = useResetPassword();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ResetPasswordFormData>({
    resolver: zodResolver(resetPasswordSchema),
    defaultValues: {
      newPassword: "",
      confirmPassword: "",
    },
  });

  const onSubmit = async (data: ResetPasswordFormData) => {
    if (!email || !token) {
      return;
    }

    try {
      await resetPasswordMutation.mutateAsync({
        email,
        token,
        newPassword: data.newPassword,
        confirmPassword: data.confirmPassword,
      });

      setIsSuccess(true);
    } catch {
      // Error is available from resetPasswordMutation.error
    }
  };

  // Invalid reset link
  if (!email || !token) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-slate-950 px-4 py-12">
        <div className="w-full max-w-md">
          <div className="rounded-2xl border border-slate-800 bg-slate-900 p-8 shadow-xl sm:p-10">
            <div className="mb-6 flex h-14 w-14 items-center justify-center rounded-full bg-red-500/10">
              <TriangleAlert className="h-7 w-7 text-red-400" />
            </div>

            <h1 className="text-2xl font-semibold tracking-tight text-white">
              Invalid reset link
            </h1>

            <p className="mt-3 text-sm leading-6 text-slate-400">
              This password reset link is invalid or incomplete. Please request
              a new password reset link and try again.
            </p>

            <Button className="mt-8 h-11 w-full">
              <Link href="/forgot-password">Request a new link</Link>
            </Button>

            <div className="mt-6 text-center">
              <Link
                href="/sign-in"
                className="inline-flex items-center text-sm text-slate-400 transition-colors hover:text-white"
              >
                <ArrowLeft className="mr-2 h-4 w-4" />
                Back to sign in
              </Link>
            </div>
          </div>
        </div>
      </div>
    );
  }

  // Successful password reset
  if (isSuccess) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-slate-950 px-4 py-12">
        <div className="w-full max-w-md">
          <div className="rounded-2xl border border-slate-800 bg-slate-900 p-8 shadow-xl sm:p-10">
            <div className="mb-6 flex h-14 w-14 items-center justify-center rounded-full bg-emerald-500/10">
              <CheckCircle2 className="h-7 w-7 text-emerald-500" />
            </div>

            <h1 className="text-2xl font-semibold tracking-tight text-white">
              Password reset successful
            </h1>

            <p className="mt-3 text-sm leading-6 text-slate-400">
              Your password has been changed successfully. You can now sign in
              to your PrimeKare account using your new password.
            </p>

            <Button className="mt-8 h-11 w-full">
              <Link href="/sign-in">Go to sign in</Link>
            </Button>
          </div>
        </div>
      </div>
    );
  }

  // Reset password form
  return (
    <div className="flex min-h-screen items-center justify-center bg-slate-950 px-4 py-12">
      <div className="w-full max-w-md">
        <div className="rounded-2xl border border-slate-800 bg-slate-900 p-8 shadow-xl sm:p-10">
          {/* Icon */}
          <div className="mb-6 flex h-14 w-14 items-center justify-center rounded-xl bg-blue-500/10">
            <KeyRound className="h-7 w-7 text-blue-500" />
          </div>

          {/* Heading */}
          <div>
            <h1 className="text-2xl font-semibold tracking-tight text-white">
              Reset your password
            </h1>

            <p className="mt-2 text-sm leading-6 text-slate-400">
              Create a new secure password for your PrimeKare account.
            </p>
          </div>

          {/* Form */}
          <form onSubmit={handleSubmit(onSubmit)} className="mt-8 space-y-5">
            {/* New Password */}
            <div className="space-y-2">
              <Label htmlFor="newPassword" className="text-slate-200">
                New password
              </Label>

              <div className="relative">
                <Input
                  id="newPassword"
                  type={showNewPassword ? "text" : "password"}
                  placeholder="Enter your new password"
                  autoComplete="new-password"
                  className="h-11 border-slate-700 bg-slate-950 pr-10 text-white placeholder:text-slate-600 focus-visible:ring-blue-500"
                  {...register("newPassword")}
                />

                <button
                  type="button"
                  onClick={() => setShowNewPassword((prev) => !prev)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-500 transition-colors hover:text-slate-300"
                  aria-label={
                    showNewPassword ? "Hide password" : "Show password"
                  }
                >
                  {showNewPassword ? (
                    <EyeOff className="h-4 w-4" />
                  ) : (
                    <Eye className="h-4 w-4" />
                  )}
                </button>
              </div>

              {errors.newPassword && (
                <p className="text-sm text-red-400">
                  {errors.newPassword.message}
                </p>
              )}
            </div>

            {/* Confirm Password */}
            <div className="space-y-2">
              <Label htmlFor="confirmPassword" className="text-slate-200">
                Confirm password
              </Label>

              <div className="relative">
                <Input
                  id="confirmPassword"
                  type={showConfirmPassword ? "text" : "password"}
                  placeholder="Confirm your new password"
                  autoComplete="new-password"
                  className="h-11 border-slate-700 bg-slate-950 pr-10 text-white placeholder:text-slate-600 focus-visible:ring-blue-500"
                  {...register("confirmPassword")}
                />

                <button
                  type="button"
                  onClick={() => setShowConfirmPassword((prev) => !prev)}
                  className="absolute right-3 top-1/2 -translate-y-1/2 text-slate-500 transition-colors hover:text-slate-300"
                  aria-label={
                    showConfirmPassword ? "Hide password" : "Show password"
                  }
                >
                  {showConfirmPassword ? (
                    <EyeOff className="h-4 w-4" />
                  ) : (
                    <Eye className="h-4 w-4" />
                  )}
                </button>
              </div>

              {errors.confirmPassword && (
                <p className="text-sm text-red-400">
                  {errors.confirmPassword.message}
                </p>
              )}
            </div>

            {/* API Error */}
            {resetPasswordMutation.isError && (
              <div className="rounded-lg border border-red-500/20 bg-red-500/10 px-4 py-3">
                <p className="text-sm text-red-400">
                  {resetPasswordMutation.error.message}
                </p>
              </div>
            )}

            {/* Submit */}
            <Button
              type="submit"
              disabled={resetPasswordMutation.isPending}
              className="h-11 w-full"
            >
              {resetPasswordMutation.isPending ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Resetting password...
                </>
              ) : (
                "Reset password"
              )}
            </Button>
          </form>

          {/* Back to Sign In */}
          <div className="mt-6 text-center">
            <Link
              href="/sign-in"
              className="inline-flex items-center text-sm text-slate-400 transition-colors hover:text-white"
            >
              <ArrowLeft className="mr-2 h-4 w-4" />
              Back to sign in
            </Link>
          </div>
        </div>

        <p className="mt-6 text-center text-xs text-slate-600">
          © {new Date().getFullYear()} PrimeKare. All rights reserved.
        </p>
      </div>
    </div>
  );
}

export default function ResetPasswordPage() {
  return (
    <Suspense
      fallback={
        <div className="flex min-h-screen items-center justify-center bg-slate-950">
          <Loader2 className="h-6 w-6 animate-spin text-slate-400" />
        </div>
      }
    >
      <ResetPasswordContent />
    </Suspense>
  );
}