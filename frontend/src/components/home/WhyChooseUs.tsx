import Link from "next/link";
import { ArrowUpRight } from "lucide-react";

const reasons = [
  {
    number: "01",
    title: "Honest Service",
    description:
      "Clear recommendations, transparent pricing, and no unnecessary repairs.",
  },
  {
    number: "02",
    title: "Experienced Technicians",
    description:
      "Skilled technicians who understand the details that keep your vehicle running properly.",
  },
  {
    number: "03",
    title: "Quality Work",
    description:
      "We use quality parts, proper tools, and careful workmanship on every vehicle.",
  },
];

export default function WhyChooseUs() {
  return (
    <section className="bg-slate-950 py-24 sm:py-32">
      <div className="mx-auto max-w-7xl px-6 lg:px-8">

        {/* Header */}
        <div className="grid gap-10 lg:grid-cols-[1fr_1.2fr] lg:gap-20">
          
          {/* Left */}
          <div>
            <p className="text-sm font-semibold uppercase tracking-[0.2em] text-slate-500">
              Why Choose Us
            </p>

            <h2 className="mt-5 text-4xl font-bold uppercase leading-[0.95] tracking-tight text-white sm:text-5xl lg:text-6xl">
              We take care
              <br />
              of your car.
            </h2>
          </div>

          {/* Right */}
          <div className="flex items-end">
            <p className="max-w-xl text-base leading-7 text-slate-400 sm:text-lg">
              Car servicing should be straightforward, honest, and
              stress-free. We focus on doing the job properly while
              giving you clear answers about what your vehicle needs.
            </p>
          </div>
        </div>

        {/* Reasons */}
        <div className="mt-20 border-t border-white/10">
          {reasons.map((reason) => (
            <div
              key={reason.number}
              className="group grid gap-6 border-b border-white/10 py-8 transition-colors duration-300 hover:bg-white/2 sm:grid-cols-[80px_1fr_auto] sm:items-center sm:gap-8"
            >
              {/* Number */}
              <span className="text-sm font-medium text-slate-600 transition-colors duration-300 group-hover:text-slate-400">
                {reason.number}
              </span>

              {/* Content */}
              <div className="grid gap-3 md:grid-cols-[280px_1fr] md:items-center">
                <h3 className="text-xl font-semibold text-white sm:text-2xl">
                  {reason.title}
                </h3>

                <p className="max-w-lg text-sm leading-6 text-slate-400">
                  {reason.description}
                </p>
              </div>

              {/* Arrow */}
              <div className="hidden h-11 w-11 items-center justify-center rounded-full border border-white/10 text-slate-500 transition-all duration-300 group-hover:-translate-y-1 group-hover:translate-x-1 group-hover:border-white/30 group-hover:text-white sm:flex">
                <ArrowUpRight className="h-5 w-5" />
              </div>
            </div>
          ))}
        </div>

        {/* Bottom CTA */}
        <div className="mt-10 flex justify-end">
          <Link
            href="/about"
            className="group inline-flex items-center gap-3 text-sm font-semibold text-white"
          >
            Learn more about us

            <span className="flex h-8 w-8 items-center justify-center rounded-full border border-white/20 transition-all duration-300 group-hover:-translate-y-1 group-hover:translate-x-1 group-hover:border-white/50">
              <ArrowUpRight className="h-4 w-4" />
            </span>
          </Link>
        </div>

      </div>
    </section>
  );
}