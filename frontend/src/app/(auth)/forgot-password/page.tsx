"use client";

import Link from "next/link";
import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import {
  ArrowLeft,
  CheckCircle2,
  Loader2,
  LockKeyhole,
  Mail,
} from "lucide-react";

import { useForgotPassword } from "@/hooks/use-auth";
import {
  forgotPasswordSchema,
  type ForgotPasswordFormData,
} from "@/validations/auth";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

export default function ForgotPasswordPage() {
  const [isSuccess, setIsSuccess] = useState(false);

  const forgotPasswordMutation = useForgotPassword();

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<ForgotPasswordFormData>({
    resolver: zodResolver(forgotPasswordSchema),
    defaultValues: {
      email: "",
    },
  });

  const onSubmit = async (data: ForgotPasswordFormData) => {
    try {
      await forgotPasswordMutation.mutateAsync(data);
      setIsSuccess(true);
    } catch {
      // Error is available from forgotPasswordMutation.error
    }
  };

  if (isSuccess) {
    return (
      <div className="flex min-h-screen items-center justify-center bg-slate-950 px-4 py-12">
        <div className="w-full max-w-md">
          <div className="rounded-2xl border border-slate-800 bg-slate-900 p-8 shadow-xl sm:p-10">
            <div className="mb-6 flex h-14 w-14 items-center justify-center rounded-full bg-emerald-500/10">
              <CheckCircle2 className="h-7 w-7 text-emerald-500" />
            </div>

            <h1 className="text-2xl font-semibold tracking-tight text-white">
              Check your email
            </h1>

            <p className="mt-3 text-sm leading-6 text-slate-400">
              If an account exists for this email, we&apos;ve sent a password
              reset link. Please check your inbox and follow the instructions.
            </p>

            <div className="mt-8">
              <Button
                variant="outline"
                className="h-11 w-full border-slate-700 bg-transparent text-slate-200 hover:bg-slate-800 hover:text-white"
              >
                <Link
                  href="/sign-in"
                  className="inline-flex items-center justify-center gap-2"
                >
                  <ArrowLeft className="h-4 w-4" />
                  Back to sign in
                </Link>
              </Button>
            </div>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="flex min-h-screen items-center justify-center bg-slate-950 px-4 py-12">
      <div className="w-full max-w-md">
        <div className="rounded-2xl border border-slate-800 bg-slate-900 p-8 shadow-xl sm:p-10">
          {/* Icon */}
          <div className="mb-6 flex h-14 w-14 items-center justify-center rounded-xl bg-blue-500/10">
            <LockKeyhole className="h-7 w-7 text-blue-500" />
          </div>

          {/* Heading */}
          <div>
            <h1 className="text-2xl font-semibold tracking-tight text-white">
              Forgot your password?
            </h1>

            <p className="mt-2 text-sm leading-6 text-slate-400">
              No worries. Enter the email associated with your PrimeKare account
              and we&apos;ll send you a password reset link.
            </p>
          </div>

          {/* Form */}
          <form onSubmit={handleSubmit(onSubmit)} className="mt-8 space-y-5">
            <div className="space-y-2">
              <Label htmlFor="email" className="text-slate-200">
                Email address
              </Label>

              <div className="relative">
                <Mail className="absolute left-3 top-1/2 h-4 w-4 -translate-y-1/2 text-slate-500" />

                <Input
                  id="email"
                  type="email"
                  placeholder="you@example.com"
                  autoComplete="email"
                  className="h-11 border-slate-700 bg-slate-950 pl-10 text-white placeholder:text-slate-600 focus-visible:ring-blue-500"
                  {...register("email")}
                />
              </div>

              {errors.email && (
                <p className="text-sm text-red-400">{errors.email.message}</p>
              )}
            </div>

            {forgotPasswordMutation.isError && (
              <div className="rounded-lg border border-red-500/20 bg-red-500/10 px-4 py-3">
                <p className="text-sm text-red-400">
                  {forgotPasswordMutation.error.message}
                </p>
              </div>
            )}

            <Button
              type="submit"
              disabled={forgotPasswordMutation.isPending}
              className="h-11 w-full"
            >
              {forgotPasswordMutation.isPending ? (
                <>
                  <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                  Sending reset link...
                </>
              ) : (
                "Send reset link"
              )}
            </Button>
          </form>

          {/* Back */}
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
