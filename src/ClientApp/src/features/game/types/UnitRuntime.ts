import type { ActionType } from "@/shared/types/action"
import type { FacingDirection, Position, StatSnapshot } from "./common"
import type { AnimatedSprite } from "pixi.js"

export type UnitRuntime = {
  id: string
  spriteRef?: AnimatedSprite | null
  action: ActionType
  hp: StatSnapshot
  mp?: StatSnapshot
  facing: FacingDirection
  position: Position
  renderPosition?: { x: number; y: number }; // пиксели для плавной отрисовки
}