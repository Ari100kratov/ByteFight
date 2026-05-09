import type { ActionType } from "@/shared/types/action"
import type { FacingDirection, Position, StatSnapshot } from "./common"
import type { SpriteAnimationDto } from "@/shared/types/spriteAnimation"

export type UnitRuntime = {
  id: string

  action: ActionType
  spriteAnimation?: SpriteAnimationDto
  facing: FacingDirection
  position: Position
  renderPosition?: { x: number; y: number }; // пиксели для плавной отрисовки
  textureHeight?: number

  hp: StatSnapshot
  mp?: StatSnapshot
}

export type UnitRuntimeUpdater = (partial: Partial<UnitRuntime>) => void