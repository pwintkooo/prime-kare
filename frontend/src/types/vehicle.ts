export interface Vehicle {
  id: number;
  plateNumber: string;
  make: string;
  model: string;
  year: number;
  status: string;
  isDeleted: boolean;
  createdAt: string;
  updatedAt: string;
  customerId: number;
}

export interface CreateVehicleRequest {
  plateNumber: string;
  make: string;
  model: string;
  year: number;
}

export interface UpdateVehicleRequest {
  plateNumber: string;
  make: string;
  model: string;
  year: number;
}