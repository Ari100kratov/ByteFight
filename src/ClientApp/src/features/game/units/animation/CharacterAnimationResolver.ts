import { ActionType } from "@/shared/types/action";
import type { SpriteAnimationDto } from "@/shared/types/spriteAnimation";
import type { useCharacterStore } from "../../state/data/character.data.store";

export class CharacterAnimationResolver {
  private get: typeof useCharacterStore.getState;

  constructor(get: typeof useCharacterStore.getState) {
    this.get = get;
  }

  getAnimation(action: ActionType): SpriteAnimationDto | undefined {
    return this.get().getSpriteAnimation(action);
  }
}
