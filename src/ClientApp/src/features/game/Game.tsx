import { Application } from "@pixi/react"
import type { Arena } from "../game-arena-page/useArena"
import { Grid } from "./Grid"
import { useState } from "react"
import { BackgroundImage } from "./BackgroundImage"
import { calculateGridLayout } from "./gridUtils"

type Props = {
  arena: Arena
}

export function Game({ arena }: Props) {
  const [canvasSize, setCanvasSize] = useState({ width: 650, height: 450 })

  const layout = calculateGridLayout(
    { width: arena.gridWidth, height: arena.gridHeight },
    canvasSize
  )

  return (
    <Application width={canvasSize.width} height={canvasSize.height}>
      <BackgroundImage
        assetKey={arena.backgroundAsset}
        layout={layout}
      />
      <Grid
        layout={layout}
      />
    </Application>
  )
}
