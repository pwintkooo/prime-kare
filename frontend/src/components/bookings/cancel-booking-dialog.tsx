"use client";

import { useState } from "react";

import { ApiError } from "@/api/apiError";
import { useUpdateBookingStatus } from "@/hooks/use-bookings";

import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogDescription,
  DialogFooter,
  DialogHeader,
  DialogTitle,
} from "@/components/ui/dialog";

interface CancelBookingDialogProps {
  bookingId: number;
  open: boolean;
  onOpenChange: (open: boolean) => void;
}

export function CancelBookingDialog({
  bookingId,
  open,
  onOpenChange,
}: CancelBookingDialogProps) {
  const updateBookingStatus = useUpdateBookingStatus();

  const [error, setError] = useState("");

  const handleCancelBooking = async () => {
    setError("");

    try {
      await updateBookingStatus.mutateAsync({
        id: bookingId,
        request: {
          status: "cancelled",
        },
      });

      onOpenChange(false);
    } catch (error) {
      if (error instanceof ApiError) {
        setError(error.message);
        return;
      }

      setError("Unable to cancel booking.");
    }
  };

  const handleOpenChange = (open: boolean) => {
    onOpenChange(open);

    if (!open) {
      setError("");
    }
  };

  return (
    <Dialog open={open} onOpenChange={handleOpenChange}>
      <DialogContent>
        <DialogHeader>
          <DialogTitle>Cancel booking?</DialogTitle>

          <DialogDescription>
            Are you sure you want to cancel this appointment? This action cannot
            be undone.
          </DialogDescription>
        </DialogHeader>

        {error && <p className="text-sm text-destructive">{error}</p>}

        <DialogFooter>
          <Button
            type="button"
            variant="outline"
            disabled={updateBookingStatus.isPending}
            onClick={() => handleOpenChange(false)}
          >
            Keep Booking
          </Button>

          <Button
            type="button"
            variant="destructive"
            disabled={updateBookingStatus.isPending}
            onClick={handleCancelBooking}
          >
            {updateBookingStatus.isPending ? "Cancelling..." : "Cancel Booking"}
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
}