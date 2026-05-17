import { Texture, Rectangle, ImageSource } from "pixi.js"
import { apiUrl } from "@/shared/config/api"
import type { ActionAssetDto } from "@/shared/types/action"

/**
 * Загружает переданные ассеты в указанном порядке и возвращает единый массив текстур.
 *
 * @param actionAssets — ассеты уже в нужном порядке воспроизведения.
 */
export async function loadActionAssets(actionAssets: ActionAssetDto[]) {
  const allTextures: Texture[] = []

  for (const asset of actionAssets) {
    const { url, frameCount } = asset.spriteAnimation
    if (!url) continue

    const textures = await loadTexturesFromUrl(url, frameCount)
    allTextures.push(...textures)
  }

  return allTextures
}

/**
 * Загружает один ассет и возвращает массив текстур для всех кадров.
 *
 * @param url - путь к ассету
 * @param frameCount - количество кадров
 */
export async function loadTexturesFromUrl(url: string, frameCount: number): Promise<Texture[]> {
  try {
    const res = await fetch(apiUrl(`/assets/${url}`))
    if (!res.ok) throw new Error(`Ошибка при загрузке ${url}`)

    const blob = await res.blob()
    const bitmap = await createImageBitmap(blob)
    const frameWidth = bitmap.width / frameCount
    const textures: Texture[] = []
    for (let i = 0; i < frameCount; i++) {
      const frame = new Rectangle(i * frameWidth, 0, frameWidth, bitmap.height)
      const source = new ImageSource({ resource: bitmap })
      textures.push(new Texture({ source, frame }))
    }

    return textures
  } catch (err) {
    console.error("Ошибка при загрузке ассета:", url, err)
    return []
  }
}

/**
 * Загружает один ассет и возвращает текстуру.
 *
 * @param url - путь к ассету
 */
export async function loadTextureFromUrl(url: string): Promise<Texture | null> {
  try {
    const res = await fetch(apiUrl(`/assets/${url}`))
    if (!res.ok) throw new Error(`Ошибка при загрузке ${url}`)

    const blob = await res.blob()
    const bitmap = await createImageBitmap(blob)

    return Texture.from(bitmap)
  } catch (err) {
    console.error("Ошибка при загрузке ассета:", url, err)
    return null
  }
}

export function getAssetUrl(assetKey?: string | null) {
  if (!assetKey) return null
  return apiUrl(`/assets/${assetKey}`)
}
