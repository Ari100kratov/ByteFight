import { useEffect, useState } from "react"
import { type Texture } from "pixi.js"
import { useTexturesStore } from "@/features/game/state/data/textures.data.store"
import type { SpriteAnimationDto } from "../types/spriteAnimation"

interface SpriteTexturesState {
  key: string
  textures: Texture[]
}

export function useSpriteTextures(spriteAnimation?: SpriteAnimationDto) {
  const getOrLoadTextures = useTexturesStore((s) => s.getOrLoadTextures)
  const [state, setState] = useState<SpriteTexturesState | null>(null)
  const url = spriteAnimation?.url
  const frameCount = spriteAnimation?.frameCount
  const key = url && frameCount !== undefined ? `${url}:${String(frameCount)}` : undefined
  const textures = state && state.key === key ? state.textures : []

  useEffect(() => {
    if (!url || frameCount === undefined) {
      return
    }

    let cancelled = false

    getOrLoadTextures(url, frameCount)
      .then((texs) => {
        if (cancelled) return
        setState({ key: `${url}:${String(frameCount)}`, textures: texs })
      })
      .catch(console.error)

    return () => {
      cancelled = true
    }
  }, [frameCount, getOrLoadTextures, url])

  return textures
}
