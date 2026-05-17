export const FacingDirection = {
  Left: 1,
  Right: 2,
} as const

export type FacingDirection = (typeof FacingDirection)[keyof typeof FacingDirection]

export interface Position {
  x: number
  y: number
}

export interface StatSnapshot {
  current: number
  max: number
}
