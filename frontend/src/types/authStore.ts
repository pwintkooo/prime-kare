import { User } from "@/types/auth";

export interface AuthState {
  user: User | null;
  token: string | null;
  hydrated: boolean;

  login: (user: User, token: string) => void;
  logout: () => void;

  sessionExpired: boolean;

  showSessionExpired: () => void;
  clearSessionExpired: () => void;
}