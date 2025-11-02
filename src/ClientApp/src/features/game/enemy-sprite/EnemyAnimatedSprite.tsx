import { useEffect, useState } from "react";
import { AnimatedSprite, Texture, Rectangle, ImageSource } from "pixi.js";
import { extend } from "@pixi/react";
import { useEnemy } from "./useEnemy";
import { useAssetBlob } from "@/shared/hooks/useAsset";
import { ActionType } from "@/shared/types/action";

extend({ AnimatedSprite, Texture, Rectangle, ImageSource });

type Props = {
  enemyId: string | undefined;
  x: number;
  y: number;
  width: number;
  height: number;
};

const defaultSpriteAnimation = {

  animationSpeed: 0.1,
  frameCount: 1,
  url: undefined,
  scale: {
    x: 1,
    y: 1
  }
}

export function EnemyAnimatedSprite({
  enemyId,
  x,
  y,
  width,
  height,
}: Props) {

  const { data: enemy } = useEnemy(enemyId)

  const spriteAnimation = enemy?.actionAssets
    .find((a) => a.actionType === ActionType.Idle)?.spriteAnimation ?? defaultSpriteAnimation

  const { data: blob } = useAssetBlob(spriteAnimation.url)

  const [textures, setTextures] = useState<Texture[]>([])

  useEffect(() => {
    if (!blob) return;

    let cancelled = false

    createImageBitmap(blob)
      .then((bitmap) => {
        if (cancelled) return;

        const frameWidth = bitmap.width / spriteAnimation.frameCount
        const texs: Texture[] = []

        for (let i = 0; i < spriteAnimation.frameCount; i++) {
          const frame = new Rectangle(i * frameWidth, 0, frameWidth, bitmap.height)
          const source = new ImageSource({ resource: bitmap })
          const texture = new Texture({ source, frame })
          texs.push(texture)
        }

        setTextures(texs)
      })
      .catch(console.error)

    return () => {
      cancelled = true
      for (const t of textures) {
        t.destroy(false)
      }
      setTextures([])
    };
  }, [blob, spriteAnimation])

  if (textures.length === 0) return null

  return (
    <pixiAnimatedSprite
      ref={(ref) => { ref?.play(); }} // запуск анимации сразу после монтирования, т.к. есть баг на стороне библиотеки
      textures={textures} // массив кадров спрайта
      x={x + width / 2}  // позиция по X (центр спрайта в клетке)
      y={y + height - 10} // позиция по Y (низ спрайта чуть выше нижней границы клетки)
      anchor={{ x: 0.5, y: 1 }} // точка привязки спрайта: центр по горизонтали, низ по вертикали
      scale={{ x: -spriteAnimation.scale.x, y: spriteAnimation.scale.y }} // отражение по горизонтали
      animationSpeed={spriteAnimation.animationSpeed}  // скорость анимации (0.1 = кадры меняются медленно)

      autoPlay={true}
      loop={true}
    />
  )
}
