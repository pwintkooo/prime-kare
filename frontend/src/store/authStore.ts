import { create } from "zustand";
import { persist } from "zustand/middleware";
import { AuthState } from "@/types/authStore";

export const useAuthStore = create<AuthState>()(
  persist(
    (set) => ({
      user: null,
      token: null,
      hydrated: false,

      sessionExpired: false,

      login: (user, token) => {
        set({
          user,
          token,
          sessionExpired: false,
        });
      },

      logout: () => {
        set({
          user: null,
          token: null,
        });
      },
      showSessionExpired: () => {
        set({
          sessionExpired: true,
        });
      },

      clearSessionExpired: () => {
        set({
          sessionExpired: false,
        });
      },
    }),

    {
      name: "primekare-auth",
      skipHydration: true,
      onRehydrateStorage: () => {
        return () => {
          useAuthStore.setState({
            hydrated: true,
          });
        };
      },
    },
  ),
);
