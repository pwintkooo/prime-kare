"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

import { useChangePassword } from "@/hooks/use-profile";
import { ApiError } from "@/api/apiError";

import {
  changePasswordSchema,
  ChangePasswordFormData,
} from "@/lib/validations/profile";

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

export function ChangePasswordDialog() {
  const changePassword = useChangePassword();

  const [open, setOpen] = useState(false);

  const [formError, setFormError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<ChangePasswordFormData>({
    resolver: zodResolver(changePasswordSchema),
    defaultValues: {
      currentPassword: "",
      newPassword: "",
      confirmPassword: "",
    },
  });

  async function onSubmit(data: ChangePasswordFormData) {
    setFormError(null);

    try {
      await changePassword.mutateAsync({
        currentPassword: data.currentPassword,
        newPassword: data.newPassword,
        confirmPassword: data.confirmPassword,
      });

      reset();
      setOpen(false);
    } catch (error) {
      if (error instanceof ApiError) {
        setFormError(error.message);
        return;
      }

      setFormError("Unable to change password.");
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
        Change Password
      </DialogTrigger>

      <DialogContent>
        <DialogHeader>
          <DialogTitle>Change Password</DialogTitle>

          <DialogDescription>
            Enter your current password and choose a new password.
          </DialogDescription>
        </DialogHeader>

        <form onSubmit={handleSubmit(onSubmit)} className="space-y-5">
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

          <div className="space-y-2">
            <Label htmlFor="newPassword">New Password</Label>

            <Input
              id="newPassword"
              type="password"
              {...register("newPassword")}
              aria-invalid={errors.newPassword ? "true" : "false"}
            />

            {errors.newPassword ? (
              <p className="text-sm text-destructive">
                {errors.newPassword.message}
              </p>
            ) : (
              <p className="text-xs text-muted-foreground">
                Minimum 8 characters, including uppercase, lowercase, number,
                and special character.
              </p>
            )}
          </div>

          <div className="space-y-2">
            <Label htmlFor="confirmPassword">Confirm Password</Label>

            <Input
              id="confirmPassword"
              type="password"
              {...register("confirmPassword")}
              aria-invalid={errors.confirmPassword ? "true" : "false"}
            />

            {errors.confirmPassword && (
              <p className="text-sm text-destructive">
                {errors.confirmPassword.message}
              </p>
            )}
          </div>

          {formError && <p className="text-sm text-destructive">{formError}</p>}

          <DialogFooter>
            <Button
              type="button"
              variant="outline"
              disabled={changePassword.isPending || isSubmitting}
              onClick={() => handleOpenChange(false)}
            >
              Cancel
            </Button>

            <Button
              type="submit"
              disabled={changePassword.isPending || isSubmitting}
            >
              {changePassword.isPending ? "Changing..." : "Change Password"}
            </Button>
          </DialogFooter>
        </form>
      </DialogContent>
    </Dialog>
  );
}
