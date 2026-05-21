import { extend, useTick } from "@pixi/react"
import { AnimatedSprite, Sprite, type Texture } from "pixi.js"
import { useEffect, useRef, useState } from "react"
import { useGridStore } from "../state/game/grid.state.store"
import { useTexturesStore } from "../state/data/textures.data.store"
import type { ArenaItemResponse } from "@/features/game-arena-page/hooks/useArena"
import { useArenaItemSelectionStore } from "../state/ui/arena-item.selection.store"

extend({ Sprite, AnimatedSprite })

interface Props {
  item: ArenaItemResponse
}

interface ItemTextureState {
  spriteKey: string
  texture: Texture | null
  textures: Texture[]
}

const HOVER_AMPLITUDE = 5
const HOVER_SPEED = 0.004

function hashToPhase(value: string) {
  let hash = 0
  for (let i = 0; i < value.length; i++) {
    hash = (hash * 31 + value.charCodeAt(i)) | 0
  }

  return Math.abs(hash) % 360
}

export function ArenaItemSprite({ item }: Props) {
  const layout = useGridStore((s) => s.layout)
  const selectItem = useArenaItemSelectionStore((s) => s.select)

  const spriteKey = `${item.sprite.url}:${String(item.sprite.frameCount)}`
  const [textureState, setTextureState] = useState<ItemTextureState | null>(null)
  const [hoverOffset, setHoverOffset] = useState(0)
  const timeRef = useRef((hashToPhase(item.placedItemId) / 180) * Math.PI)
  const texture = textureState?.spriteKey === spriteKey ? textureState.texture : null
  const textures = textureState?.spriteKey === spriteKey ? textureState.textures : []

  useTick((ticker) => {
    timeRef.current += ticker.deltaMS * HOVER_SPEED
    setHoverOffset(Math.sin(timeRef.current) * HOVER_AMPLITUDE)
  })

  useEffect(() => {
    let cancelled = false

    if (item.sprite.frameCount <= 1) {
      void useTexturesStore
        .getState()
        .getOrLoadTexture(item.sprite.url)
        .then((texture) => {
          if (!cancelled) {
            setTextureState({
              spriteKey,
              texture,
              textures: [],
            })
          }
        })
        .catch(console.error)

      return () => {
        cancelled = true
      }
    }

    void useTexturesStore
      .getState()
      .getOrLoadTextures(item.sprite.url, item.sprite.frameCount)
      .then((textures) => {
        if (!cancelled) {
          setTextureState({
            spriteKey,
            texture: null,
            textures,
          })
        }
      })
      .catch(console.error)

    return () => {
      cancelled = true
    }
  }, [item.sprite.url, item.sprite.frameCount, spriteKey])

  if (!layout) return null

  const cell = layout.cells[item.position.y][item.position.x]

  const x = cell.x + cell.width / 2
  const baseY = cell.y + cell.height / 2 + cell.height * 0.22
  const y = baseY + hoverOffset
  const zIndex = cell.y + cell.height - 50

  const scale = {
    x: item.sprite.scale.x,
    y: item.sprite.scale.y,
  }

  const handleClick = () => {
    selectItem(item.placedItemId, {
      x,
      y: y - 24,
    })
  }

  if (item.sprite.frameCount <= 1) {
    if (!texture) return null

    return (
      <pixiSprite
        texture={texture}
        x={x}
        y={y}
        zIndex={zIndex}
        anchor={{ x: 0.5, y: 1 }}
        scale={scale}
        eventMode="static"
        cursor="pointer"
        onPointerTap={handleClick}
      />
    )
  }

  if (!textures.length) return null

  return (
    <pixiAnimatedSprite
      textures={textures}
      x={x}
      y={y}
      zIndex={zIndex}
      anchor={{ x: 0.5, y: 1 }}
      scale={scale}
      animationSpeed={item.sprite.animationSpeed}
      autoPlay
      loop
      eventMode="static"
      cursor="pointer"
      onPointerTap={handleClick}
    />
  )
}
