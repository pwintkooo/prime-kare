"use client";

import Link from "next/link";
import { useQuery } from "@tanstack/react-query";
import { ChevronDown } from "lucide-react";

import { getServices } from "@/lib/api/Services";

interface ServicesMenuProps {
  isActive: boolean;
  onMouseEnter: (element: HTMLElement | null) => void;
}

export default function ServicesMenu({
  isActive,
  onMouseEnter,
}: ServicesMenuProps) {
  const { data: services = [], isLoading } = useQuery({
    queryKey: ["services"],
    queryFn: getServices,
  });

  return (
    <div className="group relative">
      {/* Services link */}
      <Link
        href="/services"
        onMouseEnter={(e) => onMouseEnter(e.currentTarget)}
        className={`relative z-10 flex items-center gap-1 rounded-full px-4 py-2 text-sm transition-colors ${
          isActive
            ? "font-medium text-blue-600"
            : "text-neutral-400 hover:bg-blue-100 hover:text-blue-900"
        }`}
      >
        Services

        <ChevronDown
          className="h-4 w-4 transition-transform duration-200 group-hover:rotate-180"
        />
      </Link>

      {/* Active indicator */}
      {isActive && (
        <span className="absolute -bottom-1 left-1/2 h-0.5 w-6 -translate-x-1/2 rounded-full bg-blue-600" />
      )}

      {/* Dropdown */}
      <div className="invisible absolute left-1/2 top-full z-50 w-64 -translate-x-1/2 pt-3 opacity-0 transition-all duration-200 group-hover:visible group-hover:opacity-100">
        <div className="rounded-xl border border-slate-200 bg-white p-2 shadow-xl">
          {isLoading ? (
            <div className="px-4 py-3 text-sm text-slate-500">
              Loading services...
            </div>
          ) : services.length === 0 ? (
            <div className="px-4 py-3 text-sm text-slate-500">
              No services available.
            </div>
          ) : (
            <>
              {services.map((service) => (
                <Link
                  key={service.id}
                  href={`/services/${service.slug}`}
                  className="block rounded-lg px-4 py-3 text-sm text-slate-700 transition-colors hover:bg-slate-100 hover:text-blue-600"
                >
                  {service.name}
                </Link>
              ))}

              <div className="my-2 border-t border-slate-200" />

              <Link
                href="/services"
                className="block rounded-lg px-4 py-3 text-sm font-medium text-blue-600 transition-colors hover:bg-blue-50"
              >
                View all services →
              </Link>
            </>
          )}
        </div>
      </div>
    </div>
  );
}