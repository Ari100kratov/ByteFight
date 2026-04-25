import { useMutation, useQueryClient } from "@tanstack/react-query"
import { ApiException, apiFetch } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys"

type RenameCharacterRequest = {
  id: string
  name: string
}

export function useRenameCharacter() {
  const queryClient = useQueryClient()

  return useMutation<void, ApiException, RenameCharacterRequest>({
    mutationFn: async ({ id, name }) => {
      const trimmedName = name.trim()

      if (!trimmedName) {
        throw new Error("Имя персонажа обязательно") as ApiException
      }

      await apiFetch<void>(`/characters/${id}/name`, {
        method: "PATCH",
        body: JSON.stringify({ name: trimmedName }),
      })
    },
    onSuccess: (_, variables) => {
      queryClient.invalidateQueries({
        queryKey: queryKeys.characters.byId(variables.id),
      })

      queryClient.invalidateQueries({
        queryKey: queryKeys.characters.byCurrentUser,
      })
    },
  })
}