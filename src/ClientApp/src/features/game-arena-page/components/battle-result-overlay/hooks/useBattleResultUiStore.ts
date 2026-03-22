import { create } from "zustand"

type BattleResultUiState = {
  openedForSessionId: string | null
  isOpen: boolean

  showForSession: (sessionId: string) => void
  open: () => void
  close: () => void
  reset: () => void
}

export const useBattleResultUiStore = create<BattleResultUiState>(set => ({
  openedForSessionId: null,
  isOpen: false,

  showForSession: (sessionId) =>
    set(state => {
      if (state.openedForSessionId === sessionId) {
        return state
      }

      return {
        openedForSessionId: sessionId,
        isOpen: true
      }
    }),

  open: () => set({ isOpen: true }),
  close: () => set({ isOpen: false }),
  reset: () => set({ openedForSessionId: null, isOpen: false })
}))