import { useMutation, useQueryClient } from "@tanstack/react-query"
import { type ApiException, apiFetch } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys"

interface RenameCharacterRequest {
  id: string
  name: string
}

export function useRenameCharacter() {
  const queryClient = useQueryClient()

  return useMutation<unknown, ApiException, RenameCharacterRequest>({
    mutationFn: async ({ id, name }) => {
      const trimmedName = name.trim()

      if (!trimmedName) {
        throw new Error("Имя персонажа обязательно") as ApiException
      }

      return apiFetch<unknown>(`/characters/${id}/name`, {
        method: "PATCH",
        body: JSON.stringify({ name: trimmedName }),
      })
    },
    onSuccess: (_, variables) => {
      void queryClient.invalidateQueries({
        queryKey: queryKeys.characters.byId(variables.id),
      })

      void queryClient.invalidateQueries({
        queryKey: queryKeys.characters.byCurrentUser,
      })
    },
  })
}
