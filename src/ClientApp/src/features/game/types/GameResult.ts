export const GameOutcome = {
  Victory: 1,
  Defeat: 2,
  Draw: 3,
  TimeoutLoss: 4,
  TurnLimitLoss: 5,
} as const;

export type GameOutcome = (typeof GameOutcome)[keyof typeof GameOutcome];

export type GameResult = {
  outcome: GameOutcome;
  winnerUnitId?: string | null;
};
