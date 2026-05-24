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
import { RENDER_LAYERS, snapPixel } from "../rendering/pixiQuality"

extend({ Container, Graphics })

type HighlightSide = "ally" | "enemy"

const HIGHLIGHT_WIDTH_RATIO = 0.38
const HIGHLIGHT_HEIGHT_RATIO = 0.1

interface HighlightTarget {
  unitId: string
  runtime: UnitRuntime
  isSelected: boolean
  side: HighlightSide
}

function getHighlightTone(side: HighlightSide, isSelected: boolean) {
  if (isSelected) {
    return {
      tone: 0xfacc15,
      fillTone: 0xfbbf24,
    }
  }

  return side === "ally"
    ? {
        tone: 0x7dd3fc,
        fillTone: 0x38bdf8,
      }
    : {
        tone: 0xfb7185,
        fillTone: 0xf43f5e,
      }
}

function getHighlightStyle(side: HighlightSide, isSelected: boolean) {
  if (isSelected) {
    return {
      intensity: 0.95,
      fillAlpha: 0.055,
      strokeAlphaBase: 0.28,
      strokeAlphaPulse: 0.16,
      strokeWidth: 2,
    }
  }

  return side === "enemy"
    ? {
        intensity: 0.7,
        fillAlpha: 0.085,
        strokeAlphaBase: 0.56,
        strokeAlphaPulse: 0.24,
        strokeWidth: 2,
      }
    : {
        intensity: 0.62,
        fillAlpha: 0.055,
        strokeAlphaBase: 0.28,
        strokeAlphaPulse: 0.16,
        strokeWidth: 1.4,
      }
}

function UnitGroundHighlight({ runtime, isSelected, side }: Omit<HighlightTarget, "unitId">) {
  const layout = useGridStore((s) => s.layout)
  const [pulse, setPulse] = useState(0)

  useTick((ticker) => {
    setPulse((value) => (value + ticker.deltaMS * 0.004) % (Math.PI * 2))
  })

  if (!layout) return null

  const cell = layout.cells[runtime.position.y][runtime.position.x]
  const x = snapPixel(runtime.renderPosition?.x ?? cell.x + cell.width / 2)
  const y = snapPixel(runtime.renderPosition?.y ?? cell.y + cell.height - 10)
  const pulseAlpha = (Math.sin(pulse) + 1) / 2
  const { tone, fillTone } = getHighlightTone(side, isSelected)
  const style = getHighlightStyle(side, isSelected)
  const width = cell.width * HIGHLIGHT_WIDTH_RATIO
  const height = cell.width * HIGHLIGHT_HEIGHT_RATIO
  const centerY = y - 3

  return (
    <pixiGraphics
      zIndex={centerY - 1000}
      draw={(g) => {
        g.clear()
        g.ellipse(x, centerY, width * 0.82, height * 0.78).fill({
          color: fillTone,
          alpha: style.fillAlpha * style.intensity,
        })
        g.setStrokeStyle({
          width: style.strokeWidth,
          color: tone,
          alpha: (style.strokeAlphaBase + pulseAlpha * style.strokeAlphaPulse) * style.intensity,
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
      side: "ally",
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
      side: "enemy",
    })
  }

  return (
    <pixiContainer sortableChildren={true} zIndex={RENDER_LAYERS.unitHighlights}>
      {[...targets.values()].map((target) => (
        <UnitGroundHighlight
          key={target.unitId}
          runtime={target.runtime}
          isSelected={target.isSelected}
          side={target.side}
        />
      ))}
    </pixiContainer>
  )
}
