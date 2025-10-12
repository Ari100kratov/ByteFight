import { useQuery } from '@tanstack/react-query'
import { apiFetch } from '@/lib/apiFetch'
import { queryKeys } from '@/lib/queryKeys'

export type Character = {
  id: string
  name: string
  userId: string
}

export function useCharacter(id: string | undefined) {
  return useQuery<Character, Error>({
    queryKey: queryKeys.characters.byId(id),
    queryFn: () => apiFetch(`/characters/${id}`),
    enabled: !!id
  })
}
