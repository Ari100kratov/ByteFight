import { create } from "zustand"
import type { ArenaEnemyResponse } from "../../arena-enemies/useArenaEnemies"
import { ActionType } from "@/shared/types/action"

export type ArenaEnemy = ArenaEnemyResponse

type ArenaEnemiesState = {
  arenaEnemies: Record<string, ArenaEnemy>
  setArenaEnemies: (enemies: ArenaEnemyResponse[]) => void
  getArenaEnemy: (id: string) => ArenaEnemy | undefined
  reset: () => void
}

export const useArenaEnemiesStore = create<ArenaEnemiesState>((set, get) => ({
  arenaEnemies: {},
  setArenaEnemies: (arenaEnemies) =>
    set({
      arenaEnemies: Object.fromEntries(
        arenaEnemies.map((e) => [e.id, { ...e, currentAction: ActionType.Idle }])
      ),
    }),
  getArenaEnemy: (id) => get().arenaEnemies[id],
  reset: () => set({ arenaEnemies: {} }),
}))
