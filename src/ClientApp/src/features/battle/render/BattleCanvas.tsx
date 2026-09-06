import { Application, extend } from '@pixi/react'
import { Container } from 'pixi.js'
import { useCallback, useEffect, useMemo, useRef, useState } from 'react'

import { HexBoard } from './HexBoard'
import { UnitTokens } from './UnitTokens'
import { EffectsLayer } from './EffectsLayer'
import { COLORS } from './battleColors'
import {
  buildPath,
  hexKey,
  hexesInRange,
  hexToPixel,
  reachableHexes,
  type HexPosition,
} from '../hex'
import { battleHub } from '../api/battleHub'
import { occupiedHexes, useBattleStore } from '../state/battle.store'
import { TerrainType, type BattleAbilityDto } from '../types'

extend({ Container })

interface BattleCanvasProps {
  sessionId: string
}

const HEX_SIZE = 44
const CAMERA_MARGIN = 70

/**
 * Игровое поле: Pixi-сцена с гексагональной доской, юнитами,
 * эффектами и обработкой указателя (перемещение, прицеливание, выбор).
 */
export function BattleCanvas({ sessionId }: BattleCanvasProps) {
  const state = useBattleStore((s) => s.state)
  const isPlayerTurn = useBattleStore((s) => s.isPlayerTurn())
  const playerUnit = useBattleStore((s) => s.playerUnit())
  const targeting = useBattleStore((s) => s.targeting)
  const previewPath = useBattleStore((s) => s.previewPath)
  const beginTargeting = useBattleStore((s) => s.beginTargeting)
  const setPreviewPath = useBattleStore((s) => s.setPreviewPath)
  const selectUnit = useBattleStore((s) => s.selectUnit)

  const [hoveredHex, setHoveredHex] = useState<HexPosition | null>(null)
  const [viewSize, setViewSize] = useState({ width: 800, height: 600 })
  const containerRef = useRef<HTMLDivElement>(null)

  useEffect(() => {
    const element = containerRef.current
    if (!element) return

    const observer = new ResizeObserver((entries) => {
      const entry = entries.at(0)
      if (entry) {
        setViewSize({ width: entry.contentRect.width, height: entry.contentRect.height })
      }
    })

    observer.observe(element)

    return () => { observer.disconnect(); }
  }, [])

  const terrain = useMemo(() => {
    const map = new Map<string, number>()

    for (const cell of state?.arena.terrain ?? []) {
      map.set(hexKey(cell), cell.terrain)
    }

    return map
  }, [state?.arena.terrain])

  const blocked = useMemo(() => {
    const set = new Set<string>()

    for (const cell of state?.arena.blocked ?? []) {
      set.add(hexKey(cell))
    }

    for (const [key, type] of terrain) {
      if (type === TerrainType.Rock || type === TerrainType.Water) {
        set.add(key)
      }
    }

    return set
  }, [state?.arena.blocked, terrain])

  const occupied = useMemo(
    () => (state ? occupiedHexes(state) : new Set<string>()),
    [state],
  )

  const movementCost = useCallback(
    (hex: HexPosition): number => {
      const type = terrain.get(hexKey(hex)) ?? TerrainType.Meadow

      return type === TerrainType.Forest || type === TerrainType.Swamp ? 2 : 1
    },
    [terrain],
  )

  const reachable = useMemo(() => {
    if (!state || !playerUnit || !isPlayerTurn || targeting || playerUnit.movePoints <= 0) {
      return new Map<string, { hex: HexPosition; cost: number; from: string | null }>()
    }

    return reachableHexes(playerUnit.position, playerUnit.movePoints, {
      width: state.arena.width,
      height: state.arena.height,
      isBlocked: (hex) => blocked.has(hexKey(hex)),
      isOccupied: (hex) => occupied.has(hexKey(hex)) && !hexOf(hex, playerUnit.position),
      movementCost,
    })
  }, [state, playerUnit, isPlayerTurn, targeting, blocked, occupied, movementCost])

  const reachableKeys = useMemo(() => new Set(reachable.keys()), [reachable])

  const targetHexes = useMemo(() => {
    if (!targeting || !playerUnit || !state) return new Set<string>()

    return computeTargetHexes(targeting.ability, playerUnit, state.units, {
      width: state.arena.width,
      height: state.arena.height,
    })
  }, [targeting, playerUnit, state])

  // Панорамирование: центр поля под размер экрана.
  const boardSize = useMemo(() => {
    const width = state?.arena.width ?? 0
    const height = state?.arena.height ?? 0
    const right = hexToPixel({ x: width - 1, y: 0 }, HEX_SIZE).x + HEX_SIZE
    const bottom = hexToPixel({ x: width % 2 === 0 ? width - 1 : width - 1, y: height - 1 }, HEX_SIZE).y + HEX_SIZE

    return { width: right + CAMERA_MARGIN, height: bottom + CAMERA_MARGIN }
  }, [state?.arena.width, state?.arena.height])

  const scale = useMemo(() => {
    const fitX = viewSize.width / Math.max(1, boardSize.width)
    const fitY = viewSize.height / Math.max(1, boardSize.height)

    return Math.min(1.4, Math.max(0.35, Math.min(fitX, fitY)))
  }, [viewSize, boardSize])

  const offsetX = Math.max(0, (viewSize.width - boardSize.width * scale) / 2)
  const offsetY = Math.max(0, (viewSize.height - boardSize.height * scale) / 2)

  const previewKeys = useMemo(() => {
    if (!previewPath) return []

    return previewPath.map(hexKey)
  }, [previewPath])

  const hexAtPointer = useCallback(
    (event: { global: { x: number; y: number } }): HexPosition | null => {
      if (!state) return null

      const localX = (event.global.x - offsetX) / scale
      const localY = (event.global.y - offsetY) / scale

      // Поиск ближайшего центра гекса.
      let best: HexPosition | null = null
      let bestDistance = Infinity

      for (let x = 0; x < state.arena.width; x++) {
        for (let y = 0; y < state.arena.height; y++) {
          const center = hexToPixel({ x, y }, HEX_SIZE)
          const distance = Math.hypot(center.x - localX, center.y - localY)

          if (distance < bestDistance) {
            bestDistance = distance
            best = { x, y }
          }
        }
      }

      return best && bestDistance <= HEX_SIZE ? best : null
    },
    [state, offsetX, offsetY, scale],
  )

  const onPointerMove = useCallback(
    (event: { global: { x: number; y: number } }) => {
      const hex = hexAtPointer(event)
      setHoveredHex(hex)

      // Предпросмотр пути при ходе игрока.
      if (hex && isPlayerTurn && !targeting && playerUnit && reachable.has(hexKey(hex))) {
        const path = buildPath(reachable, hex)
        setPreviewPath(path)
      } else if (!hex && previewPath) {
        setPreviewPath(null)
      }
    },
    [hexAtPointer, isPlayerTurn, targeting, playerUnit, reachable, setPreviewPath, previewPath],
  )

  const onPointerDown = useCallback(
    (event: { global: { x: number; y: number } }) => {
      const hex = hexAtPointer(event)
      if (!hex || !state) return

      const hexStr = hexKey(hex)
      const occupant = state.units.find((u) => !u.isDead && u.position.x === hex.x && u.position.y === hex.y)

      // Прицеливание способностью.
      if (targeting && playerUnit) {
        if (targetHexes.has(hexStr)) {
          void battleHub.submitAbility(sessionId, targeting.ability.type, hex)
        }

        beginTargeting(null)

        return
      }

      // Выбор юнита инспектором.
      if (occupant) {
        selectUnit(occupant.id)
      }

      // Перемещение в свой ход.
      if (isPlayerTurn && playerUnit && reachable.has(hexStr) && !occupant) {
        const path = buildPath(reachable, hex)

        if (path && path.length > 1) {
          void battleHub.submitMove(sessionId, path)
          setPreviewPath(null)
        }
      }
    },
    [hexAtPointer, state, targeting, playerUnit, targetHexes, beginTargeting, selectUnit, isPlayerTurn, reachable, sessionId, setPreviewPath],
  )

  if (!state) {
    return <div ref={containerRef} className="h-full w-full animate-pulse rounded-xl bg-[#11180f]" />
  }

  return (
    <div ref={containerRef} className="h-full w-full overflow-hidden rounded-xl border border-[#2c3a24]">
      <Application
        backgroundColor={COLORS.canvasBg}
        antialias
        autoDensity
        resolution={Math.min(2, window.devicePixelRatio || 1)}
      >
        <pixiContainer
          x={offsetX}
          y={offsetY}
          scale={scale}
          eventMode="static"
          hitArea={{
            contains: () => true,
          }}
          onPointerMove={onPointerMove}
          onPointerDown={onPointerDown}
        >
          <HexBoard
            width={state.arena.width}
            height={state.arena.height}
            hexSize={HEX_SIZE}
            terrain={terrain}
            blocked={blocked}
            reachable={isPlayerTurn && !targeting ? reachableKeys : new Set<string>()}
            previewPath={previewKeys}
            targetHexes={targetHexes}
            hoveredHex={hoveredHex}
          />

          <UnitTokens hexSize={HEX_SIZE} />
          <EffectsLayer hexSize={HEX_SIZE} />
        </pixiContainer>
      </Application>
    </div>
  )
}

