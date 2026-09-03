"use client";

import Link from "next/link";
import { useParams, useRouter } from "next/navigation";
import { useQuery, useMutation, useQueryClient } from "@tanstack/react-query";

import { getVehicle, deleteVehicle } from "@/lib/api/Vehicles";

import { Button } from "@/components/ui/button";

import { IndividualVehicleSkeleton } from "@/components/skeletons/IndividualVehicleSkeleton";
import { DeleteVehicleDialog } from "@/components/vehicles/DeleteVehicleDialog";

export default function VehicleDetailsPage() {
  const params = useParams<{ id: string }>();
  const id = Number(params.id);
  const queryClient = useQueryClient();
  const router = useRouter();

  const {
    data: vehicle,
    isLoading,
    isError,
  } = useQuery({
    queryKey: ["vehicle", id],
    queryFn: () => getVehicle(id),
    enabled: Number.isInteger(id) && id > 0,
  });

  const deleteMutation = useMutation({
    mutationFn: () => deleteVehicle(id),

    onSuccess: async () => {
      await queryClient.invalidateQueries({
        queryKey: ["vehicles"],
      });

      router.push("/vehicles");
    },
  });

  if (!Number.isInteger(id) || id <= 0) {
    return (
      <main className="mx-auto max-w-4xl px-6 py-12">
        <h1 className="text-2xl font-bold">Invalid Vehicle</h1>

        <p className="mt-2 text-muted-foreground">The vehicle ID is invalid.</p>

        <Button className="mt-6">
          <Link href="/vehicles">Back to Vehicles</Link>
        </Button>
      </main>
    );
  }

  if (isLoading) {
    return <IndividualVehicleSkeleton />;
  }

  if (isError || !vehicle) {
    return (
      <main className="mx-auto max-w-4xl px-6 py-12">
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
    <main className="mx-auto max-w-4xl px-6 py-12">
      <div className="mb-8">
        <Link
          href="/vehicles"
          className="text-sm text-muted-foreground hover:text-foreground"
        >
          ← Back to Vehicles
        </Link>

        <div className="mt-4 flex items-start justify-between gap-4">
          <div>
            <h1 className="text-3xl font-bold">
              {vehicle.make} {vehicle.model}
            </h1>

            <p className="mt-2 text-muted-foreground">{vehicle.plateNumber}</p>
          </div>

          <div className="flex gap-2">
            <Button>
              <Link href={`/vehicles/${vehicle.id}/edit`}>Edit Vehicle</Link>
            </Button>
            <DeleteVehicleDialog
              onConfirm={() => deleteMutation.mutate()}
              isPending={deleteMutation.isPending}
            />
          </div>
        </div>
      </div>

      <div className="overflow-hidden rounded-xl border bg-white shadow-sm">
        <div className="border-b px-6 py-4">
          <h2 className="font-semibold">Vehicle Information</h2>
        </div>

        <div className="grid gap-6 p-6 sm:grid-cols-2">
          <div>
            <p className="text-sm text-muted-foreground">Plate Number</p>

            <p className="mt-1 font-medium">{vehicle.plateNumber}</p>
          </div>

          <div>
            <p className="text-sm text-muted-foreground">Make</p>

            <p className="mt-1 font-medium">{vehicle.make}</p>
          </div>

          <div>
            <p className="text-sm text-muted-foreground">Model</p>

            <p className="mt-1 font-medium">{vehicle.model}</p>
          </div>

          <div>
            <p className="text-sm text-muted-foreground">Year</p>

            <p className="mt-1 font-medium">{vehicle.year}</p>
          </div>

          <div>
            <p className="text-sm text-muted-foreground">Status</p>

            <p className="mt-1 font-medium capitalize">{vehicle.status}</p>
          </div>

          <div>
            <p className="text-sm text-muted-foreground">Customer ID</p>

            <p className="mt-1 font-medium">{vehicle.customerId}</p>
          </div>

          <div>
            <p className="text-sm text-muted-foreground">Created</p>

            <p className="mt-1 font-medium">
              {new Date(vehicle.createdAt).toLocaleDateString()}
            </p>
          </div>

          <div>
            <p className="text-sm text-muted-foreground">Last Updated</p>

            <p className="mt-1 font-medium">
              {new Date(vehicle.updatedAt).toLocaleDateString()}
            </p>
          </div>
        </div>
      </div>
    </main>
  );
}
