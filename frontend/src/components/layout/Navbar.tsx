"use client";

import Link from "next/link";

import { Button } from "@/components/ui/button";

import Logo from "./Logo";
import Navigation from "./Navigation";
import MobileMenu from "./MobileMenu";
import { useAuthStore } from "@/store/authStore";
import UserMenu from "./UserMenu";

export default function Navbar() {
  const user = useAuthStore((state) => state.user);
  const hydrated = useAuthStore((state) => state.hydrated);

  return (
    <header className="sticky top-0 z-50 border-b bg-white/90 backdrop-blur">
      <div className="mx-auto flex h-20 max-w-7xl items-center justify-between px-6">
        <div className="flex items-center gap-4">
          <MobileMenu />
          <Logo />
        </div>

        <Navigation />

        <div className="hidden items-center gap-3 lg:flex">
          {!hydrated ? (
            <div className="h-10 w-24" />
          ) : user ? (
            <>
              <UserMenu />
              <Link href="/book-appointment">
                <Button>Book Appointment</Button>
              </Link>
            </>
          ) : (
            <>
              <Link href="/sign-in">
                <Button variant="outline">Login</Button>
              </Link>

              <Link href="/book">
                <Button>Book Appointment</Button>
              </Link>
            </>
          )}
        </div>
      </div>
    </header>
  );
}
