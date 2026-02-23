import { create } from "zustand";
import { type TurnLog } from "../types/TurnLog";
import type { GameSession } from "../types/GameSession";
import { playRuntimeLog } from "../runtime/playRuntimeLog";

interface GameRuntimeState {
  session: GameSession | null;
  setSession: (session: GameSession) => void;
  turnLogs: TurnLog[]
  setTurnLogs: (turnLogs: TurnLog[]) => void;

  queue: TurnLog[];
  isProcessing: boolean;
  enqueueTurn: (turnLog: TurnLog) => void;
  processNext: () => Promise<void>;
  reset: () => void;
}

export const useGameRuntimeStore = create<GameRuntimeState>((set, get) => ({
  session: null,
  setSession: (session) => set({ session }),

  turnLogs: [],
  setTurnLogs: (incomingTurns) => {
    set((state) => {
      const existingTurns = [...state.turnLogs]

      const existingIds = new Set<string>()
      for (const turn of existingTurns) {
        for (const log of turn.logs) {
          existingIds.add(log.id)
        }
      }

      for (const incomingTurn of incomingTurns) {
        let targetTurn = existingTurns.find(
          t => t.turnIndex === incomingTurn.turnIndex)

        if (!targetTurn) {
          targetTurn = { turnIndex: incomingTurn.turnIndex, logs: [] }
          existingTurns.push(targetTurn)
        }

        for (const log of incomingTurn.logs) {
          if (existingIds.has(log.id)) continue

          targetTurn.logs.push(log)
          existingIds.add(log.id)
        }
      }

      existingTurns.sort((a, b) => a.turnIndex - b.turnIndex)

      return { turnLogs: existingTurns }
    })
  },

  queue: [],
  isProcessing: false,

  enqueueTurn: (turnLog) => {
    set((state) => ({
      queue: [...state.queue, turnLog],
    }));

    get().processNext();
  },

  processNext: async () => {
    if (get().isProcessing) return;

    const turn = get().queue[0];
    if (!turn) return;

    set({ isProcessing: true });

    for (const entry of turn.logs) {
      // добавляем лог в стор по одному
      set((state) => {
        const turns = [...state.turnLogs]
        let targetTurn = turns.find(t => t.turnIndex === entry.turnIndex)

        if (!targetTurn) {
          targetTurn = { turnIndex: entry.turnIndex, logs: [] }
          turns.push(targetTurn)
        }

        if (!targetTurn.logs.some(l => l.id === entry.id)) {
          targetTurn.logs.push(entry)
        }

        turns.sort((a, b) => a.turnIndex - b.turnIndex)

        return { turnLogs: turns }
      })

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
      turnLogs: [],
      queue: [],
      isProcessing: false,
    }),
}));
