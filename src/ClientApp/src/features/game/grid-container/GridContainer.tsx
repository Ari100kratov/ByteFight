import { extend } from "@pixi/react"
import { Container, Graphics, Text } from "pixi.js"
import { useEffect } from "react"
import { useArenaStore } from "../state/data/arena.data.store"
import { useGridStore } from "../state/game/grid.state.store"
import { useViewportStore } from "../state/viewport/viewport.store"

extend({ Graphics, Text, Container })

export function GridContainer() {
  const arena = useArenaStore((s) => s.arena)
  const viewport = useViewportStore((s) => s.size)
  const { layout, updateLayout, showGrid } = useGridStore()

  useEffect(() => {
    if (!arena) return

    updateLayout({ width: arena.gridWidth, height: arena.gridHeight }, viewport)
  }, [arena, updateLayout, viewport])

  if (!layout || !arena) return null

  const { gridSize, offsetX, offsetY, cells } = layout
  const cellWidth = cells[0]?.[0]?.width ?? 0
  const coordinateFontSize = Math.max(8, Math.min(11, Math.floor(cellWidth * 0.16)))
  const blockedSet = new Set(
    arena.blockedPositions.map((p) => `${String(p.x)}:${String(p.y)}`),
  )
  const visibleCells = cells
    .flat()
    .filter((cell) => !blockedSet.has(`${String(cell.gridX)}:${String(cell.gridY)}`))
  const snap = (value: number) => Math.round(value) + 0.5

  if (!showGrid) return <pixiContainer x={offsetX} y={offsetY} />

  return (
    <pixiContainer x={offsetX} y={offsetY}>
      <pixiGraphics
        draw={(g) => {
          g.clear()

          const drawnEdges = new Set<string>()
          const edgesToDraw: {
            key: string
            from: { x: number; y: number }
            to: { x: number; y: number }
          }[] = []

          for (const cell of visibleCells) {
            const x = cell.gridX * cell.width
            const y = (gridSize.height - 1 - cell.gridY) * cell.height
            const edges = [
              {
                key: `v:${String(cell.gridX)}:${String(cell.gridY)}`,
                from: { x, y },
                to: { x, y: y + cell.height },
              },
              {
                key: `v:${String(cell.gridX + 1)}:${String(cell.gridY)}`,
                from: { x: x + cell.width, y },
                to: { x: x + cell.width, y: y + cell.height },
              },
              {
                key: `h:${String(cell.gridY)}:${String(cell.gridX)}`,
                from: { x, y: y + cell.height },
                to: { x: x + cell.width, y: y + cell.height },
              },
              {
                key: `h:${String(cell.gridY + 1)}:${String(cell.gridX)}`,
                from: { x, y },
                to: { x: x + cell.width, y },
              },
            ]

            for (const edge of edges) {
              if (drawnEdges.has(edge.key)) continue
              drawnEdges.add(edge.key)
              edgesToDraw.push(edge)
            }
          }

          g.setStrokeStyle({
            width: 1,
            color: 0xcbd5e1,
            alpha: 0.16,
          })

          for (const edge of edgesToDraw) {
            g.moveTo(snap(edge.from.x), snap(edge.from.y))
            g.lineTo(snap(edge.to.x), snap(edge.to.y))
          }

          g.stroke()
        }}
      />

      {visibleCells.map((cell) => {
        const localX = cell.gridX * cell.width
        const localY = (gridSize.height - 1 - cell.gridY) * cell.height

        return (
          <pixiText
            key={`${String(cell.gridX)}-${String(cell.gridY)}`}
            text={`${String(cell.gridX)},${String(cell.gridY)}`}
            x={Math.round(localX + 4)}
            y={Math.round(localY + 3)}
            style={{
              fontSize: coordinateFontSize,
              fontWeight: "700",
              fill: 0xe2e8f0,
              align: "left",
              stroke: {
                color: 0x020617,
                width: 2,
              },
            }}
            alpha={0.56}
          />
        )
      })}
    </pixiContainer>
  )
}
