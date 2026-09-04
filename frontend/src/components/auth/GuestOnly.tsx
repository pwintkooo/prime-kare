"use client";

import { ReactNode, useEffect } from "react";
import { useRouter } from "next/navigation";
import { useAuthStore } from "@/store/authStore";

interface GuestOnlyProps {
  children: ReactNode;
}

export default function GuestOnly({ children }: GuestOnlyProps) {
  const router = useRouter();

  const user = useAuthStore((state) => state.user);
  const hydrated = useAuthStore((state) => state.hydrated);

  useEffect(() => {
    if (!hydrated) {
      return;
    }

    if (user) {
      router.replace("/");
    }
  }, [hydrated, user, router]);

  if (!hydrated || user) {
    return null;
  }

  return <>{children}</>;
}