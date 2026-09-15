"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

import { useChangeEmail } from "@/hooks/use-profile";
import { ApiError } from "@/api/apiError";

import { changeEmailSchema, ChangeEmailFormData } from "@/validations/profile";

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
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

interface ChangeEmailDialogProps {
  currentEmail: string;
}

export function ChangeEmailDialog({ currentEmail }: ChangeEmailDialogProps) {
  const changeEmail = useChangeEmail();

  const [open, setOpen] = useState(false);

  const [formError, setFormError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<ChangeEmailFormData>({
    resolver: zodResolver(changeEmailSchema),
    defaultValues: {
      currentPassword: "",
      newEmail: "",
    },
  });

  async function onSubmit(data: ChangeEmailFormData) {
    setFormError(null);

    try {
      await changeEmail.mutateAsync({
        newEmail: data.newEmail,
        currentPassword: data.currentPassword,
      });

      reset();
      setOpen(false);
    } catch (error) {
      if (error instanceof ApiError) {
        setFormError(error.message);
        return;
      }

      setFormError("Unable to change email.");
    }
  }

  function handleOpenChange(value: boolean) {
    setOpen(value);

    if (!value) {
      reset();
      setFormError(null);
    }
  }

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogTrigger render={<Button variant="outline" />}>
        Change Email
      </DialogTrigger>

      <DialogContent>
        <DialogHeader>
          <DialogTitle>Change Email</DialogTitle>

          <DialogDescription>
            Enter your new email and current password to confirm this change.
          </DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
          {/* Current Email */}
          <div className="space-y-2">
            <Label htmlFor="currentEmail">Current Email</Label>

            <Input id="currentEmail" value={currentEmail} disabled />
          </div>

          {/* New Email */}
          <div className="space-y-2">
            <Label htmlFor="newEmail">New Email</Label>

            <Input
              id="newEmail"
              type="email"
              placeholder="new@example.com"
              {...register("newEmail")}
              aria-invalid={errors.newEmail ? "true" : "false"}
            />

            {errors.newEmail && (
              <p className="text-sm text-destructive">
                {errors.newEmail.message}
              </p>
            )}
          </div>

          {/* Current Password */}
          <div className="space-y-2">
            <Label htmlFor="currentPassword">Current Password</Label>

            <Input
              id="currentPassword"
              type="password"
              {...register("currentPassword")}
              aria-invalid={errors.currentPassword ? "true" : "false"}
            />

            {errors.currentPassword && (
              <p className="text-sm text-destructive">
                {errors.currentPassword.message}
              </p>
            )}
          </div>

          {/* Backend error */}
          {formError && <p className="text-sm text-destructive">{formError}</p>}

          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              disabled={changeEmail.isPending || isSubmitting}
              onClick={() => handleOpenChange(false)}
            >
              Cancel
            </Button>

            <Button
              type="submit"
              disabled={changeEmail.isPending || isSubmitting}
            >
              {changeEmail.isPending ? "Changing..." : "Change Email"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}