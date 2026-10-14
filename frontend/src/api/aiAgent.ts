import { aiAgentClient } from "./aiAgentClient.js";
import { ChatMessage, ChatResponse } from "@/types/aiAgent.js";

export async function sendChatMessage(message: ChatMessage[]): Promise<string> {
  const response = await aiAgentClient.post<ChatResponse>("/chat", { message });

  return response.data.reply;
}
