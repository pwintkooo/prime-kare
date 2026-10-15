"use client";

import { useCallback, useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuthStore } from "@/store/authStore";
import { apiClient } from "@/api/client";

const WARNING_BEFORE_EXPIRY_MS = 15 * 60 * 1000;
const CHECK_INTERVAL_MS = 1000;

function getTokenExpiry(token: string): number | null {
  try {
    const payload = token.split(".")[1];

    if (!payload) return null;

    const base64 = payload.replace(/-/g, "+").replace(/_/g, "/");

    const decoded = atob(base64);
    const claims = JSON.parse(decoded);

    if (typeof claims.exp !== "number") {
      return null;
    }

    return claims.exp * 1000;
  } catch {
    return null;
  }
}

export default function SessionMonitor() {
  const router = useRouter();

  const token = useAuthStore((state) => state.token);
  const hydrated = useAuthStore((state) => state.hydrated);
  const showSessionExpired = useAuthStore((state) => state.showSessionExpired);
  const logout = useAuthStore((state) => state.logout);
  const setSessionHasExpired = useAuthStore(
    (state) => state.setSessionHasExpired,
  );

  const expireSession = useCallback(() => {
    // Revoke the refresh token and clear the cookie if possible.
    void apiClient.post("/api/auth/logout").catch(() => {});

    // Expire the local session immediately, without waiting for the API.
    logout();
    router.replace("/sign-in");
  }, [logout, router]);

  useEffect(() => {
    if (!hydrated || !token) {
      return;
    }

    const expiry = getTokenExpiry(token);

    // Do not leave a user signed in with a token we cannot monitor.
    if (expiry === null) {
      expireSession();
      return;
    }

    let expired = false;

    const checkExpiry = () => {
      if (expired) return;

      const remaining = expiry - Date.now();

      if (remaining <= 0) {
        expired = true;
        showSessionExpired();
        setSessionHasExpired(true);
        return;
      }

      if (remaining <= WARNING_BEFORE_EXPIRY_MS) {
        showSessionExpired();
      }
    };

    // Check immediately in case the token is already near expiry.
    checkExpiry();

    const intervalId = window.setInterval(checkExpiry, CHECK_INTERVAL_MS);

    const handleVisibilityChange = () => {
      if (document.visibilityState === "visible") {
        checkExpiry();
      }
    };

    window.addEventListener("focus", checkExpiry);
    document.addEventListener("visibilitychange", handleVisibilityChange);

    return () => {
      window.clearInterval(intervalId);
      window.removeEventListener("focus", checkExpiry);
      document.removeEventListener("visibilitychange", handleVisibilityChange);
    };
  }, [hydrated, token, showSessionExpired, expireSession, setSessionHasExpired]);

  return null;
}
