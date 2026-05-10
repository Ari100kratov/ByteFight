import { useQuery } from "@tanstack/react-query"
import { ApiException, apiFetch } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys"

export type ArenaEnemySummaryResponse = {
  enemyId: string
  name: string
  count: number
}

export type ArenaItemSummaryResponse = {
  itemId: string
  name: string
  count: number
}

export type ArenaResponse = {
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
    queryFn: () => apiFetch(`/arenas?mode=${mode}`),
    enabled: !!mode
  })
}