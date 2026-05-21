import { useEffect, useState } from "react"
import { type Texture } from "pixi.js"
import { useTexturesStore } from "@/features/game/state/data/textures.data.store"

export function isTextureRenderable(texture?: Texture | null) {
  if (!texture) return false
  if (texture.destroyed) return false
  if (texture.width === 0 || texture.height === 0) return false
  return true
}

interface SpriteTextureState {
  url: string
  texture: Texture | null
}

export function useSpriteTexture(url?: string) {
  const getOrLoadTexture = useTexturesStore((s) => s.getOrLoadTexture)
  const [state, setState] = useState<SpriteTextureState | null>(null)
  const texture = state && state.url === url ? state.texture : null

  useEffect(() => {
    if (!url) {
      return
    }

    let cancelled = false
    getOrLoadTexture(url)
      .then((tex) => {
        if (cancelled) return
        setState({ url, texture: tex })
      })
      .catch(console.error)

    return () => {
      cancelled = true
    }
  }, [getOrLoadTexture, url])

  return texture
}
