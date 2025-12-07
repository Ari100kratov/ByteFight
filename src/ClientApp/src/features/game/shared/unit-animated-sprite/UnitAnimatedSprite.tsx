import { AnimatedSprite } from "pixi.js";
import { extend } from "@pixi/react";
import { useGridStore } from "../../state/game/grid.state.store";
import type { UnitRuntime } from "../../types/UnitRuntime";
import type { SpriteAnimationDto } from "@/shared/types/spriteAnimation";
import { useSpriteTextures } from "@/shared/hooks/useSpriteTextures";
import { UnitBars } from "./UnitBars";
import { FacingDirection } from "../../types/common";
import { useRef, useEffect } from "react";

extend({ AnimatedSprite });

interface Props {
  unitRuntume: UnitRuntime,
  spriteAnimation: SpriteAnimationDto,
  onSpriteReady: (sprite: AnimatedSprite) => void;
}

export function UnitAnimatedSprite({ unitRuntume, spriteAnimation, onSpriteReady }: Props) {
  const spriteRef = useRef<AnimatedSprite | null>(null);

  const textures = useSpriteTextures(spriteAnimation);
  const layout = useGridStore(s => s.layout);

  useEffect(() => {
    const s = spriteRef.current;
    if (!s) return;

    s.textures = textures;

    try {
      s.loop = true;
      s.animationSpeed = spriteAnimation.animationSpeed;
      s.play();
    } catch { }

  }, [textures, spriteAnimation.animationSpeed]);

  if (!layout || textures.length === 0)
    return null;

  const cell = layout.cells[unitRuntume.position.y][unitRuntume.position.x];

  const spriteX =
    unitRuntume.renderPosition?.x ??
    (cell.x + cell.width / 2);

  const spriteY =
    unitRuntume.renderPosition?.y ??
    (cell.y + cell.height - 10);

  const scaleX =
    unitRuntume.facing === FacingDirection.Left
      ? -spriteAnimation.scale.x
      : spriteAnimation.scale.x;

  const spriteHeight = textures[0].height * spriteAnimation.scale.y;

  const handleRef = (sprite: AnimatedSprite | null) => {
    if (!sprite || spriteRef.current) return;
    spriteRef.current = sprite;
    onSpriteReady(sprite);

    try { sprite.play(); } catch { }
  };

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
        ref={handleRef}
        textures={textures}
        x={spriteX}
        y={spriteY}
        anchor={{ x: 0.5, y: 1 }}
        scale={{ x: scaleX, y: spriteAnimation.scale.y }}
        animationSpeed={spriteAnimation.animationSpeed}
        autoPlay={false}
        loop={true}
      />
    </>
  );
}
