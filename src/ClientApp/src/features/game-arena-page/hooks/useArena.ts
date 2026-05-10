import { ApiException, apiFetch } from "@/shared/lib/apiFetch"
import { queryKeys } from "@/shared/lib/queryKeys"
import { useStoreQuery } from "@/shared/hooks/useStoreQuery"
import { useArenaStore } from "@/features/game/state/data/arena.data.store"
import type { Position } from "@/features/game/types/common"
import type { ArenaItemType } from "@/shared/types/arenaItem"
import type { SpriteAnimationDto } from "@/shared/types/spriteAnimation"

export type ArenaItemResponse = {
  placedItemId: string
  itemId: string
  name: string
  description?: string | null
  type: ArenaItemType
  value: number
  position: Position
  sprite: SpriteAnimationDto
}

export type ArenaResponse = {
  id: string
  name: string
  description?: string | null
  gridWidth: number
  gridHeight: number
  backgroundAsset: string
  startPosition: Position
  blockedPositions: Position[]
  items: ArenaItemResponse[]
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
