import type { BattleResultTone } from "./battle-result.types"

export function getBattleResultToneClass(tone: BattleResultTone) {
  switch (tone) {
    case "success":
      return "border-emerald-500/25 bg-emerald-500/10 text-emerald-700 hover:bg-emerald-500/15"

    case "danger":
      return "border-rose-500/25 bg-rose-500/10 text-rose-700 hover:bg-rose-500/15"

    case "warning":
      return "border-amber-500/25 bg-amber-500/10 text-amber-700 hover:bg-amber-500/15"

    default:
      return "border-border bg-muted/50 text-foreground/80 hover:bg-muted/70"
  }
}

export function getBattleResultOverlayToneClass(tone: BattleResultTone) {
  switch (tone) {
    case "success":
      return "border-emerald-500/35 bg-emerald-500/15"
    case "danger":
      return "border-rose-500/35 bg-rose-500/15"
    case "warning":
      return "border-amber-500/35 bg-amber-500/15"
    default:
      return "border-border bg-background/95"
  }
}