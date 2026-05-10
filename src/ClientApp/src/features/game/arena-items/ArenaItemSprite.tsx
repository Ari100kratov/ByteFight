import { extend, useTick } from "@pixi/react"
import { AnimatedSprite, Container, Sprite, Texture } from "pixi.js"
import { useEffect, useRef, useState } from "react"
import { useGridStore } from "../state/game/grid.state.store"
import { useTexturesStore } from "../state/data/textures.data.store"
import type { ArenaItemResponse } from "@/features/game-arena-page/hooks/useArena"
import { useArenaItemSelectionStore } from "../state/ui/arena-item.selection.store"

extend({ Container, Sprite, AnimatedSprite })

type Props = {
  item: ArenaItemResponse
}

const HOVER_AMPLITUDE = 5
const HOVER_SPEED = 0.004

export function ArenaItemSprite({ item }: Props) {
  const layout = useGridStore(s => s.layout)
  const [texture, setTexture] = useState<Texture | null>(null)
  const [textures, setTextures] = useState<Texture[]>([])
  const [hoverOffset, setHoverOffset] = useState(0)
  const timeRef = useRef(Math.random() * Math.PI * 2)

  useTick(ticker => {
    timeRef.current += ticker.deltaMS * HOVER_SPEED
    setHoverOffset(Math.sin(timeRef.current) * HOVER_AMPLITUDE)
  })

  useEffect(() => {
    let cancelled = false

    if (item.sprite.frameCount <= 1) {
      useTexturesStore
        .getState()
        .getOrLoadTexture(item.sprite.url)
        .then(texture => {
          if (!cancelled) setTexture(texture)
        })

      return () => {
        cancelled = true
      }
    }

    useTexturesStore
      .getState()
      .getOrLoadTextures(item.sprite.url, item.sprite.frameCount)
      .then(textures => {
        if (!cancelled) setTextures(textures)
      })

    return () => {
      cancelled = true
    }
  }, [item.sprite.url, item.sprite.frameCount])

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

  const selectItem = useArenaItemSelectionStore(s => s.select)

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