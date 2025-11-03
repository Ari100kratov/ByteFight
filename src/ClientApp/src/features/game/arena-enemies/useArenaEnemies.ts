import { ApiException, apiFetch } from "@/shared/lib/apiFetch"
import type { PositionDto } from "../types"
import { queryKeys } from "@/shared/lib/queryKeys"
import { useArenaStore } from "../state/data/arena.data.store"
import { useStoreQuery } from "@/shared/hooks/useStoreQuery"
import { useArenaEnemiesStore } from "../state/arena-enemies.store"

export type ArenaEnemyResponse = {
  id: string
  enemyId: string
  enemyName: string
  position: PositionDto
}

export function useArenaEnemies() {
  const arena = useArenaStore(s => s.arena)
  const setArenaEnemies = useArenaEnemiesStore(s => s.setArenaEnemies)

  return useStoreQuery<ArenaEnemyResponse[], ApiException>(
    {
      queryKey: queryKeys.arenaEnemies.byArenaId(arena?.id),
      queryFn: () => apiFetch(`/arenas/${arena?.id}/enemies`),
      enabled: !!arena?.id
    },
    setArenaEnemies
  )
}
