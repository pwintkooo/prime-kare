"use client";

import Image from "next/image";
import Link from "next/link";
import { ArrowUpRight } from "lucide-react";
import { useQuery } from "@tanstack/react-query";
import { getServices } from "@/api/services";
import { HomeServicesSkeleton } from "../skeletons/HomeServicesSkeleton";

export default function Services() {
  const {
    data: services,
    isLoading,
    error,
  } = useQuery({
    queryKey: ["services"],
    queryFn: getServices,
  });

  if (isLoading) {
    return <HomeServicesSkeleton />;
  }

  if (error) {
    return <p>Failed to load services.</p>;
  }

  return (
    <section className="bg-slate-950 py-24 sm:py-28">
      <div className="mx-auto max-w-7xl px-6 lg:px-8">
        {/* Header */}
        <div className="max-w-2xl">
          <p className="text-sm font-semibold uppercase tracking-[0.2em] text-blue-400">
            Our Services
          </p>

          <h2 className="mt-4 text-4xl font-bold tracking-tight text-white sm:text-5xl">
            Professional care for
            <br />
            <span className="text-slate-400">every journey.</span>
          </h2>
        </div>

        {/* Intro Image */}
        <div className="group relative mt-12 overflow-hidden rounded-3xl">
          <div className="relative aspect-16/7">
            <Image
              src="/images/home/workshop.jpg"
              alt="Professional car workshop"
              fill
              priority
              sizes="(max-width: 1280px) 100vw, 1280px"
              className="object-cover transition-transform duration-700 group-hover:scale-105"
            />

            {/* Dark overlay */}
            <div className="absolute inset-0 bg-linear-to-r from-black/50 via-black/30 to-black/10" />

            {/* Intro content */}
            <div className="absolute inset-0 flex items-end">
              <div className="max-w-2xl p-8 sm:p-10 lg:p-12">
                <p className="text-sm font-medium uppercase tracking-wider text-blue-400">
                  Automotive Expertise
                </p>

                <h3 className="mt-3 text-1xl font-bold text-white sm:text-4xl">
                  Everything your car needs,
                  <br />
                  all in one place.
                </h3>

                <p className="hidden sm:block mt-4 max-w-xl text-sm leading-6 text-slate-200 sm:text-base">
                  From regular maintenance to unexpected repairs, our
                  experienced technicians provide dependable service to keep you
                  moving.
                </p>
              </div>
            </div>
          </div>
        </div>

        {/* Service Grid */}
        <div className="mt-6 grid gap-6 sm:grid-cols-2">
          {services?.map((service) => (
            <Link
              key={service.id}
              href={`/services/${service.slug}`}
              className="group relative overflow-hidden rounded-3xl"
            >
              <div className="relative aspect-video">
                <Image
                  src={
                    service.imageUrl ??
                    "/images/placeholders/service-placeholder.jpg"
                  }
                  alt={service.imageUrl ? service.name : "Service placeholder"}
                  loading="eager"
                  fill
                  sizes="(max-width: 640px) 100vw, 50vw"
                  className="object-cover transition-transform duration-700 group-hover:scale-105"
                />

                {/* Dark overlay */}
                <div className="absolute inset-0 bg-linear-to-t from-black/50 via-black/30 to-transparent transition-colors duration-300 group-hover:from-black/95" />

                {/* Service content */}
                <div className="absolute inset-x-0 bottom-0 p-6 sm:p-8">
                  <div className="flex items-end justify-between gap-6">
                    <div>
                      <h3 className="text-1xl font-bold text-white sm:text-2xl">
                        {service.name}
                      </h3>

                      <p className="mt-2 max-w-md text-sm leading-6 text-slate-200">
                        {service.description}
                      </p>
                    </div>

                    <span className="flex h-11 w-11 shrink-0 items-center justify-center rounded-full bg-white text-slate-950 transition-all duration-300 group-hover:-translate-y-1 group-hover:translate-x-1">
                      <ArrowUpRight className="h-5 w-5" />
                    </span>
                  </div>
                </div>
              </div>
            </Link>
          ))}
        </div>

        {/* View All */}
        <div className="mt-10 text-center">
          <Link
            href="/services"
            className="inline-flex items-center gap-2 text-sm font-semibold text-white transition-colors hover:text-blue-400"
          >
            View all services
            <ArrowUpRight className="h-4 w-4" />
          </Link>
        </div>
      </div>
    </section>
  );
}
