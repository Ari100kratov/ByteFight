import { Clock3, Hourglass, ShieldAlert, Swords, Trophy } from "lucide-react"
import { GameOutcome } from "@/features/game/types/GameResult"
import type { BattleResultMeta } from "../battle-result.types"

const DESCRIPTIONS: Record<GameOutcome, string[]> = {
  [GameOutcome.Victory]: [
    "Отличная работа. Противник даже не понял, что произошло.",
    "Чистая победа. Можно открывать шампанское.",
    "Противник пал. Вы были хороши.",
    "Это было красиво. И эффективно.",
  ],

  [GameOutcome.Defeat]: [
    "Сегодня противник оказался убедительнее. Бывает.",
    "В этот раз вас переиграли. Но не в последний.",
    "Поражение — тоже часть пути.",
    "Противник оказался на шаг впереди.",
  ],

  [GameOutcome.Draw]: [
    "Вы оба старались… и оба выжили.",
    "Никто не победил. Но и не проиграл.",
    "Редкий случай — равная игра.",
    "Кажется, вы нашли достойного соперника.",
  ],

  [GameOutcome.TimeoutLoss]: [
    "Вы задумались слишком глубоко. Бой ждать не стал.",
    "Время вышло быстрее, чем идеи.",
    "Пока вы думали, время сделало ход.",
    "Иногда нужно действовать быстрее.",
  ],

  [GameOutcome.TurnLimitLoss]: [
    "План был хороший. Ходов — не хватило.",
    "Ходы закончились раньше, чем стратегия.",
    "Вы были близко. Но лимит оказался ближе.",
    "Иногда одного хода не хватает.",
  ],
}

function pickRandom<T>(arr: T[], seed?: string): T {
  if (!arr.length) {
    throw new Error("Empty array")
  }

  if (!seed) {
    return arr[Math.floor(Math.random() * arr.length)]
  }

  // простой hash от строки
  let hash = 0
  for (let i = 0; i < seed.length; i++) {
    hash = (hash * 31 + seed.charCodeAt(i)) | 0
  }

  const index = Math.abs(hash) % arr.length
  return arr[index]
}

export function getBattleResultMeta(
  outcome: GameOutcome | undefined,
  sessionId?: string
): BattleResultMeta {

  const description =
    outcome && DESCRIPTIONS[outcome]
      ? pickRandom(DESCRIPTIONS[outcome], sessionId)
      : "Результат боя получен."

  switch (outcome) {
    case GameOutcome.Victory:
      return {
        outcome,
        title: "Победа",
        description,
        tone: "success",
        icon: Trophy,
      }

    case GameOutcome.Defeat:
      return {
        outcome,
        title: "Поражение",
        description,
        tone: "danger",
        icon: ShieldAlert,
      }

    case GameOutcome.Draw:
      return {
        outcome,
        title: "Ничья",
        description,
        tone: "neutral",
        icon: Swords,
      }

    case GameOutcome.TimeoutLoss:
      return {
        outcome,
        title: "Время вышло",
        description,
        tone: "warning",
        icon: Clock3,
      }

    case GameOutcome.TurnLimitLoss:
      return {
        outcome,
        title: "Ходы закончились",
        description,
        tone: "warning",
        icon: Hourglass,
      }

    default:
      return {
        outcome,
        title: "Бой завершен",
        description: "Результат боя получен.",
        tone: "neutral",
        icon: Swords,
      }
  }
}