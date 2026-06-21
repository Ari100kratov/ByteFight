import type { LucideIcon } from "lucide-react"
import { X } from "lucide-react"
import { Badge } from "@/components/ui/badge"
import { cn } from "@/shared/lib/utils"
import { BattleResultCanvasEffects } from "./BattleResultCanvasEffects"
import { formatBattleDuration } from "./helpers/formatBattleDuration"
import { getBattleResultOverlayToneClass } from "./battle-result.styles"
import type { BattleResultTone } from "./battle-result.types"

interface BattleResultOverlayProps {
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

interface ResultInfoBlockProps {
  label: string
  value?: string | null
  meta?: string | null
}

const RESULT_SPARKLES = [
  { left: "13%", top: "22%", delay: "120ms" },
  { left: "86%", top: "24%", delay: "280ms" },
  { left: "79%", top: "74%", delay: "40ms" },
  { left: "18%", top: "72%", delay: "420ms" },
  { left: "50%", top: "12%", delay: "520ms" },
] as const

function ResultInfoBlock({ label, value, meta }: ResultInfoBlockProps) {
  if (!value) {
    return null
  }

  return (
    <div className="bg-background/70 flex items-center justify-between gap-3 rounded-xl border p-3">
      <div className="min-w-0">
        <div className="text-muted-foreground text-xs">{label}</div>

        <div className="mt-1 truncate text-base leading-tight font-semibold">{value}</div>
      </div>

      {meta && <Badge variant="secondary">{meta}</Badge>}
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
  const characterMeta = [characterClassName, characterSpecName].filter(Boolean).join(" · ")

  return (
    <div
      className="battle-result-backdrop bg-background/65 absolute inset-0 z-20 flex items-center justify-center p-4 backdrop-blur-[3px]"
      data-tone={tone}
    >
      <BattleResultCanvasEffects tone={tone} />

      <div
        data-tone={tone}
        className={cn(
          "battle-result-card text-foreground relative w-full max-w-md overflow-hidden rounded-2xl border shadow-xl backdrop-blur-md",
          getBattleResultOverlayToneClass(tone),
        )}
      >
        <div className="battle-result-light" aria-hidden="true" />
        <div className="battle-result-sweep" aria-hidden="true" />
        <div className="battle-result-sparkles" aria-hidden="true">
          {RESULT_SPARKLES.map((sparkle) => (
            <span
              key={`${sparkle.left}-${sparkle.top}`}
              className="battle-result-sparkle"
              style={{
                left: sparkle.left,
                top: sparkle.top,
                animationDelay: sparkle.delay,
              }}
            />
          ))}
        </div>

        <button
          type="button"
          onClick={onClose}
          className="text-muted-foreground hover:text-foreground absolute top-3 right-3 z-20 transition-colors"
          aria-label="Закрыть результат боя"
        >
          <X className="h-4 w-4" />
        </button>

        <div className="relative z-10 flex flex-col gap-4 p-6">
          <div className="flex items-center gap-3">
            <div className="battle-result-icon bg-background/80 flex h-12 w-12 items-center justify-center rounded-full border">
              <Icon className="h-6 w-6" />
            </div>

            <div>
              <div className="text-muted-foreground text-xs tracking-wide uppercase">
                Итоги боя
              </div>
              <div className="battle-result-title text-2xl font-semibold">{title}</div>
            </div>
          </div>

          <p className="text-muted-foreground text-sm">{description}</p>

          <div className="flex flex-col gap-3">
            <ResultInfoBlock label="Персонаж" value={characterName} meta={characterMeta} />

            <ResultInfoBlock label="Арена" value={arenaName} meta={arenaModeName} />
          </div>

          <div className="grid grid-cols-2 gap-3">
            <div className="bg-background/70 rounded-xl border p-3">
              <div className="text-muted-foreground text-xs">Ходов сыграно</div>
              <div className="text-lg font-semibold">{totalTurns}</div>
            </div>

            <div className="bg-background/70 rounded-xl border p-3">
              <div className="text-muted-foreground text-xs">Длительность</div>
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
