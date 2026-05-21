import type { ArenaResponse } from "@/features/game-arena-page/hooks/useArena"
import { create } from "zustand"
import { useArenaItemsStateStore } from "../game/arena-items.state.store"

export type Arena = ArenaResponse

interface ArenaState {
  arena?: Arena
  setArena: (arena: ArenaResponse) => void
  reset: () => void
}

export const useArenaStore = create<ArenaState>((set) => ({
  arena: undefined,
  setArena: (arenaResponse) => {
    useArenaItemsStateStore.getState().init({
      arenaId: arenaResponse.id,
      items: arenaResponse.items,
    })

    set({ arena: arenaResponse })
  },
  reset: () => {
    set({ arena: undefined })
  },
}))
