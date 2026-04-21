import type { LucideIcon } from "lucide-react"
import {
  CircleSlash,
  Clock3,
  Hourglass,
  LoaderCircle,
  ShieldAlert,
  Swords,
  Trophy,
  XCircle,
} from "lucide-react"

import { GameOutcome } from "@/features/game/types/GameResult"
import type { BattleResultTone } from "@/features/game-arena-page/components/battle-result-overlay/battle-result.types"
import { GameStatus } from "@/features/game/types/GameSession"

export type BattleHistoryResultView = {
  title: string
  tone: BattleResultTone
  Icon: LucideIcon
}

type Params = {
  status: GameStatus
  outcome?: GameOutcome | null
}

export function mapBattleHistoryResult({
  status,
  outcome,
}: Params): BattleHistoryResultView {
  switch (status) {
    case GameStatus.Pending:
      return {
        title: "В очереди",
        tone: "neutral",
        Icon: Clock3,
      }

    case GameStatus.Active:
      return {
        title: "Идет бой",
        tone: "warning",
        Icon: LoaderCircle,
      }

    case GameStatus.Aborted:
      return {
        title: "Прерван",
        tone: "neutral",
        Icon: CircleSlash,
      }

    case GameStatus.Failed:
      return {
        title: "Ошибка",
        tone: "danger",
        Icon: XCircle,
      }

    case GameStatus.Completed:
      switch (outcome) {
        case GameOutcome.Victory:
          return {
            title: "Победа",
            tone: "success",
            Icon: Trophy,
          }

        case GameOutcome.Defeat:
          return {
            title: "Поражение",
            tone: "danger",
            Icon: ShieldAlert,
          }

        case GameOutcome.Draw:
          return {
            title: "Ничья",
            tone: "neutral",
            Icon: Swords,
          }

        case GameOutcome.TimeoutLoss:
          return {
            title: "Таймаут",
            tone: "warning",
            Icon: Clock3,
          }

        case GameOutcome.TurnLimitLoss:
          return {
            title: "Лимит ходов",
            tone: "warning",
            Icon: Hourglass,
          }

        default:
          return {
            title: "Завершен",
            tone: "neutral",
            Icon: Swords,
          }
      }

    default:
      return {
        title: "Неизвестно",
        tone: "neutral",
        Icon: CircleSlash,
      }
  }
}