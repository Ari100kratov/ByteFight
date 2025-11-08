import { AnimatedSprite } from "pixi.js";
import { extend } from "@pixi/react";
import { useGridStore } from "../../state/game/grid.state.store";
import type { UnitRuntime } from "../../state/game/types/unit.runtime";
import type { SpriteAnimationDto } from "@/shared/types/spriteAnimation";
import { useSpriteTextures } from "@/shared/hooks/useSpriteTextures";
import { UnitBars } from "./UnitBars";

extend({ AnimatedSprite });

interface Props {
  unitRuntume: UnitRuntime,
  spriteAnimation: SpriteAnimationDto
}

export function UnitAnimatedSprite({ unitRuntume, spriteAnimation }: Props) {
  const textures = useSpriteTextures(spriteAnimation);
  const layout = useGridStore(s => s.layout);
  if (!layout || textures.length === 0)
    return null

  const cell = layout.cells[unitRuntume.position.y][unitRuntume.position.x]
  const scaleX = unitRuntume.facing === 'left' ? -spriteAnimation.scale.x : spriteAnimation.scale.x
  const spriteX = cell.x + cell.width / 2;
  const spriteY = cell.y + cell.height - 10;

  const spriteHeight = textures[0].height * spriteAnimation.scale.y;

  return (
    <>
      <UnitBars
        unit={unitRuntume}
        x={spriteX}
        y={spriteY}
        spriteHeight={spriteHeight}
        cellWidth={cell.width}
      />
      <pixiAnimatedSprite
        ref={(ref) => { ref?.play(); }} // запуск анимации сразу после монтирования, т.к. есть баг на стороне библиотеки
        textures={textures} // массив кадров спрайта
        x={spriteX}  // позиция по X (центр спрайта в клетке)
        y={spriteY} // позиция по Y (низ спрайта чуть выше нижней границы клетки)
        anchor={{ x: 0.5, y: 1 }} // точка привязки спрайта: центр по горизонтали, низ по вертикали
        scale={{ x: scaleX, y: spriteAnimation.scale.y }} // отражение по горизонтали
        animationSpeed={spriteAnimation.animationSpeed}  // скорость анимации (0.1 = кадры меняются медленно)

        autoPlay={true}
        loop={true}
      />
    </>
  )
}