import { useEffect, useState } from "react";
import { Texture } from "pixi.js";
import { useTexturesStore } from "@/features/game/state/data/textures.data.store";
import type { SpriteAnimationDto } from "../types/spriteAnimation";

export function useSpriteTextures(spriteAnimation?: SpriteAnimationDto) {
  const getOrLoadTextures = useTexturesStore(s => s.getOrLoadTextures);
  const [textures, setTextures] = useState<Texture[]>([]);

  useEffect(() => {
    if (!spriteAnimation) {
      setTextures([]);
      return;
    }

    let cancelled = false;

    getOrLoadTextures(spriteAnimation.url, spriteAnimation.frameCount)
      .then(texs => {
        if (!cancelled) setTextures(texs);
      })
      .catch(console.error);

    return () => { cancelled = true; };
  }, [spriteAnimation?.url, spriteAnimation?.frameCount]);

  return textures;
}
