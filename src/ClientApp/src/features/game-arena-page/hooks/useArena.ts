import { ApiException, apiFetch } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys"
import { useStoreQuery } from "@/shared/hooks/useStoreQuery"
import { useArenaStore } from "@/features/game/state/data/arena.data.store"
import type { Position } from "@/features/game/types/common"

export type ArenaResponse = {
  id: string
  name: string
  description?: string
  gridWidth: number
  gridHeight: number
  backgroundAsset?: string,
  startPosition: Position
  blockedPositions: Position[]
}

export function useArena(arenaId: string | undefined) {
  const { setArena } = useArenaStore()

  return useStoreQuery<ArenaResponse, ApiException>(
    {
      queryKey: queryKeys.arenas.byId(arenaId),
      queryFn: () => apiFetch(`/arenas/${arenaId}`),
      enabled: !!arenaId,
    },
    setArena
  )
}
