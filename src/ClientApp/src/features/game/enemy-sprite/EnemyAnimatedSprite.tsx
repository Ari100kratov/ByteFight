import { useEffect, useState } from "react";
import { AnimatedSprite, Texture, Rectangle, ImageSource } from "pixi.js";
import { extend } from "@pixi/react";
import { ActionType, useEnemy } from "./useEnemy";
import { useAsset } from "@/hooks/useAsset";

extend({ AnimatedSprite, Texture, Rectangle, ImageSource });

type Props = {
  enemyId: string | undefined;
  x: number;
  y: number;
  width: number;
  height: number;
};

export function EnemyAnimatedSprite({
  enemyId,
  x,
  y,
  width,
  height,
}: Props) {
  const actionType = ActionType.Idle;
  const frameCount = 5;

  const { data: enemy } = useEnemy(enemyId);
  const assetKey = enemy?.assets.find((a) => a.actionType === actionType)?.url;
  const { data: blob } = useAsset(assetKey);

  const [textures, setTextures] = useState<Texture[]>([]);

  useEffect(() => {
    if (!blob) return;

    let cancelled = false;

    createImageBitmap(blob)
      .then((bitmap) => {
        if (cancelled) return;

        const frameWidth = bitmap.width / frameCount;
        const texs: Texture[] = [];

        for (let i = 0; i < frameCount; i++) {
          const frame = new Rectangle(i * frameWidth, 0, frameWidth, bitmap.height);
          const source = new ImageSource({ resource: bitmap });
          const texture = new Texture({ source, frame });
          texs.push(texture);
        }

        setTextures(texs);
      })
      .catch(console.error);

    return () => {
      cancelled = true;
      for (const t of textures) {
        t.destroy(false);
      }
      setTextures([]);
    };
  }, [blob, frameCount]);

  if (textures.length === 0) return null;

  return (
    <pixiAnimatedSprite
      ref={(ref) => { ref?.play(); }} // запуск анимации сразу после монтирования, т.к. есть баг на стороне библиотеки
      textures={textures} // массив кадров спрайта
      x={x + width / 2}  // позиция по X (центр спрайта в клетке)
      y={y + height - 10} // позиция по Y (низ спрайта чуть выше нижней границы клетки)
      anchor={{ x: 0.5, y: 1 }} // точка привязки спрайта: центр по горизонтали, низ по вертикали
      scale={{ x: -1, y: 1 }} // отражение по горизонтали (зеркально)
      animationSpeed={0.1}  // скорость анимации (0.1 = кадры меняются медленно)

      autoPlay={true}
      loop={true}
    />
  );
}
