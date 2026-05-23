import { useMemo } from "react"
import { useArenaEnemiesStore } from "../state/data/arena-enemies.store"
import { useEnemiesStore } from "../state/data/enemies.data.store"
import { useEnemyStateStore } from "../state/game/enemy.state.store"
import { UnitAnimatedSprite } from "../shared/unit-animated-sprite/UnitAnimatedSprite"
import { UnitController } from "../units/controller/UnitController"
import { unitRegistry } from "../units/controller/UnitRegistry"
import { EnemyAnimationResolver } from "../units/animation/EnemyAnimationResolver"
import { useArenaItemSelectionStore } from "../state/ui/arena-item.selection.store"
import { useCharacterSelectionStore } from "../state/ui/character.selection.store"
import { useEnemySelectionStore } from "../state/ui/enemy.selection.store"

interface Props {
  arenaEnemyId: string
}

export function EnemyAnimatedSprite({ arenaEnemyId }: Props) {
  const arenaEnemy = useArenaEnemiesStore((s) =>
    arenaEnemyId ? s.arenaEnemies[arenaEnemyId] : undefined,
  )
  const runtime = useEnemyStateStore((s) =>
    arenaEnemyId ? s.arenaEnemies[arenaEnemyId] : undefined,
  )
  const selectEnemy = useEnemySelectionStore((s) => s.selectEnemy)
  const clearSelection = useEnemySelectionStore((s) => s.clearSelection)
  const selectedArenaEnemyId = useEnemySelectionStore((s) => s.selectedArenaEnemyId)

  const fallbackAnimation = useEnemiesStore((s) =>
    s.getSpriteAnimation(arenaEnemy?.enemyId, runtime?.action),
  )

  const spriteAnimation = runtime?.spriteAnimation ?? fallbackAnimation
  const resolvedArenaEnemyId = arenaEnemy?.id
  const enemyId = arenaEnemy?.enemyId

  const controller = useMemo(() => {
    if (!resolvedArenaEnemyId || !enemyId) return null

    const controller = new UnitController(
      (partial) => {
        useEnemyStateStore.getState().set(resolvedArenaEnemyId, partial)
      },
      new EnemyAnimationResolver(enemyId, useEnemiesStore.getState),
    )

    unitRegistry.bind(resolvedArenaEnemyId, controller)
    return controller
  }, [enemyId, resolvedArenaEnemyId])

  if (!runtime || !spriteAnimation || !controller) return null

  return (
    <UnitAnimatedSprite
      runtime={runtime}
      spriteAnimation={spriteAnimation}
      controller={controller}
      clickable
      selected={selectedArenaEnemyId === arenaEnemyId}
      onClick={(position) => {
        if (useEnemySelectionStore.getState().selectedArenaEnemyId === arenaEnemyId) {
          clearSelection()
          return
        }

        useCharacterSelectionStore.getState().clearSelection()
        useArenaItemSelectionStore.getState().clearSelection()
        selectEnemy(arenaEnemyId, position)
      }}
    />
  )
}
