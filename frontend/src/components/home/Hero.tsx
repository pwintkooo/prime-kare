import Image from "next/image";
import Link from "next/link";
import { ArrowUpRight } from "lucide-react";

export default function Hero() {
  return (
    <section className="relative min-h-175 overflow-hidden sm:min-h-200 lg:min-h-190">
      {/* Background Image */}
      <Image
        src="/images/home/workshop-hero.jpg"
        alt="Professional automotive workshop"
        fill
        priority
        sizes="100vw"
        className="object-cover object-[65%_center] sm:object-center"
      />

      {/* Dark overlay */}
      <div className="absolute inset-0 bg-black/20" />

      {/* Extra gradient for text readability */}
      <div className="absolute inset-0 bg-linear-to-r from-black/75 via-black/40 to-transparent" />

      {/* Hero Content */}
      <div className="relative z-10 mx-auto flex min-h-175 max-w-7xl items-start px-6 pt-24 pb-48 sm:min-h-200 sm:items-end sm:pb-64 md:pb-64 lg:min-h-190 lg:pb-36 lg:px-8">
        <div className="max-w-2xl">
          <p className="mb-3 text-xs font-semibold uppercase tracking-[0.2em] text-blue-400 sm:mb-4 sm:text-sm sm:tracking-[0.25em]">
            Professional Automotive Care
          </p>

          <h1 className="text-4xl font-bold uppercase leading-[0.95] tracking-tight text-white max-[400px]:text-3xl sm:text-6xl lg:text-8xl">
            We Keep
            <br />
            You Moving.
          </h1>

          <p className="mt-5 hidden max-w-lg text-sm leading-6 text-slate-200 sm:block sm:text-lg sm:leading-7">
            Expert automotive servicing and repairs designed to keep your
            vehicle safe, reliable, and ready for the road.
          </p>

          <div className="mt-6 flex flex-col gap-3 sm:mt-8 sm:flex-row sm:gap-4">
            <Link
              href="/appointment"
              className="inline-flex items-center justify-center gap-3 rounded-full bg-white px-5 py-3 text-sm font-semibold text-slate-950 transition hover:bg-slate-100 sm:px-6 sm:py-3.5"
            >
              Request Appointment

              <span className="flex h-7 w-7 items-center justify-center rounded-full bg-blue-600 text-white">
                <ArrowUpRight className="h-4 w-4" />
              </span>
            </Link>

            <Link
              href="/services"
              className="inline-flex items-center justify-center rounded-full border border-white/40 bg-white/10 px-5 py-3 text-sm font-semibold text-white backdrop-blur-sm transition hover:bg-white/20 sm:px-6 sm:py-3.5"
            >
              Explore Services
            </Link>
          </div>
        </div>
      </div>

      {/* Bottom Information Bar */}
      <div className="absolute bottom-0 left-0 right-0 z-20">
        <div className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
          <div className="grid rounded-t-3xl bg-slate-900/95 px-5 py-5 backdrop-blur-md sm:grid-cols-2 sm:px-6 sm:py-6 lg:grid-cols-4 lg:px-8">

            {/* Location */}
            <div className="border-b border-white/10 pb-4 sm:border-r sm:pr-6 lg:border-b-0 lg:pb-0">
              <p className="text-[10px] font-semibold uppercase tracking-wider text-slate-400 sm:text-xs">
                Location
              </p>

              <p className="mt-1.5 text-sm leading-5 text-white">
                Woodlands
                <br />
                Singapore
              </p>
            </div>

            {/* Opening Hours */}
            <div className="border-b border-white/10 py-4 sm:pl-6 lg:border-r lg:border-b-0 lg:py-0 lg:pr-6">
              <p className="text-[10px] font-semibold uppercase tracking-wider text-slate-400 sm:text-xs">
                Opening Hours
              </p>

              <p className="mt-1.5 text-sm leading-5 text-white">
                Mon – Fri · 8:00am – 6:00pm
                <br />
                Sat · 8:00am – 2:00pm
              </p>
            </div>

            {/* Experience */}
            <div className="border-b border-white/10 py-4 lg:border-r lg:border-b-0 lg:px-6 lg:py-0">
              <p className="text-[10px] font-semibold uppercase tracking-wider text-slate-400 sm:text-xs">
                Experience
              </p>

              <p className="mt-1.5 text-xl font-bold text-white">
                10+
                <span className="ml-2 text-sm font-normal text-slate-400">
                  Years
                </span>
              </p>
            </div>

            {/* Rating */}
            <div className="pt-4 sm:pl-6 lg:pt-0">
              <p className="text-[10px] font-semibold uppercase tracking-wider text-slate-400 sm:text-xs">
                Customer Rating
              </p>

              <div className="mt-1.5 flex items-center gap-2">
                <span className="text-xl font-bold text-white">4.9</span>
                <span className="text-sm text-slate-400">/ 5</span>
              </div>
            </div>

          </div>
        </div>
      </div>
    </section>
  );
}