import { useEffect, useState } from "react"
import { Sprite, Texture } from "pixi.js"
import { useAssetBlob } from "@/shared/hooks/useAsset"
import { extend } from "@pixi/react"
import { useGridStore } from "../state/game/grid.state.store"
import { useArenaStore } from "../state/data/arena.data.store"

extend({ Sprite, Texture })

export function BackgroundSprite() {
  const layout = useGridStore(s => s.layout)
  const arena = useArenaStore(s => s.arena)

  const { data: blob } = useAssetBlob(arena?.backgroundAsset)
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

  if (!texture || !layout)
    return null

  const { gridPixelWidth, gridPixelHeight, offsetX, offsetY } = layout

  return <pixiSprite
    texture={texture}
    x={offsetX}
    y={offsetY}
    width={gridPixelWidth}
    height={gridPixelHeight}
  />
}
