import Link from "next/link";

import { Button } from "@/components/ui/button";

import Logo from "./Logo";
import Navigation from "./Navigation";
import MobileMenu from "./MobileMenu";

export default function Navbar() {
  return (
    <header className="sticky top-0 z-50 border-b bg-white/90 backdrop-blur">
      <div className="mx-auto flex h-20 max-w-7xl items-center justify-between px-6">
        <div className="flex items-center gap-4">
          <MobileMenu />
          <Logo />
        </div>

        <Navigation />

        <div className="hidden lg:flex items-center gap-3">
          <Link href="/sign-in">
            <Button variant="outline">Login</Button>
          </Link>

          <Link href="/book">
            <Button>Book Appointment</Button>
          </Link>
        </div>
      </div>
    </header>
  );
}
