"use client";

import Link from "next/link";
import { useParams, useRouter } from "next/navigation";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

import { updateVehicle } from "@/api/vehicles";
import {
  updateVehicleSchema,
  UpdateVehicleFormData,
} from "@/validations/vehicle";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

import { EditVehicleSkeleton } from "@/components/skeletons/EditVehicleSkeleton";
import { useVehicle } from "@/hooks/use-vehicles";

export default function EditVehiclePage() {
  const params = useParams<{ id: string }>();
  const router = useRouter();
  const queryClient = useQueryClient();

  const id = Number(params.id);

  const { data: vehicle, isLoading, isError } = useVehicle(id);

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<UpdateVehicleFormData>({
    resolver: zodResolver(updateVehicleSchema),
    values: vehicle
      ? {
          plateNumber: vehicle.plateNumber,
          make: vehicle.make,
          model: vehicle.model,
          year: vehicle.year,
        }
      : undefined,
  });

  const updateMutation = useMutation({
    mutationFn: (data: UpdateVehicleFormData) => updateVehicle(id, data),

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["vehicle", id],
      });

      await queryClient.invalidateQueries({
        queryKey: ["vehicles"],
      });

      router.push(`/vehicles/${id}`);
    },
  });

  const onSubmit = (data: UpdateVehicleFormData) => {
    updateMutation.mutate(data);
  };

  if (!Number.isInteger(id) || id <= 0) {
    return (
      <main className="mx-auto max-w-3xl px-6 py-12">
        <h1 className="text-2xl font-bold">Invalid Vehicle</h1>

        <Button className="mt-6">
          <Link href="/vehicles">Back to Vehicles</Link>
        </Button>
      </main>
    );
  }

  if (isLoading) {
    return <EditVehicleSkeleton />;
  }

  if (isError || !vehicle) {
    return (
      <main className="mx-auto max-w-3xl px-6 py-12">
        <h1 className="text-2xl font-bold">Vehicle Not Found</h1>

        <p className="mt-2 text-muted-foreground">
          We couldn&apos;t find this vehicle.
        </p>

        <Button className="mt-6">
          <Link href="/vehicles">Back to Vehicles</Link>
        </Button>
      </main>
    );
  }

  return (
    <main className="mx-auto max-w-3xl px-6 py-12">
      <div className="mb-8">
        <Link
          href={`/vehicles/${id}`}
          className="text-sm text-muted-foreground hover:text-foreground"
        >
          ← Back to Vehicle
        </Link>

        <h1 className="mt-4 text-3xl font-bold">Edit Vehicle</h1>

        <p className="mt-2 text-muted-foreground">
          Update the vehicle information below.
        </p>
      </div>

      <div className="rounded-xl border bg-white p-6 shadow-sm">
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          <div className="space-y-2">
            <label htmlFor="plateNumber" className="text-sm font-medium">
              Plate Number
            </label>

            <Input id="plateNumber" {...register("plateNumber")} />

            {errors.plateNumber && (
              <p className="text-sm text-red-500">
                {errors.plateNumber.message}
              </p>
            )}
          </div>

          <div className="space-y-2">
            <label htmlFor="make" className="text-sm font-medium">
              Make
            </label>

            <Input id="make" {...register("make")} />

            {errors.make && (
              <p className="text-sm text-red-500">{errors.make.message}</p>
            )}
          </div>

          <div className="space-y-2">
            <label htmlFor="model" className="text-sm font-medium">
              Model
            </label>

            <Input id="model" {...register("model")} />

            {errors.model && (
              <p className="text-sm text-red-500">{errors.model.message}</p>
            )}
          </div>

          <div className="space-y-2">
            <label htmlFor="year" className="text-sm font-medium">
              Year
            </label>

            <Input
              id="year"
              type="number"
              {...register("year", {
                valueAsNumber: true,
              })}
            />

            {errors.year && (
              <p className="text-sm text-red-500">{errors.year.message}</p>
            )}
          </div>

          {updateMutation.isError && (
            <div className="rounded-md bg-red-50 p-3 text-sm text-red-600">
              {updateMutation.error instanceof Error
                ? updateMutation.error.message
                : "Failed to update vehicle."}
            </div>
          )}

          <div className="flex justify-end gap-3">
            <Button
              type="button"
              variant="outline"
              onClick={() => router.push(`/vehicles/${id}`)}
              disabled={updateMutation.isPending}
            >
              Cancel
            </Button>

            <Button type="submit" disabled={updateMutation.isPending}>
              {updateMutation.isPending ? "Saving..." : "Save Changes"}
            </Button>
          </div>
        </form>
      </div>
    </main>
  );
}
