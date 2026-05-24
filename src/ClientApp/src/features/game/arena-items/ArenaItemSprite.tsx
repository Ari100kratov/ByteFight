import { extend, useTick } from "@pixi/react"
import { AnimatedSprite, Container, Graphics, Sprite, type Texture } from "pixi.js"
import { useEffect, useRef, useState } from "react"
import { useGridStore } from "../state/game/grid.state.store"
import { useTexturesStore } from "../state/data/textures.data.store"
import type { ArenaItemResponse } from "@/features/game-arena-page/hooks/useArena"
import { useArenaItemSelectionStore } from "../state/ui/arena-item.selection.store"
import { useCharacterSelectionStore } from "../state/ui/character.selection.store"
import { useEnemySelectionStore } from "../state/ui/enemy.selection.store"
import {
  PIXEL_ART_SCALE_MODE,
  RENDER_LAYERS,
  setTextureScaleMode,
  setTexturesScaleMode,
  snapPixel,
} from "../rendering/pixiQuality"
import { useCameraStore } from "../camera/camera.store"

extend({ Sprite, AnimatedSprite, Container, Graphics })

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
const FLOAT_BASE_LIFT = 5
const ITEM_HIGHLIGHT_FILL_WIDTH_RATIO = 0.31
const ITEM_HIGHLIGHT_FILL_HEIGHT_RATIO = 0.1
const ITEM_HIGHLIGHT_WIDTH_RATIO = 0.38
const ITEM_HIGHLIGHT_HEIGHT_RATIO = 0.135
const ITEM_HIGHLIGHT_PARTICLE_X_RATIO = 0.26
const ITEM_HIGHLIGHT_PARTICLE_Y_RATIO = 0.09

function hashToPhase(value: string) {
  let hash = 0
  for (let i = 0; i < value.length; i++) {
    hash = (hash * 31 + value.charCodeAt(i)) | 0
  }

  return Math.abs(hash) % 360
}

interface ItemInteractionHighlightProps {
  x: number
  y: number
  cellWidth: number
  intensity: number
  isSelected: boolean
}

interface ItemGroundShadowProps {
  x: number
  y: number
  cellWidth: number
  intensity: number
  hoverOffset: number
}

function ItemGroundShadow({ x, y, cellWidth, intensity, hoverOffset }: ItemGroundShadowProps) {
  const liftRatio = Math.max(
    0,
    Math.min(1, (-hoverOffset + HOVER_AMPLITUDE) / (HOVER_AMPLITUDE * 2)),
  )
  const contactRatio = 1 - liftRatio
  const shadowScale = 0.74 + contactRatio * 0.18
  const shadowHeightScale = 0.78 + contactRatio * 0.14
  const shadowAlpha = 0.78 - liftRatio * 0.24

  return (
    <pixiGraphics
      zIndex={-1}
      draw={(g) => {
        g.clear()

        g.ellipse(
          x,
          y - 2,
          cellWidth * 0.25 * shadowScale,
          cellWidth * 0.064 * shadowHeightScale,
        ).fill({
          color: 0x020617,
          alpha: (0.29 + intensity * 0.05) * shadowAlpha,
        })
      }}
    />
  )
}

function ItemInteractionHighlight({
  x,
  y,
  cellWidth,
  intensity,
  isSelected,
}: ItemInteractionHighlightProps) {
  const [pulse, setPulse] = useState(0)

  useTick((ticker) => {
    setPulse((value) => (value + ticker.deltaMS * 0.005) % (Math.PI * 2))
  })

  if (intensity <= 0.02 && !isSelected) return null

  const pulseAlpha = (Math.sin(pulse) + 1) / 2
  const tone = isSelected ? 0xfacc15 : 0x7dd3fc
  const fillTone = isSelected ? 0xfbbf24 : 0x38bdf8
  const alpha = Math.max(intensity, isSelected ? 0.8 : 0)

  return (
    <pixiGraphics
      draw={(g) => {
        g.clear()
        g.ellipse(
          x,
          y - 4,
          cellWidth * ITEM_HIGHLIGHT_FILL_WIDTH_RATIO,
          cellWidth * ITEM_HIGHLIGHT_FILL_HEIGHT_RATIO,
        ).fill({
          color: fillTone,
          alpha: 0.1 * alpha,
        })
        g.setStrokeStyle({
          width: isSelected ? 2 : 1.5,
          color: tone,
          alpha: (0.34 + pulseAlpha * 0.24) * alpha,
        })
        g.ellipse(
          x,
          y - 4,
          cellWidth * ITEM_HIGHLIGHT_WIDTH_RATIO,
          cellWidth * ITEM_HIGHLIGHT_HEIGHT_RATIO,
        )
        g.stroke()

        for (let i = 0; i < 4; i++) {
          const angle = pulse + i * (Math.PI / 2)
          const particleX = x + Math.cos(angle) * cellWidth * ITEM_HIGHLIGHT_PARTICLE_X_RATIO
          const particleY = y - 24 + Math.sin(angle) * cellWidth * ITEM_HIGHLIGHT_PARTICLE_Y_RATIO

          g.circle(particleX, particleY, 1.6).fill({
            color: 0xfef3c7,
            alpha: (0.22 + pulseAlpha * 0.42) * alpha,
          })
        }
      }}
    />
  )
}

