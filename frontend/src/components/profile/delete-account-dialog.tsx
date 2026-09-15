"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";

import { ApiError } from "@/api/apiError";
import { useDeleteAccount } from "@/hooks/use-profile";
import { useAuthStore } from "@/store/authStore";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";

export function DeleteAccountDialog() {
  const router = useRouter();

  const deleteAccount = useDeleteAccount();

  const [open, setOpen] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const logout = useAuthStore((state) => state.logout);

  async function handleDelete() {
    setError(null);

    try {
      await deleteAccount.mutateAsync();

      logout();

      setOpen(false);

      router.replace("/sign-in");
    } catch (error) {
      if (error instanceof ApiError) {
        setError(error.message);
        return;
      }

      setError("Unable to delete account.");
    }
  }

  return (
    <Dialog
      open={open}
      onOpenChange={(open) => {
        setOpen(open);

        if (!open) {
          setError(null);
        }
      }}
    >
      <DialogTrigger
        render={<Button variant="destructive">Delete Account</Button>}
      />

      <DialogContent>
        <DialogHeader>
          <DialogTitle>
            Are you sure you want to delete your account?
          </DialogTitle>

          <DialogDescription>
            You will be signed out and will no longer have access to your
            account. Your upcoming appointments will also be cancelled.
          </DialogDescription>
        </DialogHeader>

        {error && <p className="text-sm text-destructive">{error}</p>}

        <DialogFooter>
          <Button
            type="button"
            variant="outline"
            onClick={() => setOpen(false)}
            disabled={deleteAccount.isPending}
          >
            Cancel
          </Button>

          <Button
            type="button"
            variant="destructive"
            onClick={handleDelete}
            disabled={deleteAccount.isPending}
          >
            {deleteAccount.isPending ? "Deleting..." : "Delete Account"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}
