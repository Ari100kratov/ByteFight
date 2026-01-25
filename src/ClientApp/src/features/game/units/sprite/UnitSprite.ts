import type { SpriteAnimationDto } from "@/shared/types/spriteAnimation";
import { AnimatedSprite, Ticker } from "pixi.js";
import { useTexturesStore } from "../../state/data/textures.data.store";
import type { UnitRuntimeUpdater } from "../../types/UnitRuntime";
import { ActionType } from "@/shared/types/action";

const SPEED = 50;

export class UnitSprite {
  private updateRuntime: UnitRuntimeUpdater;

  constructor(updateRuntime: UnitRuntimeUpdater) {
    this.updateRuntime = updateRuntime;
  }

  private sprite?: AnimatedSprite;
  private currentAnimation?: string;

  attach(sprite: AnimatedSprite) {
    this.sprite = sprite;
  }

  async playAnimation(
    animation: SpriteAnimationDto,
    action: ActionType,
    loop: boolean,
    onFrame?: (frame: number) => void
  ): Promise<void> {
    if (!this.sprite) return;

    // защита от повторного запуска
    if (this.currentAnimation === animation.url && loop) return;
    this.currentAnimation = animation.url;
    this.updateRuntime({ action });

    const textures = await useTexturesStore
      .getState()
      .getOrLoadTextures(animation.url, animation.frameCount);

    this.updateRuntime({ textureHeight: textures[0].height });

    const sprite = this.sprite;
    sprite.textures = textures;
    sprite.animationSpeed = animation.animationSpeed;
    sprite.loop = loop;
    sprite.gotoAndPlay(0);

    if (loop) return;

    return new Promise(resolve => {
      sprite.onFrameChange = onFrame;
      sprite.onComplete = () => {
        sprite.onFrameChange = undefined;
        sprite.onComplete = undefined;
        resolve();
      };
    });
  }

  moveToPx(
    target: { x: number; y: number },
    onUpdate?: (pos: { x: number; y: number }) => void
  ): Promise<void> {
    if (!this.sprite) return Promise.resolve();

    const sprite = this.sprite;
    const ticker = Ticker.shared;

    const startX = sprite.x;
    const startY = sprite.y;
    const dx = target.x - startX;
    const dy = target.y - startY;
    const dist = Math.hypot(dx, dy);

    let traveled = 0;

    return new Promise(resolve => {
      const update = (t: Ticker) => {
        const step = SPEED * (t.deltaMS / 1000);
        traveled += step;

        const k = Math.min(1, traveled / dist);
        const x = startX + dx * k;
        const y = startY + dy * k;

        sprite.x = x;
        sprite.y = y;

        onUpdate?.({ x, y });

        if (k >= 1) {
          ticker.remove(update);
          resolve();
        }
      };

      ticker.add(update);
    });
  }
}
