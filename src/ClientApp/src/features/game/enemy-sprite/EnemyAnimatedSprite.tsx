import { useMemo } from "react";
import { useArenaEnemiesStore } from "../state/data/arena-enemies.store";
import { useEnemiesStore } from "../state/data/enemies.data.store";
import { useEnemyStateStore } from "../state/game/enemy.state.store";
import { UnitAnimatedSprite } from "../shared/unit-animated-sprite/UnitAnimatedSprite";
import { UnitController } from "../units/controller/UnitController";
import { unitRegistry } from "../units/controller/UnitRegistry";
import { EnemyAnimationResolver } from "../units/animation/EnemyAnimationResolver";

type Props = {
  arenaEnemyId: string;
};

export function EnemyAnimatedSprite({ arenaEnemyId }: Props) {
  const arenaEnemy = useArenaEnemiesStore(s => s.getArenaEnemy(arenaEnemyId));
  const runtime = useEnemyStateStore(s => s.get(arenaEnemy?.id));
  const spriteAnimation = useEnemiesStore(s => s.getSpriteAnimation(arenaEnemy?.enemyId, runtime?.action));

  const controller = useMemo(() => {
    if (!arenaEnemy) return null;

    const controller = new UnitController(
      partial => useEnemyStateStore.getState().set(arenaEnemy.id, partial),
      new EnemyAnimationResolver(arenaEnemy.enemyId, useEnemiesStore.getState)
    );

    unitRegistry.bind(arenaEnemy.id, controller);
    return controller;
  }, [arenaEnemy?.id]);

  if (!runtime || !spriteAnimation || !controller)
    return null;

  return (
    <UnitAnimatedSprite
      runtime={runtime}
      spriteAnimation={spriteAnimation}
      controller={controller}
    />
  );
}
