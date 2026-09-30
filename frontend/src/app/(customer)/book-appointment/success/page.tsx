"use client";

import { Suspense, useState } from "react";
import Link from "next/link";
import { useSearchParams } from "next/navigation";
import { CheckCircle2, Copy, Home, Loader2, Download } from "lucide-react";
import { toast } from "sonner";

import { Button } from "@/components/ui/button";
import { Card, CardContent } from "@/components/ui/card";

import { downloadGuestBookingConfirmation } from "@/api/bookings";

function BookingSuccessContent() {
  const searchParams = useSearchParams();

  const referenceNumber = searchParams.get("ref");
  const token = searchParams.get("token");

  const [isDownloading, setIsDownloading] = useState(false);

  const handleCopy = async () => {
    if (!referenceNumber) return;

    await navigator.clipboard.writeText(referenceNumber);

    toast.success("Booking reference copied.");
  };

  const handleDownload = async () => {
    if (!token || !referenceNumber) {
      toast.error("Unable to download booking PDF.");
      return;
    }

    try {
      setIsDownloading(true);

      const blob = await downloadGuestBookingConfirmation(token);

      const url = window.URL.createObjectURL(blob);

      const link = document.createElement("a");

      link.href = url;
      link.download = `PrimeKare-${referenceNumber}.pdf`;

      document.body.appendChild(link);

      link.click();
      link.remove();

      window.URL.revokeObjectURL(url);
    } catch {
      toast.error("Failed to download booking PDF.");
    } finally {
      setIsDownloading(false);
    }
  };

  return (
    <div className="container mx-auto flex min-h-[70vh] max-w-2xl items-center justify-center px-4 py-10">
      <Card className="w-full">
        <CardContent className="flex flex-col items-center px-6 py-10 text-center sm:px-10">
          <div className="mb-5 flex size-16 items-center justify-center rounded-full bg-green-100">
            <CheckCircle2 className="size-8 text-green-600" />
          </div>

          <h1 className="text-2xl font-semibold sm:text-3xl">
            Appointment Requested
          </h1>

          <p className="mt-3 max-w-md text-muted-foreground">
            Your appointment request has been received successfully. We&apos;ll
            contact you using the details you provided once your appointment has
            been confirmed.
          </p>

          {referenceNumber && (
            <div className="mt-8 w-full max-w-sm rounded-lg border bg-muted/40 p-5">
              <p className="text-sm text-muted-foreground">Booking Reference</p>

              <div className="mt-2 flex items-center justify-center gap-2">
                <p className="font-mono text-lg font-semibold tracking-wide sm:text-xl">
                  {referenceNumber}
                </p>

                <Button
                  type="button"
                  variant="ghost"
                  size="icon"
                  onClick={handleCopy}
                  aria-label="Copy booking reference"
                >
                  <Copy className="size-4" />
                </Button>
              </div>

              <p className="mt-2 text-xs text-muted-foreground">
                Keep this reference number for future enquiries about your
                appointment.
              </p>
            </div>
          )}

          <div className="mt-8 rounded-lg bg-muted/50 px-5 py-4 text-sm text-muted-foreground">
            Please note that your appointment is currently pending. Our team
            will confirm the appointment after reviewing your request.
          </div>

          {token && referenceNumber && (
            <Button
              type="button"
              variant="outline"
              className="mt-6 gap-2"
              onClick={handleDownload}
              disabled={isDownloading}
            >
              {isDownloading ? (
                <Loader2 className="size-4 animate-spin" />
              ) : (
                <Download className="size-4" />
              )}

              {isDownloading ? "Downloading..." : "Download Booking PDF"}
            </Button>
          )}

          <Link href="/" className="mt-8 inline-block">
            <Button
              size="lg"
              className="min-w-44 gap-2 shadow-sm transition-all hover:-translate-y-0.5 hover:shadow-md"
            >
              <Home className="size-4" />
              Back to Home
            </Button>
          </Link>
        </CardContent>
      </Card>
    </div>
  );
}

export default function BookingSuccessPage() {
  return (
    <Suspense
      fallback={
        <div className="flex min-h-[70vh] items-center justify-center">
          <Loader2 className="size-6 animate-spin text-muted-foreground" />
        </div>
      }
    >
      <BookingSuccessContent />
    </Suspense>
  );
}
