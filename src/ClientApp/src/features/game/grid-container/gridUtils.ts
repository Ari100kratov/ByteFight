export type GridSize = { width: number; height: number }
export type CanvasSize = { width: number; height: number }

export type GridCell = {
  x: number        // абсолютные пиксели (левый верхний угол ячейки)
  y: number        // абсолютные пиксели (левый верхний угол ячейки)
  width: number
  height: number
  gridX: number    // логическая колонка (0..width-1), слева направо
  gridY: number    // логическая строка (0..height-1), 0 = снизу
}

export type GridLayout = {
  gridSize: GridSize
  cellSize: number
  gridPixelWidth: number
  gridPixelHeight: number
  offsetX: number
  offsetY: number
  cells: GridCell[][]
}

export function calculateGridLayout(gridSize: GridSize, canvasSize: CanvasSize): GridLayout {
  const cellSize = Math.min(
    canvasSize.width / gridSize.width,
    canvasSize.height / gridSize.height
  )

  const gridPixelWidth = gridSize.width * cellSize
  const gridPixelHeight = gridSize.height * cellSize

  const offsetX = (canvasSize.width - gridPixelWidth) / 2
  const offsetY = (canvasSize.height - gridPixelHeight) / 2

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
