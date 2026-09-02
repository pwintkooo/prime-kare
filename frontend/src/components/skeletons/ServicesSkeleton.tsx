import { Skeleton } from "../ui/skeleton";

export function ServicesSkeleton() {
  return (
    <div className="mt-12 grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
      {[1, 2, 3, 4, 5, 6].map((item) => (
        <div
          key={item}
          className="overflow-hidden rounded-2xl border border-slate-200 bg-white"
        >
          <Skeleton className="h-52 rounded-none bg-slate-200" />

          <div className="space-y-4 p-6">
            <Skeleton className="h-5 w-2/3 rounded bg-slate-200" />

            <Skeleton className="h-4 w-full rounded bg-slate-200" />

            <Skeleton className="h-4 w-4/5 rounded bg-slate-200" />
          </div>
        </div>
      ))}
    </div>
  );
}