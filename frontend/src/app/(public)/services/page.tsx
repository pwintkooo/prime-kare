"use client";

import { useQuery } from "@tanstack/react-query";
import Link from "next/link";
import Image from "next/image";
import { Clock, ArrowRight } from "lucide-react";

import { getServices } from "@/lib/api/Services";

export default function ServicesPage() {
  const {
    data: services = [],
    isLoading,
    isError,
  } = useQuery({
    queryKey: ["services"],
    queryFn: getServices,
  });

  return (
    <main className="bg-white">
      {/* Hero */}
      <section className="bg-slate-950">
        <div className="mx-auto max-w-7xl px-6 py-24 sm:px-8 lg:py-28">
          <div className="max-w-3xl">
            <p className="text-sm font-semibold uppercase tracking-[0.2em] text-blue-400">
              Our Services
            </p>

            <h1 className="mt-4 text-4xl font-bold tracking-tight text-white sm:text-5xl lg:text-6xl">
              Professional care for
              <span className="text-slate-400"> your vehicle.</span>
            </h1>

            <p className="mt-6 max-w-2xl text-lg leading-8 text-slate-300">
              From routine maintenance to essential repairs, PrimeKare provides
              reliable automotive services to keep your vehicle running at its
              best.
            </p>
          </div>
        </div>
      </section>

      {/* Services */}
      <section>
        <div className="mx-auto max-w-7xl px-6 py-20 sm:px-8 lg:py-28">
          <div className="flex flex-col justify-between gap-4 sm:flex-row sm:items-end">
            <div>
              <p className="text-sm font-semibold uppercase tracking-[0.2em] text-blue-600">
                What we offer
              </p>

              <h2 className="mt-3 text-3xl font-bold tracking-tight text-slate-950 sm:text-4xl">
                Choose the right service.
              </h2>
            </div>

            <p className="max-w-md text-sm leading-6 text-slate-500">
              Select a service to learn more about what&apos;s included and book
              your appointment.
            </p>
          </div>

          {/* Loading */}
          {isLoading && (
            <div className="mt-12 grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
              {[1, 2, 3, 4, 5, 6].map((item) => (
                <div
                  key={item}
                  className="overflow-hidden rounded-2xl border border-slate-200 bg-white"
                >
                  <div className="h-52 animate-pulse bg-slate-200" />

                  <div className="space-y-4 p-6">
                    <div className="h-5 w-2/3 animate-pulse rounded bg-slate-200" />
                    <div className="h-4 w-full animate-pulse rounded bg-slate-200" />
                    <div className="h-4 w-4/5 animate-pulse rounded bg-slate-200" />
                  </div>
                </div>
              ))}
            </div>
          )}

          {/* Error */}
          {isError && (
            <div className="mt-12 rounded-2xl border border-red-200 bg-red-50 p-8 text-center">
              <h3 className="font-semibold text-red-900">
                Unable to load services
              </h3>

              <p className="mt-2 text-sm text-red-600">
                Something went wrong while loading our services. Please try
                again later.
              </p>
            </div>
          )}

          {/* Empty */}
          {!isLoading && !isError && services.length === 0 && (
            <div className="mt-12 rounded-2xl border border-slate-200 p-12 text-center">
              <h3 className="font-semibold text-slate-900">
                No services available
              </h3>

              <p className="mt-2 text-sm text-slate-500">
                Please check back again later.
              </p>
            </div>
          )}

          {/* Service cards */}
          {!isLoading && !isError && services.length > 0 && (
            <div className="mt-12 grid gap-6 sm:grid-cols-2 lg:grid-cols-3">
              {services.map((service) => (
                <article
                  key={service.id}
                  className="group overflow-hidden rounded-2xl border border-slate-200 bg-white transition hover:-translate-y-1 hover:border-blue-200 hover:shadow-lg"
                >
                  {/* Image */}
                  <div className="relative h-52 overflow-hidden bg-slate-100">
                    {service.imageUrl ? (
                      <Image
                        src={service.imageUrl}
                        alt={service.name}
                        fill
                        className="h-full w-full object-cover transition duration-500 group-hover:scale-105"
                      />
                    ) : (
                      <div className="flex h-full items-center justify-center bg-slate-100 text-sm text-slate-400">
                        No image available
                      </div>
                    )}
                  </div>

                  {/* Content */}
                  <div className="p-6">
                    <h3 className="text-xl font-semibold text-slate-950">
                      {service.name}
                    </h3>

                    <p className="mt-3 line-clamp-3 text-sm leading-6 text-slate-600">
                      {service.description}
                    </p>

                    <div className="mt-5 flex items-center justify-between border-t border-slate-100 pt-5">
                      <div>
                        <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
                          From
                        </p>

                        <p className="mt-1 text-lg font-bold text-slate-950">
                          ${service.price.toFixed(2)}
                        </p>
                      </div>

                      <div className="flex items-center gap-1.5 text-sm text-slate-500">
                        <Clock className="h-4 w-4" />
                        {service.estimatedMinutes} min
                      </div>
                    </div>

                    <Link
                      href={`/services/${service.slug}`}
                      className="mt-5 flex items-center justify-center gap-2 rounded-xl bg-slate-950 px-4 py-3 text-sm font-semibold text-white transition hover:bg-blue-600"
                    >
                      View Service
                      <ArrowRight className="h-4 w-4" />
                    </Link>
                  </div>
                </article>
              ))}
            </div>
          )}
        </div>
      </section>

      {/* CTA */}
      <section className="bg-slate-950">
        <div className="mx-auto max-w-7xl px-6 py-20 text-center sm:px-8">
          <h2 className="text-3xl font-bold tracking-tight text-white">
            Ready to book your service?
          </h2>

          <p className="mx-auto mt-4 max-w-xl text-slate-400">
            Choose a service and schedule an appointment at a time that works
            for you.
          </p>

          <Link
            href="/book"
            className="mt-8 inline-flex items-center gap-2 rounded-xl bg-blue-600 px-6 py-3 text-sm font-semibold text-white transition hover:bg-blue-500"
          >
            Book an Appointment
            <ArrowRight className="h-4 w-4" />
          </Link>
        </div>
      </section>
    </main>
  );
}
