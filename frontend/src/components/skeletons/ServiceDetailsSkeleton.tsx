import { Skeleton } from "../ui/skeleton";

export function ServiceDetailsSkeleton() {
  return (
    <main className="min-h-screen bg-white">
      <div className="mx-auto max-w-7xl px-6 py-20 sm:px-8">
        <Skeleton className="h-6 w-32 rounded bg-slate-200" />

        <div className="mt-8 grid gap-12 lg:grid-cols-2">
          <Skeleton className="h-112.5 rounded-2xl bg-slate-200" />

          <div className="space-y-6">
            <Skeleton className="h-10 w-3/4 rounded bg-slate-200" />

            <Skeleton className="h-24 w-full rounded bg-slate-200" />

            <Skeleton className="h-16 w-1/2 rounded bg-slate-200" />
          </div>
        </div>
      </div>
    </main>
  );
}