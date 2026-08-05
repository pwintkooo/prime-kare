import Link from "next/link";

import Logo from "./Logo";

const footerLinks = [
  {
    title: "Company",
    links: [
      { label: "Home", href: "/" },
      { label: "Services", href: "/services" },
      { label: "About", href: "/about" },
    ],
  },
  {
    title: "Support",
    links: [
      { label: "Contact", href: "/contact" },
      { label: "Book Appointment", href: "/book" },
      { label: "Login", href: "/login" },
    ],
  },
];

export default function Footer() {
  return (
    <footer className="border-t bg-white">
      <div className="mx-auto max-w-7xl px-6 py-16">
        <div className="flex flex-col gap-12 md:flex-row md:justify-between">
          <div className="max-w-xs">
            <Logo />
            <p className="mt-4 text-sm leading-relaxed text-neutral-500">
              A short, honest line about what you do and who it's for.
            </p>
          </div>

          <div className="grid grid-cols-2 gap-10 sm:gap-16">
            {footerLinks.map((group) => (
              <div key={group.title}>
                <h3 className="text-sm font-medium text-neutral-900">
                  {group.title}
                </h3>
                <ul className="mt-4 flex flex-col gap-3">
                  {group.links.map((link) => (
                    <li key={link.label}>
                      <Link
                        href={link.href}
                        className="text-sm text-neutral-500 transition-colors hover:text-neutral-900"
                      >
                        {link.label}
                      </Link>
                    </li>
                  ))}
                </ul>
              </div>
            ))}
          </div>
        </div>

        <div className="mt-16 flex flex-col-reverse items-center justify-between gap-4 border-t pt-8 sm:flex-row">
          <p className="text-sm text-neutral-500">
            © {new Date().getFullYear()} Your Company. All rights reserved.
          </p>

          <div className="flex items-center gap-6">
            <Link
              href="/privacy"
              className="text-sm text-neutral-500 hover:text-neutral-900"
            >
              Privacy
            </Link>
            <Link
              href="/terms"
              className="text-sm text-neutral-500 hover:text-neutral-900"
            >
              Terms
            </Link>
          </div>
        </div>
      </div>
    </footer>
  );
}