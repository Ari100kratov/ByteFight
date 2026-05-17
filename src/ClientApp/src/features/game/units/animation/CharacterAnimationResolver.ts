import type { AbilityType } from "@/shared/types/ability"
import { ActionType } from "@/shared/types/action"
import type { SpriteAnimationDto } from "@/shared/types/spriteAnimation"
import type { useCharacterStore } from "../../state/data/character.data.store"
import type { UnitAnimationResolver } from "./UnitAnimationResolver"

export class CharacterAnimationResolver implements UnitAnimationResolver {
  private get: typeof useCharacterStore.getState

  constructor(get: typeof useCharacterStore.getState) {
    this.get = get
  }

  getAnimation(action: ActionType): SpriteAnimationDto | undefined {
    return this.get().getSpriteAnimation(action)
  }

  getAbilityAnimation(
    abilityType?: AbilityType,
    actionType: ActionType = ActionType.Attack,
  ): SpriteAnimationDto | undefined {
    return this.get().getAbilitySpriteAnimation(abilityType, actionType)
  }
}
