import { create } from "zustand"
import { calculateGridLayout } from "../../grid-container/gridUtils"
import type { GridLayout, GridSize } from "../../grid-container/gridUtils"
import type { ViewportSize } from "../viewport/viewport.store"

type GridState = {
  layout?: GridLayout
  updateLayout: (grid: GridSize, viewport: ViewportSize) => void
  reset: () => void
}

export const useGridStore = create<GridState>((set) => ({
  layout: undefined,

  updateLayout: (grid, viewport) =>
    set({
      layout: calculateGridLayout(grid, viewport),
    }),

  reset: () => set({ layout: undefined }),
}))