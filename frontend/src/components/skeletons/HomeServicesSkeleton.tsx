import { Skeleton } from "@/components/ui/skeleton";

export function HomeServicesSkeleton() {
  return (
    <section className="bg-slate-950 py-24 sm:py-28">
      <div className="mx-auto max-w-7xl px-6 lg:px-8">
        {/* Header */}
        <div className="max-w-2xl">
          <Skeleton className="h-4 w-32 bg-slate-800" />

          <Skeleton className="mt-5 h-12 w-80 bg-slate-800" />

          <Skeleton className="mt-2 h-12 w-64 bg-slate-800" />
        </div>

        {/* Intro */}
        <Skeleton className="mt-12 aspect-16/7 rounded-3xl bg-slate-800" />

        {/* Services */}
        <div className="mt-6 grid gap-6 sm:grid-cols-2">
          {[1, 2, 3, 4].map((item) => (
            <Skeleton
              key={item}
              className="aspect-video rounded-3xl bg-slate-800"
            />
          ))}
        </div>

        {/* View all */}
        <div className="mt-10 flex justify-center">
          <Skeleton className="h-5 w-32 bg-slate-800" />
        </div>
      </div>
    </section>
  );
}