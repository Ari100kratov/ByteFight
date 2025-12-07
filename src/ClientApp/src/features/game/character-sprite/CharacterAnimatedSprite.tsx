import { useCharacterStore } from "../state/data/character.data.store";
import { useCharacterStateStore } from "../state/game/character.state.store";
import { UnitAnimatedSprite } from "../shared/unit-animated-sprite/UnitAnimatedSprite";

export function CharacterAnimatedSprite() {
  const runtime = useCharacterStateStore(s => s.runtime)
  const spriteAnimation = useCharacterStore(s => s.getSpriteAnimation(runtime?.action));

  if (!runtime || !spriteAnimation)
    return null;

  return (
    <UnitAnimatedSprite
      unitRuntume={runtime}
      spriteAnimation={spriteAnimation}
      onSpriteReady={(ref) => { useCharacterStateStore.getState().set({ spriteRef: ref }) }}
    />
  )
}
