import { useQuery } from "@tanstack/react-query"
import { type ApiException, apiFetch } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys"

export interface CharacterCodeResponse {
  id: string
  name: string
  sourceCode: string | null
}

export function useCharacterCodes(characterId: string) {
  return useQuery<CharacterCodeResponse[], ApiException>({
    queryKey: queryKeys.characterCodes.byCharacterId(characterId),
    queryFn: () => apiFetch(`/characters/${characterId}/codes`),
  })
}
