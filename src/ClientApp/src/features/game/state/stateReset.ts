import { useArenaEnemiesStore } from "./data/arena-enemies.store"
import { useArenaStore } from "./data/arena.data.store"
import { useCharacterStore } from "./data/character.data.store"
import { useEnemiesStore } from "./data/enemies.data.store"
import { useTexturesStore } from "./data/textures.data.store"
import { useCharacterStateStore } from "./game/character.state.store"
import { useEnemyStateStore } from "./game/enemy.state.store"
import { useGridStore } from "./game/grid.state.store"

export function resetGameStores() {
  useArenaEnemiesStore.getState().reset()
  useArenaStore.getState().reset()
  useCharacterStore.getState().reset()
  useEnemiesStore.getState().reset()
  useTexturesStore.getState().reset()

  useCharacterStateStore.getState().reset()
  useEnemyStateStore.getState().reset()
  useGridStore.getState().reset()
}