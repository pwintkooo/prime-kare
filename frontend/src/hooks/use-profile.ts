import { useMutation, useQuery, useQueryClient } from "@tanstack/react-query";

import {
  getProfile,
  updateProfile,
  changeEmail,
  changePassword,
  deleteAccount,
} from "@/api/profile";

export function useProfile() {
  return useQuery({
    queryKey: ["profile"],
    queryFn: getProfile,
  });
}

export function useUpdateProfile() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: updateProfile,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["profile"],
      });
    },
  });
}

export function useChangeEmail() {
  const queryClient = useQueryClient();

  return useMutation({
    mutationFn: changeEmail,
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: ["profile"],
      });
    },
  });
}

export function useChangePassword() {
  return useMutation({
    mutationFn: changePassword,
  });
}

export function useDeleteAccount() {
  return useMutation({
    mutationFn: deleteAccount,
  });
}