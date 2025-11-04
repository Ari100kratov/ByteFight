import { useEffect, useState } from "react";
import { Texture } from "pixi.js";
import { useTexturesStore } from "@/features/game/state/data/textures.data.store";

export function isTextureRenderable(texture?: Texture | null) {
  if (!texture) return false;
  if (texture.destroyed) return false;
  if (texture._source == null) return false;
  if (texture.width === 0 || texture.height === 0) return false;
  return true;
}

export function useSpriteTexture(url?: string) {
  const getOrLoadTexture = useTexturesStore(s => s.getOrLoadTexture);
  const [texture, setTexture] = useState<Texture | null>();

  useEffect(() => {
    if (!url) {
      setTexture(null);
      return;
    }

    let cancelled = false;
    getOrLoadTexture(url)
      .then(tex => {
        if (cancelled) return;
        setTexture(tex);
      })
      .catch(console.error);

    return () => { cancelled = true; };
  }, [url]);

  return texture;
}
