import type { ArenaResponse } from "@/features/game-arena-page/hooks/useArena"
import { create } from "zustand"

export type Arena = ArenaResponse

type ArenaState = {
  arena?: Arena
  setArena: (arena: ArenaResponse) => void
  reset: () => void
}

export const useArenaStore = create<ArenaState>((set) => ({
  arena: undefined,
  setArena: (arenaResponse) => {
    const arena: Arena = {
      ...arenaResponse,
    }
    set({ arena: arena })
  },
  reset: () => set({ arena: undefined }),
}))
