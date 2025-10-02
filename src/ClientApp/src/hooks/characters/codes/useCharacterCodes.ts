import { useQuery } from "@tanstack/react-query"
import { apiFetch } from "@/lib/apiFetch"

export type CharacterCode = {
  id: string
  name: string
  sourceCode: string | null
}

export function useCharacterCodes(characterId: string) {
  return useQuery<CharacterCode[], Error>({
    queryKey: ['characterCodes', characterId],
    queryFn: () => apiFetch(`/characters/${characterId}/codes`)
  })
}
