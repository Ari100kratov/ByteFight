import { useMutation } from "@tanstack/react-query"
import { apiFetch } from "@/shared/lib/apiFetch"

export interface ChangePasswordRequest {
  currentPassword: string
  newPassword: string
}

export function useChangePassword() {
  return useMutation({
    mutationFn: async (data: ChangePasswordRequest) => {
      return apiFetch("/users/me/password", {
        method: "PUT",
        body: JSON.stringify(data),
      })
    },
  })
}