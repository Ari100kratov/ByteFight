import { useQuery } from "@tanstack/react-query"
import { apiFetch } from "@/lib/apiFetch"

export type Character = {
  id: string
  name: string
  userId: string
}

export function useCharacters() {
  return useQuery<Character[], Error>({
    queryKey: ['characters'],
    queryFn: () => apiFetch('/characters/by-current-user')
  })
}
