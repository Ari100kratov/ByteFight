import { useMemo } from "react"
import { useCharacterStore } from "../state/data/character.data.store"
import { useCharacterStateStore } from "../state/game/character.state.store"
import { UnitAnimatedSprite } from "../shared/unit-animated-sprite/UnitAnimatedSprite"
import { UnitController } from "../units/controller/UnitController"
import { unitRegistry } from "../units/controller/UnitRegistry"
import { CharacterAnimationResolver } from "../units/animation/CharacterAnimationResolver"

export function CharacterAnimatedSprite() {
  const runtime = useCharacterStateStore((s) => s.runtime)
  const fallbackAnimation = useCharacterStore((s) => s.getSpriteAnimation(runtime?.action))

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
    />
  )
}
