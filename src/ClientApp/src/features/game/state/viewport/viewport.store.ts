import { create } from "zustand"

export interface ViewportSize {
  width: number
  height: number
}

interface ViewportState {
  size: ViewportSize
  setSize: (size: ViewportSize) => void
}

export const useViewportStore = create<ViewportState>((set) => ({
  size: { width: 0, height: 0 },
  setSize: (size) => {
    set({ size })
  },
}))
