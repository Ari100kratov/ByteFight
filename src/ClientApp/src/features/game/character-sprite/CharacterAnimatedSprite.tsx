import { useMemo } from "react"
import { useCharacterStore } from "../state/data/character.data.store"
import { useCharacterStateStore } from "../state/game/character.state.store"
import { UnitAnimatedSprite } from "../shared/unit-animated-sprite/UnitAnimatedSprite"
import { UnitController } from "../units/controller/UnitController"
import { unitRegistry } from "../units/controller/UnitRegistry"
import { CharacterAnimationResolver } from "../units/animation/CharacterAnimationResolver"
import { useArenaItemSelectionStore } from "../state/ui/arena-item.selection.store"
import { useCharacterSelectionStore } from "../state/ui/character.selection.store"
import { useEnemySelectionStore } from "../state/ui/enemy.selection.store"

export function CharacterAnimatedSprite() {
  const character = useCharacterStore((s) => s.character)
  const runtime = useCharacterStateStore((s) => s.runtime)
  const fallbackAnimation = useCharacterStore((s) => s.getSpriteAnimation(runtime?.action))
  const selectedCharacterId = useCharacterSelectionStore((s) => s.selectedCharacterId)
  const selectCharacter = useCharacterSelectionStore((s) => s.selectCharacter)
  const clearCharacterSelection = useCharacterSelectionStore((s) => s.clearSelection)

  const spriteAnimation = runtime?.spriteAnimation ?? fallbackAnimation
  const runtimeId = runtime?.id

  const controller = useMemo(() => {
    if (!runtimeId) return null

    const controller = new UnitController((partial) => {
      useCharacterStateStore.getState().set(partial)
    }, new CharacterAnimationResolver(useCharacterStore.getState))

    unitRegistry.bind(runtimeId, controller)
    return controller
  }, [runtimeId])

  if (!runtime || !spriteAnimation || !controller) return null

  return (
    <UnitAnimatedSprite
      runtime={runtime}
      spriteAnimation={spriteAnimation}
      controller={controller}
      clickable
      selected={selectedCharacterId === character?.id}
      onClick={(position) => {
        if (!character) return

        if (useCharacterSelectionStore.getState().selectedCharacterId === character.id) {
          clearCharacterSelection()
          return
        }

        useEnemySelectionStore.getState().clearSelection()
        useArenaItemSelectionStore.getState().clearSelection()
        selectCharacter(character.id, position)
      }}
    />
  )
}
