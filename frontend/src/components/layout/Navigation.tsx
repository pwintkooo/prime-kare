"use client";

import { useRef, useState } from "react";
import Link from "next/link";
import { usePathname } from "next/navigation";

const navItems = [
  { label: "Home", href: "/" },
  { label: "Services", href: "/services" },
  { label: "About", href: "/about" },
  { label: "Contact", href: "/contact" },
];

export default function Navigation() {
  const pathname = usePathname();
  const containerRef = useRef<HTMLUListElement>(null);
  const [indicator, setIndicator] = useState<{ left: number; width: number } | null>(
    null
  );

  const moveIndicator = (el: HTMLElement | null) => {
    if (!el || !containerRef.current) return;
    const containerRect = containerRef.current.getBoundingClientRect();
    const rect = el.getBoundingClientRect();
    setIndicator({ left: rect.left - containerRect.left, width: rect.width });
  };

  return (
    <nav className="hidden lg:block">
      <ul
        ref={containerRef}
        onMouseLeave={() => setIndicator(null)}
        className="relative flex items-center gap-1"
      >
        {indicator && (
          <span
            className="absolute top-1/2 h-9 -translate-y-1/2 rounded-full bg-neutral-100 transition-all duration-300 ease-out"
            style={{ left: indicator.left, width: indicator.width }}
          />
        )}

        {navItems.map((item) => {
          const isActive = pathname === item.href;
          return (
            <li key={item.label} className="relative">
              <Link
                href={item.href}
                onMouseEnter={(e) => moveIndicator(e.currentTarget)}
                className={`relative z-10 block rounded-full px-4 py-2 text-sm transition-colors ${
                  isActive
                    ? "text-blue-600 font-medium"
                    : "text-netural-400 hover:text-blue-900 hover:bg-blue-100"
                }`}
              >
                {item.label}
              </Link>
              {isActive && (
                <span className="absolute -bottom-1 left-1/2 h-0.5 w-6 -translate-x-1/2 rounded-full bg-blue-600" />
              )}
            </li>
          );
        })}
      </ul>
    </nav>
  );
}