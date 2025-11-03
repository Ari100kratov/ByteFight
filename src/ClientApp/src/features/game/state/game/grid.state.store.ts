import { create } from "zustand"
import { calculateGridLayout, type GridLayout, type GridSize } from "../../grid-container/gridUtils"

export type CanvasSize = {
  width: number
  height: number
}

interface GridState {
  canvasSize: CanvasSize
  layout?: GridLayout
  setLayout: (gridSize: GridSize) => void
  setCanvasSize: (canvasSize: CanvasSize) => void
}

export const useGridStore = create<GridState>((set) => ({
  canvasSize: { width: 650, height: 450 }, // TODO: сделать динамическим/адаптивным значением
  layout: undefined,
  setLayout: (gridSize) =>
    set((state) => ({
      layout: calculateGridLayout(gridSize, state.canvasSize)
    })),
  setCanvasSize: (canvasSize) =>
    set({ canvasSize: canvasSize }),
}))
