import { create } from "zustand";

interface AuthState {
  token: string | null;
  name: string | null;
  role: string | null;

  setAuth: (
    token: string,
    name: string,
    role: string
  ) => void;

  logout: () => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  token: null,
  name: null,
  role: null,

  setAuth: (token, name, role) => {
    set({
      token,
      name,
      role,
    });

    localStorage.setItem("token", token);
    localStorage.setItem("name", name);
    localStorage.setItem("role", role);
  },

  logout: () => {
    set({
      token: null,
      name: null,
      role: null,
    });

    localStorage.removeItem("token");
    localStorage.removeItem("name");
    localStorage.removeItem("role");
  },
}));