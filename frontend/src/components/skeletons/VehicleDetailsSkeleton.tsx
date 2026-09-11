import { Skeleton } from "../ui/skeleton";

export function VehicleDetailsSkeleton() {
  return (
    <main className="mx-auto max-w-4xl px-6 py-12">
      <div className="space-y-6">
        <Skeleton className="h-8 w-48" />
        <Skeleton className="h-64 w-full rounded-xl" />
      </div>
    </main>
  );
}
