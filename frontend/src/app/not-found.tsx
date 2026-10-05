"use client";

import Link from "next/link";
import { ArrowLeft, CarFront, Home } from "lucide-react";

import { Button } from "@/components/ui/button";

export default function NotFound() {
  return (
    <main className="relative flex min-h-[75vh] items-center justify-center overflow-hidden px-6 py-16">
      {/* Background 404 */}
      <div
        aria-hidden="true"
        className="pointer-events-none absolute select-none text-[12rem] font-bold leading-none text-slate-100 sm:text-[18rem]"
      >
        404
      </div>

      <div className="relative z-10 flex max-w-xl flex-col items-center text-center">
        {/* Icon */}
        <div className="flex size-20 items-center justify-center rounded-2xl bg-blue-50 shadow-sm ring-1 ring-blue-100">
          <CarFront className="size-10 text-blue-600" />
        </div>

        {/* Error label */}
        <div className="mt-6 rounded-full border border-blue-100 bg-blue-50 px-3 py-1 text-xs font-semibold uppercase tracking-wider text-blue-600">
          Error 404
        </div>

        {/* Title */}
        <h1 className="mt-4 text-3xl font-bold tracking-tight text-slate-900 sm:text-4xl">
          Looks like you took a wrong turn
        </h1>

        {/* Description */}
        <p className="mt-4 max-w-md text-base leading-7 text-slate-500">
          The page you&apos;re looking for doesn&apos;t exist, has been moved,
          or is no longer available.
        </p>

        {/* Actions */}
        <div className="mt-8 flex flex-col gap-3 sm:flex-row">
          <Button
            variant="outline"
            className="gap-2"
            onClick={() => window.history.back()}
          >
            <ArrowLeft className="size-4" />
            Go Back
          </Button>

          <Link href="/">
            <Button className="w-full gap-2 sm:w-auto">
              <Home className="size-4" />
              Back to Home
            </Button>
          </Link>
        </div>

        {/* Help text */}
        <p className="mt-8 text-sm text-slate-400">
          Need a service instead?{" "}
          <Link
            href="/book-appointment"
            className="font-medium text-blue-600 hover:text-blue-700 hover:underline"
          >
            Book an appointment
          </Link>
        </p>
      </div>
    </main>
  );
}