export function ArenaItemSprite({ item }: Props) {
  const layout = useGridStore((s) => s.layout)
  const selectItem = useArenaItemSelectionStore((s) => s.select)
  const clearSelection = useArenaItemSelectionStore((s) => s.clearSelection)
  const selectedPlacedItemId = useArenaItemSelectionStore((s) => s.selectedPlacedItemId)

  const spriteKey = `${item.sprite.url}:${String(item.sprite.frameCount)}`
  const [textureState, setTextureState] = useState<ItemTextureState | null>(null)
  const [hoverOffset, setHoverOffset] = useState(0)
  const [hoverIntensity, setHoverIntensity] = useState(0)
  const [isHovered, setIsHovered] = useState(false)
  const timeRef = useRef((hashToPhase(item.placedItemId) / 180) * Math.PI)
  const texture = textureState?.spriteKey === spriteKey ? textureState.texture : null
  const textures = textureState?.spriteKey === spriteKey ? textureState.textures : []
  const isSelected = selectedPlacedItemId === item.placedItemId

  useTick((ticker) => {
    timeRef.current += ticker.deltaMS * HOVER_SPEED
    setHoverOffset(Math.sin(timeRef.current) * HOVER_AMPLITUDE)
    setHoverIntensity((value) => {
      const target = isHovered || isSelected ? 1 : 0
      const next = value + (target - value) * Math.min(1, ticker.deltaMS / 120)

      return Math.abs(next - value) < 0.01 ? target : next
    })
  })

  useEffect(() => {
    let cancelled = false

    if (item.sprite.frameCount <= 1) {
      void useTexturesStore
        .getState()
        .getOrLoadTexture(item.sprite.url)
        .then((texture) => {
          if (!cancelled) {
            setTextureScaleMode(texture, PIXEL_ART_SCALE_MODE)

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
          setTexturesScaleMode(textures, PIXEL_ART_SCALE_MODE)

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

  const x = snapPixel(cell.x + cell.width / 2)
  const baseY = snapPixel(cell.y + cell.height / 2 + cell.height * 0.22)
  const y = snapPixel(baseY - FLOAT_BASE_LIFT + hoverOffset)
  const zIndex = RENDER_LAYERS.items + cell.y + cell.height - 50

  const scale = {
    x: item.sprite.scale.x,
    y: item.sprite.scale.y,
  }

  const handleClick = () => {
    const camera = useCameraStore.getState()
    if (camera.isClickSuppressed()) return

    if (useArenaItemSelectionStore.getState().selectedPlacedItemId === item.placedItemId) {
      clearSelection()
      return
    }

    useCharacterSelectionStore.getState().clearSelection()
    useEnemySelectionStore.getState().clearSelection()
    selectItem(item.placedItemId, camera.worldToScreen({
      x,
      y: y - 24,
    }))
  }

  const handlePointerOver = () => {
    setIsHovered(true)
  }

  const handlePointerOut = () => {
    setIsHovered(false)
  }

  if (item.sprite.frameCount <= 1) {
    if (!texture) return null

    return (
      <pixiContainer zIndex={zIndex + hoverIntensity * 12} sortableChildren={true}>
        <ItemGroundShadow
          x={x}
          y={baseY}
          cellWidth={cell.width}
          intensity={hoverIntensity}
          hoverOffset={hoverOffset}
        />
        <ItemInteractionHighlight
          x={x}
          y={baseY}
          cellWidth={cell.width}
          intensity={hoverIntensity}
          isSelected={isSelected}
        />
        <pixiSprite
          texture={texture}
          x={x}
          y={y}
          anchor={{ x: 0.5, y: 1 }}
          scale={scale}
          eventMode="static"
          cursor="pointer"
          onPointerTap={handleClick}
          onPointerOver={handlePointerOver}
          onPointerOut={handlePointerOut}
        />
      </pixiContainer>
    )
  }

  if (!textures.length) return null

  return (
    <pixiContainer zIndex={zIndex + hoverIntensity * 12} sortableChildren={true}>
      <ItemGroundShadow
        x={x}
        y={baseY}
        cellWidth={cell.width}
        intensity={hoverIntensity}
        hoverOffset={hoverOffset}
      />
      <ItemInteractionHighlight
        x={x}
        y={baseY}
        cellWidth={cell.width}
        intensity={hoverIntensity}
        isSelected={isSelected}
      />
      <pixiAnimatedSprite
        textures={textures}
        x={x}
        y={y}
        anchor={{ x: 0.5, y: 1 }}
        scale={scale}
        animationSpeed={item.sprite.animationSpeed}
        autoPlay
        loop
        eventMode="static"
        cursor="pointer"
        onPointerTap={handleClick}
        onPointerOver={handlePointerOver}
        onPointerOut={handlePointerOut}
      />
    </pixiContainer>
  )
}
