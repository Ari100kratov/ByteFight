import { AnimatedSprite, Texture } from "pixi.js";
import { extend } from "@pixi/react";
import { useGridStore } from "../../state/game/grid.state.store";
import type { UnitRuntime } from "../../types/UnitRuntime";
import type { SpriteAnimationDto } from "@/shared/types/spriteAnimation";
import { UnitBars } from "./UnitBars";
import { FacingDirection } from "../../types/common";
import { useRef } from "react";
import { UnitController } from "../../units/controller/UnitController";

extend({ AnimatedSprite });

interface Props {
  runtime: UnitRuntime;
  spriteAnimation: SpriteAnimationDto;
  controller: UnitController;
}

export function UnitAnimatedSprite({
  runtime,
  spriteAnimation,
  controller,
}: Props) {
  const layout = useGridStore(s => s.layout);
  const spriteRef = useRef<AnimatedSprite | null>(null);

  if (!layout)
    return null;

  const cell = layout.cells[runtime.position.y][runtime.position.x];

  const spriteX =
    runtime.renderPosition?.x ??
    (cell.x + cell.width / 2);

  const spriteY =
    runtime.renderPosition?.y ??
    (cell.y + cell.height - 10);

  const scaleX =
    runtime.facing === FacingDirection.Left
      ? -spriteAnimation.scale.x
      : spriteAnimation.scale.x;

  const spriteHeight = (runtime.textureHeight ?? 0) * spriteAnimation.scale.y;

  const handleRef = (sprite: AnimatedSprite | null) => {
    if (!sprite) return;

    spriteRef.current = sprite;
    controller.sprite.attach(sprite);
    controller.notifyViewReady();
    sprite.play(); // ???
  };

  return (
    <>
      <UnitBars
        runtime={runtime}
        x={spriteX}
        y={spriteY}
        spriteHeight={spriteHeight}
        cellWidth={cell.width}
      />
      <pixiAnimatedSprite
        ref={handleRef}
        textures={spriteRef.current?.textures ?? [Texture.WHITE]}
        x={spriteX}
        y={spriteY}
        anchor={{ x: 0.5, y: 1 }}
        scale={{ x: scaleX, y: spriteAnimation.scale.y }}
        autoPlay={false}
      />
    </>
  );
}
