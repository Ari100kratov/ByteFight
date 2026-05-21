import { useMutation, useQueryClient } from "@tanstack/react-query"
import { apiFetch } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys"

export interface CreateCharacterRequest {
  name: string
  specId: string
}

export interface CreateCharacterResponse {
  id: string
}

export function useCreateCharacter() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (data: CreateCharacterRequest): Promise<string> => {
      if (!data.name.trim()) throw new Error("Имя персонажа обязательно")

      if (!data.specId) throw new Error("Не выбран класс и специализация персонажа")

      const response = await apiFetch<CreateCharacterResponse>("/characters", {
        method: "POST",
        body: JSON.stringify(data),
      })

      return response.id
    },
    onSuccess: () => {
      void queryClient.invalidateQueries({
        queryKey: queryKeys.characters.byCurrentUser,
      })
    },
  })
}
