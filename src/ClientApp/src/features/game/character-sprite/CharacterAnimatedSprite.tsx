import { AnimatedSprite, Texture, Rectangle, ImageSource } from "pixi.js";
import { extend } from "@pixi/react";
import { useGridStore } from "../state/game/grid.state.store";
import { useSpriteTextures } from "@/shared/hooks/useSpriteTextures";
import { useCharacterStore } from "../state/data/character.data.store";
import { useCharacterStateStore } from "../state/game/character.state.store";

extend({ AnimatedSprite, Texture, Rectangle, ImageSource });

export function CharacterAnimatedSprite() {
  const layout = useGridStore(s => s.layout);
  const runtime = useCharacterStateStore(s => s.runtime)
  const spriteAnimation = useCharacterStore(s => s.getSpriteAnimation(runtime?.currentAction));
  const textures = useSpriteTextures(spriteAnimation);

  if (!layout || !runtime || textures.length === 0 || !spriteAnimation)
    return null;

  const cell = layout.cells[runtime.position.y][runtime.position.x]

  return (
    <pixiAnimatedSprite
      ref={(ref) => { ref?.play(); }} // запуск анимации сразу после монтирования, т.к. есть баг на стороне библиотеки
      textures={textures} // массив кадров спрайта
      x={cell.x + cell.width / 2}  // позиция по X (центр спрайта в клетке)
      y={cell.y + cell.height - 10} // позиция по Y (низ спрайта чуть выше нижней границы клетки)
      anchor={{ x: 0.5, y: 1 }} // точка привязки спрайта: центр по горизонтали, низ по вертикали
      scale={{ x: spriteAnimation.scale.x, y: spriteAnimation.scale.y }} // отражение по горизонтали
      animationSpeed={spriteAnimation.animationSpeed}  // скорость анимации (0.1 = кадры меняются медленно)

      autoPlay={true}
      loop={true}
    />
  )
}
