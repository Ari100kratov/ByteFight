import type { SpriteAnimationDto } from "@/shared/types/spriteAnimation"
import { type AnimatedSprite, Ticker } from "pixi.js"
import { useTexturesStore } from "../../state/data/textures.data.store"
import type { UnitRuntimeUpdater } from "../../types/UnitRuntime"
import { type ActionType } from "@/shared/types/action"
import {
  PIXEL_ART_SCALE_MODE,
  setTexturesScaleMode,
  snapPixel,
} from "../../rendering/pixiQuality"

const SPEED = 70

export class UnitSprite {
  private updateRuntime: UnitRuntimeUpdater

  constructor(updateRuntime: UnitRuntimeUpdater) {
    this.updateRuntime = updateRuntime
  }

  private sprite?: AnimatedSprite
  private currentAnimation?: string
  private playVersion = 0

  attach(sprite: AnimatedSprite) {
    this.sprite = sprite
  }

  async playAnimation(
    animation: SpriteAnimationDto,
    action: ActionType,
    loop: boolean,
    onFrame?: (frame: number) => void | Promise<void>,
  ): Promise<void> {
    if (!this.sprite) return

    const version = ++this.playVersion

    if (this.currentAnimation === animation.url && loop) return

    const sprite = this.sprite

    const textures = await useTexturesStore
      .getState()
      .getOrLoadTextures(animation.url, animation.frameCount)

    if (!textures.length) return
    if (this.sprite !== sprite) return
    if (version !== this.playVersion) return

    setTexturesScaleMode(textures, PIXEL_ART_SCALE_MODE)

    this.currentAnimation = animation.url

    sprite.stop()
    sprite.onFrameChange = undefined
    sprite.onComplete = undefined

    sprite.textures = textures
    sprite.animationSpeed = animation.animationSpeed
    sprite.loop = loop

    this.updateRuntime({ action, spriteAnimation: animation, textureHeight: textures[0]?.height })

    sprite.gotoAndStop(0)

    if (loop) {
      sprite.play()
      return
    }

    let framePromise: Promise<void> | null = null
    const handledFrames = new Set<number>()

    return new Promise((resolve) => {
      sprite.onFrameChange = (frame) => {
        if (!onFrame) return
        if (handledFrames.has(frame)) return

        handledFrames.add(frame)

        const result = onFrame(frame)
        if (result instanceof Promise) {
          framePromise = result
        }
      }

      sprite.onComplete = () => {
        if (version !== this.playVersion) return

        sprite.stop()
        sprite.onFrameChange = undefined
        sprite.onComplete = undefined

        void resolveAfterFrame()
      }

      sprite.play()

      async function resolveAfterFrame() {
        if (framePromise) {
          await framePromise
        }

        resolve()
      }
    })
  }

  moveToPx(
    target: { x: number; y: number },
    onUpdate?: (pos: { x: number; y: number }) => void,
  ): Promise<void> {
    if (!this.sprite) return Promise.resolve()

    const sprite = this.sprite
    const ticker = Ticker.shared

    const startX = sprite.x
    const startY = sprite.y
    const dx = target.x - startX
    const dy = target.y - startY
    const dist = Math.hypot(dx, dy)

    let traveled = 0

    return new Promise((resolve) => {
      const update = (t: Ticker) => {
        const step = SPEED * (t.deltaMS / 1000)
        traveled += step

        const k = Math.min(1, traveled / dist)
        const x = snapPixel(startX + dx * k)
        const y = snapPixel(startY + dy * k)

        sprite.x = x
        sprite.y = y

        onUpdate?.({ x, y })

        if (k >= 1) {
          ticker.remove(update)
          resolve()
        }
      }

      ticker.add(update)
    })
  }
}
