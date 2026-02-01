import { create } from "zustand"

export type ViewportSize = {
  width: number
  height: number
}

type ViewportState = {
  size: ViewportSize
  setSize: (size: ViewportSize) => void
}

export const useViewportStore = create<ViewportState>((set) => ({
  size: { width: 0, height: 0 },
  setSize: (size) => set({ size }),
}))