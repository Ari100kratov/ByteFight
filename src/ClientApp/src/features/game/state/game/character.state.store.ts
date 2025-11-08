import { create } from "zustand"
import { ActionType } from "@/shared/types/action"
import type { UnitRuntime } from "./types/unit.runtime"

type InitPayload = {
  characterId: string
  maxHp: number
  maxMp?: number
}

type CharacterRuntimeState = {
  runtime?: UnitRuntime
  set: (runtime: Partial<UnitRuntime>) => void
  init: (payload: InitPayload) => void
  reset: () => void
}

export const useCharacterStateStore = create<CharacterRuntimeState>((set, get) => ({
  runtime: undefined,

  init: (payload) => {
    const prev = get().runtime
    if (!prev || prev.id !== payload.characterId) {
      set({
        runtime: {
          id: payload.characterId,
          currentAction: ActionType.Idle,
          position: { x: 0, y: 0 },
          hp: { current: payload.maxHp, max: payload.maxHp },
          mp: payload.maxMp ? { current: payload.maxMp, max: payload.maxMp } : undefined,
          facing: "right",
        },
      })
    }
  },

  set: (runtime) => {
    const prev = get().runtime
    if (!prev) return
    set({ runtime: { ...prev, ...runtime } })
  },

  reset: () => set({ runtime: undefined }),
}))
