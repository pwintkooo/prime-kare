"use client";

import { useState } from "react";
import Link from "next/link";
import { useBookings } from "@/hooks/use-bookings";
import { Button } from "@/components/ui/button";
import { Plus, CalendarDays, ArrowUpDown, ArrowDownUp } from "lucide-react";
import { BookingCard } from "@/components/bookings/booking-card";
import { BookingsDashboardSkeleton } from "@/components/skeletons/BookingsDashboardSkeleton";

type BookingFilter = "all" | "upcoming" | "in-progress" | "completed" | "cancelled";

const filterLabels: Record<BookingFilter, string> = {
  all: "bookings",
  upcoming: "upcoming bookings",
  "in-progress": "in progress bookings",
  completed: "completed bookings",
  cancelled: "cancelled bookings",
};

export default function BookingsPage() {
  const { data: bookings, isLoading, isError } = useBookings();
  const [filter, setFilter] = useState<BookingFilter>("all");
  const [sortOrder, setSortOrder] = useState<"asc" | "desc">("asc");

  const filteredBookings = bookings?.filter((booking) => {
    if (filter === "all") {
      return true;
    }

    if (filter === "upcoming") {
      return booking.status === "pending" || booking.status === "confirmed";
    }

    if (filter === "in-progress") {
      return booking.status === "in-progress";
    }

    if (filter === "completed") {
      return booking.status === "completed";
    }

    if (filter === "cancelled") {
      return booking.status === "cancelled" || booking.status === "no-show";
    }

    return true;
  });

  const sortedBookings = filteredBookings
    ? [...filteredBookings].sort((a, b) => {
        const dateA = new Date(`${a.bookingDate}T${a.bookingTime}`).getTime();
        const dateB = new Date(`${b.bookingDate}T${b.bookingTime}`).getTime();

        return sortOrder === "asc" ? dateA - dateB : dateB - dateA;
      })
    : [];

  if (isLoading) {
    return <BookingsDashboardSkeleton />;
  }

  if (isError) {
    return (
      <div className="space-y-6">
        <div>
          <h1 className="text-2xl font-semibold">Bookings</h1>

          <p className="text-muted-foreground">
            Manage your vehicle service appointments.
          </p>
        </div>

        <p className="text-destructive">Failed to load bookings.</p>
      </div>
    );
  }

  return (
    <div className="mx-auto max-w-6xl space-y-6 px-4 py-6 sm:px-6 lg:px-8">
      {/* Header */}
      <div className="flex items-center justify-between">
        <div>
          <h1 className="text-2xl font-semibold">Bookings</h1>

          <p className="text-muted-foreground">
            Manage your vehicle service appointments.
          </p>
        </div>

        <Button>
          <Plus />
          <Link href={"/book-appointment"}>Book a Service</Link>
        </Button>
      </div>

      <div className="flex items-center justify-between gap-4 border-b">
        <div className="flex gap-2 overflow-x-auto">
          {[
            { value: "all", label: "All" },
            { value: "upcoming", label: "Upcoming" },
            { value: "in-progress", label: "In Progress" },
            { value: "completed", label: "Completed" },
            { value: "cancelled", label: "Cancelled" },
          ].map((item) => (
            <Button
              key={item.value}
              variant={filter === item.value ? "default" : "ghost"}
              onClick={() => setFilter(item.value as BookingFilter)}
              className="shrink-0"
            >
              {item.label}
            </Button>
          ))}
        </div>

        <Button
          variant="ghost"
          size="icon"
          onClick={() =>
            setSortOrder((current) => (current === "asc" ? "desc" : "asc"))
          }
          title={sortOrder === "asc" ? "Oldest first" : "Newest first"}
        >
          {sortOrder === "asc" ? (
            <ArrowUpDown className="size-4" />
          ) : (
            <ArrowDownUp className="size-4" />
          )}
        </Button>
      </div>

      {sortedBookings && sortedBookings.length > 0 ? (
        <div className="space-y-4">
          {sortedBookings.map((booking) => (
            <BookingCard key={booking.id} booking={booking} />
          ))}
        </div>
      ) : bookings && bookings.length > 0 ? (
        <div className="flex flex-col items-center justify-center rounded-xl border border-dashed py-16 text-center">
          <CalendarDays className="mb-4 size-10 text-muted-foreground" />

          <h2 className="text-lg font-semibold">No {filterLabels[filter]}</h2>

          <p className="mt-1 text-sm text-muted-foreground">
            Try selecting a different filter.
          </p>
        </div>
      ) : (
        <div className="flex flex-col items-center justify-center rounded-xl border border-dashed py-16 text-center">
          <CalendarDays className="mb-4 size-10 text-muted-foreground" />

          <h2 className="text-lg font-semibold">No bookings yet</h2>

          <p className="mt-1 max-w-sm text-sm text-muted-foreground">
            You don&apos;t have any service bookings yet. Book a service for
            your vehicle to get started.
          </p>

          <Button className="mt-6">
            <Plus />
            <Link href={"/book-appointment"}>Book a Service</Link>
          </Button>
        </div>
      )}
    </div>
  );
}
