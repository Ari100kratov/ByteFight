import { Application, extend } from "@pixi/react"
import { Container } from "pixi.js"
import { GridContainer } from "./grid-container/GridContainer"
import { BackgroundSprite } from "./background-sprite/BackgroundSprite"
import { ArenaEnemies } from "./arena-enemies/ArenaEnemies"
import { CharacterAnimatedSprite } from "./character-sprite/CharacterAnimatedSprite"
import { ResizeHandler } from "./ResizeHandler"
import { FloatingCombatTextLayer } from "./floating-combat-text/FloatingCombatTextLayer"
import { EnemyInfoPopover } from "./enemy-info-popover/EnemyInfoPopover"
import { ArenaItems } from "./arena-items/ArenaItems"
import { ArenaItemInfoPopover } from "./arena-items/ArenaItemInfoPopover"
import { CharacterInfoPopover } from "./character-info-popover/CharacterInfoPopover"
import { CombatEffectsLayer } from "./combat-effects/CombatEffectsLayer"
import { UnitInteractionHighlightsLayer } from "./unit-interaction-highlights/UnitInteractionHighlightsLayer"
import { getCanvasResolution } from "./rendering/pixiQuality"
import { CameraBoundsSync, CameraInput, CameraStage } from "./camera/ArenaCamera"
import { CameraToolbar } from "./camera/CameraToolbar"
import { useCameraStore } from "./camera/camera.store"

extend({ Container })

export function Game() {
  const isDragging = useCameraStore((s) => s.isDragging)

  return (
    <div className="relative h-full w-full" style={{ cursor: isDragging ? "grabbing" : "grab" }}>
      <Application
        backgroundColor={0xf0f0f0}
        antialias
        autoDensity
        resolution={getCanvasResolution()}
        roundPixels
      >
        <ResizeHandler />
        <CameraBoundsSync />
        <CameraInput />

        <CameraStage>
          <BackgroundSprite />
          <GridContainer />

          <pixiContainer sortableChildren={true}>
            <ArenaItems />
            <UnitInteractionHighlightsLayer />
            <CharacterAnimatedSprite />
            <ArenaEnemies />
            <CombatEffectsLayer />
            <FloatingCombatTextLayer />
          </pixiContainer>
        </CameraStage>
      </Application>

      <CameraToolbar />

      <EnemyInfoPopover />
      <CharacterInfoPopover />
      <ArenaItemInfoPopover />
    </div>
  )
}
