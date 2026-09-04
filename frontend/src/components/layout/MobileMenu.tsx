"use client";

import { useState } from "react";
import Link from "next/link";
import { usePathname, useRouter } from "next/navigation";
import { useQuery } from "@tanstack/react-query";
import {
  Car,
  CalendarDays,
  ChevronDown,
  LogIn,
  LogOut,
  User,
} from "lucide-react";

import { Sheet, SheetContent, SheetTrigger } from "@/components/ui/sheet";

import { useAuthStore } from "@/store/authStore";
import { getServices } from "@/lib/api/Services";

const navItems = [
  { label: "Home", href: "/" },
  { label: "About", href: "/about" },
  { label: "Contact", href: "/contact" },
];

export default function MobileMenu() {
  const [open, setOpen] = useState(false);
  const [servicesOpen, setServicesOpen] = useState(false);

  const pathname = usePathname();
  const router = useRouter();

  const user = useAuthStore((state) => state.user);
  const hydrated = useAuthStore((state) => state.hydrated);
  const logout = useAuthStore((state) => state.logout);

  const { data: services = [], isLoading } = useQuery({
    queryKey: ["services"],
    queryFn: getServices,
  });

  const handleLogout = () => {
    logout();
    setOpen(false);
    router.push("/sign-in");
  };

  const closeMenu = () => {
    setOpen(false);
  };

  return (
    <Sheet open={open} onOpenChange={setOpen}>
      <SheetTrigger className="group relative z-50 flex h-6 w-6 flex-col justify-center gap-1.25 lg:hidden">
        <span
          className={`h-[1.5px] w-full bg-neutral-900 transition-all duration-300 ${
            open ? "translate-y-[3.5px] rotate-45" : ""
          }`}
        />

        <span
          className={`h-[1.5px] w-full bg-neutral-900 transition-all duration-300 ${
            open ? "translate-y-[3.5px] -rotate-45" : ""
          }`}
        />
      </SheetTrigger>

      <SheetContent
        side="left"
        className="w-screen max-w-none border-none bg-white p-0 text-neutral-900 sm:max-w-none"
      >
        <div className="flex h-full flex-col px-8 py-12 sm:px-16 sm:py-16">
          {/* Header */}
          <span className="text-lg uppercase tracking-[0.2em] text-neutral-400">
            Menu
          </span>

          {/* Navigation */}
          <div className="mt-10 flex-1 overflow-y-auto">
            <ul className="flex flex-col">
              {/* Home / About / Contact */}
              {navItems.map((item, i) => {
                const isActive = pathname === item.href;

                return (
                  <li
                    key={item.label}
                    className="animate-[fadeUp_0.5s_ease_forwards] opacity-0"
                    style={{
                      animationDelay: `${i * 70 + 100}ms`,
                    }}
                  >
                    <Link
                      href={item.href}
                      onClick={closeMenu}
                      className="group flex items-baseline border-b border-neutral-200 py-4"
                    >
                      <span
                        className={`font-display text-lg font-light italic tracking-tight transition-transform duration-300 group-hover:translate-x-2 ${
                          isActive ? "text-blue-600" : "text-neutral-900"
                        }`}
                      >
                        {item.label}
                      </span>
                    </Link>
                  </li>
                );
              })}

              {/* Services */}
              <li className="border-b border-neutral-200">
                <button
                  type="button"
                  onClick={() => setServicesOpen((value) => !value)}
                  className="flex w-full items-center justify-between py-4"
                >
                  <span className="font-display text-lg font-light italic tracking-tight">
                    Services
                  </span>

                  <ChevronDown
                    className={`h-5 w-5 transition-transform duration-300 ${
                      servicesOpen ? "rotate-180" : ""
                    }`}
                  />
                </button>

                <div
                  className={`grid transition-all duration-300 ${
                    servicesOpen
                      ? "grid-rows-[1fr] pb-4 opacity-100"
                      : "grid-rows-[0fr] opacity-0"
                  }`}
                >
                  <div className="overflow-hidden">
                    <div className="flex flex-col gap-1 pl-4">
                      {isLoading ? (
                        <p className="py-2 text-sm text-neutral-400">
                          Loading services...
                        </p>
                      ) : (
                        services.map((service) => (
                          <Link
                            key={service.id}
                            href={`services/${service.slug}`}
                            onClick={closeMenu}
                            className="py-2 text-sm text-neutral-500 transition hover:text-neutral-900"
                          >
                            {service.name}
                          </Link>
                        ))
                      )}
                    </div>
                  </div>
                  <Link
                    href="/services"
                    onClick={closeMenu}
                    className="block rounded-lg px-4 py-3 text-sm font-medium text-blue-600 transition-colors hover:bg-blue-50"
                  >
                    View all services →
                  </Link>
                </div>
              </li>
            </ul>

            {/* Book Appointment */}
            <Link
              href="/book"
              onClick={closeMenu}
              className="mt-8 flex items-center justify-center rounded-xl bg-blue-600 px-5 py-4 text-sm font-semibold text-white transition hover:bg-blue-500"
            >
              Book Appointment
            </Link>

            {/* Account */}
            {hydrated && (
              <div className="mt-8">
                <p className="mb-3 text-xs font-medium uppercase tracking-wider text-neutral-400">
                  Account
                </p>

                {user ? (
                  <div className="flex flex-col">
                    <Link
                      href="/profile"
                      onClick={closeMenu}
                      className="flex items-center gap-3 border-b border-neutral-200 py-4 text-sm"
                    >
                      <User className="h-4 w-4" />
                      Profile
                    </Link>

                    <Link
                      href="/vehicles"
                      onClick={closeMenu}
                      className="flex items-center gap-3 border-b border-neutral-200 py-4 text-sm"
                    >
                      <Car className="h-4 w-4" />
                      My Vehicles
                    </Link>

                    <Link
                      href="/appointments"
                      onClick={closeMenu}
                      className="flex items-center gap-3 border-b border-neutral-200 py-4 text-sm"
                    >
                      <CalendarDays className="h-4 w-4" />
                      Appointments
                    </Link>

                    <button
                      type="button"
                      onClick={handleLogout}
                      className="flex items-center gap-3 py-4 text-sm text-red-600"
                    >
                      <LogOut className="h-4 w-4" />
                      Sign out
                    </button>
                  </div>
                ) : (
                  <div className="flex flex-col">
                    <Link
                      href="/sign-in"
                      onClick={closeMenu}
                      className="flex items-center gap-3 border-b border-neutral-200 py-4 text-sm"
                    >
                      <LogIn className="h-4 w-4" />
                      Login
                    </Link>

                    <Link
                      href="/sign-up"
                      onClick={closeMenu}
                      className="flex items-center gap-3 py-4 text-sm"
                    >
                      <User className="h-4 w-4" />
                      Sign Up
                    </Link>
                  </div>
                )}
              </div>
            )}
          </div>

          {/* Footer */}
          <div className="mt-8 flex flex-col gap-1 text-sm text-neutral-400">
            <span>hello@yourcompany.com</span>
            <span>+65 0000 0000</span>
          </div>
        </div>
      </SheetContent>
    </Sheet>
  );
}
