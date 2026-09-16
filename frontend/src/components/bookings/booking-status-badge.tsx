import { Badge } from "@/components/ui/badge";
import { Booking } from "@/types/booking";

interface BookingStatusBadgeProps {
  status: Booking["status"];
}

const statusConfig: Record<
  Booking["status"],
  {
    label: string;
    variant: "default" | "secondary" | "destructive";
  }
> = {
  pending: {
    label: "Pending",
    variant: "secondary",
  },
  confirmed: {
    label: "Confirmed",
    variant: "default",
  },
  "in-progress": {
    label: "In Progress",
    variant: "secondary",
  },
  completed: {
    label: "Completed",
    variant: "default",
  },
  cancelled: {
    label: "Cancelled",
    variant: "destructive",
  },
  "no-show": {
    label: "No Show",
    variant: "destructive",
  },
};

export function BookingStatusBadge({ status }: BookingStatusBadgeProps) {
  const config = statusConfig[status];

  return <Badge variant={config.variant}>{config.label}</Badge>;
}