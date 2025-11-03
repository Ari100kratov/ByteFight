import { ApiException, apiFetch } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys";
import type { StatDto } from "@/shared/types/stat";
import type { ActionAssetDto } from "@/shared/types/action";
import type { CharacterClassType } from "@/features/character-class-selector/useCharacterClasses";
import { useStoreQuery } from "@/shared/hooks/useStoreQuery";
import { useCharacterStore } from "@/features/game/state/data/character.data.store";

export type CharacterResponse = {
  id: string
  name: string
  class: ClassResponse
}

export type ClassResponse = {
  id: string
  name: string
  type: CharacterClassType
  description?: string
  stats: StatDto[]
  actionAssets: ActionAssetDto[]
}

export function useCharacterDetails(characterId?: string) {
  const setCharacter = useCharacterStore(s => s.setCharacter)
  return useStoreQuery<CharacterResponse, ApiException>(
    {
      queryKey: queryKeys.characters.details(characterId),
      queryFn: () => apiFetch<CharacterResponse>(`/characters/${characterId}/details`),
      enabled: !!characterId
    },
    setCharacter
  )
}
