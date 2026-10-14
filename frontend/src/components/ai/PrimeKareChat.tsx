"use client";

import { FormEvent, useEffect, useRef, useState } from "react";
import ReactMarkdown from "react-markdown";
import remarkGfm from "remark-gfm";
import Image from "next/image";
import { useAuthStore } from "@/store/authStore";
import { sendChatMessage } from "@/api/aiAgent";
import type { ChatMessage } from "@/types/aiAgent";

const STORAGE_KEY = "primekare-ai-chat-history";

export default function PrimeKareChat() {
  const token = useAuthStore((state) => state.token);
  const hydrated = useAuthStore((state) => state.hydrated);

  const [messages, setMessages] = useState<ChatMessage[]>([]);
  const [historyReady, setHistoryReady] = useState(false);
  const [isOpen, setIsOpen] = useState(false);
  const [input, setInput] = useState("");
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState("");
  const historyInitialized = useRef(false);

  const messagesEndRef = useRef<HTMLDivElement>(null);

  // Load chat history once when the component mounts.
  useEffect(() => {
    const savedMessages = localStorage.getItem(STORAGE_KEY);

    if (savedMessages) {
      try {
        const parsed: unknown = JSON.parse(savedMessages);

        if (
          Array.isArray(parsed) &&
          parsed.every(
            (message) =>
              message &&
              (message.role === "user" || message.role === "assistant") &&
              typeof message.content === "string",
          )
        ) {
          // Queue the update after the current effect completes.
          queueMicrotask(() => {
            setMessages(parsed as ChatMessage[]);
            setHistoryReady(true);
          });

          historyInitialized.current = true;
          return;
        }
      } catch {
        // Ignore invalid saved history.
      }
    }

    queueMicrotask(() => {
      setHistoryReady(true);
    });

    historyInitialized.current = true;
  }, []);

  // Save messages after history has finished loading.
  useEffect(() => {
    if (!historyReady) return;

    try {
      localStorage.setItem(STORAGE_KEY, JSON.stringify(messages));
    } catch (error) {
      console.error("Unable to save chat history:", error);
    }
  }, [messages, historyReady]);

  // Scroll to the latest message when the conversation changes.
  useEffect(() => {
    messagesEndRef.current?.scrollIntoView({ behavior: "smooth" });
  }, [messages, loading, isOpen]);

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
    } catch (error: unknown) {
      const errorMessage =
        error instanceof Error
          ? error.message
          : "Something went wrong. Please try again.";

      setMessages((previous) => [
        ...previous,
        {
          role: "assistant",
          content: errorMessage,
        },
      ]);
    } finally {
      setLoading(false);
    }
  }

  function clearHistory() {
    setMessages([]);
    setError("");

    try {
      localStorage.removeItem(STORAGE_KEY);
    } catch {
      setError("Unable to clear saved chat history.");
    }
  }

  return (
    <>
      {/* Floating chat bubble */}
      {!isOpen && (
        <button
          type="button"
          onClick={() => setIsOpen(true)}
          aria-label="Open PrimeKare AI chat"
          className="fixed bottom-6 right-6 z-50 flex h-14 w-14 items-center justify-center rounded-full bg-blue-600 text-white shadow-xl transition hover:bg-blue-700 focus:outline-none focus:ring-4 focus:ring-blue-300"
        >
          <svg
            xmlns="http://www.w3.org/2000/svg"
            viewBox="0 0 24 24"
            fill="none"
            stroke="currentColor"
            strokeWidth="1.8"
            className="h-7 w-7"
            aria-hidden="true"
          >
            <path
              strokeLinecap="round"
              strokeLinejoin="round"
              d="M8 10h8M8 14h5m-8 6 1.8-4.1A8 8 0 1 1 20 16.5L21 20l-4.2-1.1A8 8 0 0 1 5 16.2"
            />
          </svg>
        </button>
      )}

      {/* Chat window */}
      {isOpen && (
        <section
          aria-label="PrimeKare AI chat"
          className="fixed bottom-4 right-4 z-50 flex h-[min(650px,calc(100dvh-2rem))] w-[min(400px,calc(100vw-2rem))] flex-col overflow-hidden rounded-2xl border border-slate-200 bg-white shadow-2xl sm:bottom-6 sm:right-6"
        >
          {/* Header */}
          <header className="flex items-center justify-between bg-slate-900 px-4 py-3 text-white">
            <div className="flex items-center gap-3">
              <div className="relative h-10 w-10 shrink-0 overflow-hidden rounded-full bg-slate-800">
                <Image
                  src="/images/ai/chatbot-avatar.jpg"
                  alt="PrimeKare AI assistant"
                  fill
                  sizes="40px"
                  className="object-cover"
                  priority
                />
              </div>

              <div>
                <h2 className="font-semibold">PrimeKare AI</h2>
                <p className="text-xs text-slate-300">
                  Your car workshop assistant
                </p>
              </div>
            </div>

            <div className="flex items-center gap-1">
              <button
                type="button"
                onClick={clearHistory}
                title="Clear chat history"
                aria-label="Clear chat history"
                className="rounded-lg px-3 py-2 text-sm text-slate-200 hover:bg-slate-700"
              >
                Clear
              </button>

              <button
                type="button"
                onClick={() => setIsOpen(false)}
                aria-label="Minimize chat"
                className="rounded-lg px-3 py-2 text-xl leading-none hover:bg-slate-700"
              >
                −
              </button>
            </div>
          </header>

          {/* Messages */}
          <div className="flex min-h-0 flex-1 flex-col gap-3 overflow-y-auto bg-slate-50 p-4">
            {!historyReady ? (
              <p className="text-sm text-slate-500">Loading chat history...</p>
            ) : messages.length === 0 ? (
              <div className="my-auto text-center">
                <div className="mx-auto mb-3 flex h-14 w-14 items-center justify-center rounded-full bg-blue-100 text-2xl">
                  👋
                </div>
                <h3 className="font-semibold text-slate-800">
                  Hi! Welcome to PrimeKare
                </h3>
                <p className="mt-2 text-sm text-slate-500">
                  Ask me about our services or your registered vehicles.
                </p>
              </div>
            ) : (
              messages.map((message, index) => (
                <div
                  key={`${index}-${message.role}`}
                  className={`max-w-[90%] rounded-xl p-3 ${
                    message.role === "user"
                      ? "self-end bg-blue-600 text-white"
                      : "self-start border border-slate-200 bg-white text-slate-900"
                  }`}
                >
                  <p className="mb-1 text-xs font-semibold opacity-75">
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
                            <th className="border-b border-slate-300 px-2 py-2 font-semibold">
                              {children}
                            </th>
                          ),
                          td: ({ children }) => (
                            <td className="border-b border-slate-200 px-2 py-2">
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
              ))
            )}

            {loading && (
              <p className="self-start rounded-xl bg-white p-3 text-sm text-slate-500">
                PrimeKare AI is thinking...
              </p>
            )}

            <div ref={messagesEndRef} />
          </div>

          {/* Error */}
          {error && (
            <p
              role="alert"
              className="border-t border-red-100 bg-red-50 px-4 py-2 text-sm text-red-600"
            >
              {error}
            </p>
          )}

          {/* Input */}
          <form
            onSubmit={handleSubmit}
            className="flex gap-2 border-t border-slate-200 bg-white p-3"
          >
            <input
              value={input}
              onChange={(event) => setInput(event.target.value)}
              placeholder={
                !hydrated
                  ? "Loading session..."
                  : !token
                    ? "Please log in to chat"
                    : "Ask PrimeKare AI..."
              }
              maxLength={2000}
              disabled={!hydrated || !token || loading || !historyReady}
              className="min-w-0 flex-1 rounded-full border border-slate-300 px-4 py-3 text-sm outline-none focus:border-blue-500 focus:ring-2 focus:ring-blue-100 disabled:bg-slate-100"
            />

            <button
              type="submit"
              disabled={
                !hydrated || !token || loading || !historyReady || !input.trim()
              }
              className="rounded-full bg-blue-600 px-4 py-3 text-sm font-semibold text-white hover:bg-blue-700 disabled:cursor-not-allowed disabled:opacity-50"
            >
              Send
            </button>
          </form>
        </section>
      )}
    </>
  );
}
