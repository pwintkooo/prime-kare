import { apiClient } from "./client";
import { ContactRequest } from "@/types/contact";

export async function sendContactMessage(request: ContactRequest) {
    const response = await apiClient.post("/api/contact", request);

    return response.data;
}
