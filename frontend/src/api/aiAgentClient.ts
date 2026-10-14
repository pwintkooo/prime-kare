import axios from "axios";
import { useAuthStore } from "@/store/authStore";

const AI_AGENT_URL = process.env.NEXT_PUBLIC_AI_AGENT_URL;

export const aiAgentClient = axios.create({
  baseURL: AI_AGENT_URL ?? "http://127.0.0.1:8000",
});

aiAgentClient.interceptors.request.use((config) => {
  const token = useAuthStore.getState().token;

  if (token) {
    config.headers.Authorization = `Bearer ${token}`;
  }

  return config;
});