import { create } from "zustand"

interface UnitHoverState {
  hoveredUnitId?: string
  setHoveredUnit: (unitId: string) => void
  clearHoveredUnit: (unitId: string) => void
  reset: () => void
}

export const useUnitHoverStore = create<UnitHoverState>((set, get) => ({
  hoveredUnitId: undefined,

  setHoveredUnit: (hoveredUnitId) => {
    set({ hoveredUnitId })
  },

  clearHoveredUnit: (unitId) => {
    if (get().hoveredUnitId !== unitId) return

    set({ hoveredUnitId: undefined })
  },

  reset: () => {
    set({ hoveredUnitId: undefined })
  },
}))
