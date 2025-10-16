import { extend } from "@pixi/react"
import { Graphics } from "pixi.js"
import { type GridLayout } from "./gridUtils"

extend({ Graphics })

type GridProps = {
  layout: GridLayout
}

export function Grid({ layout }: GridProps) {

  const { gridSize, cellSize, gridPixelWidth, gridPixelHeight, offsetX, offsetY } = layout

  return (
    <pixiGraphics
      x={offsetX}
      y={offsetY}
      draw={(g) => {
        g.clear()
        g.setStrokeStyle({ width: 1, color: 0xffffff, alpha: 0.3 })

        for (let x = 0; x <= gridSize.width; x++) {
          g.moveTo(x * cellSize, 0)
          g.lineTo(x * cellSize, gridPixelWidth)
        }

        for (let y = 0; y <= gridSize.height; y++) {
          g.moveTo(0, y * cellSize)
          g.lineTo(gridPixelHeight, y * cellSize)
        }

        g.stroke()
      }}
    />
  )
}
