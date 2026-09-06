import { describe, expect, it } from 'vitest'

import {
  buildPath,
  facingBetween,
  hexDistance,
  hexKey,
  hexNeighbors,
  hexesInRange,
  hexToPixel,
  pixelToHex,
  reachableHexes,
} from './hex'

describe('hex geometry', () => {
  it('соседи чётной колонки дают шесть гексов', () => {
    const neighbors = hexNeighbors({ x: 2, y: 2 }).map(hexKey).sort()

    expect(neighbors).toEqual(['1:1', '1:2', '2:1', '2:3', '3:1', '3:2'])
  })

  it('соседи нечётной колонки сдвинуты вниз', () => {
    const neighbors = hexNeighbors({ x: 1, y: 1 }).map(hexKey).sort()

    expect(neighbors).toEqual(['0:1', '0:2', '1:0', '1:2', '2:1', '2:2'])
  })

  it('в начале координат нет отрицательных соседей', () => {
    expect(hexNeighbors({ x: 0, y: 0 }).map(hexKey)).toEqual(['1:0', '0:1'])
  })

  it('расстояние считается по гексам', () => {
    expect(hexDistance({ x: 0, y: 0 }, { x: 1, y: 0 })).toBe(1)
    expect(hexDistance({ x: 0, y: 0 }, { x: 0, y: 1 })).toBe(1)
    expect(hexDistance({ x: 0, y: 0 }, { x: 4, y: 2 })).toBe(4)
    expect(hexDistance({ x: 3, y: 3 }, { x: 3, y: 3 })).toBe(0)
  })

  it('радиус 2 покрывает 19 гексов включая центр', () => {
    const cells = hexesInRange({ x: 5, y: 5 }, 2)

    expect(cells).toHaveLength(19)
    expect(cells.some((c) => c.x === 5 && c.y === 5)).toBe(true)
  })

  it('пиксельные координаты и обратное преобразование согласованы', () => {
    const hex = { x: 3, y: 2 }

    expect(pixelToHex(hexToPixel(hex, 44), 44)).toEqual(hex)
  })

  it('восемь направлений взгляда', () => {
    // Индексы секторов 0–7: 0 — восток, далее по часовой.
    expect(facingBetween({ x: 0, y: 0 }, { x: 1, y: 0 })).toBe(1) // вправо-вниз
    expect(facingBetween({ x: 0, y: 0 }, { x: 0, y: 1 })).toBe(2) // вниз
    expect(facingBetween({ x: 1, y: 0 }, { x: 0, y: 0 })).toBe(5) // влево-вверх
  })
})

describe('достижимость и пути', () => {
  const options = {
    width: 8,
    height: 8,
    isBlocked: (hex: { x: number; y: number }) => hex.x === 4 && hex.y === 1,
    isOccupied: () => false,
  }

  it('дейкстра считает стоимости с чащей дороже', () => {
    const reachable = reachableHexes({ x: 0, y: 0 }, 3, {
      ...options,
      movementCost: (hex) => (hex.x === 1 && hex.y === 0 ? 2 : 1),
    })

    expect(reachable.get(hexKey({ x: 1, y: 0 }))?.cost).toBe(2)
    expect(reachable.get(hexKey({ x: 2, y: 0 }))?.cost).toBe(3)
    expect(reachable.has(hexKey({ x: 3, y: 0 }))).toBe(false)
  })

  it('путь восстанавливается от старта до цели', () => {
    const reachable = reachableHexes({ x: 0, y: 0 }, 5, options)
    const path = buildPath(reachable, { x: 3, y: 2 })

    expect(path).not.toBeNull()
    expect(path![0]).toEqual({ x: 0, y: 0 })
    expect(path![path!.length - 1]).toEqual({ x: 3, y: 2 })

    for (let i = 1; i < path!.length; i++) {
      expect(hexDistance(path![i - 1], path![i])).toBe(1)
    }
  })

  it('непроходимые гексы не достижимы', () => {
    const reachable = reachableHexes({ x: 3, y: 1 }, 3, {
      ...options,
      isBlocked: () => true,
    })

    expect(reachable.size).toBe(1)
  })
})
