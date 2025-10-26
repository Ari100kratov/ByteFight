import { useMutation, useQueryClient } from '@tanstack/react-query'
import { apiFetch } from '@/shared/lib/apiFetch'
import { queryKeys } from '@/shared/lib/queryKeys'

export type CreateCharacterRequest = {
  name: string
  classId: string
}

export type CreateCharacterResponse = {
  id: string
}

export function useCreateCharacter() {
  const queryClient = useQueryClient()

  return useMutation({
    mutationFn: async (data: CreateCharacterRequest): Promise<string> => {
      if (!data.name?.trim())
        throw new Error('Имя персонажа обязательно')

      if (!data.classId)
        throw new Error('Не выбран класс персонажа')

      // теперь сервер возвращает { id: string }
      const response = await apiFetch<CreateCharacterResponse>('/characters', {
        method: 'POST',
        body: JSON.stringify(data),
      })

      return response.id
    },
    onSuccess: () => {
      queryClient.invalidateQueries({
        queryKey: queryKeys.characters.byCurrentUser,
      })
    },
  })
}
