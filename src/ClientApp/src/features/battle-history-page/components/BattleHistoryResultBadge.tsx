import type { LucideIcon } from "lucide-react"
import { Badge } from "@/components/ui/badge"
import { cn } from "@/shared/lib/utils"
import { getBattleResultToneClass } from "@/features/game-arena-page/components/battle-result-overlay/battle-result.styles"
import type { BattleResultTone } from "@/features/game-arena-page/components/battle-result-overlay/battle-result.types"

type BattleHistoryResultBadgeProps = {
  title: string
  tone: BattleResultTone
  Icon: LucideIcon
}

export function BattleHistoryResultBadge({
  title,
  tone,
  Icon,
}: BattleHistoryResultBadgeProps) {
  return (
    <div className="flex justify-center">
      <Badge
        variant="outline"
        className={cn(
          "inline-flex items-center gap-1.5 select-none px-3 py-1.5 text-sm font-medium",
          "min-w-[140px] justify-center",
          "[&>svg]:size-4",
          getBattleResultToneClass(tone)
        )}
      >
        <Icon className={title === "Идет бой" ? "animate-spin" : undefined} />
        <span className="truncate">{title}</span>
      </Badge>
    </div>
  )
}