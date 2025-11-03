import { useQuery } from '@tanstack/react-query'
import { ApiException, apiFetch } from '@/shared/lib/apiFetch'
import { queryKeys } from '@/shared/lib/queryKeys'

export type CharacterResponse = {
  id: string
  name: string
  classId: string
  userId: string
}

export function useCharacter(id: string | undefined) {
  return useQuery<CharacterResponse, ApiException>({
    queryKey: queryKeys.characters.byId(id),
    queryFn: () => apiFetch(`/characters/${id}`),
    enabled: !!id
  })
}
