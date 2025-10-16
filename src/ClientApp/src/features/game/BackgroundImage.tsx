import { useEffect, useState } from "react"
import { Sprite, Texture } from "pixi.js"
import { useAsset } from "@/hooks/useAsset"
import { extend } from "@pixi/react"
import type { GridLayout } from "./gridUtils"

extend({ Sprite, Texture })

type BackgroundImageProps = {
  assetKey?: string
  layout: GridLayout
}

export function BackgroundImage({ assetKey, layout }: BackgroundImageProps) {
  const { gridPixelWidth, gridPixelHeight, offsetX, offsetY } = layout

  const { data: blob } = useAsset(assetKey)
  const [texture, setTexture] = useState<Texture | null>(null)

  useEffect(() => {
    if (!blob)
      return

    let tex: Texture

    createImageBitmap(blob)
      .then((bitmap) => {
        tex = Texture.from(bitmap)
        setTexture(tex)
      })
      .catch(console.error)

    return () => {
      tex?.destroy(true)
    }

  }, [blob])

  if (!texture) return null

  return <pixiSprite
    texture={texture}
    x={offsetX}
    y={offsetY}
    width={gridPixelWidth}
    height={gridPixelHeight}
  />
}
