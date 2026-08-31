"use client";

import Image from "next/image";
import Link from "next/link";
import React, { useState } from "react";
import { SignUp } from "@/lib/api/Auth";
import PasswordRequirements from "@/components/auth/PasswordRequirements";
import ConfirmPassword from "@/components/auth/ConfirmPassword";

export default function SignUpPage() {
  const [name, setName] = useState("");
  const [email, setEmail] = useState("");
  const [phone, setPhone] = useState("");
  const [password, setPassword] = useState("");
  const [confirmPassword, setConfirmPassword] = useState("");
  const [terms, setTerms] = useState(false);
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const passwordRequirements = {
    minLength: password.length >= 8,
    uppercase: /[A-Z]/.test(password),
    lowercase: /[a-z]/.test(password),
    number: /[0-9]/.test(password),
    special: /[^A-Za-z0-9]/.test(password),
  };

  const isStrongPassword = Object.values(passwordRequirements).every(Boolean);

  const handleSubmit = async (event: React.SyntheticEvent<HTMLFormElement>) => {
    event.preventDefault();

    setError("");

    if (!isStrongPassword) {
      setError(
        "Password must be at least 8 characters and contain an uppercase letter, lowercase letter, number, and special character.",
      );
      return;
    }

    if (password !== confirmPassword) {
      setError("Passwords do not match.");
      return;
    }

    if (!terms) {
      setError("You must agree to the Terms of Service and Privacy Policy.");
      return;
    }

    try {
      setIsLoading(true);

      const response = await SignUp({
        name,
        email,
        phone,
        password,
      });

      console.log("Accont created:", response);
      window.location.href = "/sign-in";
    } catch (error) {
      setError(
        error instanceof Error ? error.message : "Something went wrong.",
      );
    } finally {
      setIsLoading(false);
    }
  };

  return (
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
            <form onSubmit={handleSubmit} className="mt-8 space-y-5">
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
                  name="name"
                  type="text"
                  autoComplete="name"
                  placeholder="John Tan"
                  value={name}
                  onChange={(e) => setName(e.target.value)}
                  className="w-full rounded-xl border border-slate-700 bg-slate-900 px-4 py-3 text-sm text-white outline-none placeholder:text-slate-500 transition focus:border-blue-400 focus:ring-2 focus:ring-blue-400/20"
                  required
                />
              </div>

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
                  name="email"
                  type="email"
                  autoComplete="email"
                  placeholder="you@example.com"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                  className="w-full rounded-xl border border-slate-700 bg-slate-900 px-4 py-3 text-sm text-white outline-none placeholder:text-slate-500 transition focus:border-blue-400 focus:ring-2 focus:ring-blue-400/20"
                  required
                />
              </div>

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
                  name="phone"
                  type="tel"
                  autoComplete="tel"
                  placeholder="+65 9123 4567"
                  value={phone}
                  onChange={(e) => setPhone(e.target.value)}
                  className="w-full rounded-xl border border-slate-700 bg-slate-900 px-4 py-3 text-sm text-white outline-none placeholder:text-slate-500 transition focus:border-blue-400 focus:ring-2 focus:ring-blue-400/20"
                  required
                />
              </div>

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
                  name="password"
                  type="password"
                  autoComplete="new-password"
                  placeholder="Create a password"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  className="w-full rounded-xl border border-slate-700 bg-slate-900 px-4 py-3 text-sm text-white outline-none placeholder:text-slate-500 transition focus:border-blue-400 focus:ring-2 focus:ring-blue-400/20"
                  required
                />
              </div>

              <PasswordRequirements password={password} />

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
                  name="confirmPassword"
                  type="password"
                  autoComplete="new-password"
                  placeholder="Confirm your password"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
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
                  name="terms"
                  type="checkbox"
                  checked={terms}
                  onChange={(e) => setTerms(e.target.checked)}
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

              {error && (
                <div className="rounded-xl border border-red-500/30 bg-red-500/10 px-4 py-3 text-sm text-red-400">
                  {error}
                </div>
              )}

              {/* Submit */}
              <button
                type="submit"
                className="w-full rounded-xl bg-blue-600 px-4 py-3 text-sm font-semibold text-white transition hover:bg-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-400 focus:ring-offset-2 focus:ring-offset-slate-950"
              >
                {isLoading ? "Creating account..." : "Create account"}
              </button>
            </form>

            {/* Sign In */}
            <p className="mt-8 text-center text-sm text-slate-400">
              Already have an account?{" "}
              <Link
                href="/login"
                className="font-semibold text-blue-400 transition hover:text-blue-300"
              >
                Sign in
              </Link>
            </p>
          </div>
        </div>
      </div>
    </main>
  );
}
