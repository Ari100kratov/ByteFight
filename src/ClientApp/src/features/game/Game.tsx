import { Application } from "@pixi/react"
import { GridContainer } from "./grid-container/GridContainer"
import { BackgroundSprite } from "./background-sprite/BackgroundSprite"
import { ArenaEnemies } from "./arena-enemies/ArenaEnemies"
import { useGridStore } from "./state/game/grid.state.store"
import { CharacterAnimatedSprite } from "./character-sprite/CharacterAnimatedSprite"

export function Game() {
  const canvasSize = useGridStore(s => s.canvasSize)
  return (
    <Application width={canvasSize.width} height={canvasSize.height}>
      <BackgroundSprite />
      <GridContainer />
      <CharacterAnimatedSprite />
      <ArenaEnemies />
    </Application>
  )
}
