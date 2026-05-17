import { create } from "zustand"
import type { ArenaItemResponse } from "@/features/game-arena-page/hooks/useArena"

type InitPayload = {
  arenaId: string
  items: ArenaItemResponse[]
}

type ArenaItemsState = {
  arenaId?: string
  items: ArenaItemResponse[]
  removedPlacedItemIds: Set<string>
  init: (payload: InitPayload) => void
  remove: (placedItemId: string) => void
  reset: () => void
}

export const useArenaItemsStateStore = create<ArenaItemsState>((set, get) => ({
  arenaId: undefined,
  items: [],
  removedPlacedItemIds: new Set(),

  init: ({ arenaId, items }) => {
    const state = get()

    if (state.arenaId === arenaId) {
      return
    }

    set({
      arenaId,
      items,
      removedPlacedItemIds: new Set(),
    })
  },

  remove: (placedItemId) => {
    set((state) => {
      const removedPlacedItemIds = new Set(state.removedPlacedItemIds)
      removedPlacedItemIds.add(placedItemId)

      return {
        removedPlacedItemIds,
        items: state.items.filter((x) => x.placedItemId !== placedItemId),
      }
    })
  },

  reset: () => {
    set({
      arenaId: undefined,
      items: [],
      removedPlacedItemIds: new Set(),
    })
  },
}))
