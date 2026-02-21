import { create } from "zustand"
import { ActionType } from "@/shared/types/action"
import { FacingDirection, type Position } from "../../types/common"
import type { UnitRuntime } from "../../types/UnitRuntime"

type InitPayload = {
  characterId: string
  maxHp: number
  maxMp?: number,
  startPosition: Position
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
          action: ActionType.Idle,
          hp: { current: payload.maxHp, max: payload.maxHp },
          mp: payload.maxMp ? { current: payload.maxMp, max: payload.maxMp } : undefined,
          facing: FacingDirection.Right,
          position: payload.startPosition,
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
