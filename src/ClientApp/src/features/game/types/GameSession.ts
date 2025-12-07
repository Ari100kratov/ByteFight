import type { GameResult } from "./GameResult";

export const GameStatus = {
  Pending: 1,
  Active: 2,
  Completed: 3,
  Aborted: 4,
  Failed: 5
} as const;

export type GameStatus = (typeof GameStatus)[keyof typeof GameStatus];

export const GameModeType = {
  Training: 0,
  PvE: 1,
  PvP: 2
} as const;

export type GameModeType = (typeof GameModeType)[keyof typeof GameModeType];

export type GameSession = {
  id: string;
  mode: GameModeType;

  startedAt: string;
  endedAt?: string | null;

  totalTurns: number;
  status: GameStatus;

  result?: GameResult | null;
};

export function isGameSessionActive(session: GameSession | undefined | null): boolean {
  if (!session) return false

  return (
    session.status === GameStatus.Pending ||
    session.status === GameStatus.Active
  )
}