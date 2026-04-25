import { ApiException, apiFetch } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys";
import type { StatDto } from "@/shared/types/stat";
import type { ActionAssetDto } from "@/shared/types/action";
import type { CharacterSpecType } from "@/features/character-class-selector/hooks/useCharacterSpecs";
import { useStoreQuery } from "@/shared/hooks/useStoreQuery";
import { useCharacterStore } from "@/features/game/state/data/character.data.store";

export type CharacterResponse = {
  id: string
  name: string
  spec: SpecResponse
}

export type SpecResponse = {
  id: string
  name: string
  className: string
  type: CharacterSpecType
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
