import { useMutation } from "@tanstack/react-query";
import { sendContactMessage } from "@/api/contact";
import { ContactRequest } from "@/types/contact";

export function useSendContactMessage() {
  return useMutation({
    mutationFn: (data: ContactRequest) => sendContactMessage(data),
  });
}