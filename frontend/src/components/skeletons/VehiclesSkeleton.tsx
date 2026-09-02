import { Skeleton } from "@/components/ui/skeleton";

export function VehiclesSkeleton() {
  return (
    <main className="min-h-screen bg-slate-50">
      <div className="mx-auto max-w-7xl px-6 py-12">
        <Skeleton className="h-8 w-48 rounded-lg" />

        <div className="mt-8 grid gap-6 md:grid-cols-2 lg:grid-cols-3">
          {[1, 2, 3].map((item) => (
            <Skeleton
              key={item}
              className="h-56 rounded-2xl bg-white shadow-sm"
            />
          ))}
        </div>
      </div>
    </main>
  );
}
