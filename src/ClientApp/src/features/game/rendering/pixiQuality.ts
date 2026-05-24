import type { Texture } from "pixi.js"

export type TextureScaleMode = "nearest" | "linear"

export const PIXEL_ART_SCALE_MODE: TextureScaleMode = "nearest"
export const SMOOTH_SCALE_MODE: TextureScaleMode = "linear"

export const RENDER_LAYERS = {
  background: -10000,
  grid: -9000,
  items: 0,
  unitHighlights: 50,
  units: 100,
  effects: 10000,
  floatingText: 10001,
} as const

interface TextureSourceWithScaleMode {
  scaleMode?: TextureScaleMode
  style?: {
    scaleMode?: TextureScaleMode
  }
  update?: () => void
}

export function setTextureScaleMode(texture: Texture | null | undefined, scaleMode: TextureScaleMode) {
  const source = texture?.source as TextureSourceWithScaleMode | undefined
  if (!source) return

  source.scaleMode = scaleMode

  if (source.style) {
    source.style.scaleMode = scaleMode
  }

  source.update?.()
}

export function setTexturesScaleMode(
  textures: readonly Texture[] | null | undefined,
  scaleMode: TextureScaleMode,
) {
  if (!textures) return

  for (const texture of textures) {
    setTextureScaleMode(texture, scaleMode)
  }
}

export function snapPixel(value: number) {
  return Math.round(value)
}

export function getCanvasResolution() {
  if (typeof window === "undefined") return 1

  return Math.max(1, window.devicePixelRatio || 1)
}
