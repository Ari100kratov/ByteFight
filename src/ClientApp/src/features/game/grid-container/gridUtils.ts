import type { Position } from "../types/common"

export interface GridSize { width: number; height: number }
export interface CanvasSize { width: number; height: number }

export interface GridCell {
  x: number // абсолютные пиксели (левый верхний угол ячейки)
  y: number // абсолютные пиксели (левый верхний угол ячейки)
  width: number
  height: number
  gridX: number // логическая колонка (0..width-1), слева направо
  gridY: number // логическая строка (0..height-1), 0 = снизу
}

export interface GridLayout {
  gridSize: GridSize
  cellSize: number
  gridPixelWidth: number
  gridPixelHeight: number
  offsetX: number
  offsetY: number
  cells: GridCell[][]
}

const MIN_CELL_SIZE = 64

export function calculateGridLayout(gridSize: GridSize, canvasSize: CanvasSize): GridLayout {
  const rawCellSize = Math.min(
    canvasSize.width / gridSize.width,
    canvasSize.height / gridSize.height,
  )
  const cellSize = Math.max(MIN_CELL_SIZE, Math.floor(rawCellSize))

  const gridPixelWidth = gridSize.width * cellSize
  const gridPixelHeight = gridSize.height * cellSize

  const offsetX = 0
  const offsetY = 0

  const cells: GridCell[][] = Array.from({ length: gridSize.height }, (_, rowIndex) => {
    const gridY = rowIndex
    return Array.from({ length: gridSize.width }, (_, col) => {
      const gridX = col
      const x = offsetX + gridX * cellSize
      const y = offsetY + (gridSize.height - 1 - gridY) * cellSize
      return {
        x,
        y,
        width: cellSize,
        height: cellSize,
        gridX,
        gridY,
      }
    })
  })

  return { gridSize, cellSize, gridPixelWidth, gridPixelHeight, offsetX, offsetY, cells }
}

const FOOT_OFFSET_Y = 10

export function gridToPixel(pos: Position, layout: GridLayout) {
  const cell = layout.cells[pos.y][pos.x]

  return {
    x: cell.x + cell.width / 2,
    y: cell.y + cell.height - FOOT_OFFSET_Y,
  }
}
