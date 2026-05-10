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
          <CharacterAnimatedSprite />
          <ArenaEnemies />
          <FloatingCombatTextLayer />
        </pixiContainer>
      </Application>

      <EnemyInfoPopover />
      <ArenaItemInfoPopover />
    </div>
  )
}
