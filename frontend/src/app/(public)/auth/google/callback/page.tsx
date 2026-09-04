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
  const type = searchParams.get("type");

  useEffect(() => {
    if (!code || !type) {
      return;
    }

    if (type === "link") {
      router.replace(`/auth/google/link?code=${encodeURIComponent(code)}`);

      return;
    }

    if (type === "login") {
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
    }
  }, [code, type, login, router]);

  if (!code || !type) {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <div className="text-center">
          <h1 className="text-xl font-semibold">Sign-in failed</h1>

          <p className="mt-2 text-muted-foreground">
            Google authentication information is missing.
          </p>
        </div>
      </div>
    );
  }

  if (type !== "login" && type !== "link") {
    return (
      <div className="flex min-h-screen items-center justify-center">
        <div className="text-center">
          <h1 className="text-xl font-semibold">Sign-in failed</h1>

          <p className="mt-2 text-muted-foreground">
            Invalid authentication request.
          </p>
        </div>
      </div>
    );
  }

  return (
    <div className="flex min-h-screen items-center justify-center">
      <p>
        {type === "link" ? "Preparing account linking..." : "Signing you in..."}
      </p>
    </div>
  );
}