import { create } from "zustand"
import { ActionType } from "@/shared/types/action"
import type { PositionDto } from "../../types"

type EnemyRuntime = {
  arenaEnemyid: string
  currentAction: ActionType
  position: PositionDto
}

type InitPayload = {
  arenaEnemyId: string
  position: PositionDto
}

type EnemyRuntimeStore = {
  arenaEnemies: Record<string, EnemyRuntime>
  init: (arenaEnemies: InitPayload[]) => void
  set: (arenaEnemyId: string, partial: Partial<EnemyRuntime>) => void
  get: (arenaEnemyId?: string) => EnemyRuntime | undefined
  reset: () => void
}

export const useEnemyStateStore = create<EnemyRuntimeStore>((set, get) => ({
  arenaEnemies: {},

  init: (arenaEnemies) => {
    const current = get().arenaEnemies
    const updated = { ...current }

    for (const { arenaEnemyId, position } of arenaEnemies) {
      if (updated[arenaEnemyId]) continue
      updated[arenaEnemyId] = {
        arenaEnemyid: arenaEnemyId,
        currentAction: ActionType.Idle,
        position,
      }
    }

    set({ arenaEnemies: updated })
  },

  set: (arenaEnemyId, partial) => {
    const current = get().arenaEnemies
    const existing = current[arenaEnemyId]
    if (!existing) return
    set({
      arenaEnemies: {
        ...current,
        [arenaEnemyId]: { ...existing, ...partial },
      },
    })
  },

  get: (arenaEnemyId) => {
    if (!arenaEnemyId)
      return undefined
    return get().arenaEnemies[arenaEnemyId]
  },

  reset: () => set({ arenaEnemies: {} }),
}))
