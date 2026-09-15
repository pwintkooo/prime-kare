"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";

import { reactivateAccount } from "@/api/auth";
import { ApiError } from "@/api/apiError";
import { useAuthStore } from "@/store/authStore";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";

interface ReactivateAccountDialogProps {
  open: boolean;
  onOpenChange: (open: boolean) => void;
  email: string;
  password: string;
}

export function ReactivateAccountDialog({
  open,
  onOpenChange,
  email,
  password,
}: ReactivateAccountDialogProps) {
  const router = useRouter();
  const login = useAuthStore((state) => state.login);

  const [isReactivating, setIsReactivating] = useState(false);

  const [error, setError] = useState<string | null>(null);

  async function handleReactivate() {
    setError(null);
    setIsReactivating(true);

    try {
      const response = await reactivateAccount({
        email,
        password,
      });

      login(response.user, response.token);

      onOpenChange(false);

      router.push("/");
    } catch (error) {
      if (error instanceof ApiError) {
        setError(error.message);
        return;
      }

      setError("Unable to reactivate your account.");
    } finally {
      setIsReactivating(false);
    }
  }

  return (
    <Dialog
      open={open}
      onOpenChange={(open) => {
        onOpenChange(open);

        if (!open) {
          setError(null);
        }
      }}
    >
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Reactivate your account?</DialogTitle>

          <DialogDescription>
            Your account is currently inactive. Reactivate it to regain access
            to your PrimeKare account.
          </DialogDescription>
        </DialogHeader>

        {error && <p className="text-sm text-destructive">{error}</p>}

        <DialogFooter>
          <Button
            type="button"
            variant="outline"
            disabled={isReactivating}
            onClick={() => onOpenChange(false)}
          >
            Cancel
          </Button>

          <Button
            type="button"
            disabled={isReactivating}
            onClick={handleReactivate}
          >
            {isReactivating ? "Reactivating..." : "Reactivate Account"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}