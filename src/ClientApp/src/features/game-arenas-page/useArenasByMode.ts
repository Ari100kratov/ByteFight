import { useQuery } from "@tanstack/react-query"
import { type ApiException, apiFetch } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys"

export interface ArenaEnemySummaryResponse {
  enemyId: string
  name: string
  count: number
}

export interface ArenaItemSummaryResponse {
  itemId: string
  name: string
  count: number
}

export interface ArenaResponse {
  id: string
  name: string
  imageUrl: string
  description?: string
  gridWidth: number
  gridHeight: number
  enemies: ArenaEnemySummaryResponse[]
  items: ArenaItemSummaryResponse[]
}

export function useArenasByMode(mode: string | undefined) {
  return useQuery<ArenaResponse[], ApiException>({
    queryKey: queryKeys.arenas.byMode(mode),
    queryFn: () => {
      if (!mode) {
        throw new Error("Game mode is required")
      }

      return apiFetch(`/arenas?mode=${mode}`)
    },
    enabled: !!mode,
  })
}
