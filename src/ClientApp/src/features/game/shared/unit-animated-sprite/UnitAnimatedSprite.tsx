import { AnimatedSprite, Container, Texture } from "pixi.js"
import { extend } from "@pixi/react"
import { useGridStore } from "../../state/game/grid.state.store"
import type { UnitRuntime } from "../../types/UnitRuntime"
import type { SpriteAnimationDto } from "@/shared/types/spriteAnimation"
import { UnitBars, UnitResourceDetails } from "./UnitBars"
import { FacingDirection } from "../../types/common"
import { useRef, useState } from "react"
import { type UnitController } from "../../units/controller/UnitController"
import { useUnitHoverStore } from "../../state/ui/unit.hover.store"

extend({ AnimatedSprite, Container })

interface Props {
  runtime: UnitRuntime
  spriteAnimation: SpriteAnimationDto
  controller: UnitController
  clickable?: boolean
  selected?: boolean
  onClick?: (position: { x: number; y: number; side?: "left" | "right" }) => void
}

export function UnitAnimatedSprite({
  runtime,
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

  const spriteX = runtime.renderPosition?.x ?? cell.x + cell.width / 2

  const spriteY = runtime.renderPosition?.y ?? cell.y + cell.height - 10

  const scaleX =
    runtime.facing === FacingDirection.Left ? -spriteAnimation.scale.x : spriteAnimation.scale.x

  const spriteHeight = (runtime.textureHeight ?? 0) * spriteAnimation.scale.y
  const isDead = runtime.hp.current <= 0
  const hoverScale = isHovered || selected ? 1.035 : 1

  const handleRef = (sprite: AnimatedSprite | null) => {
    if (!sprite) return

    spriteRef.current = sprite
    controller.sprite.attach(sprite)
    controller.notifyViewReady()
    sprite.play()
  }

  const healthPriority = runtime.hp.max > 0 ? runtime.hp.current / runtime.hp.max : 0

  const zIndex = spriteY + healthPriority + (isHovered ? 10 : selected ? 8 : 0)

  return (
    <pixiContainer zIndex={zIndex} sortableChildren={true}>
      <UnitBars
        runtime={runtime}
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
        scale={{ x: scaleX * hoverScale, y: spriteAnimation.scale.y * hoverScale }}
        autoPlay={false}
        eventMode="static"
        cursor={clickable ? "pointer" : "default"}
        zIndex={1}
        onPointerTap={() =>
          onClick?.({
            x: spriteX,
            y: spriteY - spriteHeight,
          })
        }
        onPointerOver={() => {
          setIsHovered(true)
          setHoveredUnit(runtime.id)
        }}
        onPointerOut={() => {
          setIsHovered(false)
          clearHoveredUnit(runtime.id)
        }}
      />
      {!isDead && (isHovered || selected) && (
        <UnitResourceDetails
          runtime={runtime}
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
