import { useQuery } from "@tanstack/react-query"
import { apiFetch } from "@/lib/apiFetch"
import { queryKeys } from "@/lib/queryKeys"

export type GameMode = {
  id: number
  slug: string
  name: string
  description: string
}

export function useGameModes() {
  return useQuery<GameMode[], Error>({
    queryKey: queryKeys.gameModes.all,
    queryFn: () => apiFetch("/game/modes"),
  })
}
