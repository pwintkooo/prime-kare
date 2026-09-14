"use client";

import { useState } from "react";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";
import { Mail, Phone, ShieldCheck, UserRound } from "lucide-react";

import { useProfile, useUpdateProfile } from "@/hooks/use-profile";

import { ApiError } from "@/api/apiError";

import {
  updateProfileSchema,
  UpdateProfileFormData,
} from "@/lib/validations/profile";

import { Button } from "@/components/ui/button";
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";

import { ProfilePageSkeleton } from "@/components/skeletons/ProfilePageSkeleton";
import { ChangeEmailDialog } from "@/components/profile/change-email-dialog";
import { ChangePasswordDialog } from "@/components/profile/change-password-dialog";

export default function ProfilePage() {
  const { data: profile, isLoading, isError } = useProfile();

  const updateProfile = useUpdateProfile();

  const [isEditing, setIsEditing] = useState(false);

  const [successMessage, setSuccessMessage] = useState<string | null>(null);

  const [formError, setFormError] = useState<string | null>(null);

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors, isSubmitting },
  } = useForm<UpdateProfileFormData>({
    resolver: zodResolver(updateProfileSchema),
    defaultValues: {
      name: "",
      phone: "",
    },
  });

  function handleEdit() {
    if (!profile) {
      return;
    }

    reset({
      name: profile.name,
      phone: profile.phone ?? "",
    });

    setFormError(null);
    setSuccessMessage(null);
    setIsEditing(true);
  }

  function handleCancel() {
    reset();

    setFormError(null);
    setSuccessMessage(null);
    setIsEditing(false);
  }

  async function onSubmit(data: UpdateProfileFormData) {
    setFormError(null);
    setSuccessMessage(null);

    try {
      await updateProfile.mutateAsync({
        name: data.name,
        phone: data.phone,
      });

      setSuccessMessage("Profile updated successfully.");

      reset();
      setIsEditing(false);
    } catch (error) {
      if (error instanceof ApiError) {
        setFormError(error.message);
        return;
      }

      setFormError("Unable to update profile.");
    }
  }

  if (isLoading) {
    return <ProfilePageSkeleton />;
  }

  if (isError || !profile) {
    return (
      <div className="mx-auto max-w-4xl px-4 py-8 sm:px-6">
        <p className="text-sm text-destructive">Unable to load your profile.</p>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-4xl px-4 py-8 sm:px-6">
      <div className="mb-8">
        <h1 className="text-3xl font-semibold tracking-tight">Profile</h1>

        <p className="mt-2 text-sm text-muted-foreground">
          Manage your personal information and account security.
        </p>
      </div>

      <div className="space-y-6">
        {/* Personal Information */}
        <Card>
          <CardHeader>
            <div className="flex items-center gap-3">
              <div className="rounded-lg border p-2">
                <UserRound className="size-5" />
              </div>

              <div>
                <CardTitle>Personal Information</CardTitle>

                <CardDescription>
                  Update your name and contact information.
                </CardDescription>
              </div>
            </div>
          </CardHeader>

          <CardContent>
            {successMessage && (
              <p className="mb-5 text-sm text-green-600">{successMessage}</p>
            )}

            {!isEditing ? (
              <div className="space-y-6">
                <div className="grid gap-6 sm:grid-cols-2">
                  <div className="space-y-1">
                    <p className="text-sm text-muted-foreground">Name</p>

                    <p className="font-medium">{profile.name}</p>
                  </div>

                  <div className="space-y-1">
                    <p className="text-sm text-muted-foreground">Phone</p>

                    <p className="font-medium">
                      {profile.phone || "Not provided"}
                    </p>
                  </div>
                </div>

                <div className="flex justify-end">
                  <Button variant="outline" onClick={handleEdit}>
                    Edit Information
                  </Button>
                </div>
              </div>
            ) : (
              <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
                <div className="grid gap-6 sm:grid-cols-2">
                  {/* Name */}
                  <div className="space-y-2">
                    <Label htmlFor="name">Name</Label>

                    <Input
                      id="name"
                      {...register("name")}
                      aria-invalid={errors.name ? "true" : "false"}
                    />

                    {errors.name && (
                      <p className="text-sm text-destructive">
                        {errors.name.message}
                      </p>
                    )}
                  </div>

                  {/* Phone */}
                  <div className="space-y-2">
                    <Label htmlFor="phone">Phone</Label>

                    <div className="relative">
                      <Phone className="absolute left-3 top-1/2 size-4 -translate-y-1/2 text-muted-foreground" />

                      <Input
                        id="phone"
                        {...register("phone")}
                        className="pl-9"
                        aria-invalid={errors.phone ? "true" : "false"}
                      />
                    </div>

                    {errors.phone ? (
                      <p className="text-sm text-destructive">
                        {errors.phone.message}
                      </p>
                    ) : (
                      <p className="text-xs text-muted-foreground">Optional</p>
                    )}
                  </div>
                </div>

                {formError && (
                  <p className="text-sm text-destructive">{formError}</p>
                )}

                <div className="flex justify-end gap-2">
                  <Button
                    type="button"
                    variant="outline"
                    disabled={updateProfile.isPending || isSubmitting}
                    onClick={handleCancel}
                  >
                    Cancel
                  </Button>

                  <Button
                    type="submit"
                    disabled={updateProfile.isPending || isSubmitting}
                  >
                    {updateProfile.isPending ? "Saving..." : "Save Changes"}
                  </Button>
                </div>
              </form>
            )}
          </CardContent>
        </Card>

        {/* Email */}
        <Card>
          <CardHeader>
            <div className="flex items-center gap-3">
              <div className="rounded-lg border p-2">
                <Mail className="size-5" />
              </div>

              <div>
                <CardTitle>Email Address</CardTitle>

                <CardDescription>
                  Your email is used to sign in to your account.
                </CardDescription>
              </div>
            </div>
          </CardHeader>

          <CardContent>
            <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
              <div>
                <p className="font-medium">{profile.email}</p>

                {!profile.canChangeEmail && (
                  <p className="mt-1 text-sm text-muted-foreground">
                    Email cannot be changed for an externally connected account.
                  </p>
                )}
              </div>

              {profile.canChangeEmail && (
                <ChangeEmailDialog currentEmail={profile.email} />
              )}
            </div>
          </CardContent>
        </Card>

        {/* Security */}
        <Card>
          <CardHeader>
            <div className="flex items-center gap-3">
              <div className="rounded-lg border p-2">
                <ShieldCheck className="size-5" />
              </div>

              <div>
                <CardTitle>Security</CardTitle>

                <CardDescription>
                  Manage your account password and login methods.
                </CardDescription>
              </div>
            </div>
          </CardHeader>

          <CardContent className="space-y-5">
            <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
              <div>
                <p className="font-medium">Password</p>

                <p className="text-sm text-muted-foreground">
                  {profile.canChangePassword
                    ? "Change your account password."
                    : "Password changes are unavailable for external login accounts."}
                </p>
              </div>

              {profile.canChangePassword && <ChangePasswordDialog />}
            </div>

            {profile.externalProviders.length > 0 && (
              <div className="border-t pt-5">
                <p className="font-medium">Connected Accounts</p>

                <div className="mt-3 flex flex-wrap gap-2">
                  {profile.externalProviders.map((provider) => (
                    <div
                      key={provider}
                      className="rounded-md border px-3 py-2 text-sm"
                    >
                      {provider}
                    </div>
                  ))}
                </div>
              </div>
            )}
          </CardContent>
        </Card>
      </div>
    </div>
  );
}
