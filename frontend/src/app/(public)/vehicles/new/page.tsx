"use client";

import Link from "next/link";
import React, { useState } from "react";
import { useRouter } from "next/navigation";
import { useMutation, useQueryClient } from "@tanstack/react-query";
import { useForm } from "react-hook-form";
import { zodResolver } from "@hookform/resolvers/zod";

import { createVehicle } from "@/lib/api/Vehicles";
import {
  createVehicleSchema,
  CreateVehicleFormData,
} from "@/lib/validations/vehicle";

import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";

export default function NewVehiclePage() {
  const router = useRouter();
  const queryClient = useQueryClient();
  const [serverError, setServerError] = useState("");

  const {
    register,
    handleSubmit,
    formState: { errors },
  } = useForm<CreateVehicleFormData>({
    resolver: zodResolver(createVehicleSchema),
    defaultValues: {
      plateNumber: "",
      make: "",
      model: "",
      year: undefined,
    },
  });

  const createMutation = useMutation({
    mutationFn: createVehicle,

    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["vehicles"],
      });

      router.push("/vehicles");
    },
  });

  const onSubmit = (data: CreateVehicleFormData) => {
    setServerError("");
    createMutation.mutate(data);
  };

  return (
    <main className="mx-auto max-w-3xl px-6 py-12">
      <div className="mb-8">
        <Link
          href="/vehicles"
          className="text-sm text-muted-foreground hover:text-foreground"
        >
          ← Back to Vehicles
        </Link>

        <h1 className="mt-4 text-3xl font-bold">Add New Vehicle</h1>

        <p className="mt-2 text-muted-foreground">
          Add a vehicle to your account.
        </p>
      </div>

      <div className="rounded-xl border bg-white p-6 shadow-sm">
        <form onSubmit={handleSubmit(onSubmit)} className="space-y-6">
          {/* Plate Number */}
          <div className="space-y-2">
            <label htmlFor="plateNumber" className="text-sm font-medium">
              Plate Number
            </label>

            <Input
              id="plateNumber"
              type="text"
              placeholder="e.g. SLA1234A"
              {...register("plateNumber")}
            />

            {errors.plateNumber && (
              <p className="text-sm text-red-500">
                {errors.plateNumber.message}
              </p>
            )}
          </div>

          {/* Make */}
          <div className="space-y-2">
            <label htmlFor="make" className="text-sm font-medium">
              Make
            </label>

            <Input id="make" type="text" placeholder="e.g. Toyota" {...register("make")} />

            {errors.make && (
              <p className="text-sm text-red-500">{errors.make.message}</p>
            )}
          </div>

          {/* Model */}
          <div className="space-y-2">
            <label htmlFor="model" className="text-sm font-medium">
              Model
            </label>

            <Input id="model" type="text" placeholder="e.g. Camry" {...register("model")} />

            {errors.model && (
              <p className="text-sm text-red-500">{errors.model.message}</p>
            )}
          </div>

          {/* Year */}
          <div className="space-y-2">
            <label htmlFor="year" className="text-sm font-medium">
              Year
            </label>

            <Input
              id="year"
              type="number"
              placeholder="e.g. 2022"
              {...register("year", {
                valueAsNumber: true,
              })}
            />

            {errors.year && (
              <p className="text-sm text-red-500">{errors.year.message}</p>
            )}
          </div>

          {/* API Error */}
          {createMutation.isError && (
            <div className="rounded-md bg-red-50 p-3 text-sm text-red-600">
              {createMutation.error instanceof Error
                ? createMutation.error.message
                : "Failed to create vehicle."}
            </div>
          )}

          {/* Buttons */}
          <div className="flex justify-end gap-3">
            <Button
              type="button"
              variant="outline"
              onClick={() => router.push("/vehicles")}
              disabled={createMutation.isPending}
            >
              Cancel
            </Button>

            <Button type="submit" disabled={createMutation.isPending}>
              {createMutation.isPending ? "Adding..." : "Add Vehicle"}
            </Button>
          </div>
        </form>
      </div>
    </main>
  );
}