import { useQuery } from "@tanstack/react-query"
import { ApiException, apiFetch } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys";

export const CharacterClassType = {
  Warrior: 1,
  Mage: 2,
} as const;

export type CharacterClassType = (typeof CharacterClassType)[keyof typeof CharacterClassType];

export type CharacterClassResponse = {
  id: string
  name: string
  type: CharacterClassType,
  description?: string
}

export function useCharacterClasses() {
  return useQuery<CharacterClassResponse[], ApiException>({
    queryKey: queryKeys.characterClasses.all,
    queryFn: () => apiFetch<CharacterClassResponse[]>("/character-classes"),
  })
}
