import type { LucideIcon } from "lucide-react"
import type { GameOutcome } from "@/features/game/types/GameResult"

export type BattleResultTone = "success" | "danger" | "warning" | "neutral"

export type BattleResultMeta = {
  outcome?: GameOutcome
  title: string
  description: string
  tone: BattleResultTone
  icon: LucideIcon
}