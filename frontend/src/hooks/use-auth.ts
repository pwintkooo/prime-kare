import { useMutation } from "@tanstack/react-query";
import { useRouter } from "next/navigation";

import { SignIn, SignUp, forgotPassword, resetPassword } from "@/api/auth";
import { ApiError } from "@/api/apiError";
import { useAuthStore } from "@/store/authStore";

import type {
  SignInRequest,
  SignUpRequest,
  ForgotPasswordRequest,
  ResetPasswordRequest,
} from "@/types/auth";

export function useSignIn() {
  const login = useAuthStore((state) => state.login);
  const router = useRouter();

  return useMutation({
    mutationFn: (data: SignInRequest) => SignIn(data),

    onSuccess: (response) => {
      login(response.user, response.token);
      router.push("/");
    },
  });
}

export function useSignUp() {
  const router = useRouter();

  return useMutation({
    mutationFn: (data: SignUpRequest) => SignUp(data),

    onSuccess: () => {
      router.push("/sign-in");
    },
  });
}

export function useForgotPassword() {
  return useMutation({
    mutationFn: (data: ForgotPasswordRequest) => forgotPassword(data),
  });
}

export function useResetPassword() {
  return useMutation({
    mutationFn: (data: ResetPasswordRequest) => resetPassword(data),
  });
}

export function isAccountInactiveError(error: unknown) {
  if (!(error instanceof ApiError)) {
    return false;
  }

  const data = error.data as { code?: string } | undefined;

  return data?.code === "ACCOUNT_INACTIVE";
}
