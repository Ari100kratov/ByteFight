import { create } from "zustand"
import { type Texture } from "pixi.js"
import { loadTextureFromUrl, loadTexturesFromUrl } from "@/shared/api/loadActionAssets"

type TexturesEntry = {
  url: string
  frameCount: number
  textures: Texture[]
}

type SingleTextureEntry = {
  url: string
  texture: Texture
}

type TextureStore = {
  textures: Partial<Record<string, TexturesEntry>>
  singleTextures: Partial<Record<string, SingleTextureEntry>>

  texturesPromises: Partial<Record<string, Promise<Texture[]>>>
  singleTexturePromises: Partial<Record<string, Promise<Texture | null>>>

  getOrLoadTextures: (url: string, frameCount: number) => Promise<Texture[]>
  getOrLoadTexture: (url: string) => Promise<Texture | null>
  reset: () => void
}

function getAnimationCacheKey(url: string, frameCount: number) {
  return `${url}::frames=${frameCount}`
}

function withoutRecordKey<TValue>(record: Partial<Record<string, TValue>>, key: string) {
  const next = { ...record }
  delete next[key]
  return next
}

export const useTexturesStore = create<TextureStore>((set, get) => ({
  textures: {},
  singleTextures: {},

  texturesPromises: {},
  singleTexturePromises: {},

  getOrLoadTextures: async (url, frameCount) => {
    const key = getAnimationCacheKey(url, frameCount)

    const existing = get().textures[key]
    if (existing) return existing.textures

    const pending = get().texturesPromises[key]
    if (pending !== undefined) return pending

    const promise = loadTexturesFromUrl(url, frameCount)
      .then((textures) => {
        set((state) => ({
          textures: {
            ...state.textures,
            [key]: { url, frameCount, textures },
          },
        }))

        return textures
      })
      .finally(() => {
        set((state) => ({
          texturesPromises: withoutRecordKey(state.texturesPromises, key),
        }))
      })

    set((state) => ({
      texturesPromises: {
        ...state.texturesPromises,
        [key]: promise,
      },
    }))

    return promise
  },

  getOrLoadTexture: async (url) => {
    const existing = get().singleTextures[url]
    if (existing) return existing.texture

    const pending = get().singleTexturePromises[url]
    if (pending !== undefined) return pending

    const promise = loadTextureFromUrl(url)
      .then((texture) => {
        if (texture) {
          set((state) => ({
            singleTextures: {
              ...state.singleTextures,
              [url]: { url, texture },
            },
          }))
        }

        return texture
      })
      .finally(() => {
        set((state) => ({
          singleTexturePromises: withoutRecordKey(state.singleTexturePromises, url),
        }))
      })

    set((state) => ({
      singleTexturePromises: {
        ...state.singleTexturePromises,
        [url]: promise,
      },
    }))

    return promise
  },

  reset: () => {
    const { textures, singleTextures } = get()

    Object.values(textures).forEach((entry) => {
      if (!entry) return

      entry.textures.forEach((t) => {
        t.destroy()
      })
    })

    Object.values(singleTextures).forEach((entry) => {
      if (!entry) return

      entry.texture.destroy()
    })

    set({
      textures: {},
      singleTextures: {},
      texturesPromises: {},
      singleTexturePromises: {},
    })
  },
}))