function hexOf(a: HexPosition, b: HexPosition): boolean {
  return a.x === b.x && a.y === b.y
}

/**
 * Вычисляет допустимые гексы применения способности (зеркалит сервер):
 * одиночные — по юнитам соответствующей стороны, область — по любому гексу.
 */
export function computeTargetHexes(
  ability: BattleAbilityDto,
  actor: { position: HexPosition; isPlayerSide: boolean },
  units: { id: string; isPlayerSide: boolean; isDead: boolean; position: HexPosition }[],
  arena: { width: number; height: number },
): Set<string> {
  const result = new Set<string>()

  for (const hex of hexesInRange(actor.position, ability.range)) {
    if (hex.x >= arena.width || hex.y >= arena.height) continue

    const occupant = units.find((u) => !u.isDead && u.position.x === hex.x && u.position.y === hex.y)

    switch (ability.targetType) {
      // Противник.
      case 1:
        if (occupant && occupant.isPlayerSide !== actor.isPlayerSide) {
          result.add(hexKey(hex))
        }
        break

      // Себя.
      case 2:
        if (hex.x === actor.position.x && hex.y === actor.position.y) {
          result.add(hexKey(hex))
        }
        break

      // Союзник (включая себя).
      case 3:
        if (occupant?.isPlayerSide === actor.isPlayerSide) {
          result.add(hexKey(hex))
        }
        break

      // Область: любой гекс в пределах дальности.
      default:
        result.add(hexKey(hex))
        break
    }
  }

  return result
}
