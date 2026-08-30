export interface Service {
    id: number;
    name: string;
    slug: string;
    description: string;
    price: number;
    estimatedMinutes: number;
    isActive: boolean;
    imageUrl: string | null;
    createdAt: string;
    updatedAt: string;
}

const API_URL = process.env.NEXT_PUBLIC_API_URL;

export async function getServices(): Promise<Service[]> {
    const response = await fetch(`${API_URL}/api/services`);

    if (!response.ok) {
        throw new Error("Failed to fetch services");
    }

    return response.json();
}