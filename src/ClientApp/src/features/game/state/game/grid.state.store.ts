import { create } from "zustand"
import { calculateGridLayout } from "../../grid-container/gridUtils"
import type { GridLayout, GridSize } from "../../grid-container/gridUtils"
import type { ViewportSize } from "../viewport/viewport.store"

type GridState = {
  layout?: GridLayout
  updateLayout: (grid: GridSize, viewport: ViewportSize) => void
  reset: () => void

  showGrid: boolean
  setShowGrid: (value: boolean) => void
}

const GRID_STORAGE_KEY = "arena_show_grid"

export const useGridStore = create<GridState>((set) => ({
  layout: undefined,

  updateLayout: (grid, viewport) =>
    set({
      layout: calculateGridLayout(grid, viewport),
    }),

  reset: () => set({ layout: undefined }),

  showGrid: localStorage.getItem(GRID_STORAGE_KEY) === "true",

  setShowGrid: (value) => {
    localStorage.setItem(GRID_STORAGE_KEY, String(value))
    set({ showGrid: value })
  },
}))