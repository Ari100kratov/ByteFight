import { useArenaEnemiesStore } from "./data/arena-enemies.store"
import { useArenaStore } from "./data/arena.data.store"
import { useCharacterStore } from "./data/character.data.store"
import { useEnemiesStore } from "./data/enemies.data.store"
import { useTexturesStore } from "./data/textures.data.store"
import { useGameBootstrapStore } from "./game.bootstrap.store"
import { useGameRuntimeStore } from "./game.runtime.store"
import { useCharacterStateStore } from "./game/character.state.store"
import { useEnemyStateStore } from "./game/enemy.state.store"
import { useGridStore } from "./game/grid.state.store"
import { useDamageTextStore } from "./ui/damage.text.store"
import { useEnemySelectionStore } from "./ui/enemy.selection.store"

export function resetGameStores() {
  resetGameDataStores()
  resetGameStateStores()
  resetGameUiStores()
}

function resetGameDataStores() {
  useArenaEnemiesStore.getState().reset()
  useArenaStore.getState().reset()
  useCharacterStore.getState().reset()
  useEnemiesStore.getState().reset()
  useTexturesStore.getState().reset()
  useGridStore.getState().reset()
}

function resetGameStateStores() {
  useCharacterStateStore.getState().reset()
  useEnemyStateStore.getState().reset()
  useGameRuntimeStore.getState().reset()
  useGameBootstrapStore.getState().end()
}

function resetGameUiStores() {
  useDamageTextStore.getState().reset()
  useEnemySelectionStore.getState().reset()
}