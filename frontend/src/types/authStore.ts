import { User } from "@/types/auth";

export interface AuthState {
  user: User | null;
  token: string | null;
  hydrated: boolean;

  login: (user: User, token: string) => void;
  logout: () => void;
  updateToken: (token: string) => void;

  sessionExpired: boolean;
  sessionHasExpired: boolean;
  setSessionHasExpired: (expired: boolean) => void;

  showSessionExpired: () => void;
  clearSessionExpired: () => void;
}