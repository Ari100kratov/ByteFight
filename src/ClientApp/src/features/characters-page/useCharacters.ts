import { useQuery } from "@tanstack/react-query"
import { ApiException, apiFetch } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys"

export type CharacterResponse = {
  id: string
  name: string
  className: string
  specName: string
}

export function useCharacters() {
  return useQuery<CharacterResponse[], ApiException>({
    queryKey: queryKeys.characters.byCurrentUser,
    queryFn: () => apiFetch('/characters/by-current-user')
  })
}
