import { useQuery } from "@tanstack/react-query"
import { apiFetch } from "@/lib/apiFetch"
import { queryKeys } from "@/lib/queryKeys"

export type CharacterCode = {
  id: string
  name: string
  sourceCode: string | null
}

export function useCharacterCodes(characterId: string) {
  return useQuery<CharacterCode[], Error>({
    queryKey: queryKeys.characterCodes.byCharacterId(characterId),
    queryFn: () => apiFetch(`/characters/${characterId}/codes`)
  })
}
