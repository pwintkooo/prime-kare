export function formatBookingDate(date: string): string {
  const [year, month, day] = date.split("-").map(Number);

  const bookingDate = new Date(year, month - 1, day);

  return bookingDate.toLocaleDateString("en-SG", {
    day: "numeric",
    month: "short",
    year: "numeric",
  });
}

export function formatBookingTime(time: string): string {
  const [hours, minutes] = time.split(":").map(Number);

  const bookingTime = new Date();

  bookingTime.setHours(hours, minutes, 0, 0);

  return bookingTime.toLocaleTimeString("en-SG", {
    hour: "numeric",
    minute: "2-digit",
    hour12: true,
  });
}

export function formatTimeValue(time: string): string {
  const [hour, minute] = time.split(":");

  return `${hour.padStart(2, "0")}:${minute}`;
}
