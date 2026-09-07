import axios from "axios";
import { useAuthStore } from "@/store/authStore";
import { ApiError } from "./apiError";

const API_URL = process.env.NEXT_PUBLIC_API_URL;

export const apiClient = axios.create({
  baseURL: API_URL,
  headers: {
    "Content-Type": "application/json",
  },
});

apiClient.interceptors.request.use((config) => {
  const token = useAuthStore.getState().token;

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});

apiClient.interceptors.response.use(
  (response) => response,

  (error) => {
    if (!axios.isAxiosError(error)) {
      return Promise.reject(new ApiError("Something went wrong.", 0));
    }

    const status = error.response?.status;

    if (status === 401) {
      useAuthStore.getState().showSessionExpired();

      return Promise.reject(new ApiError("Your session has expired.", 401));
    }

    const message = error.response?.data?.message ?? "Something went wrong.";

    return Promise.reject(
      new ApiError(message, status ?? 0, error.response?.data),
    );
  },
);