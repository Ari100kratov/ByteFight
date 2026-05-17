import { useMutation, useQueryClient } from "@tanstack/react-query"
import { apiFetch } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys"

export interface UpdateProfileRequest {
  email: string
  firstName: string
  lastName: string
}

export function useUpdateProfile() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (data: UpdateProfileRequest) => {
      return apiFetch("/users/me/profile", {
        method: "PUT",
        body: JSON.stringify(data),
      })
    },
    onSuccess: () => {
      void queryClient.invalidateQueries({ queryKey: queryKeys.users.current })
    },
  })
}
