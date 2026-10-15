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
      sessionHasExpired: false,

      setSessionHasExpired: (expired: boolean) =>
        set({ sessionHasExpired: expired }),

      login: (user, token) => {
        set({
          user,
          token,
          sessionExpired: false,
          sessionHasExpired: false,
        });
      },

      logout: () => {
        set({
          user: null,
          token: null,
          sessionExpired: false,
          sessionHasExpired: false,
        });
      },

      updateToken: (token) => {
        set({
          token,
          sessionExpired: false,
          sessionHasExpired: false,
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
          sessionHasExpired: false,
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
