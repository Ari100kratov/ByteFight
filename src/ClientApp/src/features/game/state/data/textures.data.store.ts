import { create } from "zustand";
import { Texture } from "pixi.js";
import { loadTexturesFromUrl } from "@/shared/api/loadActionAssets";

type TextureEntry = {
  url: string;
  textures: Texture[];
};

type TextureStore = {
  textures: Record<string, TextureEntry>;
  getOrLoadTextures: (url: string, frameCount: number) => Promise<Texture[]>;
};

export const useTexturesStore = create<TextureStore>((set, get) => ({
  textures: {},
  getOrLoadTextures: async (url, frameCount) => {
    const existing = get().textures[url];
    if (existing) return existing.textures;

    const textures = await loadTexturesFromUrl(url, frameCount)

    set((state) => ({
      textures: {
        ...state.textures,
        [url]: { url, textures: textures },
      },
    }));

    return textures;
  },
}));
