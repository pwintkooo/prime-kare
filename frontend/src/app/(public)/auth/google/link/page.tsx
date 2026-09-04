"use client";

import { FormEvent, useState } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { verifyAndLinkGoogle } from "@/lib/api/Auth";
import { useAuthStore } from "@/store/authStore";

export default function GoogleLinkPage() {
  const router = useRouter();
  const searchParams = useSearchParams();

  const login = useAuthStore((state) => state.login);

  const code = searchParams.get("code");

  const [password, setPassword] = useState("");
  const [error, setError] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    if (!code) {
      setError("Invalid account linking request.");
      return;
    }

    setError("");
    setIsLoading(true);

    try {
      const response = await verifyAndLinkGoogle(code, password);

      login(response.user, response.token);

      router.replace("/");
    } catch {
      setError("The password is incorrect or the linking request has expired.");
    } finally {
      setIsLoading(false);
    }
  }

  if (!code) {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <div className="text-center">
          <h1 className="text-xl font-semibold">Invalid request</h1>

          <p className="mt-2 text-muted-foreground">
            The Google account linking request is missing.
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="flex min-h-screen items-center justify-center px-4">
      <div className="w-full max-w-md">
        <div className="rounded-2xl border border-slate-800 bg-slate-950 p-8">
          <h1 className="text-2xl font-bold text-white">
            Link your Google account
          </h1>

          <p className="mt-2 text-sm text-slate-400">
            A PrimeKare account already exists with this Google email address.
            Enter your PrimeKare password to link your Google account.
          </p>

          <form onSubmit={handleSubmit} className="mt-6 space-y-4">
            <div>
              <label
                htmlFor="password"
                className="mb-2 block text-sm font-medium text-slate-200"
              >
                PrimeKare Password
              </label>

              <input
                id="password"
                type="password"
                value={password}
                onChange={(event) => setPassword(event.target.value)}
                placeholder="Enter your password"
                disabled={isLoading}
                required
                className="w-full rounded-xl border border-slate-700 bg-slate-900 px-4 py-3 text-sm text-white outline-none placeholder:text-slate-500 focus:border-blue-500"
              />
            </div>

            {error && <p className="text-sm text-red-400">{error}</p>}

            <button
              type="submit"
              disabled={isLoading}
              className="w-full rounded-xl bg-blue-600 px-4 py-3 text-sm font-semibold text-white transition hover:bg-blue-500 disabled:cursor-not-allowed disabled:opacity-50"
            >
              {isLoading ? "Linking account..." : "Verify & Link Google"}
            </button>
          </form>

          <button
            type="button"
            onClick={() => router.replace("/login")}
            disabled={isLoading}
            className="mt-4 w-full text-sm text-slate-400 transition hover:text-white"
          >
            Cancel
          </button>
        </div>
      </div>
    </div>
  );
}