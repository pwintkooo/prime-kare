"use client";

import { FormEvent, useState } from "react";
import ReactMarkdown from "react-markdown";
import remarkGfm from "remark-gfm";
import { useAuthStore } from "@/store/authStore";
import { sendChatMessage } from "@/api/aiAgent";
import { ChatMessage } from "@/types/aiAgent";

export default function PrimeKareChat() {
  const token = useAuthStore((state) => state.token);
  const hydrated = useAuthStore((state) => state.hydrated);

  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [input, setInput] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");

  async function handleSubmit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();

    const content = input.trim();

    if (!content || loading) return;

    if (!token) {
      setError("Please log in to use the PrimeKare assistant.");
      return;
    }

    const userMessage: ChatMessage = {
      role: "user",
      content,
    };

    const updatedMessages = [...messages, userMessage];

    setMessages(updatedMessages);
    setInput("");
    setError("");
    setLoading(true);

    try {
      const reply = await sendChatMessage(updatedMessages, token);

      setMessages([...updatedMessages, { role: "assistant", content: reply }]);
    } catch (err) {
      setError(
        err instanceof Error
          ? err.message
          : "Something went wrong. Please try again.",
      );
    } finally {
      setLoading(false);
    }
  }

  if (!hydrated) {
    return <p>Loading your session...</p>;
  }

  return (
    <section className="mx-auto flex w-full max-w-2xl flex-col gap-4">
      <h2 className="text-xl font-semibold">PrimeKare AI Assistant</h2>

      <div className="flex min-h-64 flex-col gap-3 rounded-xl border p-4">
        {messages.length === 0 && (
          <p className="text-sm text-gray-500">
            Ask about our services or your registered vehicles.
          </p>
        )}

        {messages.map((message, index) => (
          <div
            key={`${index}-${message.role}`}
            className={`max-w-[85%] rounded-xl p-3 ${
              message.role === "user"
                ? "self-end bg-blue-600 text-white"
                : "self-start bg-gray-100 text-gray-900"
            }`}
          >
            <p className="mb-2 text-xs font-semibold">
              {message.role === "user" ? "You" : "PrimeKare AI"}
            </p>

            <div className="min-w-0 overflow-x-auto text-sm leading-6">
              {message.role === "assistant" ? (
                <ReactMarkdown
                  remarkPlugins={[remarkGfm]}
                  components={{
                    table: ({ children }) => (
                      <table className="w-full border-collapse text-left">
                        {children}
                      </table>
                    ),
                    th: ({ children }) => (
                      <th className="border-b border-gray-300 px-3 py-2 font-semibold">
                        {children}
                      </th>
                    ),
                    td: ({ children }) => (
                      <td className="border-b border-gray-200 px-3 py-2">
                        {children}
                      </td>
                    ),
                    ul: ({ children }) => (
                      <ul className="my-2 list-disc space-y-1 pl-5">
                        {children}
                      </ul>
                    ),
                    ol: ({ children }) => (
                      <ol className="my-2 list-decimal space-y-1 pl-5">
                        {children}
                      </ol>
                    ),
                    p: ({ children }) => (
                      <p className="mb-2 last:mb-0">{children}</p>
                    ),
                  }}
                >
                  {message.content}
                </ReactMarkdown>
              ) : (
                <p className="whitespace-pre-wrap">{message.content}</p>
              )}
            </div>
          </div>
        ))}

        {loading && (
          <p className="text-sm text-gray-500">PrimeKare AI is thinking...</p>
        )}
      </div>

      {error && (
        <p role="alert" className="text-sm text-red-600">
          {error}
        </p>
      )}

      <form onSubmit={handleSubmit} className="flex gap-2">
        <input
          value={input}
          onChange={(event) => setInput(event.target.value)}
          placeholder="Ask PrimeKare AI..."
          maxLength={2000}
          disabled={loading || !token}
          className="min-w-0 flex-1 rounded-lg border px-4 py-3"
        />

        <button
          type="submit"
          disabled={loading || !input.trim() || !token}
          className="rounded-lg bg-blue-600 px-5 py-3 text-white disabled:opacity-50"
        >
          Send
        </button>
      </form>

      {!token && (
        <p className="text-sm text-gray-500">
          Log in to chat with the assistant.
        </p>
      )}
    </section>
  );
}
