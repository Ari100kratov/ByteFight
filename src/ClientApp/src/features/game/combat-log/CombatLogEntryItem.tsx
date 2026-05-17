import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar"
import type { GameActionLogEntry } from "../types/TurnLog"
import { formatCombatLog } from "./formatCombatLog"

interface Props {
  entry: GameActionLogEntry
}

function getInitials(name: string) {
  return name
    .trim()
    .split(/[\s—-]+/)
    .filter(Boolean)
    .slice(0, 2)
    .map((part) => part[0]?.toUpperCase() ?? "")
    .join("")
}

export function CombatLogEntryItem({ entry }: Props) {
  const time = new Date(entry.createdAt).toLocaleTimeString("ru-RU", {
    hour: "2-digit",
    minute: "2-digit",
    second: "2-digit",
  })

  const initials = getInitials(entry.actorName)
  const text = formatCombatLog({ entry })

  return (
    <div className="bg-card/50 flex gap-3 rounded-xl border p-3 shadow-sm">
      <Avatar className="h-8 w-8 rounded-lg">
        <AvatarImage src={undefined} alt={entry.actorName} />
        <AvatarFallback className="rounded-lg text-xs font-medium">{initials}</AvatarFallback>
      </Avatar>

      <div className="min-w-0 flex-1">
        <div className="text-sm leading-relaxed break-words">{text}</div>

        <div className="text-muted-foreground/70 mt-1 flex items-center gap-2 text-[11px]">
          <span>{time}</span>

          {entry.info && (
            <>
              <span>•</span>
              <span className="break-words italic">{entry.info}</span>
            </>
          )}
        </div>
      </div>
    </div>
  )
}
