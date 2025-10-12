import { useQuery } from "@tanstack/react-query"
import { apiFetch } from "@/lib/apiFetch"
import { queryKeys } from "@/lib/queryKeys"

export type Character = {
  id: string
  name: string
  userId: string
}

export function useCharacters() {
  return useQuery<Character[], Error>({
    queryKey: queryKeys.characters.byCurrentUser,
    queryFn: () => apiFetch('/characters/by-current-user')
  })
}
