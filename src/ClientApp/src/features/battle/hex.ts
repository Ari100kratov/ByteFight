/**
 * Гексагональная геометрия с плоской вершиной (flat-top), смещение odd-q:
 * X — колонка, Y — строка, нечётные колонки сдвинуты на полгекса вниз.
 * Зеркалит серверную Domain.ValueObjects.HexGeometry.
 */

export interface HexPosition {
  x: number
  y: number
}

export interface PixelPoint {
  x: number
  y: number
}

const SQRT3 = Math.sqrt(3)

/** Смещения шести соседей для чётных и нечётных колонок. */
const EVEN_COLUMN_NEIGHBORS: readonly (readonly [number, number])[] = [
  [1, 0],
  [1, -1],
  [0, -1],
  [-1, -1],
  [-1, 0],
  [0, 1],
]

const ODD_COLUMN_NEIGHBORS: readonly (readonly [number, number])[] = [
  [1, 1],
  [1, 0],
  [0, -1],
  [-1, 0],
  [-1, 1],
  [0, 1],
]

export function hexKey(hex: HexPosition): string {
  return `${String(hex.x)}:${String(hex.y)}`
}

export function sameHex(a: HexPosition, b: HexPosition): boolean {
  return a.x === b.x && a.y === b.y
}

/** Шесть соседних гексов. */
export function hexNeighbors(hex: HexPosition): HexPosition[] {
  const offsets = hex.x % 2 === 0 ? EVEN_COLUMN_NEIGHBORS : ODD_COLUMN_NEIGHBORS

  return offsets
    .map(([dx, dy]) => ({ x: hex.x + dx, y: hex.y + dy }))
    .filter((h) => h.x >= 0 && h.y >= 0)
}

/** Расстояние между гексами в шагах. */
export function hexDistance(from: HexPosition, to: HexPosition): number {
  const a = toCube(from)
  const b = toCube(to)

  return (Math.abs(a.x - b.x) + Math.abs(a.y - b.y) + Math.abs(a.z - b.z)) / 2
}

function toCube(hex: HexPosition): { x: number; y: number; z: number } {
  const x = hex.x
  const z = hex.y - (hex.x - (hex.x & 1)) / 2

  return { x, y: -x - z, z }
}

/** Все гексы в радиусе включительно. */
export function hexesInRange(center: HexPosition, radius: number): HexPosition[] {
  const result: HexPosition[] = []
  const centerCube = toCube(center)

  for (let dx = -radius; dx <= radius; dx++) {
    for (let dy = Math.max(-radius, -dx - radius); dy <= Math.min(radius, -dx + radius); dy++) {
      const cube = {
        x: centerCube.x + dx,
        y: centerCube.y + dy,
        z: centerCube.z - dx - dy,
      }

      const hex = cubeToOffsetHex(cube)

      if (hex.x >= 0 && hex.y >= 0) {
        result.push(hex)
      }
    }
  }

  return result
}

/** Центр гекса в пикселях (flat-top, odd-q). */
export function hexToPixel(hex: HexPosition, size: number): PixelPoint {
  return {
    x: size * 1.5 * hex.x,
    y: SQRT3 * size * (hex.y + 0.5 * (hex.x & 1)),
  }
}

/** Вершины плоского гекса относительно центра. */
export function flatTopHexCorners(size: number): number[] {
  const corners: number[] = []

  for (let i = 0; i < 6; i++) {
    const angle = (60 * i) * (Math.PI / 180)
    corners.push(size * Math.cos(angle), size * Math.sin(angle))
  }

  return corners
}

/** Пиксель → гекс (округление до ближайшего). */
export function pixelToHex(point: PixelPoint, size: number): HexPosition {
  const q = ((2 / 3) * point.x) / size
  const r = ((-1 / 3) * point.x + (SQRT3 / 3) * point.y) / size

  return cubeToOffsetHex(cubeRound({ x: q, y: -q - r, z: r }))
}

function cubeRound(cube: { x: number; y: number; z: number }): { x: number; y: number; z: number } {
  let rx = Math.round(cube.x)
  let ry = Math.round(cube.y)
  let rz = Math.round(cube.z)

  const dx = Math.abs(rx - cube.x)
  const dy = Math.abs(ry - cube.y)
  const dz = Math.abs(rz - cube.z)

  if (dx > dy && dx > dz) {
    rx = -ry - rz
  } else if (dy > dz) {
    ry = -rx - rz
  } else {
    rz = -rx - ry
  }

  return { x: rx, y: ry, z: rz }
}

function cubeToOffsetHex(cube: { x: number; y: number; z: number }): HexPosition {
  return {
    x: cube.x,
    y: cube.z + (cube.x - (cube.x & 1)) / 2,
  }
}

/** Направление взгляда (0–7, 0 — вправо, по часовой) из одного гекса на другой. */
export function facingBetween(from: HexPosition, to: HexPosition): number {
  const a = hexToPixel(from, 1)
  const b = hexToPixel(to, 1)

  const degrees = (Math.atan2(b.y - a.y, b.x - a.x) * 180) / Math.PI
  const normalized = ((degrees % 360) + 360) % 360

  return Math.round(normalized / 45) % 8
}

/** Дейкстра: стоимости достижения гексов в пределах очков перемещения. */
export function reachableHexes(
  start: HexPosition,
  movePoints: number,
  options: {
    width: number
    height: number
    isBlocked: (hex: HexPosition) => boolean
    isOccupied: (hex: HexPosition) => boolean
    movementCost?: (hex: HexPosition) => number
  },
): Map<string, { hex: HexPosition; cost: number; from: string | null }> {
  const movementCost = options.movementCost ?? (() => 1)
  const result = new Map<string, { hex: HexPosition; cost: number; from: string | null }>()
  result.set(hexKey(start), { hex: start, cost: 0, from: null })

  const queue: { hex: HexPosition; cost: number }[] = [{ hex: start, cost: 0 }]

  while (queue.length > 0) {
    queue.sort((a, b) => a.cost - b.cost)
    const current = queue.shift()
    if (!current) continue

    const currentKey = hexKey(current.hex)
    if ((result.get(currentKey)?.cost ?? Infinity) < current.cost) continue

    for (const neighbor of hexNeighbors(current.hex)) {
      const key = hexKey(neighbor)

      if (
        neighbor.x >= options.width ||
        neighbor.y >= options.height ||
        options.isBlocked(neighbor) ||
        options.isOccupied(neighbor)
      ) {
        continue
      }

      const stepCost = movementCost(neighbor)
      if (!Number.isFinite(stepCost)) continue

      const newCost = current.cost + stepCost
      if (newCost > movePoints) continue

      const known = result.get(key)
      if (!known || known.cost > newCost) {
        result.set(key, { hex: neighbor, cost: newCost, from: currentKey })
        queue.push({ hex: neighbor, cost: newCost })
      }
    }
  }

  return result
}

/** Восстанавливает путь от гекса к старту по карте достижимости. */
export function buildPath(
  reachable: Map<string, { hex: HexPosition; cost: number; from: string | null }>,
  target: HexPosition,
): HexPosition[] | null {
  const path: HexPosition[] = []
  let cursor: string | null = hexKey(target)

  while (cursor) {
    const node = reachable.get(cursor)
    if (!node) return null

    path.unshift(node.hex)
    cursor = node.from
  }

  return path
}
