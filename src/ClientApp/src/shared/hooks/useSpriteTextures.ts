import { useEffect, useState } from "react"
import { type Texture } from "pixi.js"
import { useTexturesStore } from "@/features/game/state/data/textures.data.store"
import type { SpriteAnimationDto } from "../types/spriteAnimation"

export function useSpriteTextures(spriteAnimation?: SpriteAnimationDto) {
  const getOrLoadTextures = useTexturesStore((s) => s.getOrLoadTextures)
  const [textures, setTextures] = useState<Texture[]>([])
  const url = spriteAnimation?.url
  const frameCount = spriteAnimation?.frameCount

  useEffect(() => {
    if (!url || frameCount === undefined) {
      setTextures([])
      return
    }

    let cancelled = false

    getOrLoadTextures(url, frameCount)
      .then((texs) => {
        if (cancelled) return
        setTextures(texs)
      })
      .catch(console.error)

    return () => {
      cancelled = true
    }
  }, [frameCount, getOrLoadTextures, url])

  return textures
}
