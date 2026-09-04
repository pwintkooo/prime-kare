"use client";

import { useRouter } from "next/navigation";
import { useAuthStore } from "@/store/authStore";
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

  const sessionExpired = useAuthStore((state) => state.sessionExpired);

  const logout = useAuthStore((state) => state.logout);

  const clearSessionExpired = useAuthStore(
    (state) => state.clearSessionExpired,
  );

  function handleConfirm() {
    clearSessionExpired();
    logout();
    router.replace("/sign-in");
  }

  return (
    <AlertDialog open={sessionExpired}>
      <AlertDialogContent>
        <AlertDialogHeader>
          <AlertDialogTitle>Session expired</AlertDialogTitle>

          <AlertDialogDescription>
            Your session has expired. Please sign in again to continue.
          </AlertDialogDescription>
        </AlertDialogHeader>

        <AlertDialogFooter>
          <Button onClick={handleConfirm}>OK</Button>
        </AlertDialogFooter>
      </AlertDialogContent>
    </AlertDialog>
  );
}