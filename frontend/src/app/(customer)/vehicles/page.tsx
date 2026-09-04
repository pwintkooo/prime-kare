"use client";

import Link from "next/link";
import { useQuery } from "@tanstack/react-query";
import { Car, Plus, CalendarDays, Pencil, ChevronRight } from "lucide-react";

import { getVehicles } from "@/lib/api/Vehicles";
import { Button } from "@/components/ui/button";
import { VehiclesSkeleton } from "@/components/skeletons/VehiclesSkeleton";

export default function VehiclesPage() {
  const {
    data: vehicles,
    isLoading,
    isError,
  } = useQuery({
    queryKey: ["vehicles"],
    queryFn: getVehicles,
  });

  if (isLoading) {
    return <VehiclesSkeleton />;
  }

  if (isError) {
    return (
      <main className="min-h-screen bg-slate-50">
        <div className="mx-auto max-w-7xl px-6 py-12">
          <div className="rounded-2xl border border-red-200 bg-red-50 p-6">
            <h2 className="font-semibold text-red-800">
              Unable to load vehicles
            </h2>

            <p className="mt-1 text-sm text-red-600">
              Something went wrong while loading your vehicles.
            </p>
          </div>
        </div>
      </main>
    );
  }

  return (
    <main className="min-h-screen bg-slate-50">
      <div className="mx-auto max-w-7xl px-6 py-12">
        {/* Header */}
        <div className="flex flex-col gap-4 sm:flex-row sm:items-center sm:justify-between">
          <div>
            <p className="text-sm font-semibold uppercase tracking-wider text-blue-600">
              My Garage
            </p>

            <h1 className="mt-2 text-3xl font-bold tracking-tight text-slate-900">
              My Vehicles
            </h1>

            <p className="mt-2 text-slate-500">
              Manage your vehicles and keep track of their service history.
            </p>
          </div>

          <Link href="/vehicles/new">
            <Button className="gap-2">
              <Plus className="h-4 w-4" />
              Add Vehicle
            </Button>
          </Link>
        </div>

        {/* Empty state */}
        {!vehicles || vehicles.length === 0 ? (
          <div className="mt-10 flex flex-col items-center justify-center rounded-2xl border border-dashed border-slate-300 bg-white px-6 py-16 text-center">
            <div className="flex h-16 w-16 items-center justify-center rounded-full bg-blue-50">
              <Car className="h-8 w-8 text-blue-600" />
            </div>

            <h2 className="mt-5 text-xl font-semibold text-slate-900">
              No vehicles yet
            </h2>

            <p className="mt-2 max-w-md text-sm text-slate-500">
              Add your first vehicle to start managing appointments and service
              history.
            </p>

            <Link href="/vehicles/new" className="mt-6">
              <Button className="gap-2">
                <Plus className="h-4 w-4" />
                Add Your First Vehicle
              </Button>
            </Link>
          </div>
        ) : (
          /* Vehicle cards */
          <div className="mt-10 grid gap-6 md:grid-cols-2 lg:grid-cols-3">
            {vehicles.map((vehicle) => (
              <div
                key={vehicle.id}
                className="group overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-sm transition hover:-translate-y-1 hover:shadow-md"
              >
                {/* Card header */}
                <div className="flex items-center justify-between border-b border-slate-100 px-6 py-5">
                  <div className="flex items-center gap-3">
                    <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-blue-50">
                      <Car className="h-6 w-6 text-blue-600" />
                    </div>

                    <div>
                      <h2 className="font-semibold text-slate-900">
                        {vehicle.make} {vehicle.model}
                      </h2>

                      <p className="text-sm text-slate-500">{vehicle.year}</p>
                    </div>
                  </div>

                  <span
                    className={`rounded-full px-3 py-1 text-xs font-medium ${
                      vehicle.status.toLowerCase() === "active"
                        ? "bg-green-50 text-green-700"
                        : "bg-slate-100 text-slate-600"
                    }`}
                  >
                    {vehicle.status}
                  </span>
                </div>

                {/* Vehicle details */}
                <div className="space-y-4 px-6 py-5">
                  <div>
                    <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
                      License Plate
                    </p>

                    <p className="mt-1 font-semibold text-slate-900">
                      {vehicle.plateNumber}
                    </p>
                  </div>

                  <div className="flex items-center gap-2 text-sm text-slate-500">
                    <CalendarDays className="h-4 w-4" />

                    <span>
                      Added {new Date(vehicle.createdAt).toLocaleDateString()}
                    </span>
                  </div>
                </div>

                {/* Actions */}
                <div className="flex items-center border-t border-slate-100 px-6 py-4">
                  <Link
                    href={`/vehicles/${vehicle.id}`}
                    className="flex flex-1 items-center justify-between text-sm font-medium text-blue-600 transition hover:text-blue-700"
                  >
                    View vehicle
                    <ChevronRight className="h-4 w-4 transition-transform group-hover:translate-x-1" />
                  </Link>

                  <Link
                    href={`/vehicles/${vehicle.id}/edit`}
                    className="ml-4 flex items-center gap-1.5 text-sm font-medium text-slate-500 transition hover:text-slate-900"
                  >
                    <Pencil className="h-4 w-4" />
                    Edit
                  </Link>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>
    </main>
  );
}
