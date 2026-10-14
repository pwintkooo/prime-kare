import axios from "axios";
import { ChatMessage, ChatResponse } from "@/types/aiAgent.js";

const AI_AGENT_URL =
  process.env.NEXT_PUBLIC_AI_AGENT_URL ?? "http://127.0.0.1:8000";

export async function sendChatMessage(
  messages: ChatMessage[],
  token: string,
): Promise<string> {
  try {
    const response = await axios.post<ChatResponse>(
      `${AI_AGENT_URL}/chat`,
      { messages: messages.slice(-10) },
      {
        headers: {
          "Content-Type": "application/json",
          Authorization: `Bearer ${token}`,
        },
      },
    );

    return response.data.reply;
  } catch (error: unknown) {
    if (axios.isAxiosError(error)) {
      // Rate limit exceeded
      if (error.response?.status === 429) {
        throw new Error(
          "You've sent too many messages. Please wait a minute before trying again.",
        );
      }

      // Extract FastAPI error details
      const detail = error.response?.data?.detail;

      if (typeof detail === "string") {
        throw new Error(detail);
      }
    }

    throw new Error(
      "Unable to contact the PrimeKare AI assistant. Please try again.",
    );
  }
}