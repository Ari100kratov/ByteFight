import { useQuery } from "@tanstack/react-query"
import { ApiException, apiFetch } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys"

export type CharacterCode = {
  id: string
  name: string
  sourceCode: string | null
}

export function useCharacterCodes(characterId: string) {
  return useQuery<CharacterCode[], ApiException>({
    queryKey: queryKeys.characterCodes.byCharacterId(characterId),
    queryFn: () => apiFetch(`/characters/${characterId}/codes`)
  })
}
