"use client";

import { useEffect } from "react";
import { useRouter, useSearchParams } from "next/navigation";
import { exchangeExternalAuthCode } from "@/lib/api/Auth";
import { useAuthStore } from "@/store/authStore";

export default function GoogleCallbackPage() {
  const router = useRouter();
  const searchParams = useSearchParams();

  const login = useAuthStore((state) => state.login);

  const code = searchParams.get("code");

  useEffect(() => {
    if (!code) {
      return;
    }

    const exchangeCode = async () => {
      try {
        const response = await exchangeExternalAuthCode(code);

        login(response.user, response.token);

        router.replace("/");
      } catch {
        router.replace("/login?error=google");
      }
    };

    exchangeCode();
  }, [code, login, router]);

  if (!code) {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <div className="text-center">
          <h1 className="text-xl font-semibold">Sign-in failed</h1>

          <p className="mt-2 text-muted-foreground">
            Google authentication code is missing.
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="flex min-h-screen items-center justify-center">
      <p>Signing you in...</p>
    </div>
  );
}
