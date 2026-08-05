"use client";

import { useState } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";
import { Sheet, SheetContent, SheetTrigger } from "@/components/ui/sheet";

const navItems = [
  { label: "Home", href: "/" },
  { label: "Services", href: "/services" },
  { label: "About", href: "/about" },
  { label: "Contact", href: "/contact" },
];

export default function MobileMenu() {
  const [open, setOpen] = useState(false);
  const pathname = usePathname();

  return (
    <Sheet open={open} onOpenChange={setOpen}>
      <SheetTrigger className="group relative z-50 flex h-6 w-6 flex-col justify-center gap-1.25 lg:hidden">
        <span
          className={`h-[1.5px] w-full bg-neutral-900 transition-all duration-300 ${
            open ? "translate-y-[6.5px] rotate-45" : ""
          }`}
        />
        <span
          className={`h-[1.5px] w-full bg-neutral-900 transition-all duration-300 ${
            open ? "translate-y-[6.5px] -rotate-45" : ""
          }`}
        />
      </SheetTrigger>

      <SheetContent
        side="left"
        className="w-screen max-w-none border-none bg-white p-0 text-neutral-900 sm:max-w-none"
      >
        <div className="flex h-full flex-col justify-between px-8 py-12 sm:px-16 sm:py-16">
          <span className="text-lg uppercase tracking-[0.2em] text-neutral-400">
            Menu
          </span>

          <ul className="flex flex-col gap-2">
            {navItems.map((item, i) => {
              const isActive = pathname === item.href;
              return (
                <li
                  key={item.label}
                  className="animate-[fadeUp_0.5s_ease_forwards] opacity-0"
                  style={{ animationDelay: `${i * 70 + 100}ms` }}
                >
                  <Link
                    href={item.href}
                    onClick={() => setOpen(false)}
                    className="group flex items-baseline gap-4 border-b border-neutral-200 py-4"
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
          </ul>

          <div className="flex flex-col gap-1 text-sm text-neutral-400">
            <span>hello@yourcompany.com</span>
            <span>+65 0000 0000</span>
          </div>
        </div>
      </SheetContent>
    </Sheet>
  );
}