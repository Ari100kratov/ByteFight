import { Sprite, Texture } from "pixi.js"
import { extend } from "@pixi/react"
import { useGridStore } from "../state/game/grid.state.store"
import { useArenaStore } from "../state/data/arena.data.store"
import { useSpriteTexture } from "@/shared/hooks/useSpriteTexture"

extend({ Sprite, Texture })

export function BackgroundSprite() {
  const layout = useGridStore(s => s.layout)
  const arena = useArenaStore(s => s.arena)
  const texture = useSpriteTexture(arena?.backgroundAsset)

  if (!layout || !texture)
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
