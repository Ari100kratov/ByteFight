import type { ActionType } from "@/shared/types/action"
import type { FacingDirection, Position, StatSnapshot } from "./common"

export type UnitRuntime = {
  id: string

  action: ActionType
  facing: FacingDirection
  position: Position
  renderPosition?: { x: number; y: number }; // пиксели для плавной отрисовки
  textureHeight?: number

  hp: StatSnapshot
  mp?: StatSnapshot
}

export type UnitRuntimeUpdater = (partial: Partial<UnitRuntime>) => void