import { extend } from "@pixi/react"
import { Container } from "pixi.js"
import { useCharacterStateStore } from "../state/game/character.state.store"
import { useEnemyStateStore } from "../state/game/enemy.state.store"
import { useGridStore } from "../state/game/grid.state.store"
import { useCombatEffectsStore, type CombatVisualEffect } from "../state/ui/combat.effects.store"
import type { UnitRuntime } from "../types/UnitRuntime"
import { CombatEffect } from "./CombatEffect"
import { RENDER_LAYERS, snapPixel } from "../rendering/pixiQuality"

extend({ Container })

interface EffectPoint {
  x: number
  bodyY: number
  groundY: number
}

function getUnitPoint(runtime: UnitRuntime, fallbackCellHeight: number): EffectPoint {
  const spriteHeight =
    runtime.textureHeight && runtime.spriteAnimation
      ? runtime.textureHeight * runtime.spriteAnimation.scale.y
      : fallbackCellHeight * 1.35
  const groundY = runtime.renderPosition?.y ?? 0

  return {
    x: snapPixel(runtime.renderPosition?.x ?? 0),
    bodyY: snapPixel(groundY - spriteHeight * 0.45),
    groundY: snapPixel(groundY),
  }
}

export function CombatEffectsLayer() {
  const layout = useGridStore((s) => s.layout)
  const effects = useCombatEffectsStore((s) => s.effects)
  const remove = useCombatEffectsStore((s) => s.remove)
  const characterRuntime = useCharacterStateStore((s) => s.runtime)
  const arenaEnemies = useEnemyStateStore((s) => s.arenaEnemies)

  if (!layout) return null

  const resolveUnitRuntime = (unitId: string) => {
    if (characterRuntime?.id === unitId) return characterRuntime

    return arenaEnemies[unitId]
  }

  const getEffectPoint = (effect: CombatVisualEffect): EffectPoint | null => {
    if (effect.anchor.type === "cell") {
      const cell = layout.cells[effect.anchor.position.y][effect.anchor.position.x]

      return {
        x: snapPixel(cell.x + cell.width / 2),
        bodyY: snapPixel(cell.y + cell.height * 0.42),
        groundY: snapPixel(cell.y + cell.height * 0.74),
      }
    }

    const runtime = resolveUnitRuntime(effect.anchor.unitId)
    if (!runtime) return null

    const cell = layout.cells[runtime.position.y][runtime.position.x]

    const renderPosition = runtime.renderPosition ?? {
      x: snapPixel(cell.x + cell.width / 2),
      y: snapPixel(cell.y + cell.height - 10),
    }

    return getUnitPoint({ ...runtime, renderPosition }, cell.height)
  }

  return (
    <pixiContainer zIndex={RENDER_LAYERS.effects}>
      {effects.map((effect) => {
        const point = getEffectPoint(effect)
        if (!point) return null

        return (
          <CombatEffect
            key={effect.id}
            kind={effect.kind}
            point={point}
            seed={effect.seed}
            onComplete={() => {
              remove(effect.id)
            }}
          />
        )
      })}
    </pixiContainer>
  )
}
