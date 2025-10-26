import { useQuery } from "@tanstack/react-query"
import { ApiException, apiFetch } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys"

export type GameMode = {
  id: number
  slug: string
  name: string
  description: string
}

export function useGameModes() {
  return useQuery<GameMode[], ApiException>({
    queryKey: queryKeys.gameModes.all,
    queryFn: () => apiFetch("/game/modes"),
  })
}
