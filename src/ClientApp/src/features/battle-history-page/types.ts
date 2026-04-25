import type { GameOutcome } from "../game/types/GameResult"
import type { GameModeType, GameStatus } from "../game/types/GameSession"

export type GameSessionListItem = {
  id: string
  mode: GameModeType

  arenaId: string
  arenaName: string

  characterName?: string | null
  characterClassName?: string | null
  characterSpecName?: string | null

  startedAt: string
  endedAt?: string | null
  totalTurns: number
  status: GameStatus
  outcome?: GameOutcome | null
}

