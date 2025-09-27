import { useMutation, useQueryClient } from '@tanstack/react-query'
import { apiFetch } from '@/lib/apiFetch'

export interface CreateCharacterRequest {
  name: string
}

export function useCreateCharacter() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (data: CreateCharacterRequest): Promise<string> => {
      if (!data.name?.trim())
        throw new Error("Имя персонажа обязательно")

      // сервер возвращает просто строку id
      return apiFetch<string>('/characters', {
        method: 'POST',
        body: JSON.stringify(data),
      })
    },
    onSuccess: () => {
      queryClient.invalidateQueries({ queryKey: ['characters'] })
    },
  })
}
