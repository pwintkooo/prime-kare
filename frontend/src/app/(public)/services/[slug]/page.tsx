"use client";

import Image from "next/image";
import Link from "next/link";
import { useParams } from "next/navigation";
import { useQuery } from "@tanstack/react-query";
import { ArrowLeft, ArrowRight, Clock } from "lucide-react";

import { getServiceBySlug } from "@/lib/api/Services";
import { IndividualServiceSkeleton } from "@/components/skeletons/IndividualServiceSkeleton";

export default function ServiceDetailsPage() {
  const params = useParams<{ slug: string }>();

  const {
    data: service,
    isLoading,
    isError,
  } = useQuery({
    queryKey: ["service", params.slug],
    queryFn: () => getServiceBySlug(params.slug),
    enabled: !!params.slug,
  });

  if (isLoading) {
    return <IndividualServiceSkeleton />;
  }

  if (isError || !service) {
    return (
      <main className="flex min-h-[70vh] items-center justify-center bg-white px-6">
        <div className="text-center">
          <h1 className="text-3xl font-bold text-slate-950">
            Service not found
          </h1>

          <p className="mt-3 text-slate-500">
            The service you&apos;re looking for doesn&apos;t exist or is no
            longer available.
          </p>

          <Link
            href="/services"
            className="mt-8 inline-flex items-center gap-2 rounded-xl bg-slate-950 px-5 py-3 text-sm font-semibold text-white transition hover:bg-blue-600"
          >
            <ArrowLeft className="h-4 w-4" />
            Back to Services
          </Link>
        </div>
      </main>
    );
  }

  return (
    <main className="bg-white">
      {/* Hero */}
      <section className="bg-slate-950">
        <div className="mx-auto max-w-7xl px-6 py-6 sm:px-8">
          <Link
            href="/services"
            className="inline-flex items-center gap-2 text-sm text-slate-400 transition hover:text-white"
          >
            <ArrowLeft className="h-4 w-4" />
            All Services
          </Link>
        </div>
      </section>

      {/* Service Details */}
      <section>
        <div className="mx-auto max-w-7xl px-6 py-16 sm:px-8 lg:py-24">
          <div className="grid items-center gap-12 lg:grid-cols-2 lg:gap-20">
            {/* Image */}
            <div className="relative h-87.5 overflow-hidden rounded-3xl bg-slate-100 sm:h-112.5">
              {service.imageUrl ? (
                <Image
                  src={service.imageUrl}
                  alt={service.name}
                  fill
                  priority
                  className="object-cover"
                />
              ) : (
                <div className="flex h-full items-center justify-center text-sm text-slate-400">
                  No image available
                </div>
              )}
            </div>

            {/* Information */}
            <div>
              <p className="text-sm font-semibold uppercase tracking-[0.2em] text-blue-600">
                PrimeKare Service
              </p>

              <h1 className="mt-4 text-4xl font-bold tracking-tight text-slate-950 sm:text-5xl">
                {service.name}
              </h1>

              <p className="mt-6 text-lg leading-8 text-slate-600">
                {service.description}
              </p>

              {/* Price + Duration */}
              <div className="mt-8 flex flex-wrap gap-4">
                <div className="rounded-2xl border border-slate-200 bg-slate-50 px-6 py-4">
                  <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
                    Starting from
                  </p>

                  <p className="mt-1 text-2xl font-bold text-slate-950">
                    ${service.price.toFixed(2)}
                  </p>
                </div>

                <div className="flex items-center gap-3 rounded-2xl border border-slate-200 bg-slate-50 px-6 py-4">
                  <Clock className="h-5 w-5 text-blue-600" />

                  <div>
                    <p className="text-xs font-medium uppercase tracking-wide text-slate-400">
                      Estimated time
                    </p>

                    <p className="mt-1 font-semibold text-slate-950">
                      {service.estimatedMinutes} minutes
                    </p>
                  </div>
                </div>
              </div>

              {/* CTA */}
              <div className="mt-8 flex flex-col gap-3 sm:flex-row">
                <Link
                  href={`/book?service=${service.slug}`}
                  className="inline-flex items-center justify-center gap-2 rounded-xl bg-blue-600 px-6 py-3.5 text-sm font-semibold text-white transition hover:bg-blue-500"
                >
                  Book This Service
                  <ArrowRight className="h-4 w-4" />
                </Link>

                <Link
                  href="/services"
                  className="inline-flex items-center justify-center rounded-xl border border-slate-200 px-6 py-3.5 text-sm font-semibold text-slate-700 transition hover:bg-slate-50"
                >
                  View All Services
                </Link>
              </div>
            </div>
          </div>
        </div>
      </section>

      {/* Why PrimeKare */}
      <section className="border-t border-slate-100 bg-slate-50">
        <div className="mx-auto max-w-7xl px-6 py-20 sm:px-8">
          <div className="max-w-2xl">
            <p className="text-sm font-semibold uppercase tracking-[0.2em] text-blue-600">
              Why PrimeKare
            </p>

            <h2 className="mt-3 text-3xl font-bold tracking-tight text-slate-950">
              Professional service you can trust.
            </h2>

            <p className="mt-4 leading-7 text-slate-600">
              We make vehicle maintenance simple and transparent. Book your
              service online and let our team take care of the rest.
            </p>
          </div>

          <div className="mt-10 grid gap-6 sm:grid-cols-3">
            <div className="rounded-2xl border border-slate-200 bg-white p-6">
              <h3 className="font-semibold text-slate-950">
                Professional Care
              </h3>

              <p className="mt-2 text-sm leading-6 text-slate-500">
                Quality service focused on keeping your vehicle in excellent
                condition.
              </p>
            </div>

            <div className="rounded-2xl border border-slate-200 bg-white p-6">
              <h3 className="font-semibold text-slate-950">
                Transparent Pricing
              </h3>

              <p className="mt-2 text-sm leading-6 text-slate-500">
                Know the starting price before you book your appointment.
              </p>
            </div>

            <div className="rounded-2xl border border-slate-200 bg-white p-6">
              <h3 className="font-semibold text-slate-950">Easy Booking</h3>

              <p className="mt-2 text-sm leading-6 text-slate-500">
                Schedule your service online without needing to call the
                workshop.
              </p>
            </div>
          </div>
        </div>
      </section>
    </main>
  );
}
