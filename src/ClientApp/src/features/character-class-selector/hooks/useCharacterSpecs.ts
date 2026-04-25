import { ApiException, apiFetch } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys"
import type { ActionAssetDto } from "@/shared/types/action"
import type { StatDto } from "@/shared/types/stat"
import { useQuery } from "@tanstack/react-query"

export const CharacterSpecType = {
  Berserker: 1,
  Guardian: 2,
  Duelist: 3,

  Pyromancer: 100,
  Luminary: 101,
  Arcanist: 102,
} as const

export type CharacterSpecType =
  (typeof CharacterSpecType)[keyof typeof CharacterSpecType]

export type CharacterSpecResponse = {
  id: string
  classId: string
  name: string
  type: CharacterSpecType
  description?: string
  stats: StatDto[]
  actionAssets: ActionAssetDto[]
}

export function useCharacterSpecsByClassId(classId?: string) {
  return useQuery<CharacterSpecResponse[], ApiException>({
    queryKey: queryKeys.characterSpecs.byClassId(classId),
    queryFn: () =>
      apiFetch<CharacterSpecResponse[]>(
        `/character-specs?classId=${encodeURIComponent(classId!)}`
      ),
    enabled: Boolean(classId),
  })
}