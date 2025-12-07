import { create } from "zustand";
import { type TurnLog, isWalk } from "../types/TurnLog";
import type { GameSession } from "../types/GameSession";
import { moveUnitSmooth } from "../animation/animationController";

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
    const { queue, isProcessing, processNext } = get();
    set({ queue: [...queue, turn] });

    if (!isProcessing) processNext();
  },

  processNext: async () => {
    const { queue, isProcessing } = get();
    if (isProcessing || queue.length === 0) return;

    const turn = queue[0];
    set({ isProcessing: true });

    for (const entry of turn.logs) {
      if (isWalk(entry)) {
        await moveUnitSmooth(entry);
      }
    }

    set((s) => ({
      queue: s.queue.slice(1),
      isProcessing: false,
    }));

    if (get().queue.length > 0) get().processNext();
  },

  reset: () =>
    set({
      session: null,
      queue: [],
      isProcessing: false,
    }),
}));
