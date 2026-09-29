import type { Metadata } from "next";
import { Geist, Geist_Mono } from "next/font/google";
import { TooltipProvider } from "@/components/ui/tooltip";
import { Toaster } from "sonner";
import QueryProvider from "@/providers/QueryProvider";
import AuthHydration from "@/providers/AuthHydration";
import SessionExpiredDialog from "@/components/auth/SessionExpiredDialog";
import "./globals.css";

const geistSans = Geist({
  variable: "--font-geist-sans",
  subsets: ["latin"],
});

const geistMono = Geist_Mono({
  variable: "--font-geist-mono",
  subsets: ["latin"],
});

export const metadata: Metadata = {
  title: "Primekare",
  description:
    "PrimeKare - Car service booking and vehicle maintenance platform.",
  icons: {
    icon: "/images/branding/primekare-icon.png",
  },
};

export default function RootLayout({
  children,
}: Readonly<{
  children: React.ReactNode;
}>) {
  return (
    <html
      lang="en"
      className={`${geistSans.variable} ${geistMono.variable} h-full antialiased`}
    >
      <body className="min-h-full flex flex-col">
        <TooltipProvider>
          <QueryProvider>
            <AuthHydration />
            {children}
            <Toaster richColors position="top-right" />
            <SessionExpiredDialog />
          </QueryProvider>
        </TooltipProvider>
      </body>
    </html>
  );
}
