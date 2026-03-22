import type { LucideIcon } from "lucide-react"
import { Badge } from "@/components/ui/badge"
import { cn } from "@/shared/lib/utils"
import { getBattleResultToneClass } from "./battle-result.styles"
import type { BattleResultTone } from "./battle-result.types"

type BattleResultTriggerProps = {
  title: string
  tone: BattleResultTone
  Icon: LucideIcon
  onClick: () => void
}

export function BattleResultTrigger({
  title,
  tone,
  Icon,
  onClick,
}: BattleResultTriggerProps) {
  return (
    <Badge
      asChild
      variant="outline"
      className={cn(
        "cursor-pointer select-none px-3 py-1.5 text-sm font-medium",
        "max-w-[240px]",
        "transition-colors",
        "[&>svg]:size-4",
        getBattleResultToneClass(tone)
      )}
    >
      <button
        type="button"
        onClick={onClick}
        aria-label={`Показать результат боя: ${title}`}
      >
        <Icon />
        <span className="truncate">{title}</span>
      </button>
    </Badge>
  )
}