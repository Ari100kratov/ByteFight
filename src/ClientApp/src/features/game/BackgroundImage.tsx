import { useEffect, useState } from "react"
import { Sprite, Texture } from "pixi.js"
import { useAsset } from "@/shared/hooks/useAsset"
import { extend } from "@pixi/react"
import type { GridLayout } from "./grid/gridUtils"

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
    if (!blob) return;

    let cancelled = false;
    let tex: Texture | null = null;

    createImageBitmap(blob)
      .then((bitmap) => {
        if (cancelled) return;
        tex = Texture.from(bitmap);
        setTexture(tex);
      })
      .catch(console.error);

    return () => {
      cancelled = true;
      tex?.destroy(true);
      setTexture(null);
    };
  }, [blob]);

  if (!texture) return null

  return <pixiSprite
    texture={texture}
    x={offsetX}
    y={offsetY}
    width={gridPixelWidth}
    height={gridPixelHeight}
  />
}
