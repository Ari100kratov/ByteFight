import type { ActionType } from "@/shared/types/action"

export type Position = {
  x: number
  y: number
}

export type FacingDirection = "left" | "right"

export type ResourceStat = {
  current: number
  max: number
}

export type UnitRuntime = {
  id: string
  currentAction: ActionType
  position: Position
  hp: ResourceStat
  mp?: ResourceStat
  facing: FacingDirection
}