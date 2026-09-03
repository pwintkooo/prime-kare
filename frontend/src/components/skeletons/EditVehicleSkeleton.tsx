import { Skeleton } from "../ui/skeleton";

export function EditVehicleSkeleton() {
  return (
    <main className="mx-auto max-w-3xl px-6 py-12">
      <div className="space-y-6">
        <Skeleton className="h-8 w-48" />
        <Skeleton className="h-80 w-full rounded-xl" />
      </div>
    </main>
  );
}
