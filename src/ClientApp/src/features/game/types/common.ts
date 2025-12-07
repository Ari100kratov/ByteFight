export const FacingDirection = {
  Left: 1,
  Right: 2,
} as const;

export type FacingDirection = typeof FacingDirection[keyof typeof FacingDirection];

export type Position = {
  x: number;
  y: number;
};

export type StatSnapshot = {
  current: number;
  max: number;
};