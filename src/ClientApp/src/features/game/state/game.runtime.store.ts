import { create } from "zustand";
import { type TurnLog } from "../types/TurnLog";
import type { GameSession } from "../types/GameSession";
import { playRuntimeLog } from "../runtime/playRuntimeLog";

interface GameRuntimeState {
  session: GameSession | null;
  queue: TurnLog[];
  isProcessing: boolean;

  setSession: (session: GameSession) => void;
  enqueueTurn: (turn: TurnLog) => void;
  processNext: () => Promise<void>;
  reset: () => void;
}

export const useGameRuntimeStore = create<GameRuntimeState>((set, get) => ({
  session: null,
  queue: [],
  isProcessing: false,

  setSession: (session) => set({ session }),

  enqueueTurn: (turn) => {
    set((state) => ({
      queue: [...state.queue, turn],
    }));

    get().processNext();
  },

  processNext: async () => {
    if (get().isProcessing) return;

    const turn = get().queue[0];
    if (!turn) return;

    set({ isProcessing: true });

    for (const entry of turn.logs) {
      await playRuntimeLog(entry);
    }

    set((s) => ({
      queue: s.queue.slice(1),
      isProcessing: false,
    }));

    // гарантированная последовательность
    queueMicrotask(() => get().processNext());
  },

  reset: () =>
    set({
      session: null,
      queue: [],
      isProcessing: false,
    }),
}));
