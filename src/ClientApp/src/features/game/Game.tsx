import { Application, extend } from "@pixi/react"
import { Container } from "pixi.js"
import { GridContainer } from "./grid-container/GridContainer"
import { BackgroundSprite } from "./background-sprite/BackgroundSprite"
import { ArenaEnemies } from "./arena-enemies/ArenaEnemies"
import { CharacterAnimatedSprite } from "./character-sprite/CharacterAnimatedSprite"
import { ResizeHandler } from "./ResizeHandler"

extend({ Container })

export function Game() {
  return (
    <Application>
      <ResizeHandler />
      <BackgroundSprite />
      <GridContainer />
      <pixiContainer sortableChildren={true}>
        <CharacterAnimatedSprite />
        <ArenaEnemies />
      </pixiContainer>
    </Application>
  )
}
