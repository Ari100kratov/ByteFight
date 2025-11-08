import { create } from "zustand"
import { ActionType } from "@/shared/types/action"
import type { PositionDto } from "../../shared/types"
import type { UnitRuntime } from "./types/unit.runtime"

type InitPayload = {
  arenaEnemyId: string
  position: PositionDto
  maxHp: number
  maxMp?: number
}

type EnemyRuntimeStore = {
  arenaEnemies: Record<string, UnitRuntime>
  init: (arenaEnemies: InitPayload[]) => void
  set: (arenaEnemyId: string, partial: Partial<UnitRuntime>) => void
  get: (arenaEnemyId?: string) => UnitRuntime | undefined
  reset: () => void
}

export const useEnemyStateStore = create<EnemyRuntimeStore>((set, get) => ({
  arenaEnemies: {},

  init: (arenaEnemies) => {
    const current = get().arenaEnemies
    const updated = { ...current }

    for (const { arenaEnemyId, position, maxHp, maxMp } of arenaEnemies) {
      if (updated[arenaEnemyId]) continue
      updated[arenaEnemyId] = {
        id: arenaEnemyId,
        currentAction: ActionType.Idle,
        position: position,
        hp: { current: maxHp, max: maxHp },
        mp: maxMp ? { current: maxMp, max: maxMp } : undefined,
        facing: "left"
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
