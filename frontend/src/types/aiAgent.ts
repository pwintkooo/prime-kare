export interface ChatMessage {
  role: "user" | "assistance";
  content: string;
}

export interface ChatResponse {
  reply: string;
}
