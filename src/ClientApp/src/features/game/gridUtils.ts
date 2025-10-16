export type GridSize = { width: number; height: number }
export type CanvasSize = { width: number; height: number }

export type GridLayout = {
  gridSize: GridSize
  cellSize: number
  gridPixelWidth: number
  gridPixelHeight: number
  offsetX: number
  offsetY: number
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

  return { gridSize, cellSize, gridPixelWidth, gridPixelHeight, offsetX, offsetY }
}
