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

    switch (status) {
      case 401:
        useAuthStore.getState().logout();
        if (
          typeof window !== "undefined" &&
          window.location.pathname !== "/sign-in"
        ) {
          window.location.href = "/sign-in";
        }
        return Promise.reject(new ApiError("Please log in again.", 401));

      case 403:
        return Promise.reject(
          new ApiError(
            "You don't have permission to perform this action.",
            403,
          ),
        );

      case 404:
        return Promise.reject(
          new ApiError("The requested Resource was not found.", 404),
        );

      case 500:
        return Promise.reject(
          new ApiError(
            "Something went wrong on our server. Please try again later.",
            500,
          ),
        );

      default:
        return Promise.reject(error);
    }
  },
);
