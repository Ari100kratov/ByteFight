import { Application, extend } from "@pixi/react"
import { Container } from "pixi.js"
import { GridContainer } from "./grid-container/GridContainer"
import { BackgroundSprite } from "./background-sprite/BackgroundSprite"
import { ArenaEnemies } from "./arena-enemies/ArenaEnemies"
import { CharacterAnimatedSprite } from "./character-sprite/CharacterAnimatedSprite"
import { ResizeHandler } from "./ResizeHandler"
import { DamageTextLayer } from "./damage-text/DamageTextLayer"
import { EnemyInfoPopover } from "./enemy-info-popover/EnemyInfoPopover"

extend({ Container })

export function Game() {
  return (
    <div className="relative h-full w-full">
      <Application backgroundColor={0xf0f0f0}>
        <ResizeHandler />
        <BackgroundSprite />
        <GridContainer />

        <pixiContainer sortableChildren={true}>
          <CharacterAnimatedSprite />
          <ArenaEnemies />
          <DamageTextLayer />
        </pixiContainer>
      </Application>

      <EnemyInfoPopover />
    </div>
  )
}
