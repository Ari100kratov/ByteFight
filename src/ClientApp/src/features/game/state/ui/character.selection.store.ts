import { create } from "zustand"
import type { InfoPopoverPosition } from "../../types/InfoPopoverPosition"

interface CharacterSelectionState {
  selectedCharacterId?: string
  position?: InfoPopoverPosition
  lastPosition?: InfoPopoverPosition
  lastDismissedCharacterId?: string
  lastDismissedAt?: number
  selectCharacter: (characterId: string, position: InfoPopoverPosition) => void
  clearSelection: () => void
  reset: () => void
}

const DISMISS_REOPEN_SUPPRESSION_MS = 300

export const useCharacterSelectionStore = create<CharacterSelectionState>((set) => ({
  selectedCharacterId: undefined,
  position: undefined,
  lastPosition: undefined,
  lastDismissedCharacterId: undefined,
  lastDismissedAt: undefined,

  selectCharacter: (selectedCharacterId, position) => {
    set((state) => {
      const dismissedAt = state.lastDismissedAt ?? 0
      const isRecentlyDismissedSameCharacter =
        state.lastDismissedCharacterId === selectedCharacterId &&
        Date.now() - dismissedAt < DISMISS_REOPEN_SUPPRESSION_MS

      if (isRecentlyDismissedSameCharacter) {
        return {
          selectedCharacterId: undefined,
          position: undefined,
          lastDismissedCharacterId: undefined,
          lastDismissedAt: undefined,
        }
      }

      return {
        selectedCharacterId,
        position,
        lastPosition: position,
        lastDismissedCharacterId: undefined,
        lastDismissedAt: undefined,
      }
    })
  },

  clearSelection: () => {
    set((state) => ({
      selectedCharacterId: undefined,
      position: undefined,
      lastPosition: state.position ?? state.lastPosition,
      lastDismissedCharacterId: state.selectedCharacterId,
      lastDismissedAt: state.selectedCharacterId ? Date.now() : undefined,
    }))
  },

  reset: () => {
    set({
      selectedCharacterId: undefined,
      position: undefined,
      lastPosition: undefined,
      lastDismissedCharacterId: undefined,
      lastDismissedAt: undefined,
    })
  },
}))
