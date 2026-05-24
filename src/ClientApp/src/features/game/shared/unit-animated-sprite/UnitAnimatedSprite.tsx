import { AnimatedSprite, Container, Graphics, Texture } from "pixi.js"
import { extend } from "@pixi/react"
import { useGridStore } from "../../state/game/grid.state.store"
import type { UnitRuntime } from "../../types/UnitRuntime"
import type { SpriteAnimationDto } from "@/shared/types/spriteAnimation"
import { UnitBars, UnitResourceDetails, type UnitSide } from "./UnitBars"
import { FacingDirection } from "../../types/common"
import { useRef, useState } from "react"
import { type UnitController } from "../../units/controller/UnitController"
import { useUnitHoverStore } from "../../state/ui/unit.hover.store"
import { RENDER_LAYERS, snapPixel } from "../../rendering/pixiQuality"
import { useCameraStore } from "../../camera/camera.store"

extend({ AnimatedSprite, Container, Graphics })

interface Props {
  runtime: UnitRuntime
  side: UnitSide
  spriteAnimation: SpriteAnimationDto
  controller: UnitController
  clickable?: boolean
  selected?: boolean
  onClick?: (position: { x: number; y: number; side?: "left" | "right" }) => void
}

interface UnitGroundShadowProps {
  x: number
  y: number
  cellWidth: number
  cellHeight: number
  alpha: number
}

interface UnitCellHitAreaProps {
  x: number
  y: number
  width: number
  height: number
  clickable?: boolean
  onTap: () => void
  onPointerOver: () => void
  onPointerOut: () => void
}

function UnitGroundShadow({ x, y, cellWidth, cellHeight, alpha }: UnitGroundShadowProps) {
  return (
    <pixiGraphics
      zIndex={-2}
      draw={(g) => {
        g.clear()
        g.ellipse(x, y - 3, cellWidth * 0.28, Math.max(4, cellHeight * 0.055)).fill({
          color: 0x020617,
          alpha,
        })
      }}
    />
  )
}

function UnitCellHitArea({
  x,
  y,
  width,
  height,
  clickable,
  onTap,
  onPointerOver,
  onPointerOut,
}: UnitCellHitAreaProps) {
  return (
    <pixiGraphics
      eventMode="static"
      cursor={clickable ? "pointer" : "default"}
      zIndex={2}
      onPointerTap={onTap}
      onPointerOver={onPointerOver}
      onPointerOut={onPointerOut}
      draw={(g) => {
        g.clear()
        g.rect(x, y, width, height).fill({
          color: 0xffffff,
          alpha: 0.001,
        })
      }}
    />
  )
}

export function UnitAnimatedSprite({
  runtime,
  side,
  spriteAnimation,
  controller,
  clickable,
  selected = false,
  onClick,
}: Props) {
  const layout = useGridStore((s) => s.layout)
  const spriteRef = useRef<AnimatedSprite | null>(null)
  const [isHovered, setIsHovered] = useState(false)
  const setHoveredUnit = useUnitHoverStore((s) => s.setHoveredUnit)
  const clearHoveredUnit = useUnitHoverStore((s) => s.clearHoveredUnit)

  if (!layout) return null

  const cell = layout.cells[runtime.position.y][runtime.position.x]

  const spriteX = snapPixel(runtime.renderPosition?.x ?? cell.x + cell.width / 2)

  const spriteY = snapPixel(runtime.renderPosition?.y ?? cell.y + cell.height - 10)

  const scaleX =
    runtime.facing === FacingDirection.Left ? -spriteAnimation.scale.x : spriteAnimation.scale.x

  const spriteHeight = (runtime.textureHeight ?? 0) * spriteAnimation.scale.y
  const isDead = runtime.hp.current <= 0

  const handleRef = (sprite: AnimatedSprite | null) => {
    if (!sprite) return

    spriteRef.current = sprite
    controller.sprite.attach(sprite)
    controller.notifyViewReady()
    sprite.play()
  }

  const healthPriority = runtime.hp.max > 0 ? runtime.hp.current / runtime.hp.max : 0

  const zIndex = RENDER_LAYERS.units + spriteY + healthPriority + (isHovered ? 10 : selected ? 8 : 0)

  const handleTap = () => {
    const camera = useCameraStore.getState()
    if (camera.isClickSuppressed()) return

    onClick?.(
      camera.worldToScreen({
        x: spriteX,
        y: spriteY - spriteHeight,
      }),
    )
  }

  const handlePointerOver = () => {
    setIsHovered(true)
    setHoveredUnit(runtime.id)
  }

  const handlePointerOut = () => {
    setIsHovered(false)
    clearHoveredUnit(runtime.id)
  }

  return (
    <pixiContainer zIndex={zIndex} sortableChildren={true}>
      <UnitGroundShadow
        x={spriteX}
        y={spriteY}
        cellWidth={cell.width}
        cellHeight={cell.height}
        alpha={isDead ? 0.1 : 0.24}
      />
      <UnitBars
        runtime={runtime}
        side={side}
        x={spriteX}
        y={spriteY}
        spriteHeight={spriteHeight}
        cellWidth={cell.width}
        cellHeight={cell.height}
      />
      <pixiAnimatedSprite
        ref={handleRef}
        // eslint-disable-next-line react-hooks/refs -- Pixi animation controller owns textures; replacing them from React causes visible flicker on animation changes.
        textures={spriteRef.current?.textures ?? [Texture.WHITE]}
        x={spriteX}
        y={spriteY}
        anchor={{ x: 0.5, y: 1 }}
        scale={{ x: scaleX, y: spriteAnimation.scale.y }}
        autoPlay={false}
        eventMode="none"
        zIndex={1}
      />
      <UnitCellHitArea
        x={cell.x}
        y={cell.y}
        width={cell.width}
        height={cell.height}
        clickable={clickable}
        onTap={handleTap}
        onPointerOver={handlePointerOver}
        onPointerOut={handlePointerOut}
      />
      {!isDead && (isHovered || selected) && (
        <UnitResourceDetails
          runtime={runtime}
          side={side}
          x={spriteX}
          y={spriteY}
          spriteHeight={spriteHeight}
          cellWidth={cell.width}
          cellHeight={cell.height}
        />
      )}
    </pixiContainer>
  )
}
