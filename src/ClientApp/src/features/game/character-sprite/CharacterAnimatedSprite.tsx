import { useCharacterStore } from "../state/data/character.data.store";
import { useCharacterStateStore } from "../state/game/character.state.store";
import { UnitAnimatedSprite } from "../shared/unit-animated-sprite/UnitAnimatedSprite";

export function CharacterAnimatedSprite() {
  const runtime = useCharacterStateStore(s => s.runtime)
  const spriteAnimation = useCharacterStore(s => s.getSpriteAnimation(runtime?.currentAction));

  if (!runtime || !spriteAnimation)
    return null;

  return (
    <UnitAnimatedSprite
      unitRuntume={runtime}
      spriteAnimation={spriteAnimation}
    />
  )
}
