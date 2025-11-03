import { useQuery } from "@tanstack/react-query"
import { ApiException, apiFetch } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys"

export type GameModeResponse = {
  id: number
  slug: string
  name: string
  description: string
}

export function useGameModes() {
  return useQuery<GameModeResponse[], ApiException>({
    queryKey: queryKeys.gameModes.all,
    queryFn: () => apiFetch("/game/modes"),
  })
}
