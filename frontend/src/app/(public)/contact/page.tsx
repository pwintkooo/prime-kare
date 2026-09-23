"use client";

import { useState } from "react";
import { zodResolver } from "@hookform/resolvers/zod";
import { useForm } from "react-hook-form";
import { Button } from "@/components/ui/button";
import {
  Mail,
  MapPin,
  Phone,
  Clock3,
  Loader2,
  CircleCheck,
} from "lucide-react";
import { useSendContactMessage } from "@/hooks/use-contact";
import {
  CreateContactFormData,
  createContactSchema,
} from "@/validations/contact";

const contactDetails = [
  {
    icon: Phone,
    title: "Phone",
    value: "+65 8535 7096",
    href: "tel:+6585357096",
  },
  {
    icon: Mail,
    title: "Email",
    value: "contact@primekare.pkoo.dev",
    href: "mailto:contact@primekare.pkoo.dev",
  },
  {
    icon: MapPin,
    title: "Workshop",
    value: "Singapore",
    href: "#",
  },
  {
    icon: Clock3,
    title: "Opening Hours",
    value: "Mon – Sat, 9:00 AM – 6:00 PM",
    href: null,
  },
];

export default function ContactPage() {
  const [isSuccess, setIsSuccess] = useState(false);

  const sendContactMessageMutation = useSendContactMessage();

  const {
    register,
    handleSubmit,
    reset,
    formState: { errors },
  } = useForm<CreateContactFormData>({
    resolver: zodResolver(createContactSchema),
    defaultValues: {
      name: "",
      email: "",
      phone: "",
      message: "",
    },
  });

  const onSubmit = async (data: CreateContactFormData) => {
    try {
      await sendContactMessageMutation.mutateAsync(data);
      reset();
      setIsSuccess(true);
    } catch {
      // Error is available from sendContactMessageMutation.error
    }
  };
  return (
    <main className="bg-white">
      {/* Hero */}
      <section className="bg-slate-950">
        <div className="mx-auto max-w-7xl px-6 py-24 sm:px-8 lg:py-28">
          <div className="max-w-3xl">
            <p className="text-sm font-semibold uppercase tracking-[0.2em] text-blue-400">
              Contact PrimeKare
            </p>

            <h1 className="mt-4 text-4xl font-bold tracking-tight text-white sm:text-5xl lg:text-6xl">
              We&apos;re here to
              <span className="text-slate-400"> help.</span>
            </h1>

            <p className="mt-6 max-w-2xl text-lg leading-8 text-slate-300">
              Have a question about our services, appointments, or your vehicle?
              Get in touch with our team.
            </p>
          </div>
        </div>
      </section>

      {/* Contact content */}
      <section>
        <div className="mx-auto grid max-w-7xl gap-12 px-6 py-20 sm:px-8 lg:grid-cols-[0.8fr_1.2fr] lg:py-28">
          {/* Details */}
          <div>
            <p className="text-sm font-semibold uppercase tracking-[0.2em] text-blue-600">
              Get in touch
            </p>

            <h2 className="mt-3 text-3xl font-bold tracking-tight text-slate-950">
              Let&apos;s talk about your vehicle.
            </h2>

            <p className="mt-5 leading-7 text-slate-600">
              Whether you&apos;re looking for a service, need help with an
              appointment, or simply have a question, feel free to reach out.
            </p>

            <div className="mt-10 space-y-6">
              {contactDetails.map((item) => {
                const Icon = item.icon;

                const content = (
                  <div className="flex gap-4">
                    <div className="flex h-11 w-11 shrink-0 items-center justify-center rounded-xl bg-blue-50 text-blue-600">
                      <Icon className="h-5 w-5" />
                    </div>

                    <div>
                      <p className="text-sm font-medium text-slate-500">
                        {item.title}
                      </p>

                      <p className="mt-1 text-sm font-semibold text-slate-900">
                        {item.value}
                      </p>
                    </div>
                  </div>
                );

                return item.href ? (
                  <a
                    key={item.title}
                    href={item.href}
                    className="block transition hover:opacity-75"
                  >
                    {content}
                  </a>
                ) : (
                  <div key={item.title}>{content}</div>
                );
              })}
            </div>
          </div>

          {/* Form */}
          <div className="rounded-2xl border border-slate-200 bg-slate-50 p-6 sm:p-8">
            <h2 className="text-xl font-semibold text-slate-950">
              Send us a message
            </h2>

            <p className="mt-2 text-sm text-slate-500">
              Fill out the form and we&apos;ll get back to you.
            </p>

            <form onSubmit={handleSubmit(onSubmit)} className="mt-8 space-y-5">
              {/* Name */}
              <div>
                <label
                  htmlFor="name"
                  className="mb-2 block text-sm font-medium text-slate-700"
                >
                  Name
                </label>

                <input
                  id="name"
                  type="text"
                  autoComplete="name"
                  placeholder="John Tan"
                  {...register("name")}
                  className={`w-full rounded-xl border bg-white px-4 py-3 text-sm outline-none transition placeholder:text-slate-400 focus:ring-2 ${
                    errors.name
                      ? "border-red-400 focus:border-red-400 focus:ring-red-400/20"
                      : "border-slate-200 focus:border-blue-500 focus:ring-blue-500/20"
                  }`}
                />

                {errors.name && (
                  <p className="text-sm text-red-400">{errors.name.message}</p>
                )}
              </div>

              {/* Email + Phone */}
              <div className="grid gap-5 sm:grid-cols-2">
                <div>
                  <label
                    htmlFor="email"
                    className="mb-2 block text-sm font-medium text-slate-700"
                  >
                    Email
                  </label>

                  <input
                    id="email"
                    type="email"
                    autoComplete="email"
                    placeholder="you@example.com"
                    {...register("email")}
                    className={`w-full rounded-xl border bg-white px-4 py-3 text-sm outline-none transition placeholder:text-slate-400 focus:ring-2 ${
                      errors.email
                        ? "border-red-400 focus:border-red-400 focus:ring-red-400/20"
                        : "border-slate-200 focus:border-blue-500 focus:ring-blue-500/20"
                    }`}
                  />

                  {errors.email && (
                    <p className="text-sm text-red-400">
                      {errors.email.message}
                    </p>
                  )}
                </div>

                <div>
                  <label
                    htmlFor="phone"
                    className="mb-2 block text-sm font-medium text-slate-700"
                  >
                    Phone
                  </label>

                  <input
                    id="phone"
                    type="tel"
                    autoComplete="phone"
                    placeholder="+65 9123 4567"
                    {...register("phone")}
                    className={`w-full rounded-xl border bg-white px-4 py-3 text-sm outline-none transition placeholder:text-slate-400 focus:ring-2 ${
                      errors.phone
                        ? "border-red-400 focus:border-red-400 focus:ring-red-400/20"
                        : "border-slate-200 focus:border-blue-500 focus:ring-blue-500/20"
                    }`}
                  />
                  {errors.phone && (
                    <p className="text-sm text-red-400">
                      {errors.phone.message}
                    </p>
                  )}
                </div>
              </div>

              {/* Message */}
              <div>
                <label
                  htmlFor="message"
                  className="mb-2 block text-sm font-medium text-slate-700"
                >
                  Message
                </label>

                <textarea
                  id="message"
                  autoComplete="message"
                  rows={6}
                  placeholder="How can we help?"
                  {...register("message")}
                  className={`w-full resize-none rounded-xl border bg-white px-4 py-3 text-sm outline-none transition placeholder:text-slate-400 focus:ring-2 ${
                    errors.message
                      ? "border-red-400 focus:border-red-400 focus:ring-red-400/20"
                      : "border-slate-200 focus:border-blue-500 focus:ring-blue-500/20"
                  }`}
                />

                {errors.message && (
                  <p className="text-sm text-red-400">
                    {errors.message.message}
                  </p>
                )}
              </div>

              {isSuccess && (
                <div className="flex items-start gap-3 rounded-xl border border-emerald-200 bg-emerald-50 px-4 py-3">
                  <CircleCheck className="mt-0.5 h-5 w-5 shrink-0 text-emerald-600" />

                  <div>
                    <p className="text-sm font-medium text-emerald-800">
                      Message sent successfully
                    </p>

                    <p className="mt-1 text-sm text-emerald-700">
                      Thanks for contacting PrimeKare. We&apos;ll get back to
                      you as soon as possible.
                    </p>
                  </div>
                </div>
              )}

              {sendContactMessageMutation.isError && (
                <div className="rounded-lg border border-red-500/20 bg-red-500/10 px-4 py-3">
                  <p className="text-sm text-red-400">
                    {sendContactMessageMutation.error.message}
                  </p>
                </div>
              )}

              <Button
                type="submit"
                disabled={sendContactMessageMutation.isPending}
                className="flex w-full items-center justify-center gap-2 rounded-xl bg-blue-600 px-4 py-3 text-sm font-semibold text-white transition hover:bg-blue-500 focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
              >
                {sendContactMessageMutation.isPending ? (
                  <>
                    <Loader2 className="mr-2 h-4 w-4 animate-spin" />
                    Sending Message...
                  </>
                ) : (
                  "Send Message"
                )}
              </Button>
            </form>
          </div>
        </div>
      </section>

      {/* Map placeholder */}
      {/* <section className="border-t">
        <div className="mx-auto max-w-7xl px-6 py-16 sm:px-8">
          <div className="flex h-72 items-center justify-center rounded-2xl bg-slate-100">
            <div className="text-center">
              <MapPin className="mx-auto h-8 w-8 text-blue-600" />

              <p className="mt-3 font-semibold text-slate-900">
                PrimeKare Workshop
              </p>

              <p className="mt-1 text-sm text-slate-500">Singapore</p>
            </div>
          </div>
        </div>
      </section> */}
    </main>
  );
}
