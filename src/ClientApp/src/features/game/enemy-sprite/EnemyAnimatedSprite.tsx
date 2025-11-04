import { AnimatedSprite, Texture, Rectangle, ImageSource } from "pixi.js";
import { extend } from "@pixi/react";
import { useArenaEnemiesStore } from "../state/data/arena-enemies.store";
import { useGridStore } from "../state/game/grid.state.store";
import { useEnemiesStore } from "../state/data/enemies.data.store";
import { useSpriteTextures } from "@/shared/hooks/useSpriteTextures";
import { useEnemyStateStore } from "../state/game/enemy.state.store";

extend({ AnimatedSprite, Texture, Rectangle, ImageSource });

type Props = {
  arenaEnemyId: string;
};

export function EnemyAnimatedSprite({ arenaEnemyId }: Props) {
  const layout = useGridStore(s => s.layout);
  const arenaEnemy = useArenaEnemiesStore(s => s.getArenaEnemy(arenaEnemyId));
  const enemyState = useEnemyStateStore(s => s.get(arenaEnemy?.id))
  const spriteAnimation = useEnemiesStore(s => s.getSpriteAnimation(arenaEnemy?.enemyId, enemyState?.currentAction));
  const textures = useSpriteTextures(spriteAnimation);

  if (!layout || !arenaEnemy || textures.length === 0 || !spriteAnimation)
    return null;

  const cell = layout.cells[arenaEnemy.position.y][arenaEnemy.position.x]

  return (
    <pixiAnimatedSprite
      ref={(ref) => { ref?.play(); }} // запуск анимации сразу после монтирования, т.к. есть баг на стороне библиотеки
      textures={textures} // массив кадров спрайта
      x={cell.x + cell.width / 2}  // позиция по X (центр спрайта в клетке)
      y={cell.y + cell.height - 10} // позиция по Y (низ спрайта чуть выше нижней границы клетки)
      anchor={{ x: 0.5, y: 1 }} // точка привязки спрайта: центр по горизонтали, низ по вертикали
      scale={{ x: -spriteAnimation.scale.x, y: spriteAnimation.scale.y }} // отражение по горизонтали
      animationSpeed={spriteAnimation.animationSpeed}  // скорость анимации (0.1 = кадры меняются медленно)

      autoPlay={true}
      loop={true}
    />
  )
}
