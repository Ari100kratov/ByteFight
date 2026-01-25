import { ActionType } from "@/shared/types/action";
import type { SpriteAnimationDto } from "@/shared/types/spriteAnimation";

export interface UnitAnimationResolver {
  getAnimation(action: ActionType): SpriteAnimationDto | undefined;
}
