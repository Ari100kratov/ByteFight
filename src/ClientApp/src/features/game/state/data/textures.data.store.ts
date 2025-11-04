import { create } from "zustand"
import { Texture } from "pixi.js"
import { loadTextureFromUrl, loadTexturesFromUrl } from "@/shared/api/loadActionAssets"

type TexturesEntry = {
  url: string
  textures: Texture[]
}

type SingleTextureEntry = {
  url: string
  texture: Texture
}

type TextureStore = {
  textures: Record<string, TexturesEntry>
  singleTextures: Record<string, SingleTextureEntry>

  getOrLoadTextures: (url: string, frameCount: number) => Promise<Texture[]>
  getOrLoadTexture: (url: string) => Promise<Texture | null>
  reset: () => void
}

export const useTexturesStore = create<TextureStore>((set, get) => ({
  textures: {},
  singleTextures: {},

  getOrLoadTextures: async (url, frameCount) => {
    const existing = get().textures[url]
    if (existing) return existing.textures

    const textures = await loadTexturesFromUrl(url, frameCount)

    set((state) => ({
      textures: {
        ...state.textures,
        [url]: { url, textures },
      },
    }))

    return textures
  },

  getOrLoadTexture: async (url) => {
    const existing = get().singleTextures[url]
    if (existing) return existing.texture

    const texture = await loadTextureFromUrl(url)
    if (texture) {
      set((state) => ({
        singleTextures: {
          ...state.singleTextures,
          [url]: { url, texture },
        },
      }))
    }

    return texture
  },

  reset: () => {
    const { textures, singleTextures } = get()

    // Освободить все текстуры из памяти GPU
    Object.values(textures).forEach(entry => {
      entry.textures.forEach(t => t.destroy())
    })

    Object.values(singleTextures).forEach(entry => {
      entry.texture.destroy()
    })

    set({ textures: {}, singleTextures: {} })
  },
}))
