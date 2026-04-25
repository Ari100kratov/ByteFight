import type { LucideIcon } from "lucide-react"
import { X } from "lucide-react"
import { cn } from "@/shared/lib/utils"
import { formatBattleDuration } from "./helpers/formatBattleDuration"
import { getBattleResultOverlayToneClass } from "./battle-result.styles"
import type { BattleResultTone } from "./battle-result.types"
import { Badge } from "@/components/ui/badge"

type BattleResultOverlayProps = {
  title: string
  description: string
  tone: BattleResultTone
  Icon: LucideIcon
  totalTurns: number
  startedAt: string
  endedAt?: string | null

  characterName?: string | null
  characterClassName?: string | null
  characterSpecName?: string | null
  arenaName?: string | null
  arenaModeName?: string | null

  onClose: () => void
}

type ResultInfoBlockProps = {
  label: string
  value?: string | null
  meta?: string | null
}

function ResultInfoBlock({
  label,
  value,
  meta,
}: ResultInfoBlockProps) {
  if (!value) {
    return null
  }

  return (
    <div className="flex items-center justify-between rounded-xl border bg-background/70 p-3 gap-3">
      <div className="min-w-0">
        <div className="text-xs text-muted-foreground">
          {label}
        </div>

        <div className="mt-1 text-base font-semibold leading-tight truncate">
          {value}
        </div>
      </div>

      {meta && (
        <Badge variant="secondary">
          {meta}
        </Badge>
      )}
    </div>
  )
}

export function BattleResultOverlay({
  title,
  description,
  tone,
  Icon,
  totalTurns,
  startedAt,
  endedAt,

  characterName,
  characterClassName,
  characterSpecName,
  arenaName,
  arenaModeName,

  onClose,
}: BattleResultOverlayProps) {
  const characterMeta = [characterClassName, characterSpecName]
    .filter(Boolean)
    .join(" · ")

  return (
    <div className="absolute inset-0 z-20 flex items-center justify-center bg-background/65 p-4 backdrop-blur-[3px]">
      <div
        className={cn(
          "relative w-full max-w-md rounded-2xl border shadow-xl backdrop-blur-md",
          getBattleResultOverlayToneClass(tone)
        )}
      >
        <button
          type="button"
          onClick={onClose}
          className="absolute right-3 top-3 text-muted-foreground transition-colors hover:text-foreground"
          aria-label="Закрыть результат боя"
        >
          <X className="h-4 w-4" />
        </button>

        <div className="flex flex-col gap-4 p-6">
          <div className="flex items-center gap-3">
            <div className="flex h-12 w-12 items-center justify-center rounded-full border bg-background/80">
              <Icon className="h-6 w-6" />
            </div>

            <div>
              <div className="text-xs uppercase tracking-wide text-muted-foreground">
                Итоги боя
              </div>
              <div className="text-2xl font-semibold">
                {title}
              </div>
            </div>
          </div>

          <p className="text-sm text-muted-foreground">
            {description}
          </p>

          <div className="flex flex-col gap-3">
            <ResultInfoBlock
              label="Персонаж"
              value={characterName}
              meta={characterMeta}
            />

            <ResultInfoBlock
              label="Арена"
              value={arenaName}
              meta={arenaModeName}
            />
          </div>

          <div className="grid grid-cols-2 gap-3">
            <div className="rounded-xl border bg-background/70 p-3">
              <div className="text-xs text-muted-foreground">Ходов сыграно</div>
              <div className="text-lg font-semibold">{totalTurns ?? "—"}</div>
            </div>

            <div className="rounded-xl border bg-background/70 p-3">
              <div className="text-xs text-muted-foreground">Длительность</div>
              <div className="text-lg font-semibold">
                {formatBattleDuration(startedAt, endedAt)}
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  )
}