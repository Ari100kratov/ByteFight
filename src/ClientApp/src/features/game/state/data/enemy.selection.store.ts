import { create } from "zustand"

type EnemySelectionState = {
  selectedArenaEnemyId?: string
  position?: { x: number; y: number }
  selectEnemy: (arenaEnemyId: string, position: { x: number; y: number }) => void
  clearSelection: () => void
}

export const useEnemySelectionStore = create<EnemySelectionState>((set) => ({
  selectedArenaEnemyId: undefined,
  position: undefined,

  selectEnemy: (selectedArenaEnemyId, position) =>
    set({ selectedArenaEnemyId, position }),

  clearSelection: () =>
    set({ selectedArenaEnemyId: undefined, position: undefined }),
}))