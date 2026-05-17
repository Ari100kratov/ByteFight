import { create } from "zustand"

type ArenaItemSelectionState = {
  selectedPlacedItemId?: string
  position?: { x: number; y: number }
  select: (placedItemId: string, position: { x: number; y: number }) => void
  clearSelection: () => void
}

export const useArenaItemSelectionStore = create<ArenaItemSelectionState>((set) => ({
  selectedPlacedItemId: undefined,
  position: undefined,

  select: (selectedPlacedItemId, position) => {
    set({ selectedPlacedItemId, position })
  },

  clearSelection: () => {
    set({
      selectedPlacedItemId: undefined,
      position: undefined,
    })
  },
}))
