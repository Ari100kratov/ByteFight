import { Texture, Rectangle, ImageSource } from "pixi.js"
import type { ActionAssetDto, ActionType } from "@/shared/types/action"

/**
 * Загружает указанные анимации (в заданном порядке) и возвращает единый массив текстур.
 * 
 * @param actionAssets — все ассеты персонажа
 * @param filter — фильтр действий, например [ActionType.Idle, ActionType.Walk, ActionType.Attack]. Также отвечает и за порядок анимаций
 */
export async function loadActionAssets(
  actionAssets: ActionAssetDto[],
  filter: ActionType[]
) {
  const allTextures: Texture[] = []

  for (const actionType of filter) {
    // 1️⃣ Берём все ассеты с нужным actionType
    const assetsOfType = actionAssets
      .filter(a => a.actionType === actionType)
      .sort((a, b) => a.variant - b.variant)

    for (const asset of assetsOfType) {
      const { url, frameCount } = asset.spriteAnimation
      if (!url) continue

      try {
        // 2️⃣ Загружаем blob
        const res = await fetch(`${import.meta.env.VITE_API_URL}/assets/${url}`)
        if (!res.ok) throw new Error(`Ошибка при загрузке ${url}`)
        const blob = await res.blob()

        // 3️⃣ Создаём bitmap
        const bitmap = await createImageBitmap(blob)
        const frameWidth = bitmap.width / frameCount

        // 4️⃣ Разрезаем на кадры
        for (let i = 0; i < frameCount; i++) {
          const frame = new Rectangle(i * frameWidth, 0, frameWidth, bitmap.height)
          const source = new ImageSource({ resource: bitmap })
          const texture = new Texture({ source, frame })
          allTextures.push(texture)
        }
      } catch (err) {
        console.error("Ошибка при загрузке ассета:", url, err)
      }
    }
  }

  return allTextures
}
