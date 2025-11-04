import { create } from "zustand"
import { ActionType } from "@/shared/types/action"
import type { PositionDto } from "../../types"

type CharacterRuntime = {
  currentAction: ActionType
  position: PositionDto
}

type CharacterRuntimeState = {
  runtime?: CharacterRuntime
  set: (runtime: Partial<CharacterRuntime>) => void
  init: () => void
  reset: () => void
}

export const useCharacterStateStore = create<CharacterRuntimeState>((set, get) => ({
  runtime: undefined,

  init: () => {
    const { runtime } = get()
    if (runtime) return
    set({ runtime: { currentAction: ActionType.Idle, position: { x: 0, y: 0 } } })
  },

  set: (runtime) => {
    const prev = get().runtime
    if (!prev) return
    set({ runtime: { ...prev, ...runtime } })
  },

  reset: () => set({ runtime: undefined }),
}))
