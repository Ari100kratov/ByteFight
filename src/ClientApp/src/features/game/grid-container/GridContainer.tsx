import { extend } from "@pixi/react"
import { Graphics, Text, Container } from "pixi.js"
import { useGridStore } from "../state/game/grid.state.store"
import { useEffect } from "react"
import { useArenaStore } from "../state/data/arena.data.store"
import { useViewportStore } from "../state/viewport/viewport.store"

extend({ Graphics, Text, Container })

export function GridContainer() {

  const arena = useArenaStore((s) => s.arena)
  const viewport = useViewportStore(s => s.size)
  const { layout, updateLayout } = useGridStore()

  useEffect(() => {
    if (!arena) return

    updateLayout(
      { width: arena.gridWidth, height: arena.gridHeight },
      viewport
    )
  }, [arena, viewport.width, viewport.height])

  if (!layout)
    return null

  const { gridSize, cellSize, gridPixelWidth, gridPixelHeight, offsetX, offsetY, cells } = layout

  // cells сейчас хранит gridY = 0 снизу, и x/y — абсолютные пиксельные позиции (левый верх ячейки)
  // Но для упрощения рендеринга поместим всё в локальный контейнер со сдвигом offsetX/offsetY,
  // и будем рисовать в локальных координатах (0..gridPixelWidth, 0..gridPixelHeight).

  return (
    <pixiContainer x={offsetX} y={offsetY}>
      <pixiGraphics
        draw={(g) => {
          g.clear()
          g.setStrokeStyle({ width: 1, color: 0xffffff, alpha: 0.25 })

          for (let x = 0; x <= gridSize.width; x++) {
            const px = x * cellSize
            g.moveTo(px, 0)
            g.lineTo(px, gridPixelHeight)
          }

          for (let y = 0; y <= gridSize.height; y++) {
            const py = y * cellSize
            g.moveTo(0, py)
            g.lineTo(gridPixelWidth, py)
          }

          g.stroke()
        }}
      />

      {cells.flat().map((cell) => {
        const localX = cell.gridX * cell.width
        const localY = (gridSize.height - 1 - cell.gridY) * cell.height
        return (
          <pixiText
            key={`${cell.gridX}-${cell.gridY}`}
            text={`(${cell.gridX}, ${cell.gridY})`}
            x={localX + 4}
            y={localY + 4}
            style={{
              fontSize: 10,
              fill: 0xffffff,
              align: "left"
            }}
            alpha={0.45}
          />
        )
      })}

      {/* <pixiGraphics draw={(g) => {
        g.beginFill(0xff0000, 0.6)
        const markerX = 0 * cellSize + 2
        const markerY = (gridSize.height - 1 - 0) * cellSize + 2
        g.drawCircle(markerX + 6, markerY + 6, 4)
        g.endFill()
      }} /> */}
    </pixiContainer>
  )
}
