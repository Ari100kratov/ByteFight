import { create } from "zustand";

interface GameBootstrapState {
  isLoading: boolean
  start: () => void
  end: () => void
}

export const useGameBootstrapStore = create<GameBootstrapState>((set) => ({
  isLoading: false,
  start: () => set({ isLoading: true }),
  end: () => set({ isLoading: false }),
}));