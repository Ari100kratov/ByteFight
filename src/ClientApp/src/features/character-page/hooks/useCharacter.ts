import { useQuery } from "@tanstack/react-query"
import { type ApiException, apiFetch } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys"

export interface CharacterResponse {
  id: string
  name: string
  classId: string
  specId: string
}

export function useCharacter(id: string | undefined) {
  return useQuery<CharacterResponse, ApiException>({
    queryKey: queryKeys.characters.byId(id),
    queryFn: () => apiFetch(`/characters/${id}`),
    enabled: !!id,
  })
}
