import Link from "next/link";
import { Button } from "@/components/ui/button";
import { CheckCircle2, Car, Clock3, ShieldCheck, Wrench } from "lucide-react";

const values = [
  {
    icon: ShieldCheck,
    title: "Trusted Service",
    description:
      "We focus on honest recommendations, quality workmanship, and transparent service.",
  },
  {
    icon: Wrench,
    title: "Professional Care",
    description:
      "Our services are designed to keep your vehicle reliable, safe, and road-ready.",
  },
  {
    icon: Clock3,
    title: "Convenient Booking",
    description:
      "Book your appointment online and manage your vehicle service history in one place.",
  },
  {
    icon: Car,
    title: "Vehicle First",
    description:
      "Every service is centered around keeping your vehicle performing at its best.",
  },
];

const benefits = [
  "Easy online appointment booking",
  "Centralised vehicle management",
  "Service history tracking",
  "Clear service information",
  "Convenient appointment management",
  "Professional automotive services",
];

export default function AboutPage() {
  return (
    <main className="bg-white">
      {/* Hero */}
      <section className="bg-slate-950">
        <div className="mx-auto max-w-7xl px-6 py-24 sm:px-8 lg:py-32">
          <div className="max-w-3xl">
            <p className="text-sm font-semibold uppercase tracking-[0.2em] text-blue-400">
              About PrimeKare
            </p>

            <h1 className="mt-4 text-4xl font-bold tracking-tight text-white sm:text-5xl lg:text-6xl">
              Better care for
              <span className="text-slate-400"> every vehicle.</span>
            </h1>

            <p className="mt-6 max-w-2xl text-lg leading-8 text-slate-300">
              PrimeKare makes vehicle maintenance simpler by bringing automotive
              services, appointment booking, and vehicle management together in
              one place.
            </p>
          </div>
        </div>
      </section>

      {/* Introduction */}
      <section className="border-b bg-white">
        <div className="mx-auto grid max-w-7xl gap-12 px-6 py-20 sm:px-8 lg:grid-cols-2 lg:items-center lg:py-28">
          <div>
            <p className="text-sm font-semibold uppercase tracking-[0.2em] text-blue-600">
              Who we are
            </p>

            <h2 className="mt-3 text-3xl font-bold tracking-tight text-slate-950 sm:text-4xl">
              Making vehicle care simple.
            </h2>

            <p className="mt-6 leading-7 text-slate-600">
              Taking care of a vehicle should not be complicated. PrimeKare
              provides a convenient way for customers to discover automotive
              services, book appointments, and keep track of their vehicles.
            </p>

            <p className="mt-4 leading-7 text-slate-600">
              Whether you need routine maintenance or a specific service,
              PrimeKare helps you stay organised and make informed decisions
              about your vehicle.
            </p>
          </div>

          <div className="rounded-2xl bg-slate-950 p-8 sm:p-10">
            <p className="text-sm font-semibold uppercase tracking-[0.2em] text-blue-400">
              Our mission
            </p>

            <p className="mt-4 text-2xl font-semibold leading-9 text-white">
              &quot;To make vehicle maintenance more convenient, transparent,
              and accessible for every driver.&quot;
            </p>
          </div>
        </div>
      </section>

      {/* Values */}
      <section className="bg-slate-50">
        <div className="mx-auto max-w-7xl px-6 py-20 sm:px-8 lg:py-28">
          <div className="max-w-2xl">
            <p className="text-sm font-semibold uppercase tracking-[0.2em] text-blue-600">
              Why PrimeKare
            </p>

            <h2 className="mt-3 text-3xl font-bold tracking-tight text-slate-950 sm:text-4xl">
              Built around your vehicle.
            </h2>

            <p className="mt-4 leading-7 text-slate-600">
              We combine professional automotive services with a simple digital
              experience.
            </p>
          </div>

          <div className="mt-12 grid gap-6 sm:grid-cols-2 lg:grid-cols-4">
            {values.map((value) => {
              const Icon = value.icon;

              return (
                <div
                  key={value.title}
                  className="rounded-2xl border border-slate-200 bg-white p-6"
                >
                  <div className="flex h-11 w-11 items-center justify-center rounded-xl bg-blue-50 text-blue-600">
                    <Icon className="h-5 w-5" />
                  </div>

                  <h3 className="mt-5 font-semibold text-slate-950">
                    {value.title}
                  </h3>

                  <p className="mt-2 text-sm leading-6 text-slate-600">
                    {value.description}
                  </p>
                </div>
              );
            })}
          </div>
        </div>
      </section>

      {/* Benefits */}
      <section>
        <div className="mx-auto grid max-w-7xl gap-12 px-6 py-20 sm:px-8 lg:grid-cols-2 lg:items-center lg:py-28">
          <div>
            <p className="text-sm font-semibold uppercase tracking-[0.2em] text-blue-600">
              Everything in one place
            </p>

            <h2 className="mt-3 text-3xl font-bold tracking-tight text-slate-950 sm:text-4xl">
              A simpler way to manage your vehicle.
            </h2>

            <p className="mt-5 leading-7 text-slate-600">
              From finding the right service to managing your appointments,
              PrimeKare keeps everything organised.
            </p>
          </div>

          <div className="grid gap-4 sm:grid-cols-2">
            {benefits.map((benefit) => (
              <div
                key={benefit}
                className="flex items-center gap-3 rounded-xl border border-slate-200 p-4"
              >
                <CheckCircle2 className="h-5 w-5 shrink-0 text-blue-600" />

                <span className="text-sm font-medium text-slate-700">
                  {benefit}
                </span>
              </div>
            ))}
          </div>
        </div>
      </section>

      {/* CTA */}
      <section className="bg-slate-950">
        <div className="mx-auto max-w-7xl px-6 py-20 text-center sm:px-8">
          <h2 className="text-3xl font-bold tracking-tight text-white">
            Ready to take better care of your vehicle?
          </h2>

          <p className="mx-auto mt-4 max-w-xl text-slate-400">
            Explore our services and book your next appointment with PrimeKare.
          </p>

          <div className="mt-8 flex flex-col justify-center gap-3 sm:flex-row">
            <Button size="lg">
              <Link href="/services">Explore Services</Link>
            </Button>

            <Button variant="outline" size="lg">
              <Link href="/contact">Contact Us</Link>
            </Button>
          </div>
        </div>
      </section>
    </main>
  );
}
