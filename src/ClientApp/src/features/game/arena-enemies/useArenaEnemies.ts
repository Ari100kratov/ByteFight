import { useQuery } from "@tanstack/react-query"
import { ApiException, apiFetch } from "@/lib/apiFetch"
import type { PositionDto } from "../types"
import { queryKeys } from "@/lib/queryKeys"

export type ArenaEnemyResponse = {
  id: string
  enemyId: string
  enemyName: string
  position: PositionDto
}

export function useArenaEnemies(arenaId?: string) {
  return useQuery<ArenaEnemyResponse[], ApiException>({
    queryKey: queryKeys.arenaEnemies.byArenaId(arenaId),
    queryFn: () => apiFetch(`/arenas/${arenaId}/enemies`),
    enabled: !!arenaId
  })
}
