import type { AbilityType } from "@/shared/types/ability"
import { type ActionType } from "@/shared/types/action"
import type { SpriteAnimationDto } from "@/shared/types/spriteAnimation"

export interface UnitAnimationResolver {
  getAnimation(action: ActionType): SpriteAnimationDto | undefined
  getAbilityAnimation(
    abilityType?: AbilityType,
    actionType?: ActionType,
  ): SpriteAnimationDto | undefined
}
