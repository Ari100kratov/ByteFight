import { ActionType } from "@/shared/types/action";
import { UnitSprite } from "../sprite/UnitSprite";
import type { UnitRuntimeUpdater } from "../../types/UnitRuntime";
import type { UnitAnimationResolver } from "../animation/UnitAnimationResolver";
import { useGridStore } from "../../state/game/grid.state.store";
import { gridToPixel } from "../../grid-container/gridUtils";
import type { AttackLogEntry, WalkLogEntry } from "../../types/TurnLog";
import type { StatSnapshot } from "../../types/common";

export class UnitController {
  sprite: UnitSprite;
  private updateRuntime: UnitRuntimeUpdater;
  private animations: UnitAnimationResolver;

  constructor(updateRuntime: UnitRuntimeUpdater, animations: UnitAnimationResolver) {
    this.updateRuntime = updateRuntime;
    this.animations = animations;
    this.sprite = new UnitSprite(updateRuntime);
  }

  private viewReady = false;

  notifyViewReady() {
    if (this.viewReady) return;
    this.viewReady = true;

    this.playIdle();
  }

  async playIdle() {
    const animation = this.animations.getAnimation(ActionType.Idle);
    if (animation) {
      await this.sprite.playAnimation(animation, ActionType.Idle, true);
    }
  }

  async playAttack(onHit: () => void) {
    const animation = this.animations.getAnimation(ActionType.Attack);
    if (!animation) return;

    await this.sprite.playAnimation(
      animation,
      ActionType.Attack,
      false,
      frame => frame === 1 && onHit()
    );

    await this.playIdle();
  }

  async playHurt(hp: StatSnapshot) {
    const animation = this.animations.getAnimation(ActionType.Hurt);
    if (!animation) return;

    await this.sprite.playAnimation(animation, ActionType.Hurt, false);
    this.updateRuntime({ hp });
    await this.playIdle();
  }

  async walkTo(
    entry: WalkLogEntry
  ) {
    const layout = useGridStore.getState().layout;
    if (!layout) throw new Error("Layout not ready");

    const animation = this.animations.getAnimation(ActionType.Walk);
    if (!animation) throw new Error("Animation not found");

    this.updateRuntime({ facing: entry.facingDirection });
    const toPx = gridToPixel(entry.to, layout);

    await Promise.all([
      this.sprite.playAnimation(animation, ActionType.Walk, true),
      this.sprite.moveToPx(toPx, px => {
        this.updateRuntime({ renderPosition: px });
      })
    ]);

    this.updateRuntime({ position: entry.to });
    await this.playIdle();
  }

  async attack(
    target: UnitController,
    entry: AttackLogEntry
  ) {
    const animation = this.animations.getAnimation(ActionType.Attack);
    if (!animation) throw new Error("Animation not found");

    this.updateRuntime({ facing: entry.facingDirection });

    await this.sprite.playAnimation(
      animation,
      ActionType.Attack,
      false,
      frame => {
        if (frame === 1) {
          target.playHurt(entry.targetHp);
        }
      }
    );

    await this.playIdle();
  }

  async death() {
    const animation = this.animations.getAnimation(ActionType.Dead);
    if (!animation) throw new Error("Animation not found");

    await this.sprite.playAnimation(animation, ActionType.Dead, false);
  }
}
