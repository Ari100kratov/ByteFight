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
  const { layout, updateLayout, showGrid } = useGridStore()

  useEffect(() => {
    if (!arena) return

    updateLayout(
      { width: arena.gridWidth, height: arena.gridHeight },
      viewport
    )
  }, [arena, viewport.width, viewport.height])

  if (!layout)
    return null

  const { gridSize, offsetX, offsetY, cells } = layout
  const blockedSet = new Set(
    arena?.blockedPositions?.map(p => `${p.x}:${p.y}`)
  )

  // cells сейчас хранит gridY = 0 снизу, и x/y — абсолютные пиксельные позиции (левый верх ячейки)
  // Но для упрощения рендеринга поместим всё в локальный контейнер со сдвигом offsetX/offsetY,
  // и будем рисовать в локальных координатах (0..gridPixelWidth, 0..gridPixelHeight).

  return (
    <pixiContainer x={offsetX} y={offsetY}>

      {/* Линии сетки */}
      {showGrid && (
        <pixiGraphics
          draw={(g) => {
            g.clear()
            g.setStrokeStyle({ width: 1, color: 0xffffff, alpha: 0.25 })

            for (const cell of cells.flat()) {

              if (blockedSet.has(`${cell.gridX}:${cell.gridY}`))
                continue

              const x = cell.gridX * cell.width
              const y = (gridSize.height - 1 - cell.gridY) * cell.height

              g.rect(x, y, cell.width, cell.height)
            }

            g.stroke()
          }}
        />
      )}

      {/* Координаты клеток */}
      {showGrid &&
        cells.flat().map((cell) => {

          if (blockedSet.has(`${cell.gridX}:${cell.gridY}`))
            return null

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
    </pixiContainer>
  )
}
