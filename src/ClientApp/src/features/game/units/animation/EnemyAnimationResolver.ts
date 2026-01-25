import { ActionType } from "@/shared/types/action";
import type { SpriteAnimationDto } from "@/shared/types/spriteAnimation";
import type { useEnemiesStore } from "../../state/data/enemies.data.store";

export class EnemyAnimationResolver {
  private enemyId: string;
  private get: typeof useEnemiesStore.getState

  constructor(enemyId: string, get: typeof useEnemiesStore.getState) {
    this.enemyId = enemyId;
    this.get = get;
  }

  getAnimation(action: ActionType): SpriteAnimationDto | undefined {
    return this.get().getSpriteAnimation(this.enemyId, action);
  }
}
