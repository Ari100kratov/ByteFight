import { extend } from "@pixi/react"
import { Container } from "pixi.js"
import { useCharacterStateStore } from "../state/game/character.state.store"
import { useEnemyStateStore } from "../state/game/enemy.state.store"
import { useGridStore } from "../state/game/grid.state.store"
import {
  FloatingCombatTextKind,
  useFloatingCombatTextStore,
} from "../state/ui/floating.combat.text.store"
import { FloatingCombatText } from "./FloatingCombatText"
import { RENDER_LAYERS, snapPixel } from "../rendering/pixiQuality"

extend({ Container })

export function FloatingCombatTextLayer() {
  const layout = useGridStore((s) => s.layout)
  const items = useFloatingCombatTextStore((s) => s.items)
  const remove = useFloatingCombatTextStore((s) => s.remove)

  const characterRuntime = useCharacterStateStore((s) => s.runtime)
  const arenaEnemies = useEnemyStateStore((s) => s.arenaEnemies)

  if (!layout) return null

  const getRuntime = (unitId: string) => {
    if (characterRuntime?.id === unitId) return characterRuntime

    return arenaEnemies[unitId]
  }

  return (
    <pixiContainer zIndex={RENDER_LAYERS.floatingText}>
      {items.map((item) => {
        const runtime = getRuntime(item.unitId)
        if (!runtime) return null

        const cell = layout.cells[runtime.position.y][runtime.position.x]

        const footX = snapPixel(runtime.renderPosition?.x ?? cell.x + cell.width / 2)
        const footY = snapPixel(runtime.renderPosition?.y ?? cell.y + cell.height - 10)
        const rawSpriteHeight =
          runtime.textureHeight && runtime.spriteAnimation
            ? runtime.textureHeight * runtime.spriteAnimation.scale.y
            : cell.height * 1.08
        const visualSpriteHeight = Math.min(
          Math.max(rawSpriteHeight, cell.height * 0.7),
          cell.height * 1.18,
        )
        const sideOffset =
          item.kind === FloatingCombatTextKind.Healing ? -cell.width * 0.34 : cell.width * 0.34

        return (
          <FloatingCombatText
            key={item.id}
            value={item.value}
            kind={item.kind}
            x={snapPixel(footX + sideOffset)}
            y={snapPixel(footY - visualSpriteHeight * 0.42)}
            onComplete={() => {
              remove(item.id)
            }}
          />
        )
      })}
    </pixiContainer>
  )
}
