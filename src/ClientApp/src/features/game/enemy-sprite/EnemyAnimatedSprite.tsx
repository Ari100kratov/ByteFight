import { useArenaEnemiesStore } from "../state/data/arena-enemies.store";
import { useEnemiesStore } from "../state/data/enemies.data.store";
import { useEnemyStateStore } from "../state/game/enemy.state.store";
import { UnitAnimatedSprite } from "../shared/unit-animated-sprite/UnitAnimatedSprite";

type Props = {
  arenaEnemyId: string;
};

export function EnemyAnimatedSprite({ arenaEnemyId }: Props) {
  const arenaEnemy = useArenaEnemiesStore(s => s.getArenaEnemy(arenaEnemyId));
  const runtime = useEnemyStateStore(s => s.get(arenaEnemy?.id))
  const spriteAnimation = useEnemiesStore(s => s.getSpriteAnimation(arenaEnemy?.enemyId, runtime?.action));

  if (!runtime || !spriteAnimation)
    return null;

  return (
    <UnitAnimatedSprite
      unitRuntume={runtime}
      spriteAnimation={spriteAnimation}
      onSpriteReady={(ref) => { useEnemyStateStore.getState().set(arenaEnemyId, { spriteRef: ref }) }}
    />
  )
}
