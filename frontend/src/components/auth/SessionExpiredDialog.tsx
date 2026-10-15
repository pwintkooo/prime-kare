"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";

import { useAuthStore } from "@/store/authStore";
import { apiClient } from "@/api/client";

import {
  AlertDialog,
  AlertDialogContent,
  AlertDialogDescription,
  AlertDialogFooter,
  AlertDialogHeader,
  AlertDialogTitle,
} from "@/components/ui/alert-dialog";

import { Button } from "@/components/ui/button";

export default function SessionExpiredDialog() {
  const router = useRouter();

  const token = useAuthStore((state) => state.token);
  const sessionExpired = useAuthStore((state) => state.sessionExpired);
  const sessionHasExpired = useAuthStore((state) => state.sessionHasExpired);

  const updateToken = useAuthStore((state) => state.updateToken);
  const clearSessionExpired = useAuthStore(
    (state) => state.clearSessionExpired,
  );
  const setSessionHasExpired = useAuthStore(
    (state) => state.setSessionHasExpired,
  );
  const logout = useAuthStore((state) => state.logout);

  const [refreshing, setRefreshing] = useState(false);
  const [signingOut, setSigningOut] = useState(false);
  const [error, setError] = useState("");

  async function handleSignOut() {
    if (signingOut || refreshing) return;

    setSigningOut(true);
    setError("");

    try {
      await apiClient.post("/api/auth/logout");
    } catch {
      // Clear the local session even if the API request fails.
    } finally {
      logout();
      clearSessionExpired();
      setSessionHasExpired(false);
      router.replace("/sign-in");
    }
  }

  async function handleContinueSession() {
    if (refreshing || signingOut || sessionHasExpired) return;

    setRefreshing(true);
    setError("");

    try {
      const response = await apiClient.post<{ token: string; user?: unknown }>(
        "/api/auth/refresh",
      );

      if (!response.data.token) {
        throw new Error("No access token returned.");
      }

      updateToken(response.data.token);
      clearSessionExpired();
      setSessionHasExpired(false);
    } catch {
      setError("Unable to continue your session. Please sign in again.");
    } finally {
      setRefreshing(false);
    }
  }

  const isOpen = Boolean(token && sessionExpired);

  return (
    <AlertDialog open={isOpen}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>
            {sessionHasExpired
              ? "Your session has expired"
              : "Your session is about to expire"}
          </AlertDialogTitle>

          <AlertDialogDescription>
            {sessionHasExpired
              ? "Your session has expired for security reasons. Please sign in again to continue."
              : "Your session will end when the current access token expires. Continue your session to stay signed in."}
          </AlertDialogDescription>
        </AlertDialogHeader>

        {error && (
          <p role="alert" className="text-sm text-destructive">
            {error}
          </p>
        )}

        <AlertDialogFooter>
          {sessionHasExpired ? (
            <Button onClick={handleSignOut} disabled={signingOut || refreshing}>
              {signingOut ? "Signing out..." : "Sign in again"}
            </Button>
          ) : (
            <>
              <Button
                variant="outline"
                onClick={handleSignOut}
                disabled={refreshing || signingOut}
              >
                {signingOut ? "Signing out..." : "Sign out"}
              </Button>

              <Button
                onClick={handleContinueSession}
                disabled={refreshing || signingOut}
              >
                {refreshing ? "Continuing..." : "Continue session"}
              </Button>
            </>
          )}
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}
