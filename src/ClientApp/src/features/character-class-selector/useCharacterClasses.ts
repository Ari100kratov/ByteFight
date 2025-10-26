import { useQuery } from "@tanstack/react-query"
import { ApiException, apiFetch } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys";
import type { StatDto } from "@/shared/types/stat";
import type { ActionAssetDto } from "@/shared/types/action";

export type CharacterClassResponse = {
  id: string
  name: string
  description?: string
  stats: StatDto[]
  actionAssets: ActionAssetDto[]
}

export function useCharacterClasses() {
  return useQuery<CharacterClassResponse[], ApiException>({
    queryKey: queryKeys.characterClasses.all,
    queryFn: () => apiFetch<CharacterClassResponse[]>("/character-classes"),
  })
}
