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

extend({ Container })

export function Game() {
  return (
    <div className="relative h-full w-full">
      <Application backgroundColor={0xf0f0f0}>
        <ResizeHandler />
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
      </Application>

      <EnemyInfoPopover />
      <CharacterInfoPopover />
      <ArenaItemInfoPopover />
    </div>
  )
}
