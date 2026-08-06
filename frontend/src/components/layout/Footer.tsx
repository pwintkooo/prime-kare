import Link from "next/link";
import {
  Mail,
  MapPin,
  Phone,
} from "lucide-react";

const quickLinks = [
  { label: "Home", href: "/" },
  { label: "Services", href: "/services" },
  { label: "About", href: "/about" },
  { label: "Contact", href: "/contact" },
];

const services = [
  { label: "Oil Change", href: "/services/oil-change" },
  { label: "Brake Service", href: "/services/brake-service" },
  { label: "Engine Diagnostics", href: "/services/engine-diagnostics" },
  { label: "Tyre Service", href: "/services/tyre-service" },
];

export default function Footer() {
  return (
    <footer className="border-t bg-slate-950 text-slate-300">
      <div className="mx-auto max-w-7xl px-6 py-16">
        <div className="grid gap-12 md:grid-cols-2 lg:grid-cols-4">

          {/* Brand */}
          <div>
            <Link
              href="/"
              className="flex items-center gap-2"
            >
              <div className="flex h-10 w-10 items-center justify-center rounded-lg bg-blue-600 font-bold text-white">
                PK
              </div>

              <div>
                <p className="font-bold text-white">
                  Prime Kare
                </p>

                <p className="text-xs text-slate-400">
                  Workshop Management
                </p>
              </div>
            </Link>

            <p className="mt-5 max-w-xs text-sm leading-6 text-slate-400">
              Professional automotive care you can trust.
              Keeping your vehicle safe, reliable, and ready
              for every journey.
            </p>

            {/* <div className="mt-6 flex gap-3">
              <a
                href="#"
                className="rounded-md p-2 transition hover:bg-slate-800 hover:text-white"
                aria-label="Facebook"
              >
                <Facebook className="h-5 w-5" />
              </a>

              <a
                href="#"
                className="rounded-md p-2 transition hover:bg-slate-800 hover:text-white"
                aria-label="Instagram"
              >
                <Instagram className="h-5 w-5" />
              </a>
            </div> */}
          </div>

          {/* Quick Links */}
          <div>
            <h3 className="font-semibold text-white">
              Quick Links
            </h3>

            <ul className="mt-5 space-y-3">
              {quickLinks.map((link) => (
                <li key={link.label}>
                  <Link
                    href={link.href}
                    className="text-sm transition hover:text-white"
                  >
                    {link.label}
                  </Link>
                </li>
              ))}
            </ul>
          </div>

          {/* Services */}
          <div>
            <h3 className="font-semibold text-white">
              Our Services
            </h3>

            <ul className="mt-5 space-y-3">
              {services.map((service) => (
                <li key={service.label}>
                  <Link
                    href={service.href}
                    className="text-sm transition hover:text-white"
                  >
                    {service.label}
                  </Link>
                </li>
              ))}
            </ul>
          </div>

          {/* Contact */}
          <div>
            <h3 className="font-semibold text-white">
              Contact Us
            </h3>

            <ul className="mt-5 space-y-4">
              <li className="flex gap-3">
                <MapPin className="mt-0.5 h-5 w-5 shrink-0 text-blue-500" />

                <span className="text-sm">
                  123 Automotive Street
                  <br />
                  Singapore 123456
                </span>
              </li>

              <li className="flex items-center gap-3">
                <Phone className="h-5 w-5 text-blue-500" />

                <a
                  href="tel:+6561234567"
                  className="text-sm hover:text-white"
                >
                  +65 6123 4567
                </a>
              </li>

              <li className="flex items-center gap-3">
                <Mail className="h-5 w-5 text-blue-500" />

                <a
                  href="mailto:hello@autohub.com"
                  className="text-sm hover:text-white"
                >
                  hello@primekare.com
                </a>
              </li>
            </ul>
          </div>
        </div>

        {/* Bottom */}
        <div className="mt-12 border-t border-slate-800 pt-8">
          <div className="flex flex-col gap-4 text-sm text-slate-500 md:flex-row md:items-center md:justify-between">
            <p>
              © 2026 Prime Kare Workshop. All rights reserved.
            </p>

            <div className="flex gap-6">
              <Link
                href="/privacy"
                className="hover:text-white"
              >
                Privacy Policy
              </Link>

              <Link
                href="/terms"
                className="hover:text-white"
              >
                Terms & Conditions
              </Link>
            </div>
          </div>
        </div>
      </div>
    </footer>
  );
}