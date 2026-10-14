import { ChatMessage, ChatResponse } from "@/types/aiAgent.js";

const AI_AGENT_URL =
  process.env.NEXT_PUBLIC_AI_AGENT_URL ?? "http://127.0.0.1:8000";

export async function sendChatMessage(
  messages: ChatMessage[],
  token: string,
): Promise<string> {
  const response = await fetch(`${AI_AGENT_URL}/chat`, {
    method: "POST",
    headers: {
      "Content-Type": "application/json",
      Authorization: `Bearer ${token}`,
    },
    body: JSON.stringify({ messages }),
  });

  if (!response.ok) {
    const error = await response.json().catch(() => null);

    throw new Error(
      error?.detail ?? "Unable to contact the PrimeKare AI assistant.",
    );
  }

  const data: ChatResponse = await response.json();
  return data.reply;
}
