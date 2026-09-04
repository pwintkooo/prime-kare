import { create } from "zustand";
import { persist } from "zustand/middleware";
import { User } from "@/lib/api/Auth";

interface AuthState {
  user: User | null;
  token: string | null;
  hydrated: boolean;

  login: (user: User, token: string) => void;
  logout: () => void;

  sessionExpired: boolean;

  showSessionExpired: () => void;
  clearSessionExpired: () => void;
}

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
