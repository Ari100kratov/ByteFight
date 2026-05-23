import { create } from "zustand"
import type { InfoPopoverPosition } from "../../types/InfoPopoverPosition"

interface EnemySelectionState {
  selectedArenaEnemyId?: string
  position?: InfoPopoverPosition
  lastPosition?: InfoPopoverPosition
  lastDismissedArenaEnemyId?: string
  lastDismissedAt?: number
  selectEnemy: (arenaEnemyId: string, position: InfoPopoverPosition) => void
  clearSelection: () => void
  reset: () => void
}

const DISMISS_REOPEN_SUPPRESSION_MS = 300

export const useEnemySelectionStore = create<EnemySelectionState>((set) => ({
  selectedArenaEnemyId: undefined,
  position: undefined,
  lastPosition: undefined,
  lastDismissedArenaEnemyId: undefined,
  lastDismissedAt: undefined,

  selectEnemy: (selectedArenaEnemyId, position) => {
    set((state) => {
      const dismissedAt = state.lastDismissedAt ?? 0
      const isRecentlyDismissedSameEnemy =
        state.lastDismissedArenaEnemyId === selectedArenaEnemyId &&
        Date.now() - dismissedAt < DISMISS_REOPEN_SUPPRESSION_MS

      if (isRecentlyDismissedSameEnemy) {
        return {
          selectedArenaEnemyId: undefined,
          position: undefined,
          lastDismissedArenaEnemyId: undefined,
          lastDismissedAt: undefined,
        }
      }

      return {
        selectedArenaEnemyId,
        position,
        lastPosition: position,
        lastDismissedArenaEnemyId: undefined,
        lastDismissedAt: undefined,
      }
    })
  },

  clearSelection: () => {
    set((state) => ({
      selectedArenaEnemyId: undefined,
      position: undefined,
      lastPosition: state.position ?? state.lastPosition,
      lastDismissedArenaEnemyId: state.selectedArenaEnemyId,
      lastDismissedAt: state.selectedArenaEnemyId ? Date.now() : undefined,
    }))
  },

  reset: () => {
    set({
      selectedArenaEnemyId: undefined,
      position: undefined,
      lastPosition: undefined,
      lastDismissedArenaEnemyId: undefined,
      lastDismissedAt: undefined,
    })
  },
}))
