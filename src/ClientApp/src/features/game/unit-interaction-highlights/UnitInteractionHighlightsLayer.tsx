import { extend, useTick } from "@pixi/react"
import { Container, Graphics } from "pixi.js"
import { useState } from "react"
import { useCharacterStateStore } from "../state/game/character.state.store"
import { useEnemyStateStore } from "../state/game/enemy.state.store"
import { useGridStore } from "../state/game/grid.state.store"
import { useCharacterStore } from "../state/data/character.data.store"
import { useCharacterSelectionStore } from "../state/ui/character.selection.store"
import { useEnemySelectionStore } from "../state/ui/enemy.selection.store"
import { useUnitHoverStore } from "../state/ui/unit.hover.store"
import type { UnitRuntime } from "../types/UnitRuntime"

extend({ Container, Graphics })

interface HighlightTarget {
  unitId: string
  runtime: UnitRuntime
  isSelected: boolean
}

function UnitGroundHighlight({ runtime, isSelected }: Omit<HighlightTarget, "unitId">) {
  const layout = useGridStore((s) => s.layout)
  const [pulse, setPulse] = useState(0)

  useTick((ticker) => {
    setPulse((value) => (value + ticker.deltaMS * 0.004) % (Math.PI * 2))
  })

  if (!layout) return null

  const cell = layout.cells[runtime.position.y][runtime.position.x]
  const x = runtime.renderPosition?.x ?? cell.x + cell.width / 2
  const y = runtime.renderPosition?.y ?? cell.y + cell.height - 10
  const pulseAlpha = (Math.sin(pulse) + 1) / 2
  const tone = isSelected ? 0xfacc15 : 0x7dd3fc
  const fillTone = isSelected ? 0xfbbf24 : 0x38bdf8
  const intensity = isSelected ? 0.95 : 0.62
  const width = cell.width * 0.34
  const height = cell.width * 0.09
  const centerY = y - 3

  return (
    <pixiGraphics
      zIndex={centerY - 1000}
      draw={(g) => {
        g.clear()
        g.ellipse(x, centerY, width * 0.82, height * 0.78).fill({
          color: fillTone,
          alpha: 0.055 * intensity,
        })
        g.setStrokeStyle({
          width: isSelected ? 2 : 1.4,
          color: tone,
          alpha: (0.28 + pulseAlpha * 0.16) * intensity,
        })
        g.ellipse(x, centerY, width, height)
        g.stroke()
      }}
    />
  )
}

export function UnitInteractionHighlightsLayer() {
  const hoveredUnitId = useUnitHoverStore((s) => s.hoveredUnitId)
  const character = useCharacterStore((s) => s.character)
  const selectedCharacterId = useCharacterSelectionStore((s) => s.selectedCharacterId)
  const selectedArenaEnemyId = useEnemySelectionStore((s) => s.selectedArenaEnemyId)
  const characterRuntime = useCharacterStateStore((s) => s.runtime)
  const arenaEnemies = useEnemyStateStore((s) => s.arenaEnemies)

  const targets = new Map<string, HighlightTarget>()

  if (
    characterRuntime &&
    (hoveredUnitId === characterRuntime.id || selectedCharacterId === character?.id)
  ) {
    targets.set(characterRuntime.id, {
      unitId: characterRuntime.id,
      runtime: characterRuntime,
      isSelected: selectedCharacterId === character?.id,
    })
  }

  for (const runtime of Object.values(arenaEnemies)) {
    if (!runtime) continue

    const isHovered = hoveredUnitId === runtime.id
    const isSelected = selectedArenaEnemyId === runtime.id
    if (!isHovered && !isSelected) continue

    targets.set(runtime.id, {
      unitId: runtime.id,
      runtime,
      isSelected,
    })
  }

  return (
    <pixiContainer sortableChildren={true} zIndex={-1000}>
      {[...targets.values()].map((target) => (
        <UnitGroundHighlight
          key={target.unitId}
          runtime={target.runtime}
          isSelected={target.isSelected}
        />
      ))}
    </pixiContainer>
  )
}
