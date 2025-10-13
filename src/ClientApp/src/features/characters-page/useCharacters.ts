import { useQuery } from "@tanstack/react-query"
import { ApiException, apiFetch } from "@/lib/apiFetch"
import { queryKeys } from "@/lib/queryKeys"

export type Character = {
  id: string
  name: string
  userId: string
}

export function useCharacters() {
  return useQuery<Character[], ApiException>({
    queryKey: queryKeys.characters.byCurrentUser,
    queryFn: () => apiFetch('/characters/by-current-user')
  })
}
