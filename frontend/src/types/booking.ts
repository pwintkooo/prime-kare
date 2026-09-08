export interface Booking {
  id: number;
  customerId: number;
  customerName: string;
  vehicleId: number;
  vehiclePlateNumber: string;
  vehicleMake: string;
  vehicleModel: string;
  serviceId: number;
  serviceName: string;
  bookingDate: string;
  bookingTime: string;
  status: string;
  isDeleted: boolean;
  notes: string | null;
  createdAt: string;
  updatedAt: string;
}

export interface CreateBookingRequest {
  vehicleId: number;
  serviceId: number;
  bookingDate: string;
  bookingTime: string;
  notes: string | null;
}

export interface UpdateBookingRequest {
  vehicleId: number;
  serviceId: number;
  bookingDate: string;
  bookingTime: string;
  notes: string | null;
}

export interface UpdateBookingStatusRequest {
  status: string;
}

export interface BookingAvailability {
  date: string;
  serviceId: number;
  availableTimes: string[];
}

export interface CreateBookingFormErrors {
  vehicleId?: string;
  serviceId?: string;
  bookingDate?: string;
  bookingTime?: string;
  notes?: string;
}
