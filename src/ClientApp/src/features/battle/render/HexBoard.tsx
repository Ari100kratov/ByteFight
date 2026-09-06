import { extend } from '@pixi/react'
import { Graphics } from 'pixi.js'

import { TERRAIN_FILL, COLORS } from './battleColors'
import { flatTopHexCorners, hexKey, hexToPixel, type HexPosition } from '../hex'

extend({ Graphics })

interface HexBoardProps {
  width: number
  height: number
  hexSize: number
  terrain: Map<string, number>
  blocked: Set<string>
  reachable: Set<string>
  previewPath: string[]
  targetHexes: Set<string>
  hoveredHex: HexPosition | null
}

/**
 * Слой гексов арены: рельеф, сетка и подсветки.
 * Рисуется одним Graphics для производительности.
 */
export function HexBoard({
  width,
  height,
  hexSize,
  terrain,
  blocked,
  reachable,
  previewPath,
  targetHexes,
  hoveredHex,
}: HexBoardProps) {
  const draw = (g: Graphics) => {
    g.clear()

    for (let x = 0; x < width; x++) {
      for (let y = 0; y < height; y++) {
        const key = hexKey({ x, y })
        const fill = blocked.has(key)
          ? COLORS.rockFill
          : TERRAIN_FILL[terrain.get(key) ?? 1] ?? COLORS.meadowFill

        drawHex(g, { x, y }, hexSize, fill, 1, COLORS.gridStroke, 0.9, 1.5)
      }
    }

    for (const key of reachable) {
      drawHex(g, parseKey(key), hexSize * 0.94, COLORS.reachableFill, COLORS.reachableAlpha)
    }

    for (const key of previewPath) {
      drawHex(g, parseKey(key), hexSize * 0.9, COLORS.pathFill, COLORS.pathAlpha)
    }

    for (const key of targetHexes) {
      drawHex(g, parseKey(key), hexSize * 0.94, COLORS.targetFill, COLORS.targetAlpha)
    }

    if (hoveredHex) {
      drawHexStroke(g, hoveredHex, hexSize, COLORS.gridStrokeHover, 2.5)
    }
  }

  return <pixiGraphics draw={draw} />
}

function parseKey(key: string): HexPosition {
  const [x, y] = key.split(':').map(Number)

  return { x, y }
}

function hexPolygon(center: { x: number; y: number }, size: number): number[] {
  const corners = flatTopHexCorners(size)
  const points: number[] = []

  for (let i = 0; i < corners.length; i += 2) {
    points.push(center.x + corners[i], center.y + corners[i + 1])
  }

  return points
}

function drawHex(
  g: Graphics,
  hex: HexPosition,
  size: number,
  fill: number,
  fillAlpha = 1,
  strokeColor?: number,
  strokeAlpha = 1,
  strokeWidth = 1,
) {
  const center = hexToPixel(hex, size)

  g.poly(hexPolygon(center, size))
  g.fill({ color: fill, alpha: fillAlpha })

  if (strokeColor !== undefined) {
    g.stroke({ color: strokeColor, alpha: strokeAlpha, width: strokeWidth })
  }
}

function drawHexStroke(g: Graphics, hex: HexPosition, size: number, color: number, width: number) {
  const center = hexToPixel(hex, size)

  g.poly(hexPolygon(center, size * 0.96))
  g.stroke({ color, width })
}
