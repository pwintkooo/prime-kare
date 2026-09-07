export interface Service {
  id: number;
  name: string;
  slug: string;
  description: string;
  price: number;
  estimatedMinutes: number;
  isActive: boolean;
  isDeleted: boolean;
  imageUrl: string | null;
  createdAt: string;
  updatedAt: string;
}